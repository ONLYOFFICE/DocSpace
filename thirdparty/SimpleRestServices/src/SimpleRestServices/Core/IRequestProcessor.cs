using System;
using System.Collections.Specialized;
using System.Net;
using JSIStudios.SimpleRESTServices.Server.EventArgs;

namespace JSIStudios.SimpleRESTServices.Core
{
    public interface IRequestProcessor
    {
        event EventHandler<RESTRequestStartedEventArgs> RequestStarted;
        event EventHandler<RESTRequestCompletedEventArgs> RequestCompleted;
        event EventHandler<RESTRequestErrorEventArgs> OnError;

        void Execute(Action<Guid> callBack, HttpStatusCode successStatus = HttpStatusCode.OK);
        void Execute(Action<Guid> callBack, NameValueCollection responseHeaders, HttpStatusCode successStatus = HttpStatusCode.OK);

        TResult Execute<TResult>(Func<Guid, TResult> callBack, HttpStatusCode successStatus = HttpStatusCode.OK);
        TResult Execute<TResult>(Func<Guid, TResult> callBack, NameValueCollection responseHeaders, HttpStatusCode successStatus = HttpStatusCode.OK);
    }
}