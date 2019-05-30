using System;
using System.Collections;
using System.Net;
using System.Runtime.Serialization;

namespace ASC.Api.Core.Middleware
{
    [DataContract]
    public class CommonApiResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public int? Count { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public HttpStatusCode StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object Response { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CommonApiError Error { get; set; }

        protected CommonApiResponse(HttpStatusCode statusCode, object response = null, Exception error = null)
        {
            Status = Convert.ToInt32((int)statusCode >= 400);
            StatusCode = statusCode;
            Response = response;
            Error = CommonApiError.FromException(error);
        }

        public static CommonApiResponse Create(HttpStatusCode statusCode, object response = null)
        {
            return new CommonApiResponse(statusCode, response);
        }

        public static CommonApiResponse CreateError(HttpStatusCode statusCode, Exception error = null)
        {
            return new CommonApiResponse(statusCode, error: error);
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
