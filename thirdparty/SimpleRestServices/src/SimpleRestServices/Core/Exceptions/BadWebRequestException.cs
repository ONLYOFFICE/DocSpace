using System;

namespace JSIStudios.SimpleRESTServices.Core.Exceptions
{
    public class BadWebRequestException : Exception
    {
        public BadWebRequestException(string message)
            : base(message)
        {
        }

        public BadWebRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}