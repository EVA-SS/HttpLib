using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpLib
{
    partial class HttpCore
    {
        /// <summary>
        /// 异步请求
        /// </summary>
        public Task requestAsync()
        {
#if NET40
            return Task.Factory.StartNew(requestAsyncCore);
#else
            return Task.Run(requestAsyncCore);
#endif
        }

        void requestAsyncCore()
        {
            var val = Go(resultMode);
            if (val != null && val.web != null)
            {
                switch (resultMode)
                {
                    case 0:
                        if (_success0 != null)
                        {
                            _success0(val.web);
                        }
                        break;
                    case 1:
                        if (_success1 != null)
                        {
                            _success1(val.web, val.str);
                        }
                        break;
                    case 2:
                        if (_success2 != null)
                        {
                            _success2(val.web, val.data);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>不下载流</returns>
        public WebResult? requestNone()
        {
            var val = Go(0);
            if (val != null) return val.web;
            return null;
        }

#if NET40
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public Task<string?> download(string savePath, string? saveName = null)
        {
            return Task.Factory.StartNew(() =>
            {
                var val = Go(3, savePath, saveName);
                if (val != null) return val.str;
                return null;
            });
        }
#else
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public async Task<string?> download(string savePath, string? saveName = null)
        {
            var val = Go(3, savePath, saveName);
            if (val != null) return val.str;
            return null;
        }
#endif

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public byte[]? requestData()
        {
            var val = Go(2);
            if (val != null) return val.data;
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public byte[]? requestData(out WebResult? result)
        {
            var val = Go(2);
            if (val != null)
            {
                result = val.web;
                return val.data;
            }
            result = null;
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字符串类型</returns>
        public string? request()
        {
            action_eventstream = null;
            var val = Go(1);
            if (val != null) return val.str;
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字符串类型</returns>
        public string? request(out WebResult? result)
        {
            action_eventstream = null;
            var val = Go(1);
            if (val != null)
            {
                result = val.web;
                return val.str;
            }
            result = null;
            return null;
        }

        Action<string?>? action_eventstream = null;
        /// <summary>
        /// 流式请求
        /// </summary>
        /// <returns>字符串类型</returns>
        public void request(Action<string?> eventstream)
        {
            action_eventstream = eventstream;
            Go(1);
        }

        #region 对象

        HttpWebRequest? req;
        HttpWebResponse? response = null;

        #endregion

        TaskResult? Go(int resultMode, string? savePath = null, string? saveName = null)
        {
            try
            {
                abort();
                CreateRequest(ref req);
                using (response = (HttpWebResponse)req.GetResponse())
                {
                    var response_max = response.ContentLength;
                    Max = response_max;
                    if (_responseProgresMax != null)
                        _responseProgresMax(response_max);
                    if (_requestBefore2 != null)
                        if (!_requestBefore2(response)) return null;

                    var _web = GetWebResult(response);

                    if (_requestBefore != null && !_requestBefore(response, _web)) return null;
                    if (_requestBefore3 != null && !_requestBefore3(_web)) return null;


                    switch (resultMode)
                    {
                        case 0:
                            using (var stream = response.GetResponseStream())
                            {
                                return null;
                            }
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
                                    if (data == null) return null;
                                    else
                                    {
                                        var encodings = option.encoding;
                                        if (encodings == null || option.autoencode) encodings = GetEncoding(response, data);
                                        return new TaskResult(_web, encodings.GetString(data));
                                    }
                                }
                            }
                            else
                            {
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    while (!reader.EndOfStream) action_eventstream(reader.ReadLine());
                                    return null;
                                }
                            }
                        case 2:
                            using (var stream = response.GetResponseStream())
                            {
                                return new TaskResult(_web, DownStream(response_max, _web, stream, out _));
                            }

                    }
                }
                response = null;
            }
            catch (Exception err)
            {
                if (err is WebException err_web && err_web.Response is HttpWebResponse response)
                {
                    Config.OnFail(this, GetWebResult(response), err);
                    if (_fail3 != null) _fail3(GetWebResult(response), err);
                    else if (_fail2 != null) _fail2((int)response.StatusCode, err);
                    else if (_fail != null) _fail(err);
                    return new TaskResult(GetWebResult(response));
                }
                else
                {
                    Config.OnFail(this, null, err);
                    if (_fail3 != null) _fail3(null, err);
                    else if (_fail2 != null) _fail2(-1, err);
                    else if (_fail != null) _fail(err);
                }
            }
            return null;
        }
        class TaskResult
        {
            public TaskResult(WebResult _web)
            {
                type = 0;
                web = _web;
            }
            public TaskResult(WebResult _web, byte[]? _data)
            {
                type = 2;
                web = _web;
                data = _data;
            }
            public TaskResult(WebResult _web, string? _str)
            {
                type = 1;
                web = _web;
                str = _str;
            }
            public WebResult web { get; set; }
            public int type { get; set; }
            public string? str { get; set; }
            public byte[]? data { get; set; }
        }

        #region 请求头-帮助

        private string GetTFName(string strItem, string replace = "-")
        {
            string strItemTarget = "";  //目标字符串
            for (int j = 0; j < strItem.Length; j++)  //strItem是原始字符串
            {
                string temp = strItem[j].ToString();
                if (Regex.IsMatch(temp, "[A-Z]"))
                {
                    temp = replace + temp.ToLower();
                }
                strItemTarget += temp;
            }
            return strItemTarget;
        }
        private void SetHeader(out bool isContentType, HttpWebRequest req, List<Val> headers, CookieContainer cookies)
        {
            isContentType = false;
            foreach (var item in headers)
            {
                string _Lower_Name = item.Key.ToLower();
                switch (_Lower_Name)
                {
                    case "host":
                        req.Host = item.Value;
                        break;
                    case "accept":
                        req.Accept = item.Value;
                        break;
                    case "user-agent":
                        req.UserAgent = item.Value;
                        break;
                    case "referer":
                        req.Referer = item.Value;
                        break;
                    case "content-type":
                        isContentType = true;
                        req.ContentType = item.Value;
                        break;
                    case "cookie":
                        #region 设置COOKIE
                        string _cookie = item.Value;

                        //var cookiePairs = _cookie.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //foreach (var cookiePair in cookiePairs)
                        //{
                        //    var index = cookiePair.IndexOf('=');

                        //    Cookies.Container.Add(req.RequestUri, new Cookie(cookiePair.Substring(0, index), cookiePair.Substring(index + 1, cookiePair.Length - index - 1)) { Domain = uri.Host });
                        //}
                        if (_cookie.IndexOf(";") >= 0)
                        {
                            string[] arrCookie = _cookie.Split(';');
                            //加载Cookie
                            //cookie_container.SetCookies(new Uri(url), cookie);
                            foreach (string sCookie in arrCookie)
                            {
                                if (string.IsNullOrEmpty(sCookie))
                                {
                                    continue;
                                }
                                if (sCookie.IndexOf("expires") > 0)
                                {
                                    continue;
                                }
                                cookies.SetCookies(req.RequestUri, sCookie);
                            }
                        }
                        else
                        {
                            cookies.SetCookies(req.RequestUri, _cookie);
                        }
                        #endregion
                        break;
                    default:
                        req.Headers.Add(item.Key, item.Value);
                        break;

                }
            }
        }


        #endregion

        #region 请求流-帮助

        private string RandomString(int length)
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
        public string IP
        {
            get
            {
                return option.IP;
            }
        }

        WebResult GetWebResult(HttpWebResponse response)
        {
            if (response == null) return null;
            var _web = new WebResult
            {
                StatusCode = (int)response.StatusCode,
                Type = response.ContentType,
                ServerHeader = string.Format("{0} {1} {2} {3} Ver:{4}.{5}", response.ResponseUri.Scheme.ToUpper(), (int)response.StatusCode, response.StatusCode, response.Server, response.ProtocolVersion.Major, response.ProtocolVersion.Minor),
                DNS = response.ResponseUri.DnsSafeHost,
                AbsoluteUri = response.ResponseUri.AbsoluteUri,
                Header = new Dictionary<string, string>()
            };
            var _header = new Dictionary<string, List<string>>();
            var _cookie = new Dictionary<string, List<string>>();

            #region 获取头信息

            foreach (string str in response.Headers.AllKeys)
            {
                string val = response.Headers[str];
                if (_header.ContainsKey(str)) _header[str].Add(val);
                else _header.Add(str, new List<string> { val });
            }
            foreach (Cookie cookie in response.Cookies)
            {
                if (_cookie.ContainsKey(cookie.Name)) _cookie[cookie.Name].Add(cookie.Value);
                else _cookie.Add(cookie.Name, new List<string> { cookie.Value });
            }

            #endregion

            foreach (var item in _header) _web.Header.Add(item.Key, string.Join("; ", item.Value));

            if (_cookie.Count > 0)
            {
                _web.Cookie = new Dictionary<string, string>();
                foreach (var item in _cookie) _web.Cookie.Add(item.Key, string.Join(";", item.Value));
            }

            return _web;
        }
        public long Val = 0, Max = 0;

        byte[]? DownStream(long response_max, WebResult _web, Stream stream, out string outfile, string? savePath = null, string? saveName = null)
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
                savePath.CreateDirectory();
                if (!savePath.EndsWith("\\") || !savePath.EndsWith("/"))
                {
                    savePath = (savePath.EndsWith("\\") ? (savePath + "/") : (savePath + "\\"));
                }
                if (saveName == null)
                {
                    saveName = option.FileName(_web);
                }
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
                    outfile = null;
                    stream_read.Close();
                    File.Delete(outfile);
                    return null;
                }
            }
        }

        byte[] GetByStream(WebResult _web, Stream stream)
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
            else
            {
                return _byte;
            }
        }

        Encoding GetEncoding(HttpWebResponse response, byte[] data)
        {
            Encoding encoding = Encoding.Default;
            Match meta = Regex.Match(Encoding.Default.GetString(data), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
            string c = string.Empty;
            if (meta != null && meta.Groups.Count > 0)
            {
                c = meta.Groups[1].Value.ToLower().Trim();
            }
            if (c.Length > 2)
            {
                try
                {
                    encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                }
                catch
                {
                    if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(response.CharacterSet))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(response.CharacterSet);
                }
            }

            return encoding;
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

        #endregion
    }
}