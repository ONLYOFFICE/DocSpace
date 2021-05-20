using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASC.Api.Core.Middleware
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var status = (HttpStatusCode)context.HttpContext.Response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                status = HttpStatusCode.InternalServerError;
            }

            var result = new ObjectResult(new ErrorApiResponse(status, context.Exception))
            {
                StatusCode = (int)status
            };

            context.Result = result; ;
        }
    }

    public class CustomResponseFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult result)
            {
                context.HttpContext.Items.TryGetValue("TotalCount", out var total);
                context.HttpContext.Items.TryGetValue("Count", out var count);
                result.Value = new SuccessApiResponse((HttpStatusCode)context.HttpContext.Response.StatusCode, result.Value, (long?)total, (int?)count);
            }

            base.OnResultExecuting(context);
        }
    }
}