using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    public static class Http
    {
        static Http()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
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
    public class WebResult
    {
        public int StatusCode { set; get; }
        public string ServerHeader { set; get; }
        public string IP { set; get; }
        public string DNS { set; get; }
        public string AbsoluteUri { set; get; }
        public string Type { set; get; }
        public List<Val> Headers { set; get; }
        public string Header { set; get; }

        public string Cookie { set; get; }
        public string SetCookie { set; get; }
        public long OriginalSize { set; get; }
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
