using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace HttpLib
{
    public partial class HttpCore
    {
        #region 回调

        #region 上传进度的回调

        Action<long, long> _requestProgres = null;
        Action<long> _requestProgresMax = null;
        /// <summary>
        /// 上传进度的回调函数
        /// </summary>
        public HttpCore requestProgres(Action<long, long> action)
        {
            _requestProgres = action;
            return this;
        }
        public HttpCore requestProgresMax(Action<long> action)
        {
            _requestProgresMax = action;
            return this;
        }

        #endregion

        #region 下载进度的回调

        Action<long, long> _responseProgres = null;
        Action<long> _responseProgresMax = null;
        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgres(Action<long, long> action)
        {
            _responseProgres = action;
            return this;
        }

        /// <summary>
        /// 下载进度的回调函数
        /// </summary>
        public HttpCore responseProgresMax(Action<long> action)
        {
            _responseProgresMax = action;
            return this;
        }

        #endregion

        #endregion

        #region 请求

        public delegate bool ActionBool<in T>(T obj);
        public delegate bool ActionBool2<in T1, in T2>(T1 arg1, T2 arg2);

        ActionBool2<HttpWebResponse, WebResult> _requestBefore = null;
        ActionBool<HttpWebResponse> _requestBefore2 = null;
        ActionBool<WebResult> _requestBefore3 = null;
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool2<HttpWebResponse, WebResult> action)
        {
            _requestBefore = action;
            return this;
        }
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool<HttpWebResponse> action)
        {
            _requestBefore2 = action;
            return this;
        }

        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(ActionBool<WebResult> action)
        {
            _requestBefore3 = action;
            return this;
        }

        Action<Exception> _fail = null;
        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        public HttpCore fail(Action<Exception> action)
        {
            _fail = action;
            return this;
        }


        Action<int, Exception> _fail2 = null;

        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        /// <param name="action">Http状态代码+错误</param>
        /// <returns></returns>
        public HttpCore fail(Action<int, Exception> action)
        {
            _fail2 = action;
            return this;
        }

        Action<WebResult, Exception> _fail3 = null;

        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        /// <param name="action">错误Http响应头+错误</param>
        /// <returns></returns>
        public HttpCore fail(Action<WebResult, Exception> action)
        {
            _fail3 = action;
            return this;
        }

        #region 接口调用成功的回调函数

        int resultMode = -1;
        Action<WebResult> _success0 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>不下载内容</returns>
        public HttpCore success(Action<WebResult> action)
        {
            resultMode = 0;
            _success0 = action;
            requestAsync();
            return this;
        }

        Action<WebResult, string> _success1 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>返回字符串</returns>
        public HttpCore success(Action<WebResult, string> action)
        {
            resultMode = 1;
            _success1 = action;
            return this;
        }

        Action<WebResult, byte[]> _success2 = null;
        /// <summary>
        /// 请求成功回调
        /// </summary>
        /// <param name="action">WebResult</param>
        /// <returns>返回字节</returns>
        public HttpCore success(Action<WebResult, byte[]> action)
        {
            resultMode = 2;
            _success2 = action;
            requestAsync();
            return this;
        }

        #endregion

        #endregion

        #region 终止

        public void abort()
        {
            if (req != null)
            {
                try
                {
                    req.Abort();
                    req = null;
                }
                catch
                { }
            }
            if (response != null)
            {
                try
                {
                    response.Close();

#if !NET40
                    response.Dispose();
#endif
                    response = null;
                }
                catch
                { }
            }
        }

        #endregion
    }

    public static class HttpCoreLib
    {
        public static void CreateDirectory(this string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public static string ToString(this Dictionary<string, string> vals)
        {
            return ToString(vals, Environment.NewLine);
        }
        public static string ToString(this Dictionary<string, string> vals, string sp)
        {
            var strs = new List<string>();
            foreach (var item in vals)
            {
                strs.Add(item.Key + "=" + item.Value);
            }
            return string.Join(sp, strs);
        }
        public static string ToString(this List<Val> vals)
        {
            return ToString(vals, Environment.NewLine);
        }
        public static string ToString(this List<Val> vals, string sp)
        {
            var strs = new List<string>();
            foreach (var item in vals)
            {
                strs.Add(item.ToString());
            }
            return string.Join(sp, strs);
        }
    }
}