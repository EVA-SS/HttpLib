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
                                if (time_txt.StartsWith("00:")) time_txt = time_txt.Substring(3);
                                SetTime(time_txt);
                            }
                            SetSpeed(_downsize);
                        }
                    }
                    else SetSpeed(0);
                }
            };
            ITask.Run(action_time);
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
            string WorkPath = SavePath + (ID ?? Guid.NewGuid().ToString()) + Path.DirectorySeparatorChar;
            WorkPath.CreateDirectory();
            long Length = PreRequest(this, out bool cSeek, out var Disposition);
            FileName ??= Uri.FileName(Disposition);
            SaveFullName = SavePath + FileName;
            var files = new List<FilesResult>();
            TotalCount = DownCount = 0;

            #region 任务分配

            long _down_length = Length;
            int _taskcount = 1;
            if (cSeek && Length > 0 && Length > 2097152)
            {
                _down_length = 2097152;
                _taskcount = (int)Math.Ceiling(Length / (_down_length * 1.0));//任务分块
            }

            List<long> ValTmp = new List<long>(_taskcount), MaxTmp = new List<long>(_taskcount);
            for (int i = 0; i < _taskcount; i++)
            {
                long _s = _down_length * i;
                long _e = _down_length;
                if ((_s + _down_length) > Length) _e = _down_length - ((_s + _down_length) - Length);

                string filename_temp = $"{i}_{_s}_{_s + _e}.temp";
                files.Add(new FilesResult(i, WorkPath + filename_temp, _s, _e));
                ValTmp.Add(0);
                MaxTmp.Add(_e);
            }
            ValTemp = ValTmp.ToArray();
            MaxValTemp = MaxTmp.ToArray();

            SetMaxValue();

            #endregion

            return DownCore(FileName, WorkPath, Length, cSeek, files, TaskCount);
        }

        string? DownCore(string fileName, string WorkPath, long Length, bool cSeek, List<FilesResult> files, int taskCount)
        {
            core.range();
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
                tasks.Add(ITask.Run(() =>
                {
                    DownOne(it, cSeek, ref isStop);
                }));
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
                var _files = new List<string>(files.Count);
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

        void DownOne(FilesResult item, bool can_range, ref bool is_stop)
        {
            long _downvalueTemp = 0;
            bool runTask = true;
            int ErrCount = 0;
            while (runTask)
            {
                try
                {
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
                        else if (!can_range)
                        {
                            file.Close();
                            fileLong = 0;
                            File.Delete(item.path);
                        }
                    }

                    using (var file = new FileStream(item.path, FileMode.OpenOrCreate))
                    {
                        var request = core.CreateRequest();
                        if (can_range) request.AddRange((long)(item.start_position + file.Length), (long)(item.start_position + item.end_position));
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
                                    if (!can_range) file.Seek(0, SeekOrigin.Begin);
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
                                    SetValue();
                                }
                                byte[] _cache = new byte[CacheSize];
                                int osize = stream.Read(_cache, 0, _cache.Length);
                                while (osize > 0)
                                {
                                    _downvalueTemp += osize;
                                    ValTemp[item.i] = _downvalueTemp;
                                    SetValue();
                                    try
                                    {
                                        resetState.WaitOne();
                                    }
                                    catch
                                    {
                                        //下载终止
                                        is_stop = true;
                                        return;
                                    }
                                    file.Write(_cache, 0, osize);
                                    osize = stream.Read(_cache, 0, _cache.Length);
                                }

                                ValTemp[item.i] = _downvalueTemp;
                                SetValue();
                            }
                        }
                    }
                    item.complete = true;
                    return;
                }
                catch
                {
                    ErrCount++;
                    _downvalueTemp = 0;
                    if (ErrCount > RetryCount) runTask = false;
                }
            }
        }

        /// <summary>
        /// 预请求
        /// </summary>
        /// <param name="core"></param>
        /// <param name="can_range">是否可以分段</param>
        /// <param name="disposition"></param>
        /// <returns>真实长度</returns>
        static long PreRequest(HttpDown core, out bool can_range, out string? disposition)
        {
            disposition = null;
            try
            {
                core.core.range();
                var request = core.core.requestNone();
                if (request.Header.ContainsKey("Content-Disposition")) disposition = request.Header["Content-Disposition"];
                var ReadLength = request.Size;
                if (ReadLength > 0)
                {
                    try
                    {
                        core.core.range(1, ReadLength - 1);
                        var request2 = core.core.requestNone();
                        long length = request2.Size;
                        can_range = length == ReadLength - 1;
                    }
                    catch
                    {
                        can_range = false;
                    }
                }
                else can_range = false;
                return ReadLength;
            }
            catch
            {
                can_range = false;
                return 0;
            }
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