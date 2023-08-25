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

namespace ASC.Notify.Engine;

[Singletone]
public class DispatchEngine
{
    private readonly ILogger _logger;
    private readonly ILogger _messagesLogger;
    private readonly Context _context;
    private readonly bool _logOnly;

    public DispatchEngine(Context context, IConfiguration configuration, ILoggerProvider options)
    {
        _logger = options.CreateLogger("ASC.Notify");
        _messagesLogger = options.CreateLogger("ASC.Notify.Messages");
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logOnly = "log".Equals(configuration["core:notify:postman"], StringComparison.InvariantCultureIgnoreCase);
        _logger.LogOnly(_logOnly);
    }

    public async Task<SendResponse> Dispatch(INoticeMessage message, string senderName, IServiceScope serviceScope)
    {
        var response = new SendResponse(message, senderName, SendResult.OK);
        if (!_logOnly)
        {
            var sender = _context.GetSender(senderName);
            if (sender != null)
            {
                response = await sender.DirectSend(message, serviceScope);
            }
            else
            {
                response = new SendResponse(message, senderName, SendResult.Impossible);
            }

            LogResponce(message, response, sender != null ? sender.SenderName : string.Empty);
        }

        LogMessage(message, senderName);

        return response;
    }

    private void LogResponce(INoticeMessage message, SendResponse response, string senderName)
    {
        if (response.Result == SendResult.Inprogress)
        {
            _logger.LogDebugResponceWithException(message.Subject, message.Recipient, senderName, response.Result, response.Exception);
        }
        else if (response.Result == SendResult.Impossible)
        {
            _logger.LogErrorResponceWithException(message.Subject, message.Recipient, senderName, response.Result, response.Exception);
        }
        else
        {
            _logger.LogDebugResponce(message.Subject, message.Recipient, senderName, response.Result);
        }
    }

    private void LogMessage(INoticeMessage message, string senderName)
    {
        try
        {
            _messagesLogger.LogMessage(
                message.Action,
                message.Recipient.Name,
                senderName,
                0 < message.Recipient.Addresses.Length ? message.Recipient.Addresses[0] : string.Empty,
                DateTime.Now,
                message.Subject,
                (message.Body ?? string.Empty).Replace(Environment.NewLine, Environment.NewLine + @"   "),
                new string('-', 80));
        }
        catch { }
    }
}