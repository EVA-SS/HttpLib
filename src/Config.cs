using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class Config
    {
        public static string? CacheFolder = null;

        #region 全局参数

        #region 请求头

        /// <summary>
        /// 用户标识
        /// </summary>
        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36";

        public static List<Val>? header = null;

        public static void headers(params Val[] vals)
        {
            header ??= new List<Val>(vals.Length);
            foreach (var item in vals)
                HttpCoreLib.AddVal(ref header, item);
        }

        #endregion

        /// <summary>
        /// 表示文件压缩和解压缩编码格式
        /// </summary>
        public static DecompressionMethods DecompressionMethod = DecompressionMethods.GZip;

        /// <summary>
        /// 全局是否自动重定向
        /// </summary>
        public static bool Redirect = false;

        /// <summary>
        /// 使用池创建
        /// </summary>
        /// <remarks></remarks>
        public static bool UsePool = false;

        public static int CacheSize = 4096;

        #endregion
    }
}
