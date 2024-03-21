using System;
using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class Config
    {
        public static List<Val> _headers = null;

        /// <summary>
        /// 设置全局请求的头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public static void header(params Val[] vals)
        {
            foreach (var val in vals)
                setVals(ref _headers, val);
        }

        public static void setVals(ref List<Val> obj, Val val)
        {
            setVals(ref obj, val.Key, val.Value);
        }
        public static void setVals(ref List<Val> obj, string key, string val)
        {
            if (obj == null)
            {
                if (val != null)
                    obj = new List<Val> { new Val(key, val) };
            }
            else
            {
                Val find = obj.Find(ab => ab.Key == key);
                if (find == null) obj.Add(new Val(key, val));
                else
                {
                    if (val != null) find.SetValue(val);
                    else obj.Remove(find);
                }
            }
        }

        /// <summary>
        /// 用户标识
        /// </summary>
        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36";


        /// <summary>
        /// 表示文件压缩和解压缩编码格式，该格式将用来压缩在 System.Net.HttpWebRequest 的响应中收到的数据
        /// </summary>
        public static DecompressionMethods DecompressionMethod = DecompressionMethods.GZip;

        /// <summary>
        /// 全局是否自动重定向
        /// </summary>
        public static bool Redirect = false;

        #region 全局错误

        public delegate void ErrEventHandler(HttpCore core, WebResult result, Exception err);

        /// <summary>
        /// 接口调用失败的回调函数（带响应头）
        /// </summary>
        public static event ErrEventHandler fail;


        public static void OnFail(HttpCore core, WebResult result, Exception err)
        {
            if (fail != null) { fail(core, result, err); }
        }

        #endregion

        #region 代理

        public static IWebProxy? _proxy = null;

        /// <summary>
        /// 全局代理
        /// </summary>
        /// <param name="address">代理服务器的 URI</param>
        public static void proxy(string address)
        {
            _proxy = new WebProxy(address);
        }
        /// <summary>
        /// 全局代理
        /// </summary>
        /// <param name="address">代理服务器的 URI</param>
        public static void proxy(Uri address)
        {
            _proxy = new WebProxy(address);
        }

        /// <summary>
        /// 全局代理
        /// </summary>
        /// <param name="host">代理主机的名称</param>
        /// <param name="port">要使用的 Host 上的端口号</param>
        public static void proxy(string host, int port)
        {
            _proxy = new WebProxy(host, port);
        }

        /// <summary>
        /// 全局代理
        /// </summary>
        /// <param name="host">代理主机的名称</param>
        /// <param name="port">要使用的 Host 上的端口号</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public static void proxy(string host, int port, string username, string password)
        {
            _proxy = new WebProxy(host, port);
            if (!string.IsNullOrEmpty(username))
            {
                _proxy.Credentials = new NetworkCredential(username, password);
            }
        }

        #endregion

        public static int CacheSize = 4096;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RetryCount = 6;
        /// <summary>
        /// 超时时长
        /// </summary>
        public static int TimeOut = 10000;
    }
}