using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HttpLib
{
    public class HttpOption
    {
        public HttpOption(string _uri) { uri = _uri; }
        public HttpOption(string _uri, HttpMethod _method)
        {
            uri = _uri;
            method = _method;
        }
        public string uri;
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
        /// 编码
        /// </summary>
        public Encoding? encoding = null;

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
                    var _uri = new Uri(uri);
                    if (IPAddress.TryParse(_uri.Host, out IPAddress? ip))
                    {
                        return ip.ToString();
                    }
                    else
                    {
                        var hostEntry = Dns.GetHostEntry(_uri.Host);
                        var ipEndPoint = new IPEndPoint(hostEntry.AddressList[0], 0);
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
                catch { }
                return null;
            }
        }

        /// <summary>
        /// 请求URL
        /// </summary>
        public string Url
        {
            get
            {
                string uri_temp = uri;

                #region 合并参数

                var param_ = new List<string>();
                if (method == HttpMethod.Get)
                {
                    if (query != null && query.Count > 0)
                    {
                        foreach (var item in query)
                            param_.Add(item.ToString());
                    }
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                            param_.Add(item.ToString());
                    }
                }
                else
                {
                    if (query != null && query.Count > 0)
                    {
                        if (query != null && query.Count > 0)
                        {
                            foreach (var item in query)
                                param_.Add(item.ToString());
                        }
                    }
                }

                #endregion

                if (param_.Count > 0)
                {
                    if (uri_temp.Contains("?"))
                    {
                        return uri + "&" + string.Join("&", param_);
                    }
                    else
                    {
                        return uri + "?" + string.Join("&", param_);
                    }
                }
                else { return uri; }
            }
        }

        public override string ToString()
        {
            return uri;
        }
    }
}
