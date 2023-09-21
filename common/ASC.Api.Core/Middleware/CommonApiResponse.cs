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

using Microsoft.AspNetCore.Http.Extensions;

namespace ASC.Api.Core.Middleware;

public abstract class CommonApiResponse
{
    public int Status { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public CommonApiResponse()
    {

    }

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
    private readonly HttpContext _httpContext;

    public object Response { get; set; }

    public int? Count
    {
        get
        {

            if (_httpContext.Items.TryGetValue("Count", out var count))
            {
                return (int?)count;
            }

            if (Response is List<object> list)
            {
                return list.Count;
            }

            if (Response is IEnumerable<object> collection)
            {
                return collection.Count();
            }

            if (Response == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

    public long? Total
    {
        get
        {
            if (_httpContext.Items.TryGetValue("TotalCount", out var total))
            {
                return (long?)total;
            }

            return null;
        }
    }

    public List<Link> Links { get; set; }

    public SuccessApiResponse()
    {

    }

    protected internal SuccessApiResponse(HttpContext httpContext, object response) : base((HttpStatusCode)httpContext.Response.StatusCode)
    {
        Status = 0;
        _httpContext = httpContext;
        Response = response;

        Links = new List<Link>(1)
        {
            new Link()
            {
                Href = httpContext.Request.GetDisplayUrl(),
                Action = httpContext.Request.Method
            }
        };
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

public class Link
{
    public string Href { get; set; }
    public string Action { get; set; }
}
