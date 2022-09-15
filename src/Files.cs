using System.IO;

namespace HttpLib
{
    /// <summary>
    /// 文件是一个简单的数据结构，它包含一个文件名和流
    /// </summary>
    public sealed class Files
    {
        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public Stream Stream { get; private set; }
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
            this.Name = name;
            this.FileName = fileName;
            this.ContentType = contentType;
            this.Size = data.Length;
            this.Stream = new MemoryStream(data);
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="fullName">文件路径</param>
        public Files(string name, string fullName)
        {
            this.Name = name;
            var fileInfo = new FileInfo(fullName);
            this.FileName = fileInfo.Name;
            string contentType = MimeMapping.GetMimeMapping(fullName);
            this.ContentType = contentType;
            this.Stream = File.OpenRead(fullName);
            this.Size = this.Stream.Length;
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="fullName">文件路径</param>
        public Files(string fullName) : this("file", fullName) { }
    }
}
