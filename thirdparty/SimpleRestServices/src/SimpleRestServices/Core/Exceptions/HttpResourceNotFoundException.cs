using System;

namespace JSIStudios.SimpleRESTServices.Core.Exceptions
{
    public class HttpResourceNotFoundException : Exception
    {
        public HttpResourceNotFoundException(string message)
            : base(message)
        {
        }
    }
}