using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpLib
{
    partial class HttpCore
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
            if (option.query == null) option.query = new List<Val>(vals.Length);
            option.query.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(IList<Val> vals)
        {
            if (option.query == null) option.query = new List<Val>(vals.Count);
            option.query.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore query(string key, string val)
        {
            if (option.query == null) option.query = new List<Val> { new Val(key, val) };
            else option.query.Add(new Val(key, val));
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore query(IDictionary<string, string> vals)
        {
            if (option.query == null) option.query = new List<Val>(vals.Count);
            foreach (var it in vals)
            {
                option.query.Add(new Val(it.Key, it.Value));
            }
            return this;
        }

        /// <summary>
        /// 请求参(GET)
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore query(object data)
        {
            var list = data.GetType().GetProperties();
            var vals = new List<Val>(list.Length);
            foreach (var it in list)
            {
                string key = it.Name;
                if (key != "_" && key.EndsWith("_")) key = key.TrimEnd('_');
                var valO = it.GetValue(data, null);
                if (valO != null) vals.Add(new Val(key, valO.ToString()));
            }
            if (vals.Count > 0) return query(vals);
            return this;
        }

        #endregion

        #region 请求参

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(params Val[] vals)
        {
            if (option.data == null) option.data = new List<Val>(vals.Length);
            option.data.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IList<Val> vals)
        {
            if (option.data == null) option.data = new List<Val>(vals.Count);
            option.data.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore data(string key, string val)
        {
            if (option.data == null) option.data = new List<Val> { new Val(key, val) };
            else option.data.Add(new Val(key, val));
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore data(IDictionary<string, string> vals)
        {
            if (option.data == null) option.data = new List<Val>(vals.Count);
            foreach (var it in vals)
            {
                option.data.Add(new Val(it.Key, it.Value));
            }
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="_data">多个参数</param>
        public HttpCore data(object _data)
        {
            var list = _data.GetType().GetProperties();
            var vals = new List<Val>(list.Length);
            foreach (var it in list)
            {
                string key = it.Name;
                if (key != "_" && key.EndsWith("_")) key = key.TrimEnd('_');
                var valO = it.GetValue(_data, null);
                if (valO != null) vals.Add(new Val(key, valO.ToString()));
            }
            if (vals.Count > 0) return data(vals);
            return this;
        }

        /// <summary>
        /// 请求参（数组）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore data(string key, IList<string> val)
        {
            if (option.method == HttpMethod.Get) throw new Exception("Get不支持数组");
            if (option.data == null) option.data = new List<Val>(val.Count);
            foreach (var it in val) option.data.Add(new Val(key + "[]", it));
            return this;
        }

        /// <summary>
        /// 请求参
        /// </summary>
        /// <param name="val">text/plain</param>
        /// <param name="contentType">类型</param>
        public HttpCore datastr(string val, string contentType = "text/plain")
        {
            option.datastr = val;
            header("Content-Type", contentType);
            return this;
        }

        #region 文件

        /// <summary>
        /// 请求参（文件）
        /// </summary>
        /// <param name="files">多个文件</param>
        public HttpCore data(params Files[] files)
        {
            if (option.file == null) option.file = new List<Files>(files.Length);
            option.file.AddRange(files);
            return this;
        }

        /// <summary>
        /// 请求参（文件）
        /// </summary>
        /// <param name="files">多个文件</param>
        public HttpCore data(IList<Files> files)
        {
            if (option.file == null) option.file = new List<Files>(files.Count);
            option.file.AddRange(files);
            return this;
        }

        #endregion

        #endregion

        #region 请求头

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore header(params Val[] vals)
        {
            if (option.header == null) option.header = new List<Val>(vals.Length);
            option.header.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore header(IList<Val> vals)
        {
            if (option.header == null) option.header = new List<Val>(vals.Count);
            option.header.AddRange(vals);
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public HttpCore header(string key, string val)
        {
            if (option.header == null) option.header = new List<Val> { new Val(key, val) };
            else option.header.Add(new Val(key, val));
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="vals">多个参数</param>
        public HttpCore header(IDictionary<string, string> vals)
        {
            if (option.header == null) option.header = new List<Val>(vals.Count);
            foreach (var it in vals)
            {
                option.header.Add(new Val(it.Key, it.Value));
            }
            return this;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="data">多个参数</param>
        public HttpCore header(object data)
        {
            var list = data.GetType().GetProperties();
            var vals = new List<Val>(list.Length);
            foreach (var it in list)
            {
                string key = GetTFName(it.Name).TrimStart('-');
                if (key != "_" && key.EndsWith("_")) key = key.TrimEnd('_');
                var valO = it.GetValue(data, null);
                if (valO != null) vals.Add(new Val(key, valO.ToString()));
            }
            if (vals.Count > 0) return header(vals);
            return this;
        }

        #endregion

        #region 代理

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="address">服务器URI</param>
        public HttpCore proxy(string address)
        {
            option.proxy = new WebProxy(address);
            return this;
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="address">服务器URI</param>
        public HttpCore proxy(Uri address)
        {
            option.proxy = new WebProxy(address);
            return this;
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="host">主机名称</param>
        /// <param name="port">端口</param>
        public HttpCore proxy(string host, int port)
        {
            option.proxy = new WebProxy(host, port);
            return this;
        }

        /// <summary>
        /// 代理
        /// </summary>
        /// <param name="host">主机名称</param>
        /// <param name="port">端口</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public HttpCore proxy(string host, int port, string? username, string? password)
        {
            option.proxy = new WebProxy(host, port);
            if (!string.IsNullOrEmpty(username)) option.proxy.Credentials = new NetworkCredential(username, password);
            return this;
        }

        #endregion

        #region 编码

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="encoding">编码</param>
        public HttpCore encoding(string encoding)
        {
            option.encoding = Encoding.GetEncoding(encoding);
            return this;
        }

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="encoding">编码</param>
        public HttpCore encoding(Encoding encoding)
        {
            option.encoding = encoding;
            return this;
        }

        /// <summary>
        /// 自动编码（识别html编码格式）
        /// </summary>
        public HttpCore autoencode(bool auto = true)
        {
            option.autoencode = auto;
            return this;
        }

        #endregion

        #region 重定向

        /// <summary>
        /// 重定向
        /// </summary>
        public HttpCore redirect(bool val = true)
        {
            option.redirect = val;
            return this;
        }

        #endregion

        #region 超时

        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="time">毫秒</param>
        public HttpCore timeout(int time)
        {
            option.timeout = time;
            return this;
        }

        #endregion

        public void CreateRequest(ref HttpWebRequest? req)
        {
            var uri = option.Url;
            CookieContainer cookies = new CookieContainer();

            #region SSL

            if (uri.Scheme.ToUpper() == "HTTPS")
                ServicePointManager.ServerCertificateValidationCallback = (_s, certificate, chain, sslPolicyErrors) => { return true; };

            #endregion

            req = (HttpWebRequest)WebRequest.Create(uri);

            if (option.proxy != null) req.Proxy = option.proxy;
            else req.Proxy = Config._proxy;
            req.Method = option.method.ToString().ToUpper();
            req.AutomaticDecompression = Config.DecompressionMethod;
            req.CookieContainer = cookies;
            req.Host = uri.Host;

            if (option.redirect) req.AllowAutoRedirect = option.redirect;
            else req.AllowAutoRedirect = Config.Redirect;
            Encoding encoding = option.encoding != null ? option.encoding : Encoding.UTF8;
            if (option.timeout > 0) req.Timeout = option.timeout;

            req.Credentials = CredentialCache.DefaultCredentials;
            req.UserAgent = Config.UserAgent;

            bool isContentType = false;
            if (Config._headers != null && Config._headers.Count > 0) SetHeader(out isContentType, req, Config._headers, cookies);

            if (option.header != null && option.header.Count > 0) SetHeader(out isContentType, req, option.header, cookies);

            #region 准备上传数据

            if (option.method != HttpMethod.Get && option.method != HttpMethod.Head)
            {
                if (!string.IsNullOrEmpty(option.datastr))
                {
                    if (!isContentType) req.ContentType = "application/text";

                    byte[] bs = encoding.GetBytes(option.datastr);
                    req.ContentLength = bs.Length;
                    using (var reqStream = req.GetRequestStream())
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
                        foreach (var it in option.data)
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

                            string separator = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", it.Key, Uri.EscapeDataString(it.Value));

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
                            foreach (byte[] it in writeDATA)
                            {
                                if (it == null)
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
                                    reqStream.Write(it, 0, it.Length);
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
                    if (!isContentType) req.ContentType = "application/x-www-form-urlencoded";

                    var param_ = new List<string>();
                    foreach (var item in option.data) param_.Add(item.ToStringEscape());

                    byte[] bs = encoding.GetBytes(string.Join("&", param_));
                    req.ContentLength = bs.Length;
                    using (var reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }
                }
            }

            #endregion
        }
    }

    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Patch,
        Options
    }
}