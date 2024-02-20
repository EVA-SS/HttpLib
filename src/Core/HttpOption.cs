using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpLib
{
    public class HttpOption
    {
        public HttpOption(string _uri) { uri = new Uri(_uri); }
        public HttpOption(string _uri, HttpMethod _method)
        {
            uri = new Uri(_uri);
            method = _method;
        }
        public HttpOption(Uri _uri, HttpMethod _method)
        {
            uri = _uri;
            method = _method;
        }

        public Uri uri;
        public HttpMethod method = HttpMethod.Get;

        /// <summary>
        /// Get请求的参数
        /// </summary>
        public List<Val>? query = null;
        /// <summary>
        /// 请求的参数
        /// </summary>
        public List<Val>? data = null;
        public string? datastr = null;
        public List<Files>? file = null;

        /// <summary>
        /// 请求头
        /// </summary>
        public List<Val>? header = null;

        /// <summary>
        /// 代理
        /// </summary>
        public IWebProxy proxy = null;

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding? encoding = null;

        /// <summary>
        /// 自动编码
        /// </summary>
        public bool autoencode = false;

        /// <summary>
        /// 请求重定向
        /// </summary>
        public bool redirect = false;

        /// <summary>
        /// 请求超时时长
        /// </summary>
        public int timeout = 0;

        /// <summary>
        /// 获取域名IP
        /// </summary>
        public string? IP
        {
            get
            {
                try
                {
                    if (IPAddress.TryParse(uri.Host, out IPAddress? ip)) return ip.ToString();
                    else
                    {
                        var hostEntry = Dns.GetHostEntry(uri.Host);
                        var ipEndPoint = new IPEndPoint(hostEntry.AddressList[0], 0);
                        string _ip = ipEndPoint.Address.ToString();
                        if (_ip.StartsWith("::")) return "127.0.0.1";
                        else return _ip;
                    }
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// 请求URL
        /// </summary>
        public Uri Url
        {
            get
            {
                #region 合并参数

                var param_ = new List<string>();
                if (query != null && query.Count > 0) foreach (var item in query) param_.Add(item.ToString());

                if (method == HttpMethod.Get && data != null && data.Count > 0) foreach (var item in data) param_.Add(item.ToString());

                #endregion

                if (param_.Count > 0)
                {
                    if (uri.AbsoluteUri.Contains("?")) return new Uri(uri.AbsoluteUri + "&" + string.Join("&", param_));
                    else return new Uri(uri.AbsoluteUri + "?" + string.Join("&", param_));
                }

                return uri;
            }
        }


        public string FileName(WebResult _web)
        {
            if (_web.Header.ContainsKey("Content-Disposition"))
            {
                string val = _web.Header["Content-Disposition"];
                if (!string.IsNullOrEmpty(val))
                {
                    var filename = val.Substring(val.ToUpper().IndexOf("filename=") + 9);
                    if (filename.Contains(";"))
                    {
                        filename = filename.Substring(0, filename.IndexOf(";"));
                        if (filename.EndsWith("\""))
                        {
                            filename = filename.Substring(1, filename.Length - 2);
                        }
                    }
                    return filename;
                }
            }
            if (uri.Query.Length > 0) return Path.GetFileName(uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.Length - uri.Query.Length));
            return Path.GetFileName(uri.AbsoluteUri);
        }

        public override string ToString()
        {
            return uri.AbsoluteUri;
        }
    }
}