using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASC.Api.Core.Middleware
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(new ErrorApiResponse((HttpStatusCode)context.HttpContext.Response.StatusCode, context.Exception));
        }
    }

    public class CustomResponseFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult result)
            {
                context.HttpContext.Items.TryGetValue("TotalCount", out var total);
                result.Value = new SuccessApiResponse((HttpStatusCode)context.HttpContext.Response.StatusCode, result.Value, (long?)total);
            }

            base.OnResultExecuting(context);
        }
    }
}