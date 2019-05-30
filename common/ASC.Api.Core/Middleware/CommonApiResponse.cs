using System;
using System.Collections;
using System.Net;
using System.Runtime.Serialization;

namespace ASC.Api.Core.Middleware
{
    [DataContract]
    public abstract class CommonApiResponse
    {
        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public HttpStatusCode StatusCode { get; set; }

        protected CommonApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public static SuccessApiResponse Create(HttpStatusCode statusCode, object response)
        {
            return new SuccessApiResponse(statusCode, response);
        }

        public static ErrorApiResponse CreateError(HttpStatusCode statusCode, Exception error)
        {
            return new ErrorApiResponse(statusCode, error);
        }
    }

    [DataContract]
    public class ErrorApiResponse : CommonApiResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public CommonApiError Error { get; set; }

        protected internal ErrorApiResponse(HttpStatusCode statusCode, Exception error) : base(statusCode)
        {
            Status = 1;
            Error = CommonApiError.FromException(error);
        }
    }

    [DataContract]
    public class SuccessApiResponse : CommonApiResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public int? Count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object Response { get; set; }

        protected internal SuccessApiResponse(HttpStatusCode statusCode, object response) : base(statusCode)
        {
            Status = 0;
            Response = response;
        }
    }

    [DataContract]
    public class CommonApiError
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Type Type { get; set; }

        [DataMember]
        public string Stack { get; set; }

        [DataMember]
        public int Hresult { get; set; }

        [DataMember]
        public IDictionary Data { get; set; }

        public static CommonApiError FromException(Exception exception)
        {
            return new CommonApiError()
            {
                Message = exception.Message,
                Type = exception.GetType(),
                Stack = exception.StackTrace,
                Hresult = exception.HResult,
                Data = exception.Data
            };
        }
    }
}
