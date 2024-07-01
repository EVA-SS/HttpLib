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
        public Task<string?> Go(int ThreadCount)
        { return Go(ThreadCount, null); }

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
        /// <param name="ThreadCount">线程数</param>
        /// <param name="FileName">文件名称</param>
        public Task<string?> Go(int ThreadCount, string? FileName)
        {
            CanSpeed = true;
#if NET40
            return Task.Factory.StartNew(() =>
            {
                return DownLoad(ThreadCount, FileName);
            });
#else
            return Task.Run(() =>
            {
                return DownLoad(ThreadCount, FileName);
            });
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

        string? DownLoad(int ThreadCount, string? FileName)
        {
            string WorkPath = SavePath + (ID ?? Guid.NewGuid().ToString()) + Path.DirectorySeparatorChar;
            WorkPath.CreateDirectory();
            long Length = HttpDownLib.PreRequest(this, ThreadCount, out bool can_range, out var disposition);
            FileName ??= Uri.FileName(disposition);
            TotalCount = DownCount = 0;

            #region 任务分配

            long SingleFileLength = Length;
            int TaskCount = 1;
            if (can_range && Length > 2097152)
            {
                SingleFileLength = 2097152;//大于2MB才做切片
                TaskCount = (int)Math.Ceiling(Length / (SingleFileLength * 1.0));//任务分块
            }

            var files = new List<FilesResult>(TaskCount);
            List<long> valTmp = new List<long>(TaskCount), maxTmp = new List<long>(TaskCount);
            for (int i = 0; i < TaskCount; i++)
            {
                long byte_start = SingleFileLength * i, byte_end = SingleFileLength;
                if ((byte_start + SingleFileLength) > Length) byte_end = SingleFileLength - ((byte_start + SingleFileLength) - Length);

                string filename_tmp = $"{i}_{byte_start}_{byte_start + byte_end}.temp";
                files.Add(new FilesResult(i, WorkPath + filename_tmp, byte_start, byte_end));
                valTmp.Add(0);
                maxTmp.Add(byte_end);
            }
            ValTmp = valTmp.ToArray();
            MaxTmp = maxTmp.ToArray();

            SetMaxValue();

            #endregion

            var option = new HttpDownOption(this, ThreadCount, FileName, SavePath, WorkPath, can_range);
            return HttpDownLib.DownLoad(option, files.ToArray());
        }

        #endregion
    }

    internal class HttpDownLib
    {
        #region 核心

        public static string? DownLoad(HttpDownOption option, FilesResult[] fileRS)
        {
            option.core.core.range();
            option.core.SetState(DownState.Downloading, null);
            option.core.CanSpeed = true;
            TestTime(option);
            bool is_stop = false;
            var tasks = new List<Task>(fileRS.Length);
            foreach (var it in fileRS)
            {
                try
                {
                    option.core.resetState.WaitOne();
                }
                catch
                {
                    is_stop = true;
                    option.core.SetState(DownState.Complete, "主动停止");
                    return null;
                }
                tasks.Add(ITask.Run(() =>
                {
                    int ErrCount = 0;
                    while (true)
                    {
                        if (DownLoadSingle(option, it, ref is_stop, out var err))
                        {
                            it.error = false;
                            it.errmsg = null;
                            return;
                        }
                        else
                        {
                            ErrCount++;
                            it.errmsg = err;
                            it.error = true;
                            if (ErrCount > option.core.RetryCount) return;
                            else Thread.Sleep(1000);
                        }
                    }
                }));
                if (tasks.Count > option.ThreadCount)
                {
                    Task.WaitAny(tasks.ToArray());
                    tasks = tasks.Where(t => t.Status == TaskStatus.Running).ToList();
                }
            }
            Task.WaitAll(tasks.ToArray());
            option.core.CanSpeed = false;
            if (is_stop) option.core.SetState(DownState.Complete, "主动停止");
            else
            {
                var errors = new List<string>(1);
                int errorcount = 0;
                foreach (var it in fileRS)
                {
                    if (it.error)
                    {
                        errorcount++;
                        if (it.errmsg != null && !errors.Contains(it.errmsg)) errors.Add(it.errmsg);
                    }
                }
                if (errorcount > 0)
                {
                    if (errors.Count > 0) option.core.SetState(DownState.Fail, string.Join(" ", errors));
                    else option.core.SetState(DownState.Fail, "下载不完全");
                    return null;
                }
                var files = new List<string>(fileRS.Length);
                foreach (var it in fileRS) files.Add(it.path);
                try
                {
                    var path = files.CombineMultipleFilesIntoSingleFile(option.SaveFullName, option.WorkPath);
                    option.core.SetState(DownState.Complete, null);
                    return path;
                }
                catch (Exception ez) { option.core.SetState(DownState.Fail, ez.Message); return null; }
            }
            return null;
        }

        static bool DownLoadSingle(HttpDownOption option, FilesResult item, ref bool is_stop, out string? error)
        {
            long DownValue = 0;
            error = null;
            try
            {
                long PreFileLength = 0;
                using (var file = new FileStream(item.path, FileMode.OpenOrCreate))
                {
                    PreFileLength = file.Length;
                    if (item.end_position > 0 && file.Length >= item.end_position)
                    {
                        DownValue += PreFileLength;
                        option.core.SetMaxValue(item.i, PreFileLength);
                        option.core.SetValue(item.i, DownValue);
                        return true;
                    }
                    else if (!option.CanRange)
                    {
                        file.Close();
                        PreFileLength = 0;
                        File.Delete(item.path);
                    }
                }

                using (var file = new FileStream(item.path, FileMode.OpenOrCreate))
                {
                    var request = option.core.core.CreateRequest();
                    if (option.CanRange) request.AddRange(item.start_position + file.Length, item.start_position + item.end_position);
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.ContentLength > 0)
                        {
                            if (file.Length >= response.ContentLength)
                            {
                                PreFileLength = response.ContentLength;
                                DownValue += PreFileLength;
                                option.core.SetMaxValue(item.i, PreFileLength);
                                option.core.SetValue(item.i, DownValue);
                                return true;
                            }
                            else
                            {
                                if (!option.CanRange) file.Seek(0, SeekOrigin.Begin);
                                option.core.SetMaxValue(item.i, response.ContentLength);
                            }
                        }
                        using (var stream = response.GetResponseStream())
                        {
                            if (PreFileLength > 0)
                            {
                                file.Seek(PreFileLength, SeekOrigin.Begin);

                                DownValue += PreFileLength;
                                option.core.SetValue(item.i, DownValue);
                            }
                            byte[] cache = new byte[option.core.CacheSize];
                            int osize = stream.Read(cache, 0, cache.Length);
                            while (osize > 0)
                            {
                                DownValue += osize;
                                option.core.SetValue(item.i, DownValue);
                                try
                                {
                                    option.core.resetState.WaitOne();
                                }
                                catch
                                {
                                    //下载终止
                                    is_stop = true;
                                    return false;
                                }
                                file.Write(cache, 0, osize);
                                osize = stream.Read(cache, 0, cache.Length);
                            }
                            option.core.SetValue(item.i, DownValue);
                        }
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                error = err.Message;
            }
            return false;
        }

        /// <summary>
        /// 计算速度
        /// </summary>
        static void TestTime(HttpDownOption option)
        {
            var tmp = new List<int>();
            long oldsize = 0;
            ITask.Run(() =>
            {
                while (option.core.State == DownState.Downloading || option.core.State == DownState.Stop)
                {
                    Thread.Sleep(1000);
                    long _downsize = option.core.Value - oldsize;
                    oldsize = option.core.Value;

                    if (_downsize > 0)
                    {
                        int se = (int)((option.core.MaxValue - oldsize) / _downsize);
                        if (se < 1)
                        {
                            option.core.SetTime(null);
                            option.core.SetSpeed(_downsize);
                        }
                        else
                        {
                            tmp.Add(se);

                            if (tmp.Count > 4)
                            {
                                int AVE = (int)Math.Ceiling(tmp.Average());
                                tmp.Clear();
                                TimeSpan timeSpan = new TimeSpan(0, 0, 0, AVE);
                                string time_txt = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";
                                if (time_txt.StartsWith("00:")) time_txt = time_txt.Substring(3);
                                option.core.SetTime(time_txt);
                            }
                            option.core.SetSpeed(_downsize);
                        }
                    }
                    else option.core.SetSpeed(0);
                }
            });
        }

        /// <summary>
        /// 预请求
        /// </summary>
        /// <param name="core"></param>
        /// <param name="can_range">是否可以分段</param>
        /// <param name="disposition"></param>
        /// <returns>真实长度</returns>
        public static long PreRequest(HttpDown core, int ThreadCount, out bool can_range, out string? disposition)
        {
            disposition = null;
            try
            {
                core.core.range();
                var request = core.core.requestNone();
                if (request.Header.ContainsKey("Content-Disposition")) disposition = request.Header["Content-Disposition"];
                var ReadLength = request.Size;
                if (ThreadCount > 1 && ReadLength > 0)
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

    internal class HttpDownOption
    {
        public HttpDownOption(HttpDown c, int threadCount, string fileName, string savePath, string workPath, bool can_range)
        {
            core = c;
            ThreadCount = threadCount;
            SaveFullName = savePath + fileName;
            WorkPath = workPath;
            CanRange = can_range;
        }

        public HttpDown core { get; set; }

        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadCount { get; set; }

        public bool CanRange { get; set; }

        public string SaveFullName { get; set; }
        public string WorkPath { get; set; }
    }

    internal class FilesResult
    {
        public FilesResult(int _i, string _path, long s, long e)
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
        public long start_position { get; set; }

        /// <summary>
        /// 文件结束位置
        /// </summary>
        public long end_position { get; set; }

        public bool error { get; set; }
        public string? errmsg { get; set; }
    }
}