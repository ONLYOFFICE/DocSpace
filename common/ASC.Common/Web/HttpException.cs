using System;
using System.Net;

namespace ASC.Common.Web
{
    public class HttpException : Exception
    {
        public int StatusCode { get; }

        public HttpException(int httpStatusCode) => StatusCode = httpStatusCode;

        public HttpException(HttpStatusCode httpStatusCode) => StatusCode = (int)httpStatusCode;

        public HttpException(int httpStatusCode, string message) : base(message) => StatusCode = httpStatusCode;

        public HttpException(HttpStatusCode httpStatusCode, string message) : base(message) => StatusCode = (int)httpStatusCode;

        public HttpException(int httpStatusCode, string message, Exception inner) : base(message, inner) => StatusCode = httpStatusCode;

        public HttpException(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner) =>
            StatusCode = (int)httpStatusCode;
    }
}