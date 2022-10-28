using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace HttpLib
{
    partial class HttpCore
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

        /// <summary>
        /// 设置自动编码
        /// </summary>
        /// <param name="val">true启用自动识别html编码格式</param>
        public HttpCore autoencode(bool val)
        {
            option.autoencode = val;
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

        public void CreateRequest(ref HttpWebRequest req)
        {
            var uri = new Uri(option.Url);
            CookieContainer cookies = new CookieContainer();

            #region SSL

            if (uri.Scheme.ToUpper() == "HTTPS")
                ServicePointManager.ServerCertificateValidationCallback = (_s, certificate, chain, sslPolicyErrors) => { return true; };

            #endregion

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
        }
    }

    public enum HttpMethod
    {
        Get,
        Head,
        Post,
        Put,
        Patch,
        Delete,
        Options
    }
}
