using System;
using System.Net.Http;

namespace Rock.Logging
{
    public class ResponseReceivedEventArgs : EventArgs
    {
        private readonly HttpResponseMessage _response;

        public ResponseReceivedEventArgs(HttpResponseMessage response)
        {
            _response = response;
        }

        public virtual HttpResponseMessage Response
        {
            get { return _response; }
        }
    }
}