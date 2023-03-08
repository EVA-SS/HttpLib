using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLib
{
    public class HttpCore
    {
        public HttpOption option;
        public HttpCore(string uri, HttpMethod method)
        {
            option = new HttpOption(uri, method);
        }
        public HttpCore(Uri uri, HttpMethod method)
        {
            option = new HttpOption(uri, method);
        }
        public HttpCore(HttpOption _option)
        {
            option = _option;
        }

        #region 请求参(GET)

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(params Val[] vals)
        {
            option.query ??= new List<Val>(vals.Length);
            foreach (var val in vals)
                HttpCoreLib.AddVal(ref option.query, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(List<Val> vals)
        {
            option.query ??= new List<Val>(vals.Count);
            foreach (var val in vals)
                HttpCoreLib.AddVal(ref option.query, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore query(string key, string val)
        {
            option.query ??= new List<Val>();
            HttpCoreLib.AddVal(ref option.query, key, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(IDictionary<string, string> vals)
        {
            option.query ??= new List<Val>(vals.Count);
            foreach (var item in vals)
                HttpCoreLib.AddVal(ref option.query, item.Key, item.Value);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore query(object obj)
        {
            var data = obj.ToDictionary();
            if (data.Count > 0)
            {
                option.query ??= new List<Val>(data.Count);
                foreach (var item in data)
                {
                    if (item.Value != null) HttpCoreLib.AddVal(ref option.query, item.Key, item.Value.ToString());
                }
            }
            return this;
        }

        #endregion

        #region 请求参

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="val">多个参数</param>
        public HttpCore data(params Val[] vals)
        {
            option.data ??= new List<Val>(vals.Length);
            foreach (var val in vals)
                HttpCoreLib.AddVal(ref option.data, val);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(List<Val> vals)
        {
            option.data ??= new List<Val>(vals.Count);
            foreach (var val in vals)
                HttpCoreLib.AddVal(ref option.data, val);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore data(string key, string val)
        {
            option.data ??= new List<Val>();
            HttpCoreLib.AddVal(ref option.data, key, val);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, string> vals)
        {
            option.data ??= new List<Val>(vals.Count);
            foreach (var val in vals)
                HttpCoreLib.AddVal(ref option.data, val.Key, val.Value);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, string[]> vals)
        {
            if (option.method == HttpMethod.Get)
            {
                throw new Exception("GET不支持数组");
            }
            option.data ??= new List<Val>(vals.Count);
            foreach (var val in vals)
                foreach (var items in val.Value)
                    HttpCoreLib.AddVal(ref option.data, val.Key + "[]", items);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, List<string>> vals)
        {
            if (option.method == HttpMethod.Get)
            {
                throw new Exception("GET不支持数组");
            }
            option.data ??= new List<Val>(vals.Count);
            foreach (var val in vals)
                foreach (var items in val.Value)
                    HttpCoreLib.AddVal(ref option.data, val.Key + "[]", items);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="val">text/plain</param>
        public HttpCore data(string val)
        {
            option.datastr = val;
            return this;
        }
        public HttpCore datastr(string val, string contentType = "text/plain")
        {
            option.datastr = val;
            header("Content-Type", contentType);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore data(object obj)
        {
            var data = obj.ToDictionary();
            if (data.Count > 0)
            {
                option.data ??= new List<Val>(data.Count);
                foreach (var item in data)
                {
                    if (item.Value != null)
                    {
                        if (item.Value is string)
                            HttpCoreLib.AddVal(ref option.data, item.Key, item.Value.ToString());
                        else if (item.Value is Files file)
                            this.data(file);
                        else if (item.Value is List<Files> files)
                            this.data(files);
                        else if (item.Value is Files[] files2)
                            this.data(files2);
                        else if (item.Value is IEnumerable list)
                        {
                            if (option.method == HttpMethod.Get)
                            {
                                var listval = new List<string?>();
                                foreach (var es in list)
                                {
                                    if (es != null) listval.Add(es.ToString());
                                }
                                option.query ??= new List<Val>(listval.Count);
                                HttpCoreLib.AddVal(ref option.query, item.Key, string.Join(",", listval));
                            }
                            else
                            {
                                foreach (var es in list)
                                {
                                    if (es != null) HttpCoreLib.AddVal(ref option.data, item.Key + "[]", es.ToString());
                                }
                            }
                        }
                        else
                            HttpCoreLib.AddVal(ref option.data, item.Key, item.Value.ToString());
                    }
                }
            }
            return this;
        }

        #region 文件

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore data(params Files[] vals)
        {
            option.file ??= new List<Files>(vals.Length);
            option.file.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore data(List<Files> vals)
        {
            option.file ??= new List<Files>(vals.Count);
            option.file.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore file(params string[] vals)
        {
            option.file ??= new List<Files>(vals.Length);
            foreach (var item in vals)
            {
                option.file.Add(new Files(item));
            }
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore file(params Files[] vals)
        {
            option.file ??= new List<Files>(vals.Length);
            option.file.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore file(List<Files> vals)
        {
            option.file ??= new List<Files>(vals.Count);
            option.file.AddRange(vals);
            return this;
        }

        #endregion

        #endregion

        #region 请求头

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public HttpCore header(params Val[] vals)
        {
            option.header ??= new List<Val>(vals.Length);
            foreach (var item in vals)
                HttpCoreLib.AddVal(ref option.header, item);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public HttpCore header(List<Val> vals)
        {
            option.header ??= new List<Val>(vals.Count);
            foreach (var item in vals)
                HttpCoreLib.AddVal(ref option.header, item);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore header(string key, string val)
        {
            option.header ??= new List<Val>();
            HttpCoreLib.AddVal(ref option.header, key, val);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public HttpCore header(IDictionary<string, string> vals)
        {
            option.header ??= new List<Val>(vals.Count);
            foreach (var item in vals)
                HttpCoreLib.AddVal(ref option.header, item.Key, item.Value);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="data">多个请求头</param>
        public HttpCore header(object obj)
        {
            var data = obj.ToDictionary();
            if (data.Count > 0)
            {
                option.header ??= new List<Val>(data.Count);
                foreach (var item in data)
                {
                    if (item.Value != null) HttpCoreLib.AddVal(ref option.header, item.Key, item.Value.ToString());
                }
            }
            return this;
        }

        #endregion

        #region 回调

        #region 上传进度的回调

        Action<long, long?>? request_progres = null;
        Action<int>? request_progres_percent = null;

        /// <summary>
        /// 上传进度的回调函数
        /// </summary>
        public HttpCore requestProgres(Action<int> action)
        {
            request_progres_percent = action;
            return this;
        }

        /// <summary>
        /// 上传进度的回调函数
        /// </summary>
        public HttpCore requestProgres(Action<long, long?> action)
        {
            request_progres = action;
            return this;
        }

        #endregion

        #region 下载进度的回调

        private Action<long, long?>? response_progres = null;
        private Action<int>? response_progres_percent = null;

        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgres(Action<int> action)
        {
            response_progres_percent = action;
            return this;
        }

        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgres(Action<long, long?> action)
        {
            response_progres = action;
            return this;
        }

        #endregion

        #endregion

        #region 编码

        public HttpCore encoding(string encoding)
        {
            option.encoding = Encoding.GetEncoding(encoding);
            return this;
        }
        public HttpCore encoding(Encoding encoding)
        {
            option.encoding = encoding;
            return this;
        }

        #endregion

        #region 重定向

        /// <summary>
        /// 设置请求重定向
        /// </summary>
        /// <param name="val">true启用重定向，false禁用重定向</param>
        public HttpCore redirect(bool val)
        {
            option.redirect = val;
            return this;
        }

        #endregion

        #region 超时

        /// <summary>
        /// 设置请求超时时长
        /// </summary>
        /// <param name="time">毫秒</param>
        public HttpCore timeout(int time)
        {
            option.timeout = time;
            return this;
        }

        #endregion

        #region 缓存

        CacheModel? _cache = null;
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="id">缓存id</param>
        public HttpCore cache(string id)
        {
            _cache = new CacheModel(id);
            return this;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="id">缓存id</param>
        /// <param name="time">有效期 分钟</param>
        public HttpCore cache(string id, int time)
        {
            _cache = new CacheModel(id) { t = time };
            return this;
        }

        class CacheModel
        {
            public CacheModel(string _id)
            {
                if (Config.CacheFolder == null) throw new Exception("先配置\"Config.CachePath\"");
                path = Config.CacheFolder;
                id = _id;
                file = path + _id;
            }
            public string id { get; set; }
            public int t = 0;
            public string path { get; set; }
            public string file { get; set; }
        }

        #endregion

        #region 请求

        private Func<HttpCore, bool>? request_before = null;
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(Func<HttpCore, bool> action)
        {
            request_before = action;
            return this;
        }

        private Func<HttpCore, HttpResponseMessage, bool>? request_after = null;

        /// <summary>
        /// 请求之后处理
        /// </summary>
        /// <param name="action">请求之后处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore after(Func<HttpCore, HttpResponseMessage, bool> action)
        {
            request_after = action;
            return this;
        }

        #endregion

        #region 终止

        public void abort()
        {
            if (cancellationToken != null) { cancellationToken.Cancel(); cancellationToken.Dispose(); cancellationToken = null; }
        }

        #endregion

        #region 请求核心

        /// <summary>
        /// 空请求
        /// </summary>
        /// <returns>不下载流</returns>
        public async Task<WebResult> requestNone()
        {
            TaskResult val = await Go(new ReMode { Mode = ReModeCode.NONE });
            return val.Web;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public async Task<WebResultData<string>> download(string savePath, string? saveName = null)
        {
            var val = await Go(new ReMode { Mode = ReModeCode.DOWN, SavePath = savePath, SaveName = saveName });
            return new WebResultData<string>(val.Web) { Data = val.Str };
        }

        /// <summary>
        /// 请求字节
        /// </summary>
        /// <returns>字节类型</returns>
        public async Task<WebResultData<byte[]>> requestData()
        {
            var val = await Go(new ReMode { Mode = ReModeCode.BYTE });
            return new WebResultData<byte[]>(val.Web) { Data = val.Data };
        }

        /// <summary>
        /// 请求字符串
        /// </summary>
        /// <returns>字符串类型</returns>
        public async Task<WebResultData<string>> request()
        {
            var val = await Go(new ReMode { Mode = ReModeCode.STR });
            return new WebResultData<string>(val.Web) { Data = val.Str };
        }

        #region 对象

        private CancellationTokenSource? cancellationToken;

        #endregion

        private async Task<TaskResult> Go(ReMode mode)
        {
            try
            {
                abort();
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
                                switch (mode.Mode)
                                {
                                    case ReModeCode.STR:
                                        return new TaskResult(new WebResult { OK = true, StatusCode = HttpStatusCode.OK }, File.ReadAllText(_cache.file));
                                    case ReModeCode.BYTE:
                                        return new TaskResult(new WebResult { OK = true, StatusCode = HttpStatusCode.OK }, File.ReadAllBytes(_cache.file));
                                }
                            }
                        }
                        else
                        {
                            switch (mode.Mode)
                            {
                                case ReModeCode.STR:
                                    return new TaskResult(new WebResult { OK = true, StatusCode = HttpStatusCode.OK }, File.ReadAllText(_cache.file));
                                case ReModeCode.BYTE:
                                    return new TaskResult(new WebResult { OK = true, StatusCode = HttpStatusCode.OK }, File.ReadAllBytes(_cache.file));
                            }
                        }
                    }
                }
                var uri = option.Url;
                CookieContainer cookies = new CookieContainer();
                using (var handler = new HttpClientHandler())
                {
                    #region SSL

                    handler.ServerCertificateCustomValidationCallback = (_s, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };

                    #endregion

                    handler.AutomaticDecompression = Config.DecompressionMethod;
                    handler.CookieContainer = cookies;
                    if (option.redirect)
                        handler.AllowAutoRedirect = option.redirect;
                    else
                        handler.AllowAutoRedirect = Config.Redirect;

                    using (var httpRequest = new HttpRequestMessage(option.method, uri))
                    {
                        httpRequest.Headers.Host = uri.Host;
                        SetHeader(httpRequest, "User-Agent", Config.UserAgent);
                        Encoding encoding = option.encoding ?? Encoding.UTF8;

                        string? ContentTypeStr = null;
                        if (Config.header != null && Config.header.Count > 0)
                            SetHeader(ref ContentTypeStr, httpRequest, Config.header, uri, cookies);
                        if (option.header != null && option.header.Count > 0)
                            SetHeader(ref ContentTypeStr, httpRequest, option.header, uri, cookies);

                        HttpClient httpClient;
                        if (request_progres == null && request_progres_percent == null && response_progres == null && response_progres_percent == null)
                        {
                            if (Config.UsePool)
                                httpClient = HttpClientFactory.Create(handler);//池
                            else
                                httpClient = new HttpClient(handler);
                        }
                        else
                        {
                            #region 注入进度

                            var progressMessageHandler = new ProgressMessageHandler(handler);
                            if (request_progres != null)
                            {
                                progressMessageHandler.HttpSendProgress += (sender, re) =>
                                {
                                    request_progres(re.BytesTransferred, re.TotalBytes);
                                };
                            }
                            else if (request_progres_percent != null)
                            {
                                progressMessageHandler.HttpSendProgress += (sender, re) =>
                                {
                                    request_progres_percent(re.ProgressPercentage);
                                };
                            }
                            if (response_progres != null)
                            {
                                progressMessageHandler.HttpReceiveProgress += (sender, re) =>
                                {
                                    response_progres(re.BytesTransferred, re.TotalBytes);
                                };
                            }
                            else if (response_progres_percent != null)
                            {
                                progressMessageHandler.HttpReceiveProgress += (sender, re) =>
                                {
                                    response_progres_percent(re.ProgressPercentage);
                                };
                            }

                            #endregion

                            httpClient = new HttpClient(progressMessageHandler);
                        }

                        using (httpClient)
                        {
                            if (option.timeout > 0)
                                httpClient.Timeout = TimeSpan.FromMilliseconds(option.timeout);

                            #region 准备上传数据

                            if (option.method != HttpMethod.Get && option.method != HttpMethod.Head)
                            {
                                if (!string.IsNullOrEmpty(option.datastr))
                                {
                                    if (ContentTypeStr == null)
                                        httpRequest.Content = new StringContent(option.datastr, encoding);
                                    else
                                        httpRequest.Content = new StringContent(option.datastr, encoding, ContentTypeStr);
                                }
                                else if (option.file != null && option.file.Count > 0)
                                {
                                    string boundary = RandomString(8);

                                    var Content = new MultipartFormDataContent(boundary);
                                    foreach (var file in option.file)
                                    {
                                        var fileByteArrayContent = new StreamContent(file.Stream);
                                        fileByteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                        fileByteArrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                        {
                                            Name = file.Name, //接口匹配name
                                            FileName = file.FileName //附件文件名
                                        };
                                        Content.Add(fileByteArrayContent);
                                    }
                                    if (option.data != null && option.data.Count > 0)
                                    {
                                        foreach (var item in option.data)
                                        {
                                            if (item.Value != null)
                                            {
                                                //var valueBytes = encoding.GetBytes(item.Value);
                                                //var byteArray = new ByteArrayContent(valueBytes);
                                                var stringContent = new StringContent(item.Value);
                                                stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                {
                                                    Name = item.Key
                                                };
                                                Content.Add(stringContent);
                                            }
                                        }
                                    }
                                    httpRequest.Content = Content;
                                }
                                else if (option.data != null && option.data.Count > 0)
                                {
                                    var dir = new Dictionary<string, string>(option.data.Count);
                                    foreach (var item in option.data)
                                    {
                                        if (item.Value != null)
                                        {
                                            if (dir.ContainsKey(item.Key)) dir[item.Key] = item.Value;
                                            else dir.Add(item.Key, item.Value);
                                        }
                                    }
                                    httpRequest.Content = new FormUrlEncodedContent(dir);
                                }
                            }

                            #endregion

                            if (request_before != null)
                                if (!request_before(this))
                                    return new TaskResult(new Exception("主动取消"));

                            cancellationToken = new CancellationTokenSource();
                            var responseMessage = await httpClient.SendAsync(httpRequest, cancellationToken.Token);
                            var _web = GetWebResult(responseMessage);

                            if (request_after != null)
                                if (!request_after(this, responseMessage))
                                    return new TaskResult(_web, new Exception("主动取消"));

                            switch (mode.Mode)
                            {
                                case ReModeCode.STR:
                                    {
                                        var result = await responseMessage.Content.ReadAsStringAsync();
                                        if (result != null)
                                        {
                                            if (_cache != null)
                                            {
                                                _cache.path.CreateDirectory();
                                                File.WriteAllText(_cache.file, result);
                                            }
                                            return new TaskResult(_web, result);
                                        }
                                        return new TaskResult(_web);
                                    }
                                case ReModeCode.BYTE:
                                    {
                                        var result = await responseMessage.Content.ReadAsByteArrayAsync();
                                        if (result != null)
                                        {
                                            if (_cache != null)
                                            {
                                                _cache.path.CreateDirectory();
                                                File.WriteAllBytes(_cache.file, result);
                                            }
                                            return new TaskResult(_web, result);
                                        }
                                        return new TaskResult(_web);
                                    }
                                case ReModeCode.DOWN:
                                    {
                                        if (mode.SavePath == null) return new TaskResult(_web);
                                        if (!Directory.Exists(mode.SavePath)) Directory.CreateDirectory(mode.SavePath);
                                        if (!mode.SavePath.EndsWith("\\") || !mode.SavePath.EndsWith("/"))
                                        {
                                            if (!mode.SavePath.EndsWith("\\"))
                                            {
                                                mode.SavePath += "\\";
                                            }
                                            else
                                            {
                                                mode.SavePath += "/";
                                            }
                                        }
                                        mode.SaveName ??= FileName(responseMessage.Content);
                                        var outfile = mode.SavePath + mode.SaveName;
                                        using (var fileStream = new FileStream(outfile, FileMode.Create, FileAccess.Write))
                                        {
                                            using var stream = await responseMessage.Content.ReadAsStreamAsync();
                                            byte[] buffer = new byte[Config.CacheSize];
                                            int length;
                                            while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                // 写入到文件
                                                fileStream.Write(buffer, 0, length);
                                            }
                                        }
                                        return new TaskResult(_web, outfile);
                                    }
                                default: return new TaskResult(_web);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                return new TaskResult(err);
            }
        }
        private class TaskResult
        {
            public TaskResult(WebResult web)
            {
                Web = web;
            }
            public TaskResult(WebResult web, byte[] _data)
            {
                Web = web;
                Data = _data;
            }
            public TaskResult(WebResult web, string _str)
            {
                Web = web;
                Str = _str;
            }
            public TaskResult(WebResult web, Exception err)
            {
                Web = new WebResult(web, err);
            }
            public TaskResult(Exception err)
            {
                Web = new WebResult(err);
            }
            public WebResult Web { get; set; }
            public string? Str { get; set; }
            public byte[]? Data { get; set; }
        }
        private class ReMode
        {
            public ReModeCode Mode { get; set; }
            public string? SavePath { get; set; }
            public string? SaveName { get; set; }
        }

        #region 请求头-帮助

        private static void SetHeader(ref string? ContentTypeStr, HttpRequestMessage req, List<Val> headers, Uri uri, CookieContainer cookies)
        {
            foreach (var item in headers)
            {
                if (item.Value != null)
                {
                    string _Lower_Name = item.Key.ToLower();
                    if (_Lower_Name == "cookie")
                    {
                        string _cookie = item.Value;
                        if (_cookie.Contains(';'))
                        {
                            string[] arrCookie = _cookie.Split(';');
                            foreach (string sCookie in arrCookie)
                            {
                                if (string.IsNullOrEmpty(sCookie) || sCookie.IndexOf("expires") > 0) continue;
                                cookies.SetCookies(uri, sCookie);
                            }
                        }
                        else
                        {
                            cookies.SetCookies(uri, _cookie);
                        }
                    }
                    else
                    {
                        if (_Lower_Name == "content-type") ContentTypeStr = item.Value;
                        else
                        {
                            if (req.Headers.Contains(item.Key)) req.Headers.Remove(item.Key);
                            req.Headers.Add(item.Key, item.Value);
                        }
                    }
                }
            }
        }
        private static void SetHeader(HttpRequestMessage req, string key, string? val)
        {
            if (req.Headers.Contains(key)) req.Headers.Remove(key);
            if (val != null) req.Headers.Add(key, val);
        }

        #endregion

        #region 请求流-帮助

        private static string RandomString(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[length];

            Random rd = new Random();

            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        #endregion

        #region 响应流-帮助

        /// <summary>
        /// 获取域名IP
        /// </summary>
        public string? IP
        {
            get
            {
                return option.IP;
            }
        }

        private static WebResult GetWebResult(HttpResponseMessage response)
        {
            var _web = new WebResult
            {
                Uri = response.RequestMessage.RequestUri,
                OK = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Header = new Dictionary<string, string>(response.Headers.Count())
            };
            if (response.Content != null)
            {
                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType != null)
                    _web.Type = response.Content.Headers.ContentType.MediaType;
                _web.HeaderContent = new Dictionary<string, string>(response.Content.Headers.Count());
                foreach (var item in response.Content.Headers)
                {
                    _web.HeaderContent.Add(item.Key, string.Join("; ", item.Value));
                }
            }
            foreach (var item in response.Headers)
            {
                _web.Header.Add(item.Key, string.Join("; ", item.Value));
            }

            return _web;
        }
        public string FileName(HttpContent content)
        {
            if (content.Headers.ContentDisposition != null && content.Headers.ContentDisposition.FileName != null)
            {
                return content.Headers.ContentDisposition.FileName;
            }
            return Path.GetFileName(option.uri.AbsoluteUri);
        }

        #endregion

        #endregion

        public override string ToString()
        {
            return option.uri.AbsoluteUri;
        }
    }

    enum ReModeCode
    {
        /// <summary>
        /// 仅请求
        /// </summary>
        NONE,
        /// <summary>
        /// 返回字符串
        /// </summary>
        STR,
        /// <summary>
        /// 返回字节流
        /// </summary>
        BYTE,
        /// <summary>
        /// 下载文件
        /// </summary>
        DOWN,
    }

    public static class HttpCoreLib
    {
        public static void CreateDirectory(this string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
        public static void AddVal(ref List<Val> obj, Val val)
        {
            AddVal(ref obj, val.Key, val.Value);
        }

        public static void AddVal(ref List<Val> obj, string key, string? val)
        {
            var find = obj.Find(ab => ab.Key == key);
            if (find == null) obj.Add(new Val(key, val));
            else
            {
                if (val != null) find.SetValue(val);
                else obj.Remove(find);
            }
        }

        public static string ToString(this Dictionary<string, string> vals)
        {
            return ToString(vals, Environment.NewLine);
        }
        public static string ToString(this Dictionary<string, string> vals, string sp)
        {
            var strs = new List<string>();
            foreach (var item in vals)
            {
                strs.Add(item.Key + "=" + item.Value);
            }
            return string.Join(sp, strs);
        }
        public static string ToString(this List<Val> vals)
        {
            return ToString(vals, Environment.NewLine);
        }
        public static string ToString(this List<Val> vals, string sp)
        {
            var strs = new List<string>();
            foreach (var item in vals)
            {
                strs.Add(item.ToString());
            }
            return string.Join(sp, strs);
        }
    }
}
