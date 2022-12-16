using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class Config
    {
        public static List<Val>? _headers = null;

        /// <summary>
        /// 设置全局请求的头
        /// </summary>
        /// <param name="vals">多个请求头</param>
        public static void header(params Val[] vals)
        {
            if (_headers == null)
                _headers = new List<Val>(vals.Length);
            foreach (var val in vals)
                setVals(ref _headers, val);
        }

        public static void setVals(ref List<Val> obj, Val val)
        {
            setVals(ref obj, val.Key, val.Value);
        }
        public static void setVals(ref List<Val> obj, string key, string val)
        {
            var find = obj.Find(ab => ab.Key == key);
            if (find == null) obj.Add(new Val(key, val));
            else
            {
                if (val != null) find.SetValue(val);
                else obj.Remove(find);
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

        public static int CacheSize = 4096;
    }
}
