using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Rock.Defaults.Implementation;
using Rock.Logging.Configuration;
using Rock.Serialization;

namespace Rock.Logging
{
    public class HttpEndpointLogProvider : ILogProvider
    {
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IHttpClientFactory> _httpClientFactory;

        public HttpEndpointLogProvider()
        {
            ContentType = "application/json";

            _serializer = new Lazy<ISerializer>(
                () =>
                    string.IsNullOrEmpty(SerializerType)
                        ? Default.JsonSerializer
                        : CreateInstance<ISerializer>(SerializerType, "SerializerType"));

            _httpClientFactory = new Lazy<IHttpClientFactory>(
                () =>
                    string.IsNullOrEmpty(HttpClientFactoryType)
                        ? new DefaultHttpClientFactory()
                        : CreateInstance<IHttpClientFactory>(HttpClientFactoryType, "HttpClientFactoryType"));
        }

        public HttpEndpointLogProvider(
            string endpoint,
            string contentType = "application/json",
            ISerializer serializer = null,
            IHttpClientFactory httpClientFactory = null)
        {
            serializer = serializer ?? Default.JsonSerializer;
            httpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();

            Endpoint = endpoint;
            ContentType = contentType;
            SerializerType = serializer.GetType().AssemblyQualifiedName;
            HttpClientFactoryType = httpClientFactory.GetType().AssemblyQualifiedName;

            _serializer = new Lazy<ISerializer>(() => serializer);
            _httpClientFactory = new Lazy<IHttpClientFactory>(() => httpClientFactory);
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
            postContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

            using (var httpClient = _httpClientFactory.Value.CreateHttpClient())
            {
                var response = await httpClient.PostAsync(Endpoint, postContent);
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

        private static T CreateInstance<T>(string assemblyQualifiedType, string propertyName)
        {
            var type = Type.GetType(assemblyQualifiedType);

            if (type == null)
            {
                throw new LogConfigurationException(
                    string.Format(
                        "Unable to locate a type by name of '{0}' for the '{1}' property." +
                        " Use the assembly qualified name of the the type in order to ensure" +
                        " that the type can be located.", assemblyQualifiedType, propertyName));
            }

            if (!typeof(T).IsAssignableFrom(type)
                || typeof(T) == type)
            {
                throw new LogConfigurationException(
                    string.Format(
                        "The '{0}' type does not implement the '{1}' interface.", type, typeof(T)));
            }

            var constructors = type.GetConstructors();

            Func<ConstructorInfo, bool> isDefaultConstructor =
                c => c.GetParameters().Length == 0;

            if (constructors.Any(isDefaultConstructor))
            {
                return (T)Activator.CreateInstance(type);
            }

            // Also, if there is a public constructor where all of its parameters have
            // a default value, use it.
            Func<ConstructorInfo, bool> allParametersHaveDefaultValue =
                c => c.GetParameters().All(p => p.HasDefaultValue);

            if (constructors.Any(allParametersHaveDefaultValue))
            {
                var parameters =
                    constructors.First(allParametersHaveDefaultValue)
                        .GetParameters();

                var args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].DefaultValue;
                }

                return (T)Activator.CreateInstance(type, args);
            }

            throw new LogConfigurationException(
                string.Format(
                    "Unable to find suitable constructor for {0}. There must be either " +
                    "a) a public parameterless constructor; or b) a public constructor " +
                    "whose parameters all have default values.", type));
        }
    }
}
