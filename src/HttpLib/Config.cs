using System;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class Config
    {
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

        public static IWebProxy _proxy = null;

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

    }
}
