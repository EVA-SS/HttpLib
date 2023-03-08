using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http.Handlers
{
    /// <summary>
    /// Wraps an inner <see cref="T:System.Net.Http.HttpContent" /> in order to insert a <see cref="T:System.Net.Http.Handlers.ProgressStream" /> on writing data.
    /// </summary>
    internal class ProgressContent : HttpContent
    {
        private readonly HttpContent _innerContent;

        private readonly ProgressMessageHandler _handler;

        private readonly HttpRequestMessage _request;

        public ProgressContent(HttpContent innerContent, ProgressMessageHandler handler, HttpRequestMessage request)
        {
            _innerContent = innerContent;
            _handler = handler;
            _request = request;
            innerContent.Headers.CopyTo(base.Headers);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            ProgressStream stream2 = new ProgressStream(stream, _handler, _request, null);
            return _innerContent.CopyToAsync(stream2);
        }

        protected override bool TryComputeLength(out long length)
        {
            long? contentLength = _innerContent.Headers.ContentLength;
            if (contentLength.HasValue)
            {
                length = contentLength.Value;
                return true;
            }
            length = -1L;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _innerContent.Dispose();
            }
        }
    }
}
