using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace ASC.Api.Core.Middleware
{
    [DataContract]
    public abstract class CommonApiResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
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
        [DataMember(EmitDefaultValue = false, Order = 3)]
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
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public int? Count { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public long? Total { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public object Response { get; set; }

        protected internal SuccessApiResponse(HttpStatusCode statusCode, object response, long? total = null, int? count = null) : base(statusCode)
        {
            Status = 0;
            Response = response;
            Total = total;

            if (count.HasValue)
            {
                Count = count;
            }
            else
            {
                if (response is List<object> list)
                {
                    Count = list.Count;
                }
                else if (response is IEnumerable<object> collection)
                {
                    Count = collection.Count();
                }
                else if (response == null)
                {
                    Count = 0;
                }
                else
                {
                    Count = 1;
                }
            }
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
