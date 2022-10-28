using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpLib
{
    public class HttpOption
    {
        public string uri = null;
        public HttpMethod method = HttpMethod.Get;


        /// <summary>
        /// Get请求的参数
        /// </summary>
        public List<Val> query = null;
        /// <summary>
        /// 请求的参数
        /// </summary>
        public List<Val> data = null;
        public string datastr = null;
        public List<Files> file = null;

        /// <summary>
        /// 请求头
        /// </summary>
        public List<Val> header = null;

        /// <summary>
        /// 代理
        /// </summary>
        public IWebProxy proxy = null;

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding encoding = Encoding.UTF8;

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
        /// 保活
        /// </summary>
        public bool keepAlive = false;

        /// <summary>
        /// 获取域名IP
        /// </summary>
        public string IP
        {
            get
            {
                try
                {
                    var _uri = new Uri(uri);
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
                if (query != null && query.Count > 0)
                {
                    if (query != null && query.Count > 0)
                    {
                        foreach (var item in query)
                            param_.Add(item.ToString());
                    }
                }

                #endregion

                if (param_.Count > 0)
                {
                    if (uri_temp.Contains("?"))
                    {
                        return uri + string.Join("&", param_);
                    }
                    else
                    {
                        return uri + "?" + string.Join("&", param_);
                    }
                }
                else { return uri; }
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
            var _uri = new Uri(uri);
            if (_uri.Query.Length > 0)
                return Path.GetFileName(uri.Substring(0, uri.Length - _uri.Query.Length));
            return Path.GetFileName(uri);
        }

        public override string ToString()
        {
            return uri;
        }
    }
}
