using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    public static class Http
    {
        static Http()
        {
#if NET40
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
#elif NET50 || NET45 || NET46
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#elif NET47 || NET48 || NETCOREAPP3_0 || NETCOREAPP3_1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#elif NETSTANDARD2_1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#elif NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif

        }
        public static HttpCore Get(string url)
        {
            return Core(url, HttpMethod.Get);
        }

        public static HttpCore Head(string url)
        {
            return Core(url, HttpMethod.Head);
        }

        public static HttpCore Post(string url)
        {
            return Core(url, HttpMethod.Post);
        }

        public static HttpCore Put(string url)
        {
            return Core(url, HttpMethod.Put);
        }

        public static HttpCore Patch(string url)
        {
            return Core(url, HttpMethod.Patch);
        }

        public static HttpCore Delete(string url)
        {
            return Core(url, HttpMethod.Delete);
        }


        public static HttpCore Core(string url, HttpMethod method)
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
        public string IP { set; get; }
        public string DNS { set; get; }
        public string AbsoluteUri { set; get; }
        /// <summary>
        /// 响应指示类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 格式化后的响应头
        /// </summary>
        public List<Val> Headers { set; get; }

        /// <summary>
        /// 响应头
        /// </summary>
        public string Header { set; get; }

        public string Cookie { set; get; }
        public string SetCookie { set; get; }
        /// <summary>
        /// 流原始大小
        /// </summary>
        public long OriginalSize { set; get; }

        /// <summary>
        /// 流大小
        /// </summary>
        public long Size { set; get; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 重定向网址 302
        /// </summary>
        public string Location { get; set; }
    }
}
