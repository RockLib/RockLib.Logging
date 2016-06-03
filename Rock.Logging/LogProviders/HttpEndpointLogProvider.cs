using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Rock.Net.Http;
using Rock.Serialization;

namespace Rock.Logging
{
    public class HttpEndpointLogProvider : ILogProvider
    {
        private const string DefaultContentType = "application/json";

        private readonly string _endpoint;
        private readonly LogLevel _loggingLevel;
        private readonly string _contentType;
        private readonly Func<ILogEntry, string> _serializeLogEntry; 
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpEndpointLogProvider(
            string endpoint,
            LogLevel loggingLevel,
            string contentType = DefaultContentType,
            ISerializer serializer = null,
            IHttpClientFactory httpClientFactory = null,
            bool serializeAsConcreteType = true)
        {
            serializer = serializer ?? GetDefaultSerializer();
            httpClientFactory = httpClientFactory ?? GetDefaultHttpClientFactory();

            _endpoint = endpoint;
            _loggingLevel = loggingLevel;
            _contentType = contentType;
            _serializeLogEntry =
                serializeAsConcreteType
                    ? (Func<ILogEntry, string>)(entry => serializer.SerializeToString(entry, entry.GetType()))
                    : entry => serializer.SerializeToString(entry);
            _httpClientFactory = httpClientFactory;
        }

        public event EventHandler<ResponseReceivedEventArgs> ResponseReceived;

        public LogLevel LoggingLevel
        {
            get { return _loggingLevel; }
        }

        public async Task WriteAsync(ILogEntry entry)
        {
            var serializedEntry = _serializeLogEntry(entry);

            var postContent = new StringContent(serializedEntry);
            postContent.Headers.ContentType = new MediaTypeHeaderValue(_contentType);

            using (var httpClient = _httpClientFactory.CreateHttpClient())
            {
                var response = await httpClient.PostAsync(_endpoint, postContent).ConfigureAwait(false);
                OnResponseReceived(response);
            }
        }

        protected virtual void OnResponseReceived(HttpResponseMessage response)
        {
            var handler = ResponseReceived;
            if (handler != null)
            {
                handler(this, new ResponseReceivedEventArgs(response));
            }
        }

        private static ISerializer GetDefaultSerializer()
        {
            return DefaultJsonSerializer.Current;
        }

        private static IHttpClientFactory GetDefaultHttpClientFactory()
        {
            return DefaultHttpClientFactory.Current;
        }
    }
}
