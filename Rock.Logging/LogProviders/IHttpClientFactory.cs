using System.Net.Http;

namespace Rock.Logging
{
    public interface IHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}