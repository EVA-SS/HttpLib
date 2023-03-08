using System.IO;
using System.Net.Http.Internal;

namespace System.Net.Http.Handlers
{
    internal class ProgressWriteAsyncResult : AsyncResult
    {
        private static readonly AsyncCallback _writeCompletedCallback = WriteCompletedCallback;

        private readonly Stream _innerStream;

        private readonly ProgressStream _progressStream;

        private readonly int _count;

        public ProgressWriteAsyncResult(Stream innerStream, ProgressStream progressStream, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _innerStream = innerStream;
            _progressStream = progressStream;
            _count = count;
            try
            {
                IAsyncResult asyncResult = innerStream.BeginWrite(buffer, offset, count, _writeCompletedCallback, this);
                if (asyncResult.CompletedSynchronously)
                {
                    WriteCompleted(asyncResult);
                }
            }
            catch (Exception exception)
            {
                Complete(completedSynchronously: true, exception);
            }
        }

        private static void WriteCompletedCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }
            ProgressWriteAsyncResult progressWriteAsyncResult = (ProgressWriteAsyncResult)result.AsyncState;
            try
            {
                progressWriteAsyncResult.WriteCompleted(result);
            }
            catch (Exception exception)
            {
                progressWriteAsyncResult.Complete(completedSynchronously: false, exception);
            }
        }

        private void WriteCompleted(IAsyncResult result)
        {
            _innerStream.EndWrite(result);
            _progressStream.ReportBytesSent(_count, base.AsyncState);
            Complete(result.CompletedSynchronously);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ProgressWriteAsyncResult>(result);
        }
    }
}
