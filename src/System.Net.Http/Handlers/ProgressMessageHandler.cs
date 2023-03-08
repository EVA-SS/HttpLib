using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Handlers
{
    /// <summary>
    /// The <see cref="T:System.Net.Http.Handlers.ProgressMessageHandler" /> provides a mechanism for getting progress event notifications
    /// when sending and receiving data in connection with exchanging HTTP requests and responses.
    /// Register event handlers for the events <see cref="E:System.Net.Http.Handlers.ProgressMessageHandler.HttpSendProgress" /> and <see cref="E:System.Net.Http.Handlers.ProgressMessageHandler.HttpReceiveProgress" />
    /// to see events for data being sent and received.
    /// </summary>
    public class ProgressMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Occurs every time the client sending data is making progress.
        /// </summary>
        public event EventHandler<HttpProgressEventArgs> HttpSendProgress;

        /// <summary>
        /// Occurs every time the client receiving data is making progress.
        /// </summary>
        public event EventHandler<HttpProgressEventArgs> HttpReceiveProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.Http.Handlers.ProgressMessageHandler" /> class.
        /// </summary>
        public ProgressMessageHandler()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.Http.Handlers.ProgressMessageHandler" /> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler to which this handler submits requests.</param>
        public ProgressMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddRequestProgress(request);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (this.HttpReceiveProgress != null && response != null && response.Content != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await AddResponseProgressAsync(request, response);
            }
            return response;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Net.Http.Handlers.ProgressMessageHandler.HttpSendProgress" /> event.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="e">The <see cref="T:System.Net.Http.Handlers.HttpProgressEventArgs" /> instance containing the event data.</param>
        protected internal virtual void OnHttpRequestProgress(HttpRequestMessage request, HttpProgressEventArgs e)
        {
            if (this.HttpSendProgress != null)
            {
                this.HttpSendProgress(request, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Net.Http.Handlers.ProgressMessageHandler.HttpReceiveProgress" /> event.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="e">The <see cref="T:System.Net.Http.Handlers.HttpProgressEventArgs" /> instance containing the event data.</param>
        protected internal virtual void OnHttpResponseProgress(HttpRequestMessage request, HttpProgressEventArgs e)
        {
            if (this.HttpReceiveProgress != null)
            {
                this.HttpReceiveProgress(request, e);
            }
        }

        private void AddRequestProgress(HttpRequestMessage request)
        {
            if (this.HttpSendProgress != null && request != null && request.Content != null)
            {
                HttpContent httpContent2 = (request.Content = new ProgressContent(request.Content, this, request));
            }
        }

        private async Task<HttpResponseMessage> AddResponseProgressAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            HttpContent httpContent = new StreamContent(new ProgressStream(await response.Content.ReadAsStreamAsync(), this, request, response));
            response.Content.Headers.CopyTo(httpContent.Headers);
            response.Content = httpContent;
            return response;
        }
    }
}
