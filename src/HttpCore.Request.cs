﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpLib
{
    partial class HttpCore
    {
        /// <summary>
        /// 请求（不下载流）
        /// </summary>
        public ResultResponse requestNone()
        {
            var val = Go(0);
            return val.web;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public string? download(string savePath, string? saveName = null)
        {
            var val = Go(3, savePath, saveName);
            return val.str;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public string? download(out ResultResponse result, string savePath, string? saveName = null)
        {
            var val = Go(3, savePath, saveName);
            result = val.web;
            return val.str;
        }

        /// <summary>
        /// 请求（返回字节）
        /// </summary>
        /// <param name="result">响应结果</param>
        public byte[]? requestData(out ResultResponse result)
        {
            var val = Go(2);
            result = val.web;
            return val.data;
        }

        /// <summary>
        /// 请求（返回字节）
        /// </summary>
        public byte[]? requestData()
        {
            var val = Go(2);
            return val.data;
        }

        /// <summary>
        /// 请求（返回字符串）
        /// </summary>
        public string? request()
        {
            action_eventstream = null;
            var val = Go(1);
            return val.str;
        }

        /// <summary>
        /// 请求（返回字符串）
        /// </summary>
        /// <param name="result">响应结果</param>
        public string? request(out ResultResponse result)
        {
            action_eventstream = null;
            var val = Go(1);
            result = val.web;
            return val.str;
        }

        Action<string?>? action_eventstream = null;
        /// <summary>
        /// 流式请求（返回字符串）
        /// </summary>
        public string? request(Action<string?> eventstream)
        {
            action_eventstream = eventstream;
            var val = Go(1);
            return val.str;
        }

        /// <summary>
        /// 流式请求（返回字符串）
        /// </summary>
        /// <param name="result">响应结果</param>
        public string? request(Action<string?> eventstream, out ResultResponse result)
        {
            action_eventstream = eventstream;
            var val = Go(1);
            result = val.web;
            return val.str;
        }

        #region 对象

        HttpWebRequest? req;
        HttpWebResponse? response = null;

        #endregion

        TaskResult Go(int resultMode, string? savePath = null, string? saveName = null)
        {
            var r = GoCore(resultMode, savePath, saveName);
            req = null;
            response = null;
            return r;
        }
        TaskResult GoCore(int resultMode, string? savePath = null, string? saveName = null)
        {
            try
            {
                if (_cache != null)
                {
                    if (File.Exists(_cache.file))
                    {
                        if (_cache.t > 0)
                        {
                            var t = File.GetCreationTime(_cache.file);
                            var elapsedTicks = DateTime.Now.Ticks - t.Ticks;
                            var elapsedSpan = new TimeSpan(elapsedTicks);
                            if (elapsedSpan.TotalMinutes < _cache.t)
                            {
                                switch (resultMode)
                                {
                                    case 1:
                                        return new TaskResult(new ResultResponse(option.Url)
                                        {
                                            StatusCode = 200
                                        }, File.ReadAllText(_cache.file));
                                    case 2:
                                        return new TaskResult(new ResultResponse(option.Url)
                                        {
                                            StatusCode = 200
                                        }, File.ReadAllBytes(_cache.file));
                                }
                            }
                        }
                        else
                        {
                            switch (resultMode)
                            {
                                case 1:
                                    return new TaskResult(new ResultResponse(option.Url)
                                    {
                                        StatusCode = 200
                                    }, File.ReadAllText(_cache.file));
                                case 2:
                                    return new TaskResult(new ResultResponse(option.Url)
                                    {
                                        StatusCode = 200
                                    }, File.ReadAllBytes(_cache.file));
                            }
                        }
                    }
                }
                abort();
                req = CreateRequest();
                action_Request?.Invoke(req);
                using (response = (HttpWebResponse)req.GetResponse())
                {
                    var response_max = response.ContentLength;
                    Max = response_max;
                    _responseProgresMax?.Invoke(response_max);

                    var _web = new ResultResponse(response);
                    if (action_before != null && !action_before(response, _web)) return new TaskResult(_web);

                    switch (resultMode)
                    {
                        case 3:
                            using (var stream = response.GetResponseStream())
                            {
                                DownStream(response_max, _web, stream, out var outfile, savePath, saveName);
                                return new TaskResult(_web, outfile);
                            }
                        case 1:
                            if (action_eventstream == null)
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    var data = DownStream(response_max, _web, stream, out _);
                                    if (data == null) return new TaskResult(_web);
                                    else
                                    {
                                        var encodings = option.encoding;
                                        if (encodings == null || option.autoencode) encodings = GetEncoding(response, data);

                                        var result = encodings.GetString(data);
                                        if (_cache != null)
                                        {
                                            _cache.path.CreateDirectory();
                                            File.WriteAllText(_cache.file, result);
                                        }
                                        return new TaskResult(_web, result);
                                    }
                                }
                            }
                            else
                            {
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    while (!reader.EndOfStream) action_eventstream(reader.ReadLine());
                                    return new TaskResult(_web);
                                }
                            }
                        case 2:
                            using (var stream = response.GetResponseStream())
                            {
                                var result = DownStream(response_max, _web, stream, out _);
                                if (result != null && _cache != null)
                                {
                                    _cache.path.CreateDirectory();
                                    File.WriteAllBytes(_cache.file, result);
                                }
                                return new TaskResult(_web, result);
                            }
                        case 0:
                        default:
                            using (var stream = response.GetResponseStream())
                            {
                                return new TaskResult(_web);
                            }
                    }
                }
            }
            catch (Exception err)
            {
                if (err is WebException err_web && err_web.Response is HttpWebResponse response)
                {
                    var web = new ResultResponse(response, err);
                    Config.OnFail(this, web);
                    action_fail?.Invoke(web);
                    if (response.ContentLength > 0)
                    {
                        switch (resultMode)
                        {
                            case 1:
                                using (var stream = response.GetResponseStream())
                                {
                                    var data = DownStream(response.ContentLength, web, stream, out _);
                                    if (data == null) return new TaskResult(web);
                                    else
                                    {
                                        var encodings = option.encoding;
                                        if (encodings == null || option.autoencode) encodings = GetEncoding(response, data);
                                        var result = encodings.GetString(data);
                                        return new TaskResult(web, result);
                                    }
                                }
                        }
                    }
                    return new TaskResult(web);
                }
                else
                {
                    var web = new ResultResponse(option.Url, err);
                    Config.OnFail(this, web);
                    action_fail?.Invoke(web);
                    return new TaskResult(web);
                }
            }
        }

        class TaskResult
        {
            public TaskResult(ResultResponse _web)
            {
                type = 0;
                web = _web;
            }
            public TaskResult(ResultResponse _web, byte[]? _data)
            {
                type = 2;
                web = _web;
                data = _data;
            }
            public TaskResult(ResultResponse _web, string? _str)
            {
                type = 1;
                web = _web;
                str = _str;
            }
            public ResultResponse web { get; set; }
            public int type { get; set; }
            public string? str { get; set; }
            public byte[]? data { get; set; }
        }

        #region 请求头-帮助

        string GetTFName(string strItem, string replace = "-")
        {
            string strItemTarget = "";  //目标字符串
            for (int i = 0; i < strItem.Length; i++)  //strItem是原始字符串
            {
                string temp = strItem[i].ToString();
                if (Regex.IsMatch(temp, "[A-Z]")) temp = replace + temp.ToLower();
                strItemTarget += temp;
            }
            return strItemTarget;
        }
        void SetHeader(out bool isContentType, HttpWebRequest req, List<Val> headers, CookieContainer cookies)
        {
            isContentType = true;
            foreach (var it in headers)
            {
                if (it.Value != null)
                {
                    switch (it.Key.ToLower())
                    {
                        case "host":
                            req.Host = it.Value;
                            break;
                        case "accept":
                            req.Accept = it.Value;
                            break;
                        case "user-agent":
                            req.UserAgent = it.Value;
                            break;
                        case "referer":
                            req.Referer = it.Value;
                            break;
                        case "content-type":
                            isContentType = false;
                            req.ContentType = it.Value;
                            break;
                        case "cookie":
                            if (it.Value != null)
                            {
                                if (it.Value.IndexOf(";") >= 0)
                                {
                                    var Cookies = it.Value.Split(';');
                                    foreach (string cook in Cookies)
                                    {
                                        if (string.IsNullOrEmpty(cook)) continue;
                                        if (cook.IndexOf("expires") > 0) continue;
                                        cookies.SetCookies(req.RequestUri, cook);
                                    }
                                }
                                else cookies.SetCookies(req.RequestUri, it.Value);
                            }
                            break;
                        default:
                            req.Headers.Add(it.Key, it.Value);
                            break;
                    }
                }
            }
        }

        #endregion

        #region 响应流-帮助

        /// <summary>
        /// 获取域名IP
        /// </summary>
        public string? IP { get => option.IP; }

        public long Val = 0, Max = 0;

        byte[]? DownStream(long response_max, ResultResponse _web, Stream stream, out string? outfile, string? savePath = null, string? saveName = null)
        {
            Max = response_max;
            long response_val = 0;
            Val = response_val;
            Stream stream_read;
            if (savePath == null)
            {
                outfile = null;
                stream_read = new MemoryStream();
            }
            else
            {
                savePath = savePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                savePath.CreateDirectory();
                saveName ??= option.FileName(_web);
                outfile = savePath + saveName;
                stream_read = new FileStream(outfile, FileMode.Create);
            }

            using (stream_read)
            {
                byte[] buffer = new byte[Config.CacheSize];
                if (_responseProgres == null)
                {
                    int rsize = 0;
                    while ((rsize = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream_read.Write(buffer, 0, rsize);
                        response_val += rsize;
                        Val = response_val;
                    }
                }
                else
                {
                    _responseProgres(response_val, response_max);
                    int rsize = 0;
                    while ((rsize = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream_read.Write(buffer, 0, rsize);
                        response_val += rsize;
                        _responseProgres(response_val, response_max);
                        Val = response_val;
                    }
                }
                _web.OriginalSize = _web.Size = response_val;
                if (response_val > 0)
                {
                    if (savePath == null) return GetByStream(_web, stream_read);
                    else return null;
                }
                else
                {
                    if (outfile != null) File.Delete(outfile);
                    outfile = null;
                    stream_read.Close();
                    return null;
                }
            }
        }

        byte[] GetByStream(ResultResponse _web, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] _byte = new byte[stream.Length];
            stream.Read(_byte, 0, _byte.Length);
            stream.Seek(0, SeekOrigin.Begin);
            string fileclass = "";
            try
            {
                using (var r = new BinaryReader(stream))
                {
                    byte buffer = r.ReadByte();
                    fileclass = buffer.ToString();
                    buffer = r.ReadByte();
                    fileclass += buffer.ToString();
                }
            }
            catch { }

            if (fileclass == "31139")
            {
                byte[] data = Decompress(_byte);
                _web.Size = data.Length;
                return data;
            }
            else return _byte;
        }

        Encoding GetEncoding(HttpWebResponse response, byte[] data)
        {
            var meta = Regex.Match(Encoding.Default.GetString(data), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
            if (meta != null && meta.Groups.Count > 0)
            {
                var code = meta.Groups[1].Value.ToLower().Trim();
                if (code.Length > 2)
                {
                    try
                    {
                        return Encoding.GetEncoding(code.Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch { }
                }
            }
            try
            {
                if (string.IsNullOrEmpty(response.CharacterSet)) return Encoding.UTF8;
                else return Encoding.GetEncoding(response.CharacterSet);
            }
            catch { return Encoding.UTF8; }
        }

        ///  <summary> 
        /// 解压字符串
        ///  </summary> 
        ///  <param name="data"></param> 
        ///  <returns></returns> 
        byte[] Decompress(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        using (var msreader = new MemoryStream())
                        {
                            var buffer = new byte[Config.CacheSize];
                            while (true)
                            {
                                var reader = zip.Read(buffer, 0, buffer.Length);
                                if (reader <= 0)
                                {
                                    break;
                                }
                                msreader.Write(buffer, 0, reader);
                            }
                            msreader.Position = 0;
                            buffer = msreader.ToArray();
                            return buffer;
                        }
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        internal HttpWebRequest CreateRequest()
        {
            var uri = option.Url;

            var cookies = new CookieContainer();

            #region SSL

            ServicePointManager.ServerCertificateValidationCallback ??= (_s, certificate, chain, sslPolicyErrors) => { return true; };

            #endregion

            var req = (HttpWebRequest)WebRequest.Create(uri);

            if (option.proxy != null) req.Proxy = option.proxy;
            else req.Proxy = Config._proxy;
            req.Method = option.method.ToString().ToUpper();
            req.AutomaticDecompression = Config.DecompressionMethod;
            req.CookieContainer = cookies;
            req.Host = uri.Host;
            if (_range != null) req.AddRange(_range[0], _range[1]);

            if (option.redirect) req.AllowAutoRedirect = option.redirect;
            else req.AllowAutoRedirect = Config.Redirect;
            var encoding = option.encoding ?? Encoding.UTF8;
            if (option.timeout > 0) req.Timeout = option.timeout;

            req.Credentials = CredentialCache.DefaultCredentials;
            req.UserAgent = Config.UserAgent;

            bool setContentType = true;
            if (Config.headers != null && Config.headers.Count > 0) SetHeader(out setContentType, req, Config.headers, cookies);
            if (option.header != null && option.header.Count > 0) SetHeader(out setContentType, req, option.header, cookies);

            #region 准备上传数据

            if (option.method != HttpMethod.Get && option.method != HttpMethod.Head)
            {
                if (!string.IsNullOrEmpty(option.datastr))
                {
                    if (setContentType) req.ContentType = "text/plain";

                    var bs = encoding.GetBytes(option.datastr);
                    req.ContentLength = bs.Length;
                    using (var stream = req.GetRequestStream())
                    {
                        stream.Write(bs, 0, bs.Length);
                    }
                }
                else if (option.file != null && option.file.Count > 0)
                {
                    string boundary = 8.RandomString();
                    req.ContentType = "multipart/form-data; boundary=" + boundary;

                    byte[] startbyteOnes = encoding.GetBytes("--" + boundary + "\r\n"),
                        startbytes = encoding.GetBytes("\r\n--" + boundary + "\r\n"),
                        endbytes = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

                    var writeDATA = new List<byte[]?>((option.data == null ? 0 : option.data.Count * 2) + option.file.Count * 3 + 1);

                    int countB = 0;

                    #region 规划文件大小

                    if (option.data != null && option.data.Count > 0)
                    {
                        foreach (var it in option.data)
                        {
                            if (it.Value == null) continue;
                            if (countB == 0) writeDATA.Add(startbyteOnes);
                            else writeDATA.Add(startbytes);
                            countB++;
                            string separator = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", it.Key, it.Value);
                            writeDATA.Add(encoding.GetBytes(separator));
                        }
                    }

                    foreach (var file in option.file)
                    {
                        if (countB == 0) writeDATA.Add(startbyteOnes);
                        else writeDATA.Add(startbytes);
                        countB++;
                        string separator = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", file.Name, file.FileName, file.ContentType);
                        writeDATA.Add(encoding.GetBytes(separator));
                        writeDATA.Add(null);
                    }
                    writeDATA.Add(endbytes);

                    #endregion

                    long size = writeDATA.Sum(it => it != null ? it.Length : 0) + option.file.Sum(it => it.Size);
                    req.ContentLength = size;

                    #region 注入进度

                    _requestProgresMax?.Invoke(size);
                    if (_requestProgres == null)
                    {
                        using (var stream = req.GetRequestStream())
                        {
                            int fileIndex = 0;
                            foreach (var it in writeDATA)
                            {
                                if (it == null)
                                {
                                    var file = option.file[fileIndex];
                                    fileIndex++;
                                    using (file.Stream)
                                    {
                                        file.Stream.Seek(0, SeekOrigin.Begin);
                                        int bytesRead = 0;
                                        byte[] buffer = new byte[Config.CacheSize];
                                        while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            stream.Write(buffer, 0, bytesRead);
                                        }
                                    }
                                }
                                else stream.Write(it, 0, it.Length);
                            }
                        }
                    }
                    else
                    {
                        long request_value = 0;
                        _requestProgres(request_value, size);
                        using (var stream = req.GetRequestStream())
                        {
                            int fileIndex = 0;
                            foreach (var it in writeDATA)
                            {
                                if (it == null)
                                {
                                    var file = option.file[fileIndex];
                                    fileIndex++;
                                    using (file.Stream)
                                    {
                                        file.Stream.Seek(0, SeekOrigin.Begin);
                                        int bytesRead = 0;
                                        byte[] buffer = new byte[Config.CacheSize];
                                        while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            stream.Write(buffer, 0, bytesRead);
                                            request_value += bytesRead;
                                            _requestProgres(request_value, size);
                                        }
                                    }
                                }
                                else
                                {
                                    stream.Write(it, 0, it.Length);
                                    request_value += it.Length;
                                    _requestProgres(request_value, size);
                                }
                            }
                        }
                    }

                    #endregion
                }
                else if (option.data != null && option.data.Count > 0)
                {
                    if (setContentType) req.ContentType = "application/x-www-form-urlencoded";

                    var param_ = new List<string>(option.data.Count);
                    foreach (var it in option.data)
                    {
                        if (it.Value == null) continue;
                        param_.Add(it.ToStringEscape());
                    }

                    var bs = encoding.GetBytes(string.Join("&", param_));
                    req.ContentLength = bs.Length;
                    using (var stream = req.GetRequestStream())
                    {
                        stream.Write(bs, 0, bs.Length);
                    }
                }
            }

            #endregion

            return req;
        }

        #endregion
    }
}