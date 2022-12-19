using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace HttpLib
{
    /// <summary>
    /// 快捷的HTTP请求
    /// </summary>
    public static class Http
    {
        public static HttpCore Get(this string url)
        {
            return Core(url, HttpMethod.Get);
        }

        public static HttpCore Post(this string url)
        {
            return Core(url, HttpMethod.Post);
        }

        public static HttpCore Put(this string url)
        {
            return Core(url, HttpMethod.Put);
        }

        public static HttpCore Delete(this string url)
        {
            return Core(url, HttpMethod.Delete);
        }

        public static HttpCore Core(this string url, HttpMethod method)
        {
            return new HttpCore(url, method);
        }

        #region 缓存

        public static string? Cache(this string ID, int time = 0)
        {
            var file = Config.CacheFolder + ID;
            if (System.IO.File.Exists(Config.CacheFolder + ID))
            {
                if (time > 0)
                {
                    var t = System.IO.File.GetCreationTime(file);
                    var elapsedTicks = DateTime.Now.Ticks - t.Ticks;
                    var elapsedSpan = new TimeSpan(elapsedTicks);
                    if (elapsedSpan.TotalMinutes < time) return System.IO.File.ReadAllText(file);
                }
                else
                    return System.IO.File.ReadAllText(file);
            }
            return null;
        }
        public static byte[]? CacheData(this string ID, int time = 0)
        {
            var file = Config.CacheFolder + ID;
            if (System.IO.File.Exists(Config.CacheFolder + ID))
            {
                if (time > 0)
                {
                    var t = System.IO.File.GetCreationTime(file);
                    var elapsedTicks = DateTime.Now.Ticks - t.Ticks;
                    var elapsedSpan = new TimeSpan(elapsedTicks);
                    if (elapsedSpan.TotalDays < time) return System.IO.File.ReadAllBytes(file);
                }
                else
                    return System.IO.File.ReadAllBytes(file);
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// 请求响应
    /// </summary>
    public class WebResult
    {
        public WebResult() { }
        public WebResult(Exception err) => Exception = err;
        public WebResult(WebResult web)
        {
            if (web != null)
            {
                OK = web.OK;
                StatusCode = web.StatusCode;
                Type = web.Type;
                Header = web.Header;
                Exception = web.Exception;
            }
        }
        public WebResult(WebResult web, Exception err)
        {
            Exception = err;
            OK = web.OK;
            StatusCode = web.StatusCode;
            Type = web.Type;
            Header = web.Header;
            Exception = web.Exception;
        }

        /// <summary>
        /// 是否算成功响应
        /// </summary>
        public bool OK { set; get; }

        /// <summary>
        /// 状态代码
        /// </summary>
        public HttpStatusCode StatusCode { set; get; }

        /// <summary>
        /// 响应指示类型
        /// </summary>
        public string? Type { set; get; }

        /// <summary>
        /// 响应头
        /// </summary>
        public Dictionary<string, string>? Header { set; get; }

        /// <summary>
        /// 内容响应头
        /// </summary>
        public Dictionary<string, string>? HeaderContent { set; get; }

        /// <summary>
        /// 错误异常
        /// </summary>
        public Exception? Exception { get; set; }
    }

    public class WebResultData<T> : WebResult
    {
        public WebResultData(WebResult web) : base(web) { }

        /// <summary>
        /// 响应内容
        /// </summary>
        public T? Data { set; get; }
    }
}
