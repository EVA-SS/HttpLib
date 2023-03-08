using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace System.Net.Http
{
    public static class HttpClientFactory
    {
        /// <summary>
        /// Creates a new <see cref="T:System.Net.Http.HttpClient" /> instance configured with the handlers provided and with an
        /// <see cref="T:System.Net.Http.HttpClientHandler" /> as the innermost handler.
        /// </summary>
        /// <param name="handlers">An ordered list of <see cref="T:System.Net.Http.DelegatingHandler" /> instances to be invoked as an 
        /// <see cref="T:System.Net.Http.HttpRequestMessage" /> travels from the <see cref="T:System.Net.Http.HttpClient" /> to the network and an 
        /// <see cref="T:System.Net.Http.HttpResponseMessage" /> travels from the network back to <see cref="T:System.Net.Http.HttpClient" />.
        /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
        /// an outbound request message but last for an inbound response message.</param>
        /// <returns>An <see cref="T:System.Net.Http.HttpClient" /> instance with the configured handlers.</returns>
        public static HttpClient Create(params DelegatingHandler[] handlers)
        {
            return Create(new HttpClientHandler(), handlers);
        }

        /// <summary>
        /// Creates a new <see cref="T:System.Net.Http.HttpClient" /> instance configured with the handlers provided and with the
        /// provided <paramref name="innerHandler" /> as the innermost handler.
        /// </summary>
        /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
        /// <param name="handlers">An ordered list of <see cref="T:System.Net.Http.DelegatingHandler" /> instances to be invoked as an 
        /// <see cref="T:System.Net.Http.HttpRequestMessage" /> travels from the <see cref="T:System.Net.Http.HttpClient" /> to the network and an 
        /// <see cref="T:System.Net.Http.HttpResponseMessage" /> travels from the network back to <see cref="T:System.Net.Http.HttpClient" />.
        /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
        /// an outbound request message but last for an inbound response message.</param>
        /// <returns>An <see cref="T:System.Net.Http.HttpClient" /> instance with the configured handlers.</returns>
        public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
        {
            return new HttpClient(CreatePipeline(innerHandler, handlers));
        }

        /// <summary>
        /// Creates an instance of an <see cref="T:System.Net.Http.HttpMessageHandler" /> using the <see cref="T:System.Net.Http.DelegatingHandler" /> instances
        /// provided by <paramref name="handlers" />. The resulting pipeline can be used to manually create <see cref="T:System.Net.Http.HttpClient" />
        /// or <see cref="T:System.Net.Http.HttpMessageInvoker" /> instances with customized message handlers.
        /// </summary>
        /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
        /// <param name="handlers">An ordered list of <see cref="T:System.Net.Http.DelegatingHandler" /> instances to be invoked as part 
        /// of sending an <see cref="T:System.Net.Http.HttpRequestMessage" /> and receiving an <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
        /// an outbound request message but last for an inbound response message.</param>
        /// <returns>The HTTP message channel.</returns>
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
        {
            if (innerHandler == null)
            {
                throw Error.ArgumentNull("innerHandler");
            }
            if (handlers == null)
            {
                return innerHandler;
            }
            HttpMessageHandler httpMessageHandler = innerHandler;
            foreach (DelegatingHandler item in handlers.Reverse())
            {
                if (item == null)
                {
                    throw Error.Argument("handlers", "The '{0}' list is invalid because it contains one or more null items.", typeof(DelegatingHandler).Name);
                }
                if (item.InnerHandler != null)
                {
                    throw Error.Argument("handlers", "The '{0}' list is invalid because the property '{1}' of '{2}' is not null.", typeof(DelegatingHandler).Name, "InnerHandler", item.GetType().Name);
                }
                item.InnerHandler = httpMessageHandler;
                httpMessageHandler = item;
            }
            return httpMessageHandler;
        }
    }
}
