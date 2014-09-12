using System.Net.Http;

namespace Rock.Logging
{
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }
    }
}