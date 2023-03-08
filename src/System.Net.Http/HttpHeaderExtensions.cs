using System.Collections.Generic;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    internal static class HttpHeaderExtensions
    {
        public static void CopyTo(this HttpContentHeaders fromHeaders, HttpContentHeaders toHeaders)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> fromHeader in fromHeaders)
            {
                toHeaders.TryAddWithoutValidation(fromHeader.Key, fromHeader.Value);
            }
        }
    }
}
