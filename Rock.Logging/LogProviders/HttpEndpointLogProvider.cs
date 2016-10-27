using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Rock.Net.Http;
using Rock.Serialization;

namespace Rock.Logging
{
    public class HttpEndpointLogProvider : ILogProvider, IDisposable
    {
        private const string DefaultContentType = "application/json";
        private const int DefaultAdditionalHttpClientCycleMilliseconds = 0;

        private readonly string _endpoint;
        private readonly LogLevel _loggingLevel;
        private readonly string _contentType;
        private readonly int _additionalHttpClientCycleMilliseconds;
        private readonly ISerializer _serializer;
        private readonly Func<ILogEntry, string> _serializeLogEntry; 
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly Timer _httpClientCycleTimer;
        private HttpClient _workingHttpClient;
        private HttpClient _httpClientToDisposeOfNext;

        public HttpEndpointLogProvider(
            string endpoint,
            LogLevel loggingLevel,
            string contentType,
            ISerializer serializer,
            IHttpClientFactory httpClientFactory,
            bool serializeAsConcreteType)
            : this(endpoint, loggingLevel, contentType, serializer, httpClientFactory,
                  serializeAsConcreteType, DefaultAdditionalHttpClientCycleMilliseconds)
        {
        }

        public HttpEndpointLogProvider(
            string endpoint,
            LogLevel loggingLevel,
            string contentType = DefaultContentType,
            ISerializer serializer = null,
            IHttpClientFactory httpClientFactory = null,
            bool serializeAsConcreteType = true,
            int additionalHttpClientCycleMilliseconds = DefaultAdditionalHttpClientCycleMilliseconds)
        {
            _serializer = serializer ?? GetDefaultSerializer();
            _httpClientFactory = httpClientFactory ?? GetDefaultHttpClientFactory();

            _endpoint = endpoint;
            _loggingLevel = loggingLevel;
            _contentType = contentType;
            _additionalHttpClientCycleMilliseconds = additionalHttpClientCycleMilliseconds;
            _serializeLogEntry =
                serializeAsConcreteType
                    ? (Func<ILogEntry, string>)(entry => _serializer.SerializeToString(entry, entry.GetType()))
                    : entry => _serializer.SerializeToString(entry);

            _workingHttpClient = _httpClientFactory.CreateHttpClient();
            _httpClientCycleTimer = new Timer(CycleHttpClient, null, CycleDueTime, Timeout.InfiniteTimeSpan);
        }

        public event EventHandler<ResponseReceivedEventArgs> ResponseReceived;

        public LogLevel LoggingLevel
        {
            get { return _loggingLevel; }
        }

        public string Endpoint
        {
            get { return _endpoint; }
        }

        public string ContentType
        {
            get { return _contentType; }
        }

        public ISerializer Serializer
        {
            get { return _serializer; }
        }

        public IHttpClientFactory HttpClientFactory
        {
            get { return _httpClientFactory; }
        }

        public int AdditionalHttpClientCycleMilliseconds
        {
            get { return _additionalHttpClientCycleMilliseconds; }
        }

        private TimeSpan CycleDueTime =>
            _workingHttpClient.Timeout + TimeSpan.FromMilliseconds(1000 + _additionalHttpClientCycleMilliseconds);

        public async Task WriteAsync(ILogEntry entry)
        {
            var serializedEntry = _serializeLogEntry(entry);

            var postContent = new StringContent(serializedEntry);
            postContent.Headers.ContentType = new MediaTypeHeaderValue(_contentType);

            HttpResponseMessage response;

            try
            {
                response = await _workingHttpClient.PostAsync(_endpoint, postContent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new HttpEndpointLogProviderException(
                    "Error sending serialized log entry via HTTP POST.",
                    ex, _endpoint, _contentType);
            }

            OnResponseReceived(response);
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

        private void CycleHttpClient(object state)
        {
            var oldHttpClient = Interlocked.Exchange(ref _workingHttpClient, _httpClientFactory.CreateHttpClient());
            if (_httpClientToDisposeOfNext != null) _httpClientToDisposeOfNext.Dispose();
            _httpClientToDisposeOfNext = oldHttpClient;
            _httpClientCycleTimer.Change(CycleDueTime, Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _httpClientCycleTimer.Change(Timeout.Infinite, Timeout.Infinite);
            using (var waitHandle = new AutoResetEvent(false)) if (_httpClientCycleTimer.Dispose(waitHandle)) waitHandle.WaitOne();
            if (_httpClientToDisposeOfNext != null) _httpClientToDisposeOfNext.Dispose();
            _workingHttpClient.Dispose();
        }
    }
}
