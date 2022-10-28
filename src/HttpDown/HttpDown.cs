using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public Task<string> Go()
        { return Go(Environment.ProcessorCount, null); }

        /// <summary>
        /// 下载-自定义下载线程数
        /// </summary>
        public Task<string> Go(int TaskCount)
        { return Go(TaskCount, null); }

        /// <summary>
        /// 下载-自定义保存文件名称
        /// </summary>
        /// <param name="FileName"></param>
        public Task<string> Go(string FileName)
        { return Go(Environment.ProcessorCount, FileName); }

        #endregion

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="TaskCount">线程数</param>
        /// <param name="FileName">文件名称</param>
        public Task<string> Go(int TaskCount, string FileName)
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

        string DownCore(int TaskCount, string FileName)
        {
            //(option.Uri.AbsoluteUri).Md5_16()
            string WorkPath = SavePath + (ID == null ? Guid.NewGuid().ToString() : ID) + Path.DirectorySeparatorChar;
            WorkPath.CreateDirectory();
            long Length = this.WebGet(Uri, out bool cSeek, out string Disposition);
            if (FileName == null) FileName = GetFileName(Disposition);
            SaveFullName = SavePath + FileName;
            var files = new List<FilesResult>();
            ValTemp.Clear(); MaxValTemp.Clear();
            TotalCount = DownCount = 0;

            #region 任务分配

            if (Uri.AbsoluteUri.ToLower().EndsWith("m3u8"))
            {
                if (FileName.ToLower().EndsWith("m3u8")) FileName = FileName.Substring(0, FileName.Length - 4) + "mp4";
                //M3u8下载
                string m3u8Content;
                if (core != null) m3u8Content = core.request();
                else m3u8Content = Uri.GetWebSource(UserAgent);
                if (!string.IsNullOrEmpty(m3u8Content))
                {
                    #region 解析M3u8

                    string[] m3u8CurrentKey = new string[] { "NONE", "", "" };
                    File.WriteAllText(Path.Combine(WorkPath, "raw.m3u8"), m3u8Content);
                    string BaseUrl = null;
                    if (new Regex("#YUMING\\|(.*)").IsMatch(m3u8Content))
                        BaseUrl = new Regex("#YUMING\\|(.*)").Match(m3u8Content).Groups[1].Value;
                    else
                        BaseUrl = Url;

                    //存放分部的所有信息(#EXT-X-DISCONTINUITY)
                    var parts = new List<List<HLSSegments>>();
                    //存放分片的所有信息
                    var segments = new List<HLSSegments>();
                    var segInfo = new HLSSegments();
                    bool isM3u = false, isAd = false, hasAd = false, isEndlist = false, expectSegment = false, isIFramesOnly = false, isIndependentSegments = false, DelAd = false;
                    long segIndex = 0;
                    long startIndex = 0;
                    int targetDuration = 0;
                    double totalDuration = 0;

                    using (var sr = new StringReader(m3u8Content))
                    {
                        string line;
                        double segDuration = 0;
                        string segUrl = string.Empty;
                        //#EXT-X-BYTERANGE:<n>[@<o>]
                        long expectByte = -1; //parm n
                        long startByte = 0;  //parm o

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;
                            if (line.StartsWith(HLSTags.ext_m3u))
                                isM3u = true;
                            //只下载部分字节
                            else if (line.StartsWith(HLSTags.ext_x_byterange))
                            {
                                string[] t = line.Replace(HLSTags.ext_x_byterange + ":", "").Split('@');
                                if (t.Length > 0)
                                {
                                    if (t.Length == 1)
                                    {
                                        expectByte = Convert.ToInt64(t[0]);
                                        segInfo.expectByte = expectByte;
                                    }
                                    if (t.Length == 2)
                                    {
                                        expectByte = Convert.ToInt64(t[0]);
                                        startByte = Convert.ToInt64(t[1]);
                                        segInfo.expectByte = expectByte;
                                        segInfo.startByte = startByte;
                                    }
                                }
                                expectSegment = true;
                            }
                            //国家地理去广告
                            else if (line.StartsWith("#UPLYNK-SEGMENT"))
                            {
                                if (line.Contains(",ad"))
                                    isAd = true;
                                else if (line.Contains(",segment"))
                                    isAd = false;
                            }
                            //国家地理去广告
                            else if (isAd)
                            {
                                continue;
                            }
                            //解析定义的分段长度
                            else if (line.StartsWith(HLSTags.ext_x_targetduration))
                            {
                                targetDuration = Convert.ToInt32(Convert.ToDouble(line.Replace(HLSTags.ext_x_targetduration + ":", "").Trim()));
                            }
                            //解析起始编号
                            else if (line.StartsWith(HLSTags.ext_x_media_sequence))
                            {
                                segIndex = Convert.ToInt64(line.Replace(HLSTags.ext_x_media_sequence + ":", "").Trim());
                                startIndex = segIndex;
                            }
                            else if (line.StartsWith(HLSTags.ext_x_discontinuity_sequence)) { }
                            else if (line.StartsWith(HLSTags.ext_x_program_date_time)) { }
                            //解析不连续标记，需要单独合并（timestamp不同）
                            else if (line.StartsWith(HLSTags.ext_x_discontinuity))
                            {
                                //修复优酷去除广告后的遗留问题
                                if (hasAd && parts.Count > 0)
                                {
                                    segments = parts[parts.Count - 1];
                                    parts.RemoveAt(parts.Count - 1);
                                    hasAd = false;
                                    continue;
                                }
                                //常规情况的#EXT-X-DISCONTINUITY标记，新建part
                                if (!hasAd && segments.Count > 1)
                                {
                                    parts.Add(segments);
                                    segments = new List<HLSSegments>();
                                }
                            }
                            else if (line.StartsWith(HLSTags.ext_x_cue_out)) { }
                            else if (line.StartsWith(HLSTags.ext_x_cue_out_start)) { }
                            else if (line.StartsWith(HLSTags.ext_x_cue_span)) { }
                            else if (line.StartsWith(HLSTags.ext_x_version)) { }
                            else if (line.StartsWith(HLSTags.ext_x_allow_cache)) { }
                            //解析KEY
                            else if (line.StartsWith(HLSTags.ext_x_key))
                            {
                                //m3u8CurrentKey = ParseKey(line);
                            }
                            //解析分片时长(暂时不考虑标题属性)
                            else if (line.StartsWith(HLSTags.extinf))
                            {
                                string[] tmp = line.Replace(HLSTags.extinf + ":", "").Split(',');
                                segDuration = Convert.ToDouble(tmp[0]);
                                segInfo.index = segIndex;
                                segInfo.method = m3u8CurrentKey[0];
                                //是否有加密，有的话写入KEY和IV
                                if (m3u8CurrentKey[0] != "NONE")
                                {
                                    segInfo.method = m3u8CurrentKey[1];
                                    //没有读取到IV，自己生成
                                    if (m3u8CurrentKey[2] == "")
                                        segInfo.iv = "0x" + Convert.ToString(segIndex, 2).PadLeft(32, '0');
                                    else
                                        segInfo.iv = m3u8CurrentKey[2];
                                }
                                totalDuration += segDuration;
                                segInfo.duration = segDuration;
                                expectSegment = true;
                                segIndex++;
                            }
                            //解析STREAM属性
                            else if (line.StartsWith(HLSTags.ext_x_stream_inf))
                            {
                                //        expectPlaylist = true;
                                //        string bandwidth = Global.GetTagAttribute(line, "BANDWIDTH");
                                //        string average_bandwidth = Global.GetTagAttribute(line, "AVERAGE-BANDWIDTH");
                                //        string codecs = Global.GetTagAttribute(line, "CODECS");
                                //        string resolution = Global.GetTagAttribute(line, "RESOLUTION");
                                //        string frame_rate = Global.GetTagAttribute(line, "FRAME-RATE");
                                //        string hdcp_level = Global.GetTagAttribute(line, "HDCP-LEVEL");
                                //        string audio = Global.GetTagAttribute(line, "AUDIO");
                                //        string video = Global.GetTagAttribute(line, "VIDEO");
                                //        string subtitles = Global.GetTagAttribute(line, "SUBTITLES");
                                //        string closed_captions = Global.GetTagAttribute(line, "CLOSED-CAPTIONS");
                                //        extList = new string[] { bandwidth, average_bandwidth, codecs, resolution,
                                //frame_rate,hdcp_level,audio,video,subtitles,closed_captions };
                            }
                            else if (line.StartsWith(HLSTags.ext_x_i_frame_stream_inf)) { }
                            else if (line.StartsWith(HLSTags.ext_x_media))
                            {
                                //if (Global.GetTagAttribute(line, "TYPE") == "AUDIO")
                                //    MEDIA_AUDIO.Add(Global.GetTagAttribute(line, "GROUP-ID"), CombineURL(BaseUrl, Global.GetTagAttribute(line, "URI")));
                                //if (Global.GetTagAttribute(line, "TYPE") == "SUBTITLES")
                                //{
                                //    if (!MEDIA_SUB.ContainsKey(Global.GetTagAttribute(line, "GROUP-ID")))
                                //        MEDIA_SUB.Add(Global.GetTagAttribute(line, "GROUP-ID"), CombineURL(BaseUrl, Global.GetTagAttribute(line, "URI")));
                                //}
                            }
                            else if (line.StartsWith(HLSTags.ext_x_playlist_type)) { }
                            else if (line.StartsWith(HLSTags.ext_i_frames_only))
                            {
                                isIFramesOnly = true;
                            }
                            else if (line.StartsWith(HLSTags.ext_is_independent_segments))
                            {
                                isIndependentSegments = true;
                            }
                            //m3u8主体结束
                            else if (line.StartsWith(HLSTags.ext_x_endlist))
                            {
                                if (segments.Count > 0)
                                    parts.Add(segments);
                                segments = new List<HLSSegments>();
                                isEndlist = true;
                            }
                            //#EXT-X-MAP
                            else if (line.StartsWith(HLSTags.ext_x_map))
                            {
                            }
                            else if (line.StartsWith(HLSTags.ext_x_start)) { }
                            //评论行不解析
                            else if (line.StartsWith("#")) continue;
                            //空白行不解析
                            else if (line.StartsWith("\r\n")) continue;
                            //解析分片的地址
                            else if (expectSegment)
                            {
                                segUrl = BaseUrl.CombineURL(line);
                                if (Url.Contains("?__gda__")) segUrl += new Regex("\\?__gda__.*").Match(Url).Value;
                                segInfo.Uri = segUrl;
                                segments.Add(segInfo);
                                segInfo = new HLSSegments();
                                //优酷的广告分段则清除此分片
                                //需要注意，遇到广告说明程序对上文的#EXT-X-DISCONTINUITY做出的动作是不必要的，
                                //其实上下文是同一种编码，需要恢复到原先的part上
                                if (DelAd && segUrl.Contains("ccode=") && segUrl.Contains("/ad/") && segUrl.Contains("duration="))
                                {
                                    segments.RemoveAt(segments.Count - 1);
                                    segIndex--;
                                    hasAd = true;
                                }
                                //优酷广告(4K分辨率测试)
                                if (DelAd && segUrl.Contains("ccode=0902") && segUrl.Contains("duration="))
                                {
                                    segments.RemoveAt(segments.Count - 1);
                                    segIndex--;
                                    hasAd = true;
                                }
                                expectSegment = false;
                            }
                        }
                    }

                    if (isM3u == false)
                    {
                        SetState(DownState.Fail, "无法读取m3u8");
                        return null;
                    }

                    long countInfo = segIndex - startIndex;
                    long originalCountInfo = countInfo;
                    //jsonM3u8Info.Add("targetDuration", targetDuration);
                    //jsonM3u8Info.Add("totalDuration", totalDuration);

                    string DurStart = "", DurEnd = "";
                    long RangeStart = 0, RangeEnd = -1;
                    //根据DurRange来生成分片Range
                    if (DurStart != "" || DurEnd != "")
                    {
                        double secStart = 0;
                        double secEnd = -1;

                        if (DurEnd == "")
                        {
                            secEnd = totalDuration;
                        }

                        //时间码
                        Regex reg2 = new Regex(@"(\d+):(\d+):(\d+)");
                        if (reg2.IsMatch(DurStart))
                        {
                            int HH = Convert.ToInt32(reg2.Match(DurStart).Groups[1].Value);
                            int MM = Convert.ToInt32(reg2.Match(DurStart).Groups[2].Value);
                            int SS = Convert.ToInt32(reg2.Match(DurStart).Groups[3].Value);
                            secStart = SS + MM * 60 + HH * 60 * 60;
                        }
                        if (reg2.IsMatch(DurEnd))
                        {
                            int HH = Convert.ToInt32(reg2.Match(DurEnd).Groups[1].Value);
                            int MM = Convert.ToInt32(reg2.Match(DurEnd).Groups[2].Value);
                            int SS = Convert.ToInt32(reg2.Match(DurEnd).Groups[3].Value);
                            secEnd = SS + MM * 60 + HH * 60 * 60;
                        }

                        bool flag1 = false;
                        bool flag2 = false;
                        if (secEnd - secStart > 0)
                        {
                            double dur = 0; //当前时间
                            foreach (var part in parts)
                            {
                                foreach (var seg in part)
                                {
                                    dur += seg.duration;
                                    if (flag1 == false && dur > secStart)
                                    {
                                        RangeStart = seg.index;
                                        flag1 = true;
                                    }

                                    if (flag2 == false && dur >= secEnd)
                                    {
                                        RangeEnd = seg.index;
                                        flag2 = true;
                                    }
                                }
                            }
                        }
                    }

                    //根据Range来清除部分分片
                    if (RangeStart != 0 || RangeEnd != -1)
                    {
                        if (RangeEnd == -1)
                            RangeEnd = (int)(segIndex - startIndex - 1);
                        int newCount = 0;
                        double newTotalDuration = 0;
                        var newParts = new List<List<HLSSegments>>();
                        foreach (var part in parts)
                        {
                            var newPart = new List<HLSSegments>();
                            foreach (var seg in part)
                            {
                                if (RangeStart <= seg.index && seg.index <= RangeEnd)
                                {
                                    newPart.Add(seg);
                                    newCount++;
                                    newTotalDuration += seg.duration;
                                }
                            }
                            if (newPart.Count != 0)
                                newParts.Add(newPart);
                        }
                        parts = newParts;
                        countInfo = newCount;
                        totalDuration = newTotalDuration;
                    }

                    #endregion

                    bool isVOD = isEndlist;

                    var segCount = countInfo;
                    var oriCount = originalCountInfo; //原始分片数量

                    int total = Convert.ToInt32(segCount);
                    this.TotalCount = total;
                    int PartsCount = parts.Count;
                    string segsPadZero = string.Empty.PadRight(Convert.ToString(oriCount).Length, '0');
                    string partsPadZero = string.Empty.PadRight(Convert.ToString(parts.Count).Length, '0');
                    var TotalCount = total;
                    //点播
                    if (isVOD)
                    {
                        #region 任务分配

                        List<string> works = new List<string>();
                        int index = 0;
                        for (int i = 0; i < parts.Count; i++)
                        {
                            var part = parts[i];
                            foreach (var seg in part)
                            {
                                string path = WorkPath + "Part_" + i.ToString(partsPadZero) + Path.DirectorySeparatorChar;
                                if (!works.Contains(path))
                                {
                                    works.Add(path);
                                }
                                string filename_temp = seg.index.ToString(segsPadZero) + ".ts";
                                files.Add(new FilesResult
                                {
                                    tsurl = seg.Uri,
                                    index = index,
                                    path = path + filename_temp,
                                    start_position = seg.startByte,
                                    end_position = seg.expectByte
                                });
                                ValTemp.Add(index, 0);
                                MaxValTemp.Add(index, seg.expectByte);
                                index++;
                            }
                        }
                        foreach (var item in works)
                        {
                            item.CreateDirectory();
                        }

                        #endregion
                    }
                    else
                    {
                        SetState(DownState.Fail, "无法下载直播");
                        return null;
                    }
                }
            }
            else
            {
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
                    if ((_s + _down_length) > Length)
                    {
                        _e = _down_length - ((_s + _down_length) - Length);
                    }

                    string filename_temp = $"{i}_{_s}_{_s + _e}.temp";
                    files.Add(new FilesResult
                    {
                        index = i,
                        path = WorkPath + filename_temp,
                        start_position = _s,
                        end_position = _e
                    });
                    ValTemp.Add(i, 0);
                    MaxValTemp.Add(i, _e);
                }
            }
            SetMaxValue();

            #endregion

            var path_ = DownCore(FileName, WorkPath, Length, cSeek, files, TaskCount);
            return path_;
        }

        string DownCore(string fileName, string WorkPath, long Length, bool cSeek, List<FilesResult> files, int taskCount)
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
            if (isStop)
            {
                SetState(DownState.Complete, "主动停止");
            }
            else
            {
                List<string> _files = new List<string>();
                foreach (FilesResult item in files)
                {
                    if (!item.complete)
                    {
                        SetState(DownState.Fail, "下载不完全");
                        return null;
                    }
                    _files.Add(item.path);
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
            if (item.tsurl != null)
            {
                cSeek = false;
                uri = new Uri(item.tsurl);
            }
            long _downvalueTemp = 0;
            bool runTask = true;
            int ErrCount = 0;
            while (runTask)
            {
                try
                {
                    HttpWebRequest request = null;
                    if (core != null) core.CreateRequest(ref request);
                    else
                    {
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Proxy = null;
                        request.Host = uri.Host;
                        request.Accept = "*/*";
                        request.UserAgent = UserAgent;
                        request.Method = "GET";
                        request.Timeout = TimeOut;
                        request.ReadWriteTimeout = request.Timeout; //重要
                        request.AllowAutoRedirect = true;
                        request.KeepAlive = false;
                    }

                    long fileLong = 0;

                    using (var file = new FileStream(item.path, FileMode.OpenOrCreate))
                    {
                        fileLong = file.Length;
                        if (item.end_position > 0 && file.Length >= item.end_position)
                        {
                            item.complete = true;
                            _downvalueTemp += fileLong;
                            ValTemp[item.index] = MaxValTemp[item.index] = fileLong;
                            SetMaxValue();
                            SetValue();
                            return;
                        }
                        else if (!cSeek && item.tsurl == null)
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
                        using (HttpWebResponse p = (HttpWebResponse)request.GetResponse())
                        {
                            ErrCount = 0;
                            if (p.ContentLength > 0)
                            {
                                if (file.Length >= p.ContentLength)
                                {
                                    fileLong = p.ContentLength;
                                    item.complete = true;
                                    _downvalueTemp += fileLong;
                                    ValTemp[item.index] = MaxValTemp[item.index] = fileLong;
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
                                    MaxValTemp[item.index] = p.ContentLength;
                                    SetMaxValue();
                                }
                            }
                            using (Stream stream = p.GetResponseStream())
                            {
                                if (fileLong > 0)
                                {
                                    file.Seek(fileLong, SeekOrigin.Begin);

                                    _downvalueTemp += fileLong;
                                    ValTemp[item.index] = fileLong;
                                }
                                byte[] _cache = new byte[CacheSize];
                                int osize = stream.Read(_cache, 0, _cache.Length);
                                bool isRun = true;
                                while (isRun)
                                {
                                    if (osize > 0)
                                    {
                                        _downvalueTemp += osize;
                                        ValTemp[item.index] = _downvalueTemp;

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

                    if (ErrCount > RetryCount)
                    {
                        runTask = false;
                    }
                }
            }
        }

        #region M3u8

        class HLSSegments
        {
            public long index { get; set; }
            public long startByte { get; set; }
            public long expectByte { get; set; }
            public string method { get; set; }
            public string key { get; set; }
            public string iv { get; set; }
            public double duration { get; set; }
            public string Uri { get; set; }
        }
        class HLSTags
        {
            public static string ext_m3u = "#EXTM3U";
            public static string ext_x_targetduration = "#EXT-X-TARGETDURATION";
            public static string ext_x_media_sequence = "#EXT-X-MEDIA-SEQUENCE";
            public static string ext_x_discontinuity_sequence = "#EXT-X-DISCONTINUITY-SEQUENCE";
            public static string ext_x_program_date_time = "#EXT-X-PROGRAM-DATE-TIME";
            public static string ext_x_media = "#EXT-X-MEDIA";
            public static string ext_x_playlist_type = "#EXT-X-PLAYLIST-TYPE";
            public static string ext_x_key = "#EXT-X-KEY";
            public static string ext_x_stream_inf = "#EXT-X-STREAM-INF";
            public static string ext_x_version = "#EXT-X-VERSION";
            public static string ext_x_allow_cache = "#EXT-X-ALLOW-CACHE";
            public static string ext_x_endlist = "#EXT-X-ENDLIST";
            public static string extinf = "#EXTINF";
            public static string ext_i_frames_only = "#EXT-X-I-FRAMES-ONLY";
            public static string ext_x_byterange = "#EXT-X-BYTERANGE";
            public static string ext_x_i_frame_stream_inf = "#EXT-X-I-FRAME-STREAM-INF";
            public static string ext_x_discontinuity = "#EXT-X-DISCONTINUITY";
            public static string ext_x_cue_out_start = "#EXT-X-CUE-OUT";
            public static string ext_x_cue_out = "#EXT-X-CUE-OUT-CONT";
            public static string ext_is_independent_segments = "#EXT-X-INDEPENDENT-SEGMENTS";
            public static string ext_x_scte35 = "#EXT-OATCLS-SCTE35";
            public static string ext_x_cue_start = "#EXT-X-CUE-OUT";
            public static string ext_x_cue_end = "#EXT-X-CUE-IN";
            public static string ext_x_cue_span = "#EXT-X-CUE-SPAN";
            public static string ext_x_map = "#EXT-X-MAP";
            public static string ext_x_start = "#EXT-X-START";
        }

        #endregion

        #endregion
    }

    public static class Api
    {
        public static string GetWebSource(this Uri url, string UserAgent, string headers = "", int TimeOut = 60000)
        {
            string htmlCode = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "GET";
                    webRequest.Proxy = null;
                    webRequest.UserAgent = UserAgent;
                    webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                    webRequest.Timeout = TimeOut;  //设置超时
                    webRequest.KeepAlive = false;
                    webRequest.AllowAutoRedirect = true;  //自动跳转
                    if (url.AbsoluteUri.Contains("pcvideo") && url.AbsoluteUri.Contains(".titan.mgtv.com"))
                    {
                        webRequest.UserAgent = "";
                        if (!url.AbsoluteUri.Contains("/internettv/"))
                            webRequest.Referer = "https://player.mgtv.com/mgtv_v6_player/PlayerCore.swf";
                        webRequest.Headers.Add("Cookie", "MQGUID");
                    }
                    //添加headers
                    if (headers != "")
                    {
                        foreach (string att in headers.Split('|'))
                        {
                            try
                            {
                                if (att.Split(':')[0].ToLower() == "referer")
                                    webRequest.Referer = att.Substring(att.IndexOf(":") + 1);
                                else if (att.Split(':')[0].ToLower() == "user-agent")
                                    webRequest.UserAgent = att.Substring(att.IndexOf(":") + 1);
                                else if (att.Split(':')[0].ToLower() == "range")
                                    webRequest.AddRange(Convert.ToInt32(att.Substring(att.IndexOf(":") + 1).Split('-')[0], Convert.ToInt32(att.Substring(att.IndexOf(":") + 1).Split('-')[1])));
                                else if (att.Split(':')[0].ToLower() == "accept")
                                    webRequest.Accept = att.Substring(att.IndexOf(":") + 1);
                                else
                                    webRequest.Headers.Add(att);
                            }
                            catch (Exception e)
                            {
                                //Debug.WriteLine(e.Message);
                            }
                        }
                    }
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    if (webResponse.ContentEncoding != null
                        && webResponse.ContentEncoding.ToLower() == "gzip") //如果使用了GZip则先解压
                    {
                        using (Stream streamReceive = webResponse.GetResponseStream())
                        {
                            using (var zipStream =
                                new GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                            {
                                using (StreamReader sr = new StreamReader(zipStream, Encoding.UTF8))
                                {
                                    htmlCode = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                    else
                    {
                        using (Stream streamReceive = webResponse.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(streamReceive, Encoding.UTF8))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                    }

                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                    if (webRequest != null)
                    {
                        webRequest.Abort();
                    }
                    break;
                }
                catch (Exception e)  //捕获所有异常
                {
                    //Debug.WriteLine(e.Message);
                    Thread.Sleep(1000); //1秒后重试
                    continue;
                }
            }

            return htmlCode;
        }
        /// <summary>
        /// 文件合并函数,
        /// 可将任意个子文件合并为一个,为fileSplit()的逆过程
        /// delet标识是否删除原文件, change对data的首字节进行解密
        /// </summary>
        public static string CombineMultipleFilesIntoSingleFile(this List<string> fileIn, string filePath, string WorkPath)
        {
            using (Stream mergeFile = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter AddWriter = new BinaryWriter(mergeFile))
                {
                    //按序号排序
                    int i = 0;
                    foreach (string file in fileIn)
                    {
                        i++;
                        using (FileStream fs = new FileStream(file, FileMode.Open))
                        {
                            using (BinaryReader TempReader = new BinaryReader(fs))
                            {

                                //由于一个文件拆分成多个文件时，每个文件最后都会拼接上结尾符"\0"，导致总长度多出(n-1)个字符，需要需要针对前面(n-1)个文件去除最后的"\0"。
                                if (i == fileIn.Count)
                                {
                                    AddWriter.Write(TempReader.ReadBytes((int)fs.Length));
                                }
                                else
                                {
                                    AddWriter.Write(TempReader.ReadBytes((int)fs.Length - 1));
                                }
                            }
                        }
                    }
                }
                //删除临时文件夹
                foreach (string file in fileIn)
                {
                    File.Delete(file);
                }
                Directory.Delete(WorkPath, true);
            }
            return filePath;
        }

        public static long WebGet(this HttpDown core, Uri uri, out bool canSeek, out string Disposition)
        {
            Disposition = null;
            long _Length = 0;
            try
            {
                HttpWebRequest request = null;
                if (core.core != null) core.core.CreateRequest(ref request);
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Host = uri.Host;
                    request.Accept = "*/*";
                    request.UserAgent = Config.UserAgent;
                    request.Method = "GET";
                    request.Timeout = Config.TimeOut;
                    request.ReadWriteTimeout = request.Timeout; //重要
                    request.AllowAutoRedirect = true;
                    request.KeepAlive = false;
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Disposition = response.Headers["Content-Disposition"];
                    if (string.IsNullOrEmpty(Disposition)) Disposition = null;
                    _Length = response.ContentLength;
                }
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
                    HttpWebRequest request = null;
                    if (core.core != null) core.core.CreateRequest(ref request);
                    else
                    {
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Host = uri.Host;
                        request.Accept = "*/*";
                        request.UserAgent = Config.UserAgent;
                        request.Method = "GET";
                        request.Timeout = Config.TimeOut;
                        request.ReadWriteTimeout = request.Timeout; //重要
                        request.AllowAutoRedirect = true;
                        request.KeepAlive = false;
                    }
                    request.AddRange(1, _Length - 1);
                    using (HttpWebResponse p = (HttpWebResponse)request.GetResponse())
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
            else
            {
                canSeek = false;
            }
            return _Length;
        }

        #region M3u8

        /// <summary>
        /// 拼接Baseurl和RelativeUrl
        /// </summary>
        /// <param name="baseurl">Baseurl</param>
        /// <param name="url">RelativeUrl</param>
        /// <returns></returns>
        public static string CombineURL(this string baseurl, string url)
        {
            Uri uri1 = new Uri(baseurl);  //这里直接传完整的URL即可
            Uri uri2 = new Uri(uri1, url);
            ForceCanonicalPathAndQuery(uri2);  //兼容XP的低版本.Net
            url = uri2.ToString();
            return url;
        }
        static void ForceCanonicalPathAndQuery(Uri uri)
        {
            string paq = uri.PathAndQuery; // need to access PathAndQuery
            FieldInfo flagsFieldInfo = typeof(Uri).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            ulong flags = (ulong)flagsFieldInfo.GetValue(uri);
            flags &= ~((ulong)0x30); // Flags.PathNotCanonical|Flags.QueryNotCanonical
            flagsFieldInfo.SetValue(uri, flags);
        }

        #endregion
    }
    public class FilesResult
    {
        public int index { get; set; }
        public string tsurl { get; set; }
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
