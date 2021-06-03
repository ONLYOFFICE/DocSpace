using System;

namespace JSIStudios.SimpleRESTServices.Server.EventArgs
{
    public class RESTRequestStartedEventArgs : System.EventArgs
    {
        public Guid RequestId { get; private set; }

        public string Request { get; private set; }

        public RESTRequestStartedEventArgs(Guid requestId, string request)
            : base()
        {
            RequestId = requestId;
            Request = request;
        }
    }
}