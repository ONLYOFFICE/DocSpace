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
public class MessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageSender _sender;
    private readonly HttpRequest _request;
    private readonly MessageFactory _messageFactory;
    private readonly MessagePolicy _messagePolicy;

    public MessageService(
        IConfiguration configuration,
        MessageFactory messageFactory,
        DbMessageSender sender,
        MessagePolicy messagePolicy,
        ILogger<MessageService> logger)
    {
        if (configuration["messaging:enabled"] != "true")
        {
            return;
        }

        _sender = sender;
        _messagePolicy = messagePolicy;
        _messageFactory = messageFactory;
        _logger = logger;
    }

    public MessageService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        MessageFactory messageFactory,
        DbMessageSender sender,
        MessagePolicy messagePolicy,
        ILogger<MessageService> logger)
        : this(configuration, messageFactory, sender, messagePolicy, logger)
    {
        _request = httpContextAccessor?.HttpContext?.Request;
    }

    #region HttpRequest

    public async Task SendAsync(MessageAction action)
    {
        await SendRequestMessageAsync(action);
    }

    public async Task SendAsync(MessageAction action, string d1)
    {
        await SendRequestMessageAsync(action, description: d1);
    }

    public async Task SendAsync(MessageAction action, string d1, IEnumerable<string> d2)
    {
        await SendRequestMessageAsync(action, description: new[] { d1, string.Join(", ", d2) });
    }

    public async Task SendAsync(string loginName, MessageAction action)
    {
        await SendRequestMessageAsync(action, loginName: loginName);
    }

    #endregion

    #region HttpRequest & Target

    public async Task SendAsync(MessageAction action, MessageTarget target)
    {
        await SendRequestMessageAsync(action, target);
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, DateTime? dateTime, string d1)
    {
        await SendRequestMessageAsync(action, target, dateTime: dateTime, description: d1);
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, string d1)
    {
        await SendRequestMessageAsync(action, target, description: d1);
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, string d1, Guid userId)
    {
        if (TryAddNotificationParam(action, userId, out var parametr))
        {
            await SendRequestMessageAsync(action, target, description: new[] { d1, parametr });
        }
        else
        {
            await SendRequestMessageAsync(action, target, description: d1);
        }
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, string d1, string d2)
    {
        await SendRequestMessageAsync(action, target, description: new[] { d1, d2 });
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, IEnumerable<string> d1)
    {
        await SendRequestMessageAsync(action, target, description: string.Join(", ", d1));
    }

    public async Task SendAsync(MessageAction action, MessageTarget target, IEnumerable<string> d1, List<Guid> userIds, EmployeeType userType)
    {
        if (TryAddNotificationParam(action, userIds, out var parametr, userType))
        {
            await SendRequestMessageAsync(action, target, description: new[] { string.Join(", ", d1), parametr });
        }
        else
        {
            await SendRequestMessageAsync(action, target, description: string.Join(", ", d1));
        }
    }

    public async Task SendAsync(string loginName, MessageAction action, MessageTarget target)
    {
        await SendRequestMessageAsync(action, target, loginName);
    }

    #endregion

    private async Task SendRequestMessageAsync(MessageAction action, MessageTarget target = null, string loginName = null, DateTime? dateTime = null, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        if (_request == null)
        {
            _logger.DebugEmptyHttpRequest(action);

            return;
        }

        var message = await _messageFactory.CreateAsync(_request, loginName, dateTime, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        await _sender.SendAsync(message);
    }

    #region HttpHeaders

    public async Task SendHeadersMessageAsync(MessageAction action)
    {
        await SendRequestHeadersMessageAsync(action);
    }
    public async Task SendHeadersMessageAsync(MessageAction action, params string[] description)
    {
        await SendRequestHeadersMessageAsync(action, description: description);
    }

    public async Task SendHeadersMessageAsync(MessageAction action, MessageTarget target, IDictionary<string, StringValues> httpHeaders, string d1)
    {
        await SendRequestHeadersMessageAsync(action, target, httpHeaders, d1);
    }

    public async Task SendHeadersMessageAsync(MessageAction action, MessageTarget target, IDictionary<string, StringValues> httpHeaders, IEnumerable<string> d1)
    {
        await SendRequestHeadersMessageAsync(action, target, httpHeaders, d1?.ToArray());
    }

    private async Task SendRequestHeadersMessageAsync(MessageAction action, MessageTarget target = null, IDictionary<string, StringValues> httpHeaders = null, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        if (httpHeaders == null && _request != null)
        {
            httpHeaders = _request.Headers?.ToDictionary(k => k.Key, v => v.Value);
        }

        var message = await _messageFactory.CreateAsync(httpHeaders, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        await _sender.SendAsync(message);
    }

    #endregion

    #region Initiator

    public async Task SendAsync(MessageInitiator initiator, MessageAction action, params string[] description)
    {
        await SendInitiatorMessageAsync(initiator.ToString(), action, null, description);
    }

    #endregion

    #region Initiator & Target

    public async Task SendAsync(MessageInitiator initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        await SendInitiatorMessageAsync(initiator.ToString(), action, target, description);
    }

    #endregion

    private async Task SendInitiatorMessageAsync(string initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        var message = await _messageFactory.CreateAsync(_request, initiator, null, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        await _sender.SendAsync(message);
    }
    public async Task<int> SendLoginMessageAsync(MessageUserData userData, MessageAction action)
    {
        if (_sender == null)
        {
            return 0;
        }

        var message = await _messageFactory.CreateAsync(_request, userData, action);
        if (!_messagePolicy.Check(message))
        {
            return 0;
        }

        return await _sender.SendAsync(message);
    }

    private bool TryAddNotificationParam(MessageAction action, Guid userId, out string parametr)
    {
        return TryAddNotificationParam(action, new List<Guid> { userId }, out parametr);
    }

    private bool TryAddNotificationParam(MessageAction action, List<Guid> userIds, out string parametr, EmployeeType userType = 0)
    {
        parametr = "";

        if (action == MessageAction.UsersUpdatedType)
        {
            parametr = JsonSerializer.Serialize(new AdditionalNotificationInfo
            {
                UserIds = userIds,
                UserRole = (int)userType
            });
        }
        else if (action == MessageAction.UserCreated || action == MessageAction.UserUpdated)
        {
            parametr = JsonSerializer.Serialize(new AdditionalNotificationInfo
            {
                UserIds = userIds
            });
        }
        else
        {
            return false;
        }

        return true;
    }
}

public class AdditionalNotificationInfo
{
    public int RoomId { get; set; }
    public string RoomTitle { get; set; }
    public string RoomOldTitle { get; set; }
    public string RootFolderTitle { get; set; }
    public int UserRole { get; set; }
    public List<Guid> UserIds { get; set; }
}
