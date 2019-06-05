using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Api.Core.Middleware
{
    public class ResponseWrapper
    {
        private readonly RequestDelegate next;

        public ResponseWrapper(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Exception error = null;
            var currentBody = context.Response.Body;

            using var memoryStream = new MemoryStream();

            context.Response.Body = memoryStream;

            try
            {
                await next(context);

                if(context.Response.StatusCode == 401)
                {
                    error = new AuthenticationException(HttpStatusCode.Unauthorized.ToString());
                }
            }
            catch(AuthenticationException exception)
            {
                context.Response.StatusCode = 401;
                error = exception;
            }
            catch(Exception exception)
            {
                context.Response.StatusCode = 500;
                error = exception;
            }

            context.Response.Body = currentBody;
            memoryStream.Seek(0, SeekOrigin.Begin);

            ResponseParser responseParser;

            switch (context.Request.RouteValues["format"])
            {
                case "xml":
                    responseParser = new XmlResponseParser();
                    break;
                case "json":
                default:
                    responseParser = new JsonResponseParser();
                    break;
            }

            var readToEnd = new StreamReader(memoryStream).ReadToEnd();
            await context.Response.WriteAsync(responseParser.WrapAndWrite((HttpStatusCode)context.Response.StatusCode, readToEnd, error));
        }

    }

    public static class ResponseWrapperExtensions
    {
        public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseWrapper>();
        }
    }
}