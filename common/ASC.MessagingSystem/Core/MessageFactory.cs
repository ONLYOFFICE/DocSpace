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

    public MessageFactory(AuthContext authContext, TenantManager tenantManager, ILog logger)
    {
        _authContext = authContext;
        _tenantManager = tenantManager;
        _logger = logger;
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
            _logger.LogError("Error while parse Http Request for {0} type of event: {1}", action, ex);

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
            _logger.LogError(string.Format("Error while parse Http Message for \"{0}\" type of event: {1}", action, ex));

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
            _logger.LogError(string.Format("Error while parse Initiator Message for \"{0}\" type of event: {1}", action, ex));

            return null;
        }
    }
}
