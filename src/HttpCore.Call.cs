using System;
using System.Net;

namespace HttpLib
{
    partial class HttpCore
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

        Func<HttpWebResponse, ResultResponse, bool> action_before = null;
        /// <summary>
        /// 请求之前处理
        /// </summary>
        /// <param name="action">请求之前处理回调</param>
        /// <returns>返回true继续 反之取消请求</returns>
        public HttpCore before(Func<HttpWebResponse, ResultResponse, bool> action)
        {
            action_before = action;
            return this;
        }

        Action<ResultResponse> action_fail = null;
        /// <summary>
        /// 接口调用失败的回调函数
        /// </summary>
        /// <param name="action">错误Http响应头+错误</param>
        /// <returns></returns>
        public HttpCore fail(Action<ResultResponse> action)
        {
            action_fail = action;
            return this;
        }

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
}