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
        public HttpOption option;
        public HttpCore(string uri, HttpMethod method)
        {
            option = new HttpOption { uri = uri, method = method };
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
            foreach (var val in vals)
                Config.setVals(ref option.query, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(List<Val> vals)
        {
            foreach (var val in vals)
                Config.setVals(ref option.query, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore query(string key, string val)
        {
            Config.setVals(ref option.query, key, val);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(IDictionary<string, string> vals)
        {
            foreach (var item in vals)
                Config.setVals(ref option.query, item.Key, item.Value);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore query(object data)
        {
            foreach (var item in data.GetType().GetProperties())
            {
                string key = item.Name;
                if (key != "_")
                    key = key.TrimEnd('_');
                object valO = item.GetValue(data, null);
                if (valO != null)
                    Config.setVals(ref option.query, key, valO.ToString());
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
            foreach (var val in vals)
                Config.setVals(ref option.data, val);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(List<Val> vals)
        {
            foreach (var val in vals)
                Config.setVals(ref option.data, val);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore data(string key, string val)
        {
            Config.setVals(ref option.data, key, val);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, string> vals)
        {
            foreach (var val in vals)
                Config.setVals(ref option.data, val.Key, val.Value);
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
                throw new Exception("Get不支持数组");
            }
            foreach (var val in vals)
                foreach (var items in val.Value)
                    Config.setVals(ref option.data, val.Key + "[]", items);
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
                throw new Exception("Get不支持数组");
            }
            foreach (var val in vals)
                foreach (var items in val.Value)
                    Config.setVals(ref option.data, val.Key + "[]", items);
            return this;
        }

        /// <summary>
        /// 请求的参数
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore data(object data)
        {
            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (var item in properties)
            {
                object valO = item.GetValue(data, null);
                if (valO != null)
                {
                    string key = item.Name;
                    if (key != "_")
                    {
                        key = key.TrimEnd('_');
                    }
                    string tname = valO.GetType().Name;
                    if (typeof(System.Collections.IList).IsAssignableFrom(valO.GetType()))
                    {
                        if (option.method == HttpMethod.Get)
                            throw new Exception("Get不支持数组");
                        foreach (var items in valO as System.Collections.IList)
                            Config.setVals(ref option.data, key + "[]", items.ToString());
                    }
                    else
                        Config.setVals(ref option.data, key, valO.ToString());
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
            option.datastr = val;
            return this;
        }

        #region 文件

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore data(params Files[] vals)
        {
            if (option.file == null)
            {
                option.file = new List<Files>();
            }
            option.file.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="vals">多个文件</param>
        public HttpCore data(List<Files> vals)
        {
            if (option.file == null)
            {
                option.file = new List<Files>();
            }
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
            foreach (var item in vals)
                Config.setVals(ref option.header, item);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public HttpCore header(List<Val> vals)
        {
            foreach (var item in vals)
                Config.setVals(ref option.header, item);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore header(string key, string val)
        {
            Config.setVals(ref option.header, key, val);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public HttpCore header(IDictionary<string, string> vals)
        {
            foreach (var item in vals)
                Config.setVals(ref option.header, item.Key, item.Value);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="header">多个请求头</param>
        public HttpCore header(object header)
        {
            PropertyInfo[] properties = header.GetType().GetProperties();
            foreach (var item in properties)
            {
                string key = GetTFName(item.Name).TrimStart('-');
                string val = item.GetValue(header, null).ToString();
                Config.setVals(ref option.header, key, val);
            }
            return this;
        }

        #endregion

        #region 代理

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="address">代理服务器的 URI</param>
        public HttpCore proxy(string address)
        {
            option.proxy = new WebProxy(address);
            return this;
        }
        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="address">代理服务器的 URI</param>
        public HttpCore proxy(Uri address)
        {
            option.proxy = new WebProxy(address);
            return this;
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="host">代理主机的名称</param>
        /// <param name="port">要使用的 Host 上的端口号</param>
        public HttpCore proxy(string host, int port)
        {
            option.proxy = new WebProxy(host, port);
            return this;
        }
        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="host">代理主机的名称</param>
        /// <param name="port">要使用的 Host 上的端口号</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public HttpCore proxy(string host, int port, string username, string password)
        {
            option.proxy = new WebProxy(host, port);
            if (!string.IsNullOrEmpty(username))
            {
                option.proxy.Credentials = new NetworkCredential(username, password);
            }
            return this;
        }

        #endregion

        #region 回调

        #region 上传进度的回调

        Action<long, long> _requestProgres = null;
        Action<long> _requestProgresMax = null;
        /// <summary>
        /// 上传进度的回调函数
        /// </summary>
        public HttpCore requestProgres(Action<long, long> action)
        {
            _requestProgres = action;
            return this;
        }
        public HttpCore requestProgresMax(Action<long> action)
        {
            _requestProgresMax = action;
            return this;
        }

        #endregion

        #region 下载进度的回调

        Action<long, long> _responseProgres = null;
        Action<long> _responseProgresMax = null;
        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgres(Action<long, long> action)
        {
            _responseProgres = action;
            return this;
        }

        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgresMax(Action<long> action)
        {
            _responseProgresMax = action;
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

        #region 保活

        public HttpCore keepAlive(bool keepAlive = true)
        {
            option.keepAlive = keepAlive;
            return this;
        }

        #endregion

        #region 请求

        public delegate bool ActionBool<in T>(T obj);
        public delegate bool ActionBool2<in T1, in T2>(T1 arg1, T2 arg2);

        ActionBool2<HttpWebResponse, WebResult> _requestBefore = null;
        ActionBool<HttpWebResponse> _requestBefore2 = null;
        ActionBool<WebResult> _requestBefore3 = null;
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool2<HttpWebResponse, WebResult> action)
        {
            _requestBefore = action;
            return this;
        }
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool<HttpWebResponse> action)
        {
            _requestBefore2 = action;
            return this;
        }

        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool<WebResult> action)
        {
            _requestBefore3 = action;
            return this;
        }

        Action<Exception> _fail = null;
        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        public HttpCore fail(Action<Exception> action)
        {
            _fail = action;
            return this;
        }


        Action<int, Exception> _fail2 = null;

        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        /// <param name="action">Http状态代码+错误</param>
        /// <returns></returns>
        public HttpCore fail(Action<int, Exception> action)
        {
            _fail2 = action;
            return this;
        }

        Action<WebResult, Exception> _fail3 = null;

        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        /// <param name="action">错误Http响应头+错误</param>
        /// <returns></returns>
        public HttpCore fail(Action<WebResult, Exception> action)
        {
            _fail3 = action;
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
            requestAsync();
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
            requestAsync();
            return this;
        }

        #endregion

        #endregion

        #region 终止

        public void abort()
        {
            if (req != null)
            {
                try
                {
                    req.Abort();
                    req = null;
                }
                catch
                { }
            }
            if (response != null)
            {
                try
                {
                    response.Close();

#if !NET40
                    response.Dispose();
#endif
                    response = null;
                }
                catch
                { }
            }
        }

        #endregion

        #region 请求核心

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
        public WebResult requestNone()
        {
            TaskResult val = Go(0);
            if (val != null)
            {
                return val.web;
            }
            return null;
        }

#if NET40
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public string download(string savePath, string saveName = null)
        {
            var val = Go(3, savePath, saveName);
            if (val != null)
            {
                return val.str;
            }
            return null;
        }
#else
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="savePath">保存目录</param>
        /// <param name="saveName">保存文件名称（为空自动获取）</param>
        /// <returns>返回保存文件路径，为空下载失败</returns>
        public async Task<string> download(string savePath, string saveName = null)
        {
            var val = Go(3, savePath, saveName);
            if (val != null)
            {
                return val.str;
            }
            return null;
        }
#endif

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public byte[] requestData()
        {
            TaskResult val = Go(2);
            if (val != null)
            {
                return val.data;
            }
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public byte[] requestData(out WebResult result)
        {
            TaskResult val = Go(2);
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
        public string request()
        {
            TaskResult val = Go(1);
            if (val != null)
                return val.str;
            return null;
        }

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <returns>字节类型</returns>
        public string request(out WebResult result)
        {
            TaskResult val = Go(1);
            if (val != null)
            {
                result = val.web;
                return val.str;
            }
            result = null;
            return null;
        }

        #region 对象

        HttpWebRequest req;
        HttpWebResponse response = null;

        #endregion

        private TaskResult Go(int resultMode, string savePath = null, string saveName = null)
        {
            try
            {
                var uri = new Uri(option.Url);
                CookieContainer cookies = new CookieContainer();

                #region SSL

                if (uri.Scheme.ToUpper() == "HTTPS")
                {
                    ServicePointManager.ServerCertificateValidationCallback = (_s, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }

                #endregion

                abort();
                req = (HttpWebRequest)HttpWebRequest.Create(uri);
                if (option.proxy != null)
                    req.Proxy = option.proxy;
                else
                    req.Proxy = Config._proxy;
                req.KeepAlive = option.keepAlive;
                req.Method = option.method.ToString().ToUpper();
                req.AutomaticDecompression = Config.DecompressionMethod;
                req.CookieContainer = cookies;
                req.Host = uri.Host;

                if (option.redirect)
                    req.AllowAutoRedirect = option.redirect;
                else
                    req.AllowAutoRedirect = Config.Redirect;
                Encoding encoding = option.encoding != null ? option.encoding : Encoding.UTF8;
                if (option.timeout > 0)
                    req.Timeout = option.timeout;

                req.Credentials = CredentialCache.DefaultCredentials;
                req.UserAgent = Config.UserAgent;

                bool isContentType = false;
                if (Config._headers != null && Config._headers.Count > 0)
                {
                    SetHeader(out isContentType, req, Config._headers, cookies);
                }
                if (option.header != null && option.header.Count > 0)
                {
                    SetHeader(out isContentType, req, option.header, cookies);
                }

                #region 准备上传数据

                if (option.method != HttpMethod.Get && option.method != HttpMethod.Head)
                {
                    if (!string.IsNullOrEmpty(option.datastr))
                    {
                        if (!isContentType)
                        {
                            req.ContentType = "application/text";
                        }
                        byte[] bs = encoding.GetBytes(option.datastr);
                        req.ContentLength = bs.Length;
                        using (Stream reqStream = req.GetRequestStream())
                        {
                            reqStream.Write(bs, 0, bs.Length);
                        }
                    }
                    else if (option.file != null && option.file.Count > 0)
                    {
                        string boundary = RandomString(8);
                        req.ContentType = "multipart/form-data; boundary=" + boundary;

                        byte[] startbyteOnes = encoding.GetBytes("--" + boundary + "\r\n");
                        byte[] startbytes = encoding.GetBytes("\r\n--" + boundary + "\r\n");
                        byte[] endbytes = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

                        List<byte[]> writeDATA = new List<byte[]>();
                        int countB = 0;

                        #region 规划文件大小

                        if (option.data != null && option.data.Count > 0)
                        {
                            foreach (var item in option.data)
                            {
                                if (countB == 0)
                                {
                                    writeDATA.Add(startbyteOnes);
                                }
                                else
                                {
                                    writeDATA.Add(startbytes);
                                }
                                countB++;

                                string separator = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", item.Key, Uri.EscapeDataString(item.Value));

                                writeDATA.Add(encoding.GetBytes(separator));
                            }
                        }

                        foreach (Files file in option.file)
                        {
                            if (countB == 0)
                            {
                                writeDATA.Add(startbyteOnes);
                            }
                            else
                            {
                                writeDATA.Add(startbytes);
                            }
                            countB++;

                            string separator = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", file.Name, file.FileName, file.ContentType);

                            writeDATA.Add(encoding.GetBytes(separator));

                            writeDATA.Add(null);

                        }
                        writeDATA.Add(endbytes);

                        #endregion

                        long size = writeDATA.Sum(ab => ab != null ? ab.Length : 0) + option.file.Sum(ab => ab.Size);
                        req.ContentLength = size;

                        #region 注入进度

                        if (_requestProgresMax != null)
                            _requestProgresMax(size);
                        if (_requestProgres == null)
                        {
                            using (var reqStream = req.GetRequestStream())
                            {
                                int fileIndex = 0;
                                foreach (byte[] item in writeDATA)
                                {
                                    if (item == null)
                                    {
                                        Files file = option.file[fileIndex];
                                        fileIndex++;
                                        using (file.Stream)
                                        {
                                            file.Stream.Seek(0, SeekOrigin.Begin);
                                            int bytesRead = 0;
                                            byte[] buffer = new byte[Config.CacheSize];
                                            while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                reqStream.Write(buffer, 0, bytesRead);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        reqStream.Write(item, 0, item.Length);
                                    }
                                }
                            }
                        }
                        else
                        {
                            long request_val = 0;
                            _requestProgres(request_val, size);

                            using (var reqStream = req.GetRequestStream())
                            {
                                int fileIndex = 0;
                                foreach (byte[] item in writeDATA)
                                {
                                    if (item == null)
                                    {
                                        Files file = option.file[fileIndex];
                                        fileIndex++;
                                        using (file.Stream)
                                        {
                                            file.Stream.Seek(0, SeekOrigin.Begin);
                                            int bytesRead = 0;
                                            byte[] buffer = new byte[Config.CacheSize];
                                            while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                reqStream.Write(buffer, 0, bytesRead);
                                                request_val += bytesRead;
                                                _requestProgres(request_val, size);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        reqStream.Write(item, 0, item.Length);
                                        request_val += item.Length;
                                        _requestProgres(request_val, size);
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    else if (option.data != null && option.data.Count > 0)
                    {
                        if (!isContentType)
                        {
                            req.ContentType = "application/x-www-form-urlencoded";
                        }
                        var param_ = new List<string>();
                        foreach (var item in option.data)
                        {
                            param_.Add(item.ToStringEscape());
                        }
                        byte[] bs = encoding.GetBytes(string.Join("&", param_));
                        req.ContentLength = bs.Length;
                        using (var reqStream = req.GetRequestStream())
                            reqStream.Write(bs, 0, bs.Length);
                    }
                }

                #endregion

                using (response = (HttpWebResponse)req.GetResponse())
                {
                    var response_max = response.ContentLength;
                    Max = response_max;
                    if (_responseProgresMax != null)
                        _responseProgresMax(response_max);
                    if (_requestBefore2 != null)
                        if (!_requestBefore2(response))
                            return null;

                    WebResult _web = GetWebResult(response);

                    if (_requestBefore != null)
                        if (!_requestBefore(response, _web))
                            return null;
                    if (_requestBefore3 != null)
                        if (!_requestBefore3(_web))
                            return null;

                    switch (resultMode)
                    {
                        case 0:
                            using (Stream stream = response.GetResponseStream())
                                return null;
                        case 3:
                            using (Stream stream = response.GetResponseStream())
                            {
                                DownStream(response_max, _web, stream, out string outfile, savePath, saveName);
                                return new TaskResult(_web, outfile);
                            }
                        case 1:
                            using (Stream stream = response.GetResponseStream())
                            {
                                byte[] data = DownStream(response_max, _web, stream, out string outfile);
                                if (data == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    Encoding encodings = (option.encoding != null ? option.encoding : GetEncoding(response, data));
                                    return new TaskResult(_web, encodings.GetString(data));
                                }
                            }
                        case 2:
                            using (Stream stream = response.GetResponseStream())
                            {
                                return new TaskResult(_web, DownStream(response_max, _web, stream, out string outfile));
                            }
                    }
                }
                response = null;
            }
            catch (Exception err)
            {
                if (err is WebException)
                {
                    Config.OnFail(this, GetWebResult((err as WebException).Response as HttpWebResponse), err);
                    if (_fail3 != null)
                        _fail3(GetWebResult((err as WebException).Response as HttpWebResponse), err);
                    else if (_fail2 != null)
                    {
                        HttpWebResponse response = (err as WebException).Response as HttpWebResponse;
                        _fail2(response == null ? 0 : (int)response.StatusCode, err);
                    }
                    else if (_fail != null)
                        _fail(err);
                    return new TaskResult(GetWebResult((err as WebException).Response as HttpWebResponse));
                }
                else
                {
                    Config.OnFail(this, null, err);
                    if (_fail3 != null)
                        _fail3(null, err);
                    else if (_fail2 != null)
                        _fail2(-1, err);
                    else if (_fail != null)
                        _fail(err);
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
            public TaskResult(WebResult _web, byte[] _data)
            {
                type = 2;
                web = _web;
                data = _data;
            }
            public TaskResult(WebResult _web, string _str)
            {
                type = 1;
                web = _web;
                str = _str;
            }
            public WebResult web { get; set; }
            public int type { get; set; }
            public string str { get; set; }
            public byte[] data { get; set; }
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

        public WebResult GetWebResult(HttpWebResponse response)
        {
            if (response == null) { return null; }
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
                if (_header.ContainsKey(str))
                    _header[str].Add(val);
                else
                    _header.Add(str, new List<string> { val });
            }
            foreach (Cookie cookie in response.Cookies)
            {
                if (_cookie.ContainsKey(cookie.Name))
                    _cookie[cookie.Name].Add(cookie.Value);
                else
                    _cookie.Add(cookie.Name, new List<string> { cookie.Value });
            }

            #endregion

            foreach (var item in _header)
                _web.Header.Add(item.Key, string.Join("; ", item.Value));

            if (_cookie.Count > 0)
            {
                _web.Cookie = new Dictionary<string, string>();
                foreach (var item in _cookie)
                    _web.Cookie.Add(item.Key, string.Join(";", item.Value));
            }

            return _web;
        }
        public long Val = 0, Max = 0;

        byte[] DownStream(long response_max, WebResult _web, Stream stream, out string outfile, string savePath = null, string saveName = null)
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
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
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

        #endregion

        public override string ToString()
        {
            return option.uri;
        }
    }

    public static class HttpCoreLib
    {
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
