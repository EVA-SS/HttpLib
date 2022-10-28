namespace HttpLib
{
    public static class HttpDownExtension
    {
        /// <summary>
        /// 下载
        /// </summary>
        public static HttpDown downLoad(this HttpCore core, string savePath)
        {
            return new HttpDown(core, savePath);
        }
    }
}
