using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Handlers
{
    /// <summary>
    /// This implementation of <see cref="T:System.Net.Http.Internal.DelegatingStream" /> registers how much data has been 
    /// read (received) versus written (sent) for a particular HTTP operation. The implementation
    /// is client side in that the total bytes to send is taken from the request and the total
    /// bytes to read is taken from the response. In a server side scenario, it would be the
    /// other way around (reading the request and writing the response).
    /// </summary>
    internal class ProgressStream : System.Net.Http.Internal.DelegatingStream
    {
        private readonly ProgressMessageHandler _handler;

        private readonly HttpRequestMessage _request;

        private long _bytesReceived;

        private long? _totalBytesToReceive;

        private long _bytesSent;

        private long? _totalBytesToSend;

        public ProgressStream(Stream innerStream, ProgressMessageHandler handler, HttpRequestMessage request, HttpResponseMessage response)
            : base(innerStream)
        {
            if (request.Content != null)
            {
                _totalBytesToSend = request.Content.Headers.ContentLength;
            }
            if (response != null && response.Content != null)
            {
                _totalBytesToReceive = response.Content.Headers.ContentLength;
            }
            _handler = handler;
            _request = request;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = base.InnerStream.Read(buffer, offset, count);
            ReportBytesReceived(num, null);
            return num;
        }

        public override int ReadByte()
        {
            int num = base.InnerStream.ReadByte();
            ReportBytesReceived((num != -1) ? 1 : 0, null);
            return num;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int num = await base.InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
            ReportBytesReceived(num, null);
            return num;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.InnerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int num = base.InnerStream.EndRead(asyncResult);
            ReportBytesReceived(num, asyncResult.AsyncState);
            return num;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.InnerStream.Write(buffer, offset, count);
            ReportBytesSent(count, null);
        }

        public override void WriteByte(byte value)
        {
            base.InnerStream.WriteByte(value);
            ReportBytesSent(1, null);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await base.InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
            ReportBytesSent(count, null);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return new ProgressWriteAsyncResult(base.InnerStream, this, buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            ProgressWriteAsyncResult.End(asyncResult);
        }

        internal void ReportBytesSent(int bytesSent, object userState)
        {
            if (bytesSent > 0)
            {
                _bytesSent += bytesSent;
                int progressPercentage = 0;
                if (_totalBytesToSend.HasValue && _totalBytesToSend != 0)
                {
                    progressPercentage = (int)(100 * _bytesSent / _totalBytesToSend).Value;
                }
                _handler.OnHttpRequestProgress(_request, new HttpProgressEventArgs(progressPercentage, userState, _bytesSent, _totalBytesToSend));
            }
        }

        private void ReportBytesReceived(int bytesReceived, object userState)
        {
            if (bytesReceived > 0)
            {
                _bytesReceived += bytesReceived;
                int progressPercentage = 0;
                if (_totalBytesToReceive.HasValue && _totalBytesToReceive != 0)
                {
                    progressPercentage = (int)(100 * _bytesReceived / _totalBytesToReceive).Value;
                }
                _handler.OnHttpResponseProgress(_request, new HttpProgressEventArgs(progressPercentage, userState, _bytesReceived, _totalBytesToReceive));
            }
        }
    }
}
