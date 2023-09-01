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

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<CustomExceptionFilterAttribute> _logger;

    public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        var status = (HttpStatusCode)context.HttpContext.Response.StatusCode;
        string message = null;

        if (status == HttpStatusCode.OK)
        {
            status = HttpStatusCode.InternalServerError;
        }

        var withStackTrace = true;

        var exception = context.Exception.GetBaseException();

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
            case BruteForceCredentialException:
            case RecaptchaException:
                status = HttpStatusCode.Forbidden;
                withStackTrace = false;
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

        _logger.LogCritical(exception,
    $"error during executing {context.HttpContext.Request?.Method}: {context.HttpContext.Request?.Path.Value}");

        var result = new ObjectResult(new ErrorApiResponse(status, exception, message, withStackTrace))
        {
            StatusCode = (int)status
        };

        context.Result = result;
    }
}

public class CustomResponseFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult result)
        {
            result.Value = new SuccessApiResponse(context.HttpContext, result.Value);
        }

        base.OnResultExecuting(context);
    }
}