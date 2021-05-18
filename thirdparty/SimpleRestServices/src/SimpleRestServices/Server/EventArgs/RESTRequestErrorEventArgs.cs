using System;

namespace JSIStudios.SimpleRESTServices.Server.EventArgs
{
    public class RESTRequestErrorEventArgs : System.EventArgs
    {
        public Guid RequestId { get; private set; }

        public Exception Error { get; private set; }

        public RESTRequestErrorEventArgs(Guid requestId, Exception error)
            : base()
        {
            RequestId = requestId;
            Error = error;
        }
    }
}