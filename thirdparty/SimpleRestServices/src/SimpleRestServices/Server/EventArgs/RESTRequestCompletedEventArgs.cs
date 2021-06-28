using System;

namespace JSIStudios.SimpleRESTServices.Server.EventArgs
{
    public class RESTRequestCompletedEventArgs : System.EventArgs
    {
        public Guid RequestId { get; private set; }

        public object Response { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public RESTRequestCompletedEventArgs(Guid requestId, object response, TimeSpan executionTime)
            : base()
        {
            RequestId = requestId;
            Response = response;
            ExecutionTime = executionTime;

        }
    }
}