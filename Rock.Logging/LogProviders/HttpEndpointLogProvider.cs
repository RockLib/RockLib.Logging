using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Rock.Defaults.Implementation;
using Rock.Reflection;
using Rock.Serialization;

namespace Rock.Logging
{
    public class HttpEndpointLogProvider : ILogProvider
    {
        private const string DefaultContentType = "application/json";

        private static ISerializer GetDefaultSerializer()
        {
            return Default.JsonSerializer;
        }

        private static IHttpClientFactory GetDefaultHttpClientFactory()
        {
            return new DefaultHttpClientFactory();
        }

        private readonly Lazy<string> _endpoint;
        private readonly Lazy<string> _contentType;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IHttpClientFactory> _httpClientFactory;

        public HttpEndpointLogProvider()
        {
            _endpoint = new Lazy<string>(() => Endpoint);
            _contentType = new Lazy<string>(() => ContentType);
            _serializer = new Lazy<ISerializer>(() => SlowFactory.CreateInstance<ISerializer>(SerializerType));
            _httpClientFactory = new Lazy<IHttpClientFactory>(() => SlowFactory.CreateInstance<IHttpClientFactory>(HttpClientFactoryType));
            
            ContentType = DefaultContentType;
            SerializerType = GetDefaultSerializer().GetType().AssemblyQualifiedName;
            HttpClientFactoryType = GetDefaultHttpClientFactory().GetType().AssemblyQualifiedName;
        }

        public HttpEndpointLogProvider(
            string endpoint,
            string contentType = DefaultContentType,
            ISerializer serializer = null,
            IHttpClientFactory httpClientFactory = null)
        {
            serializer = serializer ?? GetDefaultSerializer();
            httpClientFactory = httpClientFactory ?? GetDefaultHttpClientFactory();

            _endpoint = new Lazy<string>(() => endpoint);
            _contentType = new Lazy<string>(() => contentType);
            _serializer = new Lazy<ISerializer>(() => serializer);
            _httpClientFactory = new Lazy<IHttpClientFactory>(() => httpClientFactory);

            Endpoint = endpoint;
            ContentType = contentType;
            SerializerType = serializer.GetType().AssemblyQualifiedName;
            HttpClientFactoryType = httpClientFactory.GetType().AssemblyQualifiedName;
        }

        public event EventHandler<ResponseReceivedEventArgs> ResponseReceived;

        public string Endpoint { get; set; }
        public string ContentType { get; set; }
        public string SerializerType { get; set; }
        public string HttpClientFactoryType { get; set; }

        public async Task WriteAsync(LogEntry entry)
        {
            var serializedEntry = _serializer.Value.SerializeToString(entry);

            var postContent = new StringContent(serializedEntry);
            postContent.Headers.ContentType = new MediaTypeHeaderValue(_contentType.Value);

            using (var httpClient = _httpClientFactory.Value.CreateHttpClient())
            {
                var response = await httpClient.PostAsync(_endpoint.Value, postContent);
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
    }
}
