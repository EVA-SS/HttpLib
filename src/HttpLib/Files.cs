using System.IO;
using System.Web;

namespace HttpLib
{
    /// <summary>
    /// Files is a simple data structre that holds a file name, and stream
    /// </summary>
    public sealed class Files
    {
        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public Stream Stream { get; private set; }
        public long Size { get; private set; }

        /// <summary>
        /// Create a new NamedFileStream
        /// </summary>
        /// <param name="name">Form name for file</param>
        /// <param name="fileName">Name of file</param>
        /// <param name="contentType">Content type of file</param>
        /// <param name="stream">File Stream</param>
        public Files(string name, string fileName, string contentType, Stream stream)
        {
            this.Name = name;
            this.FileName = fileName;
            this.ContentType = contentType;
            this.Size = stream.Length;
            this.Stream = stream;
        }
        public Files(string name, string fullName)
        {
            this.Name = name;
            FileInfo fileInfo = new FileInfo(fullName);
            this.FileName = fileInfo.Name;

            string contentType = MimeMapping.GetMimeMapping(fullName);

            this.ContentType = contentType;
            this.Stream = File.OpenRead(fullName);
            this.Size = this.Stream.Length;
        }
        public Files(string fullName) : this("file", fullName)
        {

        }
    }
}
