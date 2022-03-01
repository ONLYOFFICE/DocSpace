namespace ASC.Api.Core.Middleware;

public abstract class CommonApiResponse
{
    public int Status { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    protected CommonApiResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;

    }
}

public class ErrorApiResponse : CommonApiResponse
{
    public CommonApiError Error { get; set; }

    protected internal ErrorApiResponse(HttpStatusCode statusCode, Exception error, string message, bool withStackTrace) : base(statusCode)
    {
        Status = 1;
        Error = CommonApiError.FromException(error, message, withStackTrace);
    }
}

public class SuccessApiResponse : CommonApiResponse
{
    public int? Count { get; set; }
    public long? Total { get; set; }
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

public class CommonApiError
{
    public string Message { get; set; }
    public string Type { get; set; }
    public string Stack { get; set; }
    public int Hresult { get; set; }

    public static CommonApiError FromException(Exception exception, string message, bool withStackTrace)
    {
        var result = new CommonApiError()
        {
            Message = message ?? exception.Message
        };

        if (withStackTrace)
        {
            result.Type = exception.GetType().ToString();
            result.Stack = exception.StackTrace;
            result.Hresult = exception.HResult;
        }

        return result;
    }
}
