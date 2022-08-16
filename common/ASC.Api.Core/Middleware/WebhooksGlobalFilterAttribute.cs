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

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Api.Core.Middleware;

[Scope]
public class WebhooksGlobalFilterAttribute : ResultFilterAttribute, IDisposable
{
    private readonly MemoryStream _stream;
    private Stream _bodyStream;
    private readonly IWebhookPublisher _webhookPublisher;
    private readonly ILogger<WebhooksGlobalFilterAttribute> _logger;
    private static readonly List<string> _methodList = new List<string> { "POST", "UPDATE", "DELETE" };

    public WebhooksGlobalFilterAttribute(IWebhookPublisher webhookPublisher, ILogger<WebhooksGlobalFilterAttribute> logger)
    {
        _stream = new MemoryStream();
        _webhookPublisher = webhookPublisher;
        _logger = logger;
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (!Skip(context.HttpContext))
        {
            _bodyStream = context.HttpContext.Response.Body;
            context.HttpContext.Response.Body = _stream;
        }

        base.OnResultExecuting(context);
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await base.OnResultExecutionAsync(context, next);

        if (context.Cancel || Skip(context.HttpContext))
        {
            return;
        }

        if (_stream != null && _stream.CanRead)
        {
            _stream.Position = 0;
            await _stream.CopyToAsync(_bodyStream);
            context.HttpContext.Response.Body = _bodyStream;

            try
            {
                var (method, routePattern) = GetData(context.HttpContext);

                var resultContent = Encoding.UTF8.GetString(_stream.ToArray());
                var eventName = $"method: {method}, route: {routePattern}";

                _webhookPublisher.Publish(eventName, JsonSerializer.Serialize(context.HttpContext.Request.Headers.ToDictionary(r => r.Key, v => v.Value)), resultContent);
            }
            catch (Exception e)
            {
                _logger.ErrorWithException(e);
            }
        }
    }

    public void Dispose()
    {
        if (_stream != null)
        {
            _stream.Dispose();
        }
    }

    private (string, string) GetData(HttpContext context)
    {
        var method = context.Request.Method;
        var endpoint = (RouteEndpoint)context.GetEndpoint();
        var routePattern = endpoint?.RoutePattern.RawText;

        return (method, routePattern);
    }

    private bool Skip(HttpContext context)
    {
        var (method, routePattern) = GetData(context);

        if (!_methodList.Contains(method))
        {
            return true;
        }

        if (routePattern == null)
        {
            return true;
        }

        return false;
    }
}