// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Api.Core.Middleware;

// problem: https://github.com/aspnet/Logging/issues/677
public class UnhandledExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public UnhandledExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context,
                            ILogger<UnhandledExceptionMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (LogError(ex))
        {
           await OnException(context, ex);
        }

        bool LogError(Exception ex)
        {
            logger.LogError(ex,
                $"Request {context.Request?.Method}: {context.Request?.Path.Value} failed");

            return true;
        }

    }

    public async Task OnException(HttpContext context, Exception exception)
    {
        var status = (HttpStatusCode)context.Response.StatusCode;
        string message = null;

        if (status == HttpStatusCode.OK)
        {
            status = HttpStatusCode.InternalServerError;
        }

        var withStackTrace = true;
                
        switch (exception)
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
            case AuthenticationException:
                status = HttpStatusCode.Unauthorized;
                withStackTrace = false;
                break;
            case InvalidOperationException:
                status = HttpStatusCode.Forbidden;
                break;
            case TenantQuotaException:
            case BillingNotFoundException:
                status = HttpStatusCode.PaymentRequired;
                break;
        }

        var result = new ErrorApiResponse(status, exception, message, withStackTrace);      

        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsJsonAsync(result);
    }
}

public static class UnhandledExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseUnhandledExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnhandledExceptionMiddleware>();
    }
}