using System.IO;

namespace HttpLib
{
    /// <summary>
    /// 文件
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="contentType">文件类型</param>
        /// <param name="stream">字节流</param>
        public Files(string name, string fileName, string contentType, byte[] data)
        {
            Name = name;
            FileName = fileName;
            ContentType = contentType;
            Size = data.Length;
            Stream = new MemoryStream(data);
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="fullName">文件路径</param>
        public Files(string name, string fullName)
        {
            Name = name;
            var fileInfo = new FileInfo(fullName);
            FileName = fileInfo.Name;
            ContentType = MimeMapping.GetMimeMapping(fullName);
            Stream = File.OpenRead(fullName);
            Size = Stream.Length;
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="fullName">文件路径</param>
        public Files(string fullName) : this("file", fullName) { }
    }
}