using System;

namespace JSIStudios.SimpleRESTServices.Core.Exceptions
{
    public class HttpHeaderNotFoundException : Exception
    {
        public string Name { get; set; }
        public HttpHeaderNotFoundException(string name, string message)
            : base(message)
        {
            Name = name;
        }

        public HttpHeaderNotFoundException(string name)
        {
            Name = name;
        }

        public HttpHeaderNotFoundException()
        {
        }
    }
}