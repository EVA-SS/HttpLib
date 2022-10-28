using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 快捷的HTTP请求
    /// </summary>
    public static class Http
    {
        static Http()
        {
#if NET40
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
#elif NETSTANDARD2_0
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#elif NETSTANDARD2_1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.SystemDefault | SecurityProtocolType.Ssl3;
#elif NET48
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#else
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#endif
        }

        public static HttpCore Get(this string url)
        {
            return Core(url, HttpMethod.Get);
        }

        public static HttpCore Head(this string url)
        {
            return Core(url, HttpMethod.Head);
        }

        public static HttpCore Post(this string url)
        {
            return Core(url, HttpMethod.Post);
        }

        public static HttpCore Put(this string url)
        {
            return Core(url, HttpMethod.Put);
        }

        public static HttpCore Patch(this string url)
        {
            return Core(url, HttpMethod.Patch);
        }

        public static HttpCore Delete(this string url)
        {
            return Core(url, HttpMethod.Delete);
        }


        public static HttpCore Core(this string url, HttpMethod method)
        {
            return new HttpCore(url, method);
        }
    }

    /// <summary>
    /// 请求响应
    /// </summary>
    public class WebResult
    {
        /// <summary>
        /// 状态代码
        /// </summary>
        public int StatusCode { set; get; }
        /// <summary>
        /// 服务头
        /// </summary>
        public string ServerHeader { set; get; }
        public string DNS { set; get; }
        public string AbsoluteUri { set; get; }
        /// <summary>
        /// 响应指示类型
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// 响应头
        /// </summary>
        public Dictionary<string, string> Header { set; get; }
        public Dictionary<string, string> Cookie { set; get; }

        /// <summary>
        /// 流原始大小
        /// </summary>
        public long OriginalSize { set; get; }

        /// <summary>
        /// 流大小
        /// </summary>
        public long Size { set; get; }
    }
}
