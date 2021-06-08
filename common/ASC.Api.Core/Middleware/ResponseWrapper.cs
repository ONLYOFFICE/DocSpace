using System;
using System.Net;
using System.Security;

using ASC.Common.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASC.Api.Core.Middleware
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var status = (HttpStatusCode)context.HttpContext.Response.StatusCode;
            string message = null;

            if (status == HttpStatusCode.OK)
            {
                status = HttpStatusCode.InternalServerError;
            }

            switch (context.Exception)
            {
                case ItemNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = "The record could not be found";
                    break;
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = "Invalid arguments";
                    break;
                case SecurityException:
                    status = HttpStatusCode.Forbidden;
                    message = "Access denied";
                    break;
                case InvalidOperationException:
                    status = HttpStatusCode.Forbidden;
                    break;
            }

            var result = new ObjectResult(new ErrorApiResponse(status, context.Exception, message))
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