using System;

namespace Rock.Logging
{
    public class HttpEndpointLogProviderException : Exception
    {
        private readonly string _endpoint;
        private readonly string _contentType;

        public HttpEndpointLogProviderException(
            string message,
            Exception innerException,
            string endpoint,
            string contentType)
            : base(message, innerException)
        {
            _endpoint = endpoint;
            _contentType = contentType;
        }

        public string Endpoint
        {
            get { return _endpoint; }
        }

        public string ContentType
        {
            get { return _contentType; }
        }
    }
}