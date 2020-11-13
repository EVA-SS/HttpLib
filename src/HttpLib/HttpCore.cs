using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpLib
{
    public class HttpCore
    {
        private string url;
        private HttpMethod method;
        public HttpCore(string url, HttpMethod method)
        {
            this.url = url;
            this.method = method;
        }

        #region 对外的参数

        #endregion

        #region 请求的参数

        List<Val> _datas = null;
        string _datastr = null;
        List<Files> _files = null;

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="val">单个参数</param>
        public HttpCore data(Val val)
        {
            if (_datas == null)
            {
                _datas = new List<Val> { val };
            }
            else
            {
                Val find = _datas.Find(ab => ab.Key == val.Key);
                if (find == null)
                {
                    _datas.Add(val);
                }
                else
                {
                    find.SetValue(val.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(List<Val> vals)
        {
            if (_datas == null)
            {
                _datas = new List<Val>();
            }
            foreach (Val val in vals)
            {
                Val find = _datas.Find(ab => ab.Key == val.Key);
                if (find == null)
                {
                    _datas.Add(val);
                }
                else
                {
                    find.SetValue(val.Value);
                }
            }
            return this;
        }


        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, string> vals)
        {
            if (_datas == null)
            {
                _datas = new List<Val>();
            }
            foreach (var item in vals)
            {
                Val find = _datas.Find(ab => ab.Key == item.Key);
                if (find == null)
                {
                    _datas.Add(new Val(item.Key, item.Value));
                }
                else
                {
                    find.SetValue(item.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="data">多个头/文件地址</param>
        public HttpCore data(object data)
        {
            PropertyInfo[] properties = data.GetType().GetProperties();
            if (_datas == null)
            {
                _datas = new List<Val>();
            }
            foreach (var item in properties)
            {
                string val = Uri.EscapeDataString(item.GetValue(data, null).ToString());
                if (File.Exists(val))
                {
                    this.data(new Files(item.Name, val));
                }
                else
                {
                    Val find = _datas.Find(ab => ab.Key == item.Name);
                    if (find == null)
                    {
                        _datas.Add(new Val(item.Name, val));
                    }
                    else
                    {
                        find.SetValue(val);
                    }
                }

            }
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="val">application/text</param>
        public HttpCore data(string val)
        {
            _datastr = val;
            return this;
        }

        #region 文件

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore data(List<Files> vals)
        {
            if (_files == null)
            {
                _files = new List<Files>();
            }
            _files.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">单个文件</param>
        public HttpCore data(Files val)
        {
            if (_files == null)
            {
                _files = new List<Files>();
            }
            _files.Add(val);
            return this;
        }

        #endregion

        #endregion

        #region 设置请求的头

        List<Val> _headers = null;

        /// <summary>
        /// 设置请求的头
        /// </summary>
        /// <param name="val">单个头</param>
        public HttpCore header(Val val)
        {
            if (_headers == null)
            {
                _headers = new List<Val> { val };
            }
            else
            {
                Val find = _headers.Find(ab => ab.Key == val.Key);
                if (find == null)
                {
                    _headers.Add(val);
                }
                else
                {
                    find.SetValue(val.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// 设置请求的头
        /// </summary>
        /// <param name="vals">多个头</param>
        public HttpCore header(List<Val> vals)
        {
            if (_headers == null)
            {
                _headers = new List<Val>();
            }
            foreach (Val val in vals)
            {
                Val find = _headers.Find(ab => ab.Key == val.Key);
                if (find == null)
                {
                    _headers.Add(val);
                }
                else
                {
                    find.SetValue(val.Value);
                }
            }
            return this;
        }


        /// <summary>
        /// 设置请求的头
        /// </summary>
        /// <param name="vals">多个头</param>
        public HttpCore header(IDictionary<string, string> vals)
        {
            if (_headers == null)
            {
                _headers = new List<Val>();
            }
            foreach (var item in vals)
            {
                Val find = _headers.Find(ab => ab.Key == item.Key);
                if (find == null)
                {
                    _headers.Add(new Val(item.Key, item.Value));
                }
                else
                {
                    find.SetValue(item.Value);
                }
            }
            return this;
        }



        /// <summary>
        /// 设置请求的头
        /// </summary>
        /// <param name="header">多个头</param>
        public HttpCore header(object header)
        {
            PropertyInfo[] properties = header.GetType().GetProperties();
            if (_headers == null)
            {
                _headers = new List<Val>();
            }
            foreach (var item in properties)
            {
                string key = GetTFName(item.Name);
                string val = Uri.EscapeDataString(item.GetValue(header, null).ToString());
                Val find = _headers.Find(ab => ab.Key == key);
                if (find == null)
                {
                    _headers.Add(new Val(key, val));
                }
                else
                {
                    find.SetValue(val);
                }
            }
            return this;
        }
        #endregion

        #region 编码

        Encoding _encoding = null;
        public HttpCore encoding(string encoding)
        {
            _encoding = Encoding.GetEncoding(encoding);
            return this;
        }
        public HttpCore encoding(Encoding encoding)
        {
            _encoding = encoding;
            return this;
        }
        #endregion

        #region 重定向

        bool _redirect = false;
        /// <summary>
        /// 设置请求重定向
        /// </summary>
        /// <param name="val">true启用重定向，false禁用重定向</param>
        public HttpCore redirect(bool val)
        {
            _redirect = val;
            return this;
        }
        #endregion

        #region 超时

        int _time = 0;
        /// <summary>
        /// 设置请求超时时长
        /// </summary>
        /// <param name="time">毫秒</param>
        public HttpCore timeout(int time)
        {
            _time = time;
            return this;
        }

        #endregion

        #region 回调

        long requestMin = 0, requestMax = 0, responseMin = 0;
        long? responseMax = null;
        Action<long, long> _requestProgress = null;
        /// <summary>
        /// 上传进度的回调函数
        /// </summary>
        public HttpCore requestProgress(Action<long, long> action)
        {
            _requestProgress = action;
            return this;
        }

        Action<long, long?> _responseProgress = null;
        /// <summary>
        /// 相应进度的回调函数
        /// </summary>
        public HttpCore responseProgress(Action<long, long?> action)
        {
            _responseProgress = action;
            return this;
        }
        #endregion

        #region 请求

        Action<Exception> _fail = null;
        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        public HttpCore fail(Action<Exception> action)
        {
            _fail = action;
            return this;
        }

        #region 接口调用成功的回调函数

        int resultMode = -1;
        Action<WebResult> _success0 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>不下载内容</returns>
        public HttpCore success(Action<WebResult> action)
        {
            resultMode = 0;
            _success0 = action;
            return this;
        }

        Action<WebResult, string> _success1 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>返回字符串</returns>
        public HttpCore success(Action<WebResult, string> action)
        {
            resultMode = 1;
            _success1 = action;
            return this;
        }

        Action<WebResult, byte[]> _success2 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>返回字节</returns>
        public HttpCore success(Action<WebResult, byte[]> action)
        {
            resultMode = 2;
            _success2 = action;
            return this;
        }

        #endregion

        #endregion

        #region 请求核心
        /// <summary>
        /// 异步请求
        /// </summary>
        public Task RequestAsync()
        {
            return Task.Run(() =>
            {
                TaskResult val = Go(resultMode);
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
                                _success1(val.web, val.val != null ? val.val.ToString() : null);
                            }
                            break;
                        case 2:
                            if (_success2 != null)
                            {
                                _success2(val.web, val.val != null ? (byte[])val.val : null);
                            }
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public byte[] RequestByByte()
        {
            TaskResult val = Go(2);
            if (val != null)
            {
                return val.val != null ? (byte[])val.val : null;
            }
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字符串类型</returns>
        public string RequestByString()
        {
            TaskResult val = Go(1);
            if (val != null)
            {
                return val.val != null ? val.val.ToString() : null;
            }
            return null;
        }

        private TaskResult Go(int resultMode)
        {
            requestMin = requestMax = responseMin = 0;
            responseMax = null;

            try
            {
                string urlTemp = url;
                if (method == HttpMethod.Get && (_datas != null && _datas.Count > 0))
                {
                    string param = "";
                    foreach (Val item in _datas)
                    {
                        if (string.IsNullOrEmpty(param))
                        {
                            if (urlTemp.Contains("?"))
                            {
                                param += "&" + item.Key + "=" + Uri.EscapeDataString(item.Value);
                            }
                            else
                            {
                                param = "?" + item.Key + "=" + Uri.EscapeDataString(item.Value);
                            }
                        }
                        else
                        {
                            param += "&" + item.Key + "=" + Uri.EscapeDataString(item.Value);
                        }
                    }
                    urlTemp += param;
                }
                Uri uri = new Uri(urlTemp);
                CookieContainer cookies = new CookieContainer();

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.Method = method.ToString().ToUpper();
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.CookieContainer = cookies;
                request.Host = uri.Host;
                request.AllowAutoRedirect = _redirect;

                Encoding encoding = (_encoding != null ? _encoding : Encoding.UTF8);

                if (_time > 0)
                {
                    request.Timeout = _time;
                }

                request.Credentials = CredentialCache.DefaultCredentials;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";

                #region SSL
                if (uri.Scheme.ToUpper() == "HTTPS")
                {
                    ServicePointManager.ServerCertificateValidationCallback = (_s, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }
                #endregion

                bool isContentType = false;
                if (_headers != null && _headers.Count > 0)
                {
                    SetHeader(out isContentType, request, _headers, cookies);
                }


                #region 准备上传数据

                if (method != HttpMethod.Get && method != HttpMethod.Head)
                {
                    if (!string.IsNullOrEmpty(_datastr))
                    {
                        if (!isContentType)
                        {
                            request.ContentType = "application/text";
                        }
                        Upload(request, encoding.GetBytes(_datastr));
                    }
                    else if (_files != null && _files.Count > 0)
                    {
                        string boundary = RandomString(8);
                        request.ContentType = "multipart/form-data; boundary=" + boundary;

                        string beginBoundary = "\r\n--" + boundary + "\r\n";
                        byte[] startbytes = encoding.GetBytes(beginBoundary);
                        byte[] endbytes = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

                        List<byte[]> writeDATA = new List<byte[]>();

                        #region 规划文件大小

                        if (_datas != null && _datas.Count > 0)
                        {
                            foreach (Val item in _datas)
                            {
                                writeDATA.Add(startbytes);

                                string separator = string.Format("content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}", item.Key, Uri.EscapeDataString(item.Value));

                                writeDATA.Add(encoding.GetBytes(separator));
                            }
                            //writeDATA.Add(endbytes);
                        }

                        foreach (Files file in _files)
                        {
                            writeDATA.Add(startbytes);

                            string separator = string.Format("content-disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", file.Name, file.FileName, file.ContentType);

                            writeDATA.Add(encoding.GetBytes(separator));

                            writeDATA.Add(null);

                        }
                        writeDATA.Add(endbytes);

                        #endregion

                        long size = writeDATA.Sum(ab => ab != null ? ab.Length : 0) + _files.Sum(ab => ab.Size);
                        requestMax = size;
                        if (_requestProgress != null)
                        {
                            _requestProgress(requestMin, requestMax);
                        }

                        using (Stream reqStream = request.GetRequestStream())
                        {
                            int fileIndex = 0;
                            foreach (byte[] item in writeDATA)
                            {
                                if (item == null)
                                {
                                    Files file = _files[fileIndex];
                                    fileIndex++;
                                    using (file.Stream)
                                    {
                                        file.Stream.Seek(0, SeekOrigin.Begin);
                                        int bytesRead = 0;
                                        byte[] buffer = new byte[4096];
                                        while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            reqStream.Write(buffer, 0, bytesRead);
                                            requestMin += bytesRead;
                                            if (_requestProgress != null)
                                            {
                                                _requestProgress(requestMin, requestMax);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    reqStream.Write(item, 0, item.Length);
                                    requestMin += item.Length;
                                    if (_requestProgress != null)
                                    {
                                        _requestProgress(requestMin, requestMax);
                                    }
                                }
                            }
                        }

                    }
                    else if (_datas != null && _datas.Count > 0)
                    {
                        if (!isContentType)
                        {
                            request.ContentType = "application/x-www-form-urlencoded";
                        }
                        string param = "";
                        foreach (Val item in _datas)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                param = item.Key + "=" + Uri.EscapeDataString(item.Value);
                            }
                            else
                            {
                                param += "&" + item.Key + "=" + Uri.EscapeDataString(item.Value);
                            }
                        }
                        Upload(request, encoding.GetBytes(param));
                    }
                }
                #endregion

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    WebResult _web = new WebResult
                    {
                        StatusCode = (int)response.StatusCode,
                        Type = response.ContentType,
                        ServerHeader = string.Format("{0} {1} {2} {3} Ver:{4}.{5}", response.ResponseUri.Scheme.ToUpper(), (int)response.StatusCode, response.StatusCode, response.Server, response.ProtocolVersion.Major, response.ProtocolVersion.Minor),
                        IP = getIP(response.ResponseUri),
                        DNS = response.ResponseUri.DnsSafeHost,
                        AbsoluteUri = response.ResponseUri.AbsoluteUri,
                        Headers = new List<Val>()
                    };
                    if (response.ContentLength > 0)
                    {
                        responseMax = response.ContentLength;
                        if (_responseProgress != null)
                        {
                            _responseProgress(responseMin, responseMax);
                        }
                    }

                    #region 获取头信息

                    string header = "";
                    foreach (string str in response.Headers.AllKeys)
                    {
                        _web.Headers.Add(new Val(str, response.Headers[str]));
                        header = header + str + ":" + response.Headers[str] + "\r\n";
                        if (str == "Content-Disposition")
                        {
                            string Disposition = response.Headers[str];
                            if (!string.IsNullOrEmpty(Disposition))
                            {
                                string filename = "";
                                filename = Disposition.Substring(Disposition.IndexOf("filename=") + 9);
                                if (filename.Contains(";"))
                                {
                                    filename = filename.Substring(0, filename.IndexOf(";"));
                                    if (filename.EndsWith("\""))
                                    {
                                        filename = filename.Substring(1, filename.Length - 2);
                                    }
                                }
                                _web.FileName = filename;
                            }
                        }
                        else if (str == "Location")
                        {
                            string Location = response.Headers[str];
                            if (!string.IsNullOrEmpty(Location))
                            {
                                _web.Location = Location;
                            }
                        }
                        else if (str == "Set-Cookie")
                        {
                            string SetCookie = response.Headers[str];
                            if (!string.IsNullOrEmpty(SetCookie))
                            {
                                if (_web.SetCookie == null)
                                {
                                    _web.SetCookie = SetCookie;
                                }
                                else
                                {
                                    _web.SetCookie += ";" + SetCookie;
                                }
                            }
                        }
                    }
                    string cookie = "";
                    foreach (Cookie str in response.Cookies)
                    {
                        cookie = cookie + str.Name + "=" + str.Value + ";";
                    }
                    _web.Header = header;
                    _web.Cookie = cookie;

                    #endregion

                    switch (resultMode)
                    {
                        case 0:
                            using (Stream stream = response.GetResponseStream())
                            {
                                return null;
                            }
                        case 1:
                            using (Stream stream = response.GetResponseStream())
                            {
                                byte[] data = DownStream(_web, stream);
                                if (data == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    Encoding encodings = (_encoding != null ? _encoding : GetEncoding(response, data));
                                    return new TaskResult(_web, encodings.GetString(data));
                                }
                            }
                        case 2:
                            using (Stream stream = response.GetResponseStream())
                            {
                                return new TaskResult(_web, DownStream(_web, stream));
                            }
                    }
                }
            }
            catch (Exception ez)
            {
                if (_fail != null)
                {
                    _fail(ez);
                }
            }
            return null;
        }

        class TaskResult
        {
            public TaskResult(WebResult web, object val)
            {
                this.web = web;
                this.val = val;
            }
            public WebResult web { get; set; }
            public object val { get; set; }
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
            foreach (Val item in headers)
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
                        SetHeaderValue(req.Headers, item.Key, item.Value);
                        break;

                }
            }
        }
        private void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as System.Collections.Specialized.NameValueCollection;
                collection[name] = value;
            }
        }

        #endregion

        #region 请求流-帮助

        private void Upload(HttpWebRequest req, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                Upload(req, stream, data.Length);
            }
        }
        private void Upload(HttpWebRequest req, Stream stream, long length)
        {
            //long requestMin = 0, requestMax = 0, responseMin = 0;
            //long? responseMax = null;
            requestMax = req.ContentLength = length;
            byte[] buffer = new byte[4096];

            using (Stream reqStream = req.GetRequestStream())
            {
                int contentLen = 0;
                while ((contentLen = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    reqStream.Write(buffer, 0, contentLen);
                    contentLen = stream.Read(buffer, 0, buffer.Length);
                    requestMin += contentLen;
                    if (_requestProgress != null)
                    {
                        _requestProgress(requestMin, requestMax);
                    }
                }
            }
        }

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
        public string getIP(Uri _uri)
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(_uri.Host, out ip))
            {
                return ip.ToString();
            }
            else
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(_uri.Host);
                IPEndPoint ipEndPoint = new IPEndPoint(hostEntry.AddressList[0], 0);
                string _ip = ipEndPoint.Address.ToString();
                if (_ip.StartsWith("::"))
                {
                    return "127.0.0.1";
                }
                else
                {
                    return _ip;
                }
            }
        }

        byte[] DownStream(WebResult _web, Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int osize = 0, rsize = 0;
                while ((rsize = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, rsize);
                    osize += rsize;
                    responseMin = osize;
                    if (_responseProgress != null)
                    {
                        _responseProgress(responseMin, responseMax);
                    }
                }
                _web.OriginalSize = _web.Size = osize;
                if (osize > 0)
                {
                    return GetByStream(_web, memoryStream);
                }
                else { return null; }
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
                using (BinaryReader r = new BinaryReader(stream))
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
                var ms = new MemoryStream(data);
                var zip = new GZipStream(ms, CompressionMode.Decompress);
                var msreader = new MemoryStream();
                var buffer = new byte[0x1000];
                while (true)
                {
                    var reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch
            {
                return data;
            }
        }

        #endregion

        #endregion
    }
}
