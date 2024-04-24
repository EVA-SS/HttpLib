using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLib
{
    public partial class HttpDown : IDisposable
    {
        #region 下载

        #region 多样下载

        /// <summary>
        /// 下载-自动
        /// </summary>
        public Task<string?> Go()
        { return Go(Environment.ProcessorCount, null); }

        /// <summary>
        /// 下载-自定义下载线程数
        /// </summary>
        public Task<string?> Go(int TaskCount)
        { return Go(TaskCount, null); }

        /// <summary>
        /// 下载-自定义保存文件名称
        /// </summary>
        /// <param name="FileName"></param>
        public Task<string?> Go(string FileName)
        { return Go(Environment.ProcessorCount, FileName); }

        #endregion

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="TaskCount">线程数</param>
        /// <param name="FileName">文件名称</param>
        public Task<string?> Go(int TaskCount, string? FileName)
        {
            canSpeed = true;
#if NET40
            return Task.Factory.StartNew(() =>
            {
                return DownCore(TaskCount, FileName);
            });
#else
            return Task.Run(() =>
            {
                return DownCore(TaskCount, FileName);
            });
#endif
        }

        #endregion

        #region 计算速度

        void TestTime()
        {
            var times = new List<int>();
            long oldsize = 0;
            var action_time = () =>
            {
                while (_State == DownState.Downloading || _State == DownState.Stop)
                {
                    Thread.Sleep(1000);
                    SetValue();
                    long _downsize = _Value - oldsize;
                    oldsize = _Value;

                    if (_downsize > 0)
                    {
                        int se = (int)((_MaxValue - oldsize) / _downsize);
                        if (se < 1)
                        {
                            SetTime(null);
                            SetSpeed(_downsize);
                        }
                        else
                        {
                            times.Add(se);

                            if (times.Count > 4)
                            {
                                int AVE = (int)Math.Ceiling(times.Average());
                                times.Clear();
                                TimeSpan timeSpan = new TimeSpan(0, 0, 0, AVE);
                                string time_txt = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";
                                if (time_txt.StartsWith("00:"))
                                {
                                    time_txt = time_txt.Substring(3);
                                }
                                SetTime(time_txt);
                            }
                            SetSpeed(_downsize);
                        }
                    }
                    else
                    {
                        SetSpeed(0);
                    }

                }
            };

#if NET40
            Task.Factory.StartNew(action_time);
#else
            Task.Run(action_time);
#endif
        }

        #endregion

        #region 功能

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Suspend()
        {
            resetState.Reset();
            SetState(DownState.Stop);
        }
        /// <summary>
        /// 恢复下载
        /// </summary>
        public void Resume()
        {
            SetState(DownState.Downloading);
            resetState.Set();
        }

        #endregion

        public override string ToString()
        {
            return Url;
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void Dispose()
        {
            //终止task线程
            resetState.Set();
            resetState.Dispose();
        }

        #region 核心

        string? DownCore(int TaskCount, string? FileName)
        {
            //(option.Uri.AbsoluteUri).Md5_16()
            string WorkPath = SavePath + (ID ?? Guid.NewGuid().ToString()) + Path.DirectorySeparatorChar;
            WorkPath.CreateDirectory();
            long Length = WebGet(this, out bool cSeek, out var Disposition);
            FileName ??= Uri.FileName(Disposition);
            SaveFullName = SavePath + FileName;
            var files = new List<FilesResult>();
            ValTemp.Clear();
            MaxValTemp.Clear();
            TotalCount = DownCount = 0;

            #region 任务分配

            long _down_length = Length;
            int _taskcount = 1;
            if (cSeek && Length > 0 && Length > 2097152)
            {
                _down_length = 2097152;
                _taskcount = (int)Math.Ceiling(Length / (_down_length * 1.0));//任务分块
            }

            for (int i = 0; i < _taskcount; i++)
            {
                long _s = _down_length * i;
                long _e = _down_length;
                if ((_s + _down_length) > Length) _e = _down_length - ((_s + _down_length) - Length);

                string filename_temp = $"{i}_{_s}_{_s + _e}.temp";
                files.Add(new FilesResult(i, WorkPath + filename_temp, _s, _e));
                ValTemp.Add(i, 0);
                MaxValTemp.Add(i, _e);
            }

            SetMaxValue();

            #endregion

            return DownCore(FileName, WorkPath, Length, cSeek, files, TaskCount);
        }

        string? DownCore(string fileName, string WorkPath, long Length, bool cSeek, List<FilesResult> files, int taskCount)
        {
            SetState(DownState.Downloading);
            TestTime();
            bool isStop = false;
            var tasks = new List<Task>();
            foreach (var it in files)
            {
                try
                {
                    resetState.WaitOne();
                }
                catch
                {
                    isStop = true;
                    SetState(DownState.Complete, "主动停止");
                    return null;
                }
#if NET40
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    DownOne(it, cSeek, ref isStop);
                }));
#else
                tasks.Add(Task.Run(() =>
                {
                    DownOne(it, cSeek, ref isStop);
                }));
#endif
                if (tasks.Count > taskCount)
                {
                    Task.WaitAny(tasks.ToArray());
                    tasks = tasks.Where(t => t.Status == TaskStatus.Running).ToList();
                }
            }
            Task.WaitAll(tasks.ToArray());
            canSpeed = false;
            if (isStop) SetState(DownState.Complete, "主动停止");
            else
            {
                var _files = new List<string>();
                foreach (FilesResult it in files)
                {
                    if (!it.complete)
                    {
                        SetState(DownState.Fail, "下载不完全");
                        return null;
                    }
                    _files.Add(it.path);
                }
                string path = _files.CombineMultipleFilesIntoSingleFile(SaveFullName, WorkPath);
                SetState(DownState.Complete);
                return path;
            }
            return null;
        }

        void DownOne(FilesResult item, bool cSeek, ref bool isStop)
        {
            Uri uri = Uri;
            long _downvalueTemp = 0;
            bool runTask = true;
            int ErrCount = 0;
            while (runTask)
            {
                try
                {
                    var request = core.CreateRequest();
                    long fileLong = 0;

                    using (var file = new FileStream(item.path, FileMode.OpenOrCreate))
                    {
                        fileLong = file.Length;
                        if (item.end_position > 0 && file.Length >= item.end_position)
                        {
                            item.complete = true;
                            _downvalueTemp += fileLong;
                            ValTemp[item.i] = MaxValTemp[item.i] = fileLong;
                            SetMaxValue();
                            SetValue();
                            return;
                        }
                        else if (!cSeek)
                        {
                            file.Close();
                            fileLong = 0;
                            File.Delete(item.path);
                        }
                    }

                    using (FileStream file = new FileStream(item.path, FileMode.OpenOrCreate))
                    {
                        if (cSeek)
                        {
                            request.AddRange((long)(item.start_position + file.Length), (long)(item.start_position + item.end_position));
                        }
                        using (var p = (HttpWebResponse)request.GetResponse())
                        {
                            ErrCount = 0;
                            if (p.ContentLength > 0)
                            {
                                if (file.Length >= p.ContentLength)
                                {
                                    fileLong = p.ContentLength;
                                    item.complete = true;
                                    _downvalueTemp += fileLong;
                                    ValTemp[item.i] = MaxValTemp[item.i] = fileLong;
                                    SetMaxValue();
                                    SetValue();
                                    return;
                                }
                                else
                                {
                                    if (!cSeek)
                                    {
                                        file.Seek(0, SeekOrigin.Begin);
                                    }
                                    MaxValTemp[item.i] = p.ContentLength;
                                    SetMaxValue();
                                }
                            }
                            using (var stream = p.GetResponseStream())
                            {
                                if (fileLong > 0)
                                {
                                    file.Seek(fileLong, SeekOrigin.Begin);

                                    _downvalueTemp += fileLong;
                                    ValTemp[item.i] = fileLong;
                                }
                                byte[] _cache = new byte[CacheSize];
                                int osize = stream.Read(_cache, 0, _cache.Length);
                                bool isRun = true;
                                while (isRun)
                                {
                                    if (osize > 0)
                                    {
                                        _downvalueTemp += osize;
                                        ValTemp[item.i] = _downvalueTemp;

                                        try
                                        {
                                            resetState.WaitOne();
                                        }
                                        catch
                                        {
                                            //下载终止
                                            isStop = true;
                                            return;
                                        }

                                        file.Write(_cache, 0, osize);
                                        osize = stream.Read(_cache, 0, _cache.Length);
                                    }
                                    else { isRun = false; }

                                }
                            }

                        }
                    }
                    item.complete = true;
                }
                catch
                {
                    ErrCount++;
                    _downvalueTemp = 0;
                    if (ErrCount > RetryCount) runTask = false;
                }
            }
        }

        static long WebGet(HttpDown core, out bool canSeek, out string? Disposition)
        {
            Disposition = null;
            long _Length = 0;
            try
            {
                var request = core.core.CreateRequest();
            }
            catch
            {
                canSeek = false;
                _Length = 0;
            }
            if (_Length > 0)
            {
                try
                {
                    var request = core.core.CreateRequest();
                    request.AddRange(1, _Length - 1);
                    using (var p = (HttpWebResponse)request.GetResponse())
                    {
                        long length = p.ContentLength;
                        canSeek = length == _Length - 1;
                    }
                }
                catch
                {
                    canSeek = false;
                }
            }
            else canSeek = false;
            return _Length;
        }

        #endregion
    }

    public class FilesResult
    {
        public FilesResult(int _i, string _path, double s, double e)
        {
            i = _i;
            path = _path;
            start_position = s;
            end_position = e;
        }
        public int i { get; set; }

        /// <summary>
        /// 文件保存地址
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 文件开始位置
        /// </summary>
        public double start_position { get; set; }

        /// <summary>
        /// 文件结束位置
        /// </summary>
        public double end_position { get; set; }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        public bool complete { get; set; }
    }
}