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

    public void Send(MessageAction action)
    {
        SendRequestMessage(null, null, action, null, null);
    }

    public void Send(MessageAction action, string d1)
    {
        SendRequestMessage(null, null, action, null, null, d1);
    }

    public void Send(MessageAction action, string d1, string d2)
    {
        SendRequestMessage(null, null, action, null, null, d1, d2);
    }

    public void Send(MessageAction action, string d1, string d2, string d3)
    {
        SendRequestMessage(null, null, action, null, null, d1, d2, d3);
    }

    public void Send(MessageAction action, string d1, string d2, string d3, string d4)
    {
        SendRequestMessage(null, null, action, null, null, d1, d2, d3, d4);
    }

    public void Send(MessageAction action, IEnumerable<string> d1, string d2)
    {
        SendRequestMessage(null, null, action, null, null, string.Join(", ", d1), d2);
    }

    public void Send(MessageAction action, string d1, IEnumerable<string> d2)
    {
        SendRequestMessage(null, null, action, null, null, d1, string.Join(", ", d2));
    }

    public void Send(MessageAction action, string d1, string d2, IEnumerable<string> d3)
    {
        SendRequestMessage(null, null, action, null, null, d1, d2, string.Join(", ", d3));
    }

    public void Send(MessageAction action, IEnumerable<string> d1)
    {
        SendRequestMessage(null, null, action, null, null, string.Join(", ", d1));
    }

    public void Send(string loginName, MessageAction action)
    {
        SendRequestMessage(loginName, null, action, null, null);
    }

    public void Send(string loginName, MessageAction action, string d1)
    {
        SendRequestMessage(loginName, null, action, null, null, d1);
    }

    #endregion

    #region HttpRequest & Target

    public void Send(MessageAction action, MessageTarget target)
    {
        SendRequestMessage(null, null, action, null, target);
    }
    public void Send(DateTime? dateTime, MessageAction action, MessageTarget target, string d1)
    {
        SendRequestMessage(null, dateTime, action, null, target, d1);
    }

    public void Send(MessageAction action, MessageTarget target, string d1) // Test
    {
        SendRequestMessage(null, null, action, null, target, d1);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2) // Test
    {
        SendRequestMessage(null, null, action, null, target, d1, d2);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, string d3)
    {
        SendRequestMessage(null, null, action, null, target, d1, d2, d3);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, string d3, string d4)
    {
        SendRequestMessage(null, null, action, null, target, d1, d2, d3, d4);
    }

    public void Send(MessageAction action, MessageTarget target, IEnumerable<string> d1, string d2)
    {
        SendRequestMessage(null, null, action, null, target, string.Join(", ", d1), d2);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, IEnumerable<string> d2)
    {
        SendRequestMessage(null, null, action, null, target, d1, string.Join(", ", d2));
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, IEnumerable<string> d3)
    {
        SendRequestMessage(null, null, action, null, target, d1, d2, string.Join(", ", d3));
    }

    public void Send(MessageAction action, MessageTarget target, IEnumerable<string> d1)
    {
        SendRequestMessage(null, null, action, null, target, string.Join(", ", d1));
    }

    public void Send(string loginName, MessageAction action, MessageTarget target)
    {
        SendRequestMessage(loginName, null, action, null, target);
    }

    public void Send(string loginName, MessageAction action, MessageTarget target, string d1)
    {
        SendRequestMessage(loginName, null, action, null ,target, d1);
    }

    #endregion

    public void Send(MessageAction action, string context, MessageTarget target, string d1) // new
    {
        SendRequestMessage(null, null, action, context ,target, d1);
    }

    public void Send(MessageAction action, string context, MessageTarget target, string d1, string d2) // new
    {
        SendRequestMessage(null, null, action, context, target, d1, d2);
    }

    private void SendRequestMessage(string loginName, DateTime? dateTime, MessageAction action, string context, MessageTarget target, params string[] description)
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

        var message = _messageFactory.Create(_request, loginName, dateTime, action, context, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }

    #region HttpHeaders

    public void Send(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action)
    {
        SendHeadersMessage(userData, httpHeaders, action, null, null);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action)
    {
        SendHeadersMessage(null, httpHeaders, action, null, null);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, string d1)
    {
        SendHeadersMessage(null, httpHeaders, action, null, null, d1);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, IEnumerable<string> d1)
    {
        SendHeadersMessage(null, httpHeaders, action, null, null, d1?.ToArray());
    }

    public void Send(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target)
    {
        SendHeadersMessage(userData, httpHeaders, action, null, target);
    }

    #endregion

    #region HttpHeaders & Target

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target)
    {
        SendHeadersMessage(null, httpHeaders, action, null, target);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target, string d1)
    {
        SendHeadersMessage(null, httpHeaders, action, null, target, d1);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target, IEnumerable<string> d1) //Test
    {
        SendHeadersMessage(null, httpHeaders, action, null, target, d1?.ToArray());
    }

    #endregion

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, string context, MessageTarget target, IEnumerable<string> d1)
    {
        SendHeadersMessage(null, httpHeaders, action, context, target, d1?.ToArray());
    }

    private void SendHeadersMessage(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action, string context, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        var message = _messageFactory.Create(userData, httpHeaders, action, context, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }

    #region Initiator

    public void Send(MessageInitiator initiator, MessageAction action, params string[] description)
    {
        SendInitiatorMessage(initiator.ToString(), action, null, null, description);
    }

    #endregion

    #region Initiator & Target

    public void Send(MessageInitiator initiator, MessageAction action, MessageTarget target, params string[] description) // Test
    {
        SendInitiatorMessage(initiator.ToString(), action, null ,target, description);
    }

    #endregion

    public void Send(MessageInitiator initiator, MessageAction action, string context, MessageTarget target, params string[] description)
    {
        SendInitiatorMessage(initiator.ToString(), action, context, target, description);
    }

    private void SendInitiatorMessage(string initiator, MessageAction action, string context, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        var message = _messageFactory.Create(_request, initiator, null, action, context, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }

    public int SendLoginMessage(MessageUserData userData, MessageAction action)
    {
        if (_sender == null)
        {
            return 0;
        }

        var message = _messageFactory.Create(_request, userData, action);
        if (!_messagePolicy.Check(message))
        {
            return 0;
        }

        return _sender.Send(message);
    }
}
