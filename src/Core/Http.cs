﻿using System;
using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 快捷的HTTP请求
    /// </summary>
    public static class Http
    {
        #region SSL

        static Http()
        {
#if NET40
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
#elif NET45 || NET46 || NETSTANDARD2_0 || NETSTANDARD2_1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#elif NET48
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#else
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#endif
        }

        #endregion

        /// <summary>
        /// GET 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Get(this string url)
        {
            return Core(url, HttpMethod.Get);
        }
        /// <summary>
        /// GET 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Get(this Uri url)
        {
            return Core(url, HttpMethod.Get);
        }

        /// <summary>
        /// POST 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Post(this string url)
        {
            return Core(url, HttpMethod.Post);
        }
        /// <summary>
        /// POST 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Post(this Uri url)
        {
            return Core(url, HttpMethod.Post);
        }

        /// <summary>
        /// Put 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Put(this string url)
        {
            return Core(url, HttpMethod.Put);
        }
        /// <summary>
        /// Put 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Put(this Uri url)
        {
            return Core(url, HttpMethod.Put);
        }

        /// <summary>
        /// Delete 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Delete(this string url)
        {
            return Core(url, HttpMethod.Delete);
        }
        /// <summary>
        /// Delete 请求
        /// </summary>
        /// <param name="url">地址</param>
        public static HttpCore Delete(this Uri url)
        {
            return Core(url, HttpMethod.Delete);
        }

        public static HttpCore Core(this string url, HttpMethod method)
        {
            return new HttpCore(url, method);
        }
        public static HttpCore Core(this Uri url, HttpMethod method)
        {
            return new HttpCore(url, method);
        }
    }

    /// <summary>
    /// 响应结果
    /// </summary>
    public class ResultResponse
    {
        public ResultResponse(Uri uri)
        {
            Uri = uri;
            OriginalSize = Size = -1;
            Header = Cookie = new Dictionary<string, string>(0);
        }
        public ResultResponse(Uri uri, Exception ex) : this(uri) { Exception = ex; }
        public ResultResponse(HttpWebResponse data, Exception ex) : this(data) { Exception = ex; }
        public ResultResponse(HttpWebResponse data)
        {
            OriginalSize = Size = -1;
            StatusCode = (int)data.StatusCode;
            Type = data.ContentType;
            ServerHeader = string.Join(" ", new string[] {
                data.ResponseUri.Scheme.ToUpper(),
                StatusCode.ToString(),
                data.StatusCode.ToString(),
                data.Server,
                "Ver:" + data.ProtocolVersion.ToString()
            });
            Uri = data.ResponseUri;
            if (data.Headers.Count > 0 || data.Cookies.Count > 0)
            {
                if (data.Headers.Count > 0)
                {
                    var header = new Dictionary<string, List<string>>(data.Headers.AllKeys.Length);
                    foreach (string it in data.Headers.AllKeys)
                    {
                        var val = data.Headers[it];
                        if (val == null) continue;
                        if (header.ContainsKey(it)) header[it].Add(val);
                        else header.Add(it, new List<string> { val });
                    }
                    Header = new Dictionary<string, string>(header.Count);
                    foreach (var it in header) Header.Add(it.Key, string.Join("; ", it.Value));
                }
                else Header = new Dictionary<string, string>(0);

                if (data.Cookies.Count > 0)
                {
                    var cookie = new Dictionary<string, List<string>>(data.Cookies.Count);
                    foreach (Cookie it in data.Cookies)
                    {
                        if (cookie.ContainsKey(it.Name)) cookie[it.Name].Add(it.Value);
                        else cookie.Add(it.Name, new List<string> { it.Value });
                    }
                    Cookie = new Dictionary<string, string>(cookie.Count);
                    foreach (var it in cookie) Cookie.Add(it.Key, string.Join(";", it.Value));
                }
                else Cookie = new Dictionary<string, string>(0);
            }
            else Header = Cookie = new Dictionary<string, string>(0);
        }

        /// <summary>
        /// 指示HTTP响应是否成功 range 200-299
        /// </summary>
        public bool IsSuccessStatusCode { get => StatusCode >= 200 && StatusCode <= 299; }

        /// <summary>
        /// 状态代码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 服务头
        /// </summary>
        public string? ServerHeader { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 响应指示类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 响应头
        /// </summary>
        public Dictionary<string, string> Header { get; set; }
        public Dictionary<string, string> Cookie { get; set; }

        /// <summary>
        /// 流原始大小
        /// </summary>
        public long OriginalSize { get; set; }

        /// <summary>
        /// 流大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 错误异常
        /// </summary>
        public Exception? Exception { get; set; }
    }
}