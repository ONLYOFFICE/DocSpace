/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.MessagingSystem.Core;

[Scope]
public class MessageFactory
{
    private readonly ILog _logger;
    private const string UserAgentHeader = "User-Agent";
    private const string ForwardedHeader = "X-Forwarded-For";
    private const string HostHeader = "Host";
    private const string RefererHeader = "Referer";

    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;

    public MessageFactory(AuthContext authContext, TenantManager tenantManager, IOptionsMonitor<ILog> options)
    {
        _authContext = authContext;
        _tenantManager = tenantManager;
        _logger = options.CurrentValue;
    }

    public EventMessage Create(HttpRequest request, string initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        try
        {
            return new EventMessage
            {
                Ip = request != null ? request.Headers[ForwardedHeader].ToString() ?? request.GetUserHostAddress() : null,
                Initiator = initiator,
                Date = DateTime.UtcNow,
                TenantId = _tenantManager.GetCurrentTenant().Id,
                UserId = _authContext.CurrentAccount.ID,
                Page = request?.GetTypedHeaders().Referer?.ToString(),
                Action = action,
                Description = description,
                Target = target,
                UAHeader = request?.Headers[UserAgentHeader].FirstOrDefault()
            };
        }
        catch (Exception ex)
        {
            _logger.ErrorFormat("Error while parse Http Request for {0} type of event: {1}", action, ex);

            return null;
        }
    }

    public EventMessage Create(MessageUserData userData, IDictionary<string, StringValues> headers, MessageAction action, MessageTarget target, params string[] description)
    {
        try
        {
            var message = new EventMessage
            {
                Date = DateTime.UtcNow,
                TenantId = userData == null ? _tenantManager.GetCurrentTenant().Id : userData.TenantId,
                UserId = userData == null ? _authContext.CurrentAccount.ID : userData.UserId,
                Action = action,
                Description = description,
                Target = target
            };

            if (headers != null)
            {
                var userAgent = headers.ContainsKey(UserAgentHeader) ? headers[UserAgentHeader].ToString() : null;
                var forwarded = headers.ContainsKey(ForwardedHeader) ? headers[ForwardedHeader].ToString() : null;
                var host = headers.ContainsKey(HostHeader) ? headers[HostHeader].ToString() : null;
                var referer = headers.ContainsKey(RefererHeader) ? headers[RefererHeader].ToString() : null;

                message.Ip = forwarded ?? host;
                message.UAHeader = userAgent;
                message.Page = referer;
            }

            return message;
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Error while parse Http Message for \"{0}\" type of event: {1}", action, ex));

            return null;
        }
    }

    public EventMessage Create(string initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        try
        {
            return new EventMessage
            {
                Initiator = initiator,
                Date = DateTime.UtcNow,
                TenantId = _tenantManager.GetCurrentTenant().Id,
                Action = action,
                Description = description,
                Target = target
            };
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Error while parse Initiator Message for \"{0}\" type of event: {1}", action, ex));

            return null;
        }
    }
}
