using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLib
{
    /// <summary>
    /// 多线程下载
    /// </summary>
    public static class HttpDown
    {
        public async static Task<string?> DownLoad(this HttpCore core, HttpDownOption option)
        {
            #region 预处理

            using (var client = core.GetClient(null))
            {
                var response = await client.client.SendAsync(client.request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                if (response.Content != null && response.Content.Headers.ContentLength.HasValue)
                {
                    //有长度才可以多线程
                    long ContentLength = response.Content.Headers.ContentLength.Value;
                    if (option.name == null) option.name = HttpCore.FileName(response.Content, core);
                    response.Dispose();
                    client.Dispose();
                    return await DownLoadBefore(core, ContentLength, option, option.name);
                }
            }

            #endregion

            return null;
        }

        async static Task<string?> DownLoadBefore(HttpCore core, long ContentLength, HttpDownOption option, string save_name)
        {
            if (ContentLength > option.min_size) return await DownLoadDivide(core, ContentLength, option, save_name);
            else
            {
                if (await DownLoad(core, option, ContentLength, option.path + save_name)) return option.path + option.name;
            }
            return null;
        }
        async static Task<string?> DownLoadDivide(HttpCore core, long ContentLength, HttpDownOption option, string save_name)
        {
            int thread_count = option.thread_count ?? System.Environment.ProcessorCount;

            var tasks = new List<Task>();
            var files = new List<FileSeg>();
            string? r = null;
            bool run = true;
            if (option.progres != null)
            {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                Task.Run(() =>
                {
                    while (run)
                    {
                        System.Threading.Thread.Sleep(1000);
                        var val = new List<long>();
                        for (int i = 0; i < files.Count; i++) val.Add(files[i].prog);
                        var _val = val.Sum();
                        option.progres(_val, ContentLength);
                    }
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            }
            await Task.Run(() =>
            {
                #region 分配任务

                int taskcount = (int)Math.Ceiling(ContentLength / (option.min_size * 1.0));//任务分块

                for (int i = 0; i < taskcount; i++)
                {
                    long start = option.min_size * i, end = option.min_size;
                    if ((start + option.min_size) > ContentLength) end = option.min_size - ((start + option.min_size) - ContentLength);

                    string filename_temp = $"{i}_{start}_{start + end}.temp";
                    files.Add(new FileSeg
                    {
                        index = i,
                        path = option.path + filename_temp,
                        start = start,
                        end = end
                    });
                }

                foreach (var it in files)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        it.complete = DownLoad(core, it).Result;
                    }));
                    if (tasks.Count >= option.thread_count)
                    {
                        Task.WaitAny(tasks.ToArray());
                        tasks = tasks.Where(t => t.Status == TaskStatus.Running).ToList();
                    }
                }
                Task.WaitAll(tasks.ToArray());

                var _files = new List<string>();
                foreach (var it in files)
                {
                    if (it.complete) { _files.Add(it.path); }
                    else return;
                }
                r = _files.CombineMultipleFilesIntoSingleFile(option.path + save_name);

                #endregion
            });
            run = false;
            return r;
        }

        /// <summary>
        /// 断点续传下载
        /// </summary>
        async static Task<bool> DownLoad(HttpCore core, HttpDownOption option, long end, string full_name)
        {
            try
            {
                if (File.Exists(full_name))
                {
                    using (var fileStream = new FileStream(full_name, FileMode.Open, FileAccess.ReadWrite))
                    {
                        long file_len = fileStream.Length;
                        if (end > file_len)
                        {
                            fileStream.Seek(file_len, SeekOrigin.Begin);
                            using (var client = core.GetClient(new System.Net.Http.Headers.RangeHeaderValue(file_len, end), option.progres == null ? null : (val, max) =>
                            {
                                option.progres(file_len + val, end);
                            }))
                            {
                                var response = await client.client.SendAsync(client.request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(full_name, FileMode.Create, FileAccess.Write))
                    {
                        using (var client = core.GetClient(null, option.progres == null ? null : (val, max) =>
                        {
                            option.progres(val, end);
                        }))
                        {
                            var response = await client.client.SendAsync(client.request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                            await response.Content.CopyToAsync(fileStream);
                        }
                    }
                }
                return true;
            }
            catch { }
            return false;
        }
        async static Task<bool> DownLoad(HttpCore core, FileSeg seg)
        {
            try
            {
                if (File.Exists(seg.path))
                {
                    using (var fileStream = new FileStream(seg.path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        long file_len = fileStream.Length;
                        if (seg.end > file_len)
                        {
                            seg.prog = file_len;
                            fileStream.Seek(file_len, SeekOrigin.Begin);
                            using (var client = core.GetClient(new System.Net.Http.Headers.RangeHeaderValue(seg.start + file_len, seg.start + seg.end), (val, max) =>
                            {
                                seg.prog = file_len + val;
                            }))
                            {
                                var response = await client.client.SendAsync(client.request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(seg.path, FileMode.Create, FileAccess.Write))
                    {
                        using (var client = core.GetClient(new System.Net.Http.Headers.RangeHeaderValue(seg.start, seg.start + seg.end), (val, max) =>
                        {
                            seg.prog = val;
                        }))
                        {
                            var response = await client.client.SendAsync(client.request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                            await response.Content.CopyToAsync(fileStream);
                        }
                    }
                }
                seg.prog = seg.end;
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 文件合并函数,
        /// </summary>
        public static string CombineMultipleFilesIntoSingleFile(this List<string> fileIn, string filePath)
        {
            using (var mergeFile = new FileStream(filePath, FileMode.Create))
            {
                using (var AddWriter = new BinaryWriter(mergeFile))
                {
                    //按序号排序
                    int i = 0;
                    foreach (string file in fileIn)
                    {
                        i++;
                        using (var fs = new FileStream(file, FileMode.Open))
                        {
                            using (var TempReader = new BinaryReader(fs))
                            {
                                //由于一个文件拆分成多个文件时，每个文件最后都会拼接上结尾符"\0"，导致总长度多出(n-1)个字符，需要需要针对前面(n-1)个文件去除最后的"\0"。
                                if (i == fileIn.Count) AddWriter.Write(TempReader.ReadBytes((int)fs.Length));
                                else AddWriter.Write(TempReader.ReadBytes((int)fs.Length - 1));
                            }
                        }
                    }
                }
                //删除临时文件夹
                foreach (string file in fileIn)
                {
                    File.Delete(file);
                }
            }
            return filePath;
        }

        class FileSeg
        {
            public int index { get; set; }

            /// <summary>
            /// 文件保存地址
            /// </summary>
            public string path { get; set; }

            /// <summary>
            /// 文件开始位置
            /// </summary>
            public long start { get; set; }

            /// <summary>
            /// 文件结束位置
            /// </summary>
            public long end { get; set; }

            public long prog { get; set; }

            /// <summary>
            /// 是否下载完成
            /// </summary>
            public bool complete { get; set; }
        }
    }

    public class HttpDownOption
    {
        public HttpDownOption(string _path)
        {
            if (!_path.EndsWith("\\") || !_path.EndsWith("/"))
            {
                if (_path.EndsWith("\\")) _path += "/";
                else _path += "\\";
            }
            path = _path;
        }

        public HttpDownOption(string _path, string _name) : this(_path)
        {
            name = _name;
        }
        public HttpDownOption(string _path, string _name, int _count) : this(_path)
        {
            name = _name;
            thread_count = _count;
        }
        public HttpDownOption(string _path, int _count) : this(_path)
        {
            thread_count = _count;
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// 线程数量（默认处理器数）
        /// </summary>
        public int? thread_count { get; set; }

        /// <summary>
        /// 最小字节（默认2mb）
        /// </summary>
        public int min_size { get; set; } = 2097152;

        public Action<long, long>? progres { get; set; }
    }
}
