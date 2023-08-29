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

namespace ASC.Core.Notify;

public class EmailSenderSink : Sink
{
    private static readonly string _senderName = Configuration.Constants.NotifyEMailSenderSysName;
    private readonly INotifySender _sender;

    public EmailSenderSink(INotifySender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public override async Task<SendResponse> ProcessMessage(INoticeMessage message, IServiceScope scope)
    {
        if (message.Recipient.Addresses == null || message.Recipient.Addresses.Length == 0)
        {
            return new SendResponse(message, _senderName, SendResult.IncorrectRecipient);
        }

        var responce = new SendResponse(message, _senderName, default(SendResult));
        try
        {
            var m = await scope.ServiceProvider.GetRequiredService<EmailSenderSinkMessageCreator>().CreateNotifyMessageAsync(message, _senderName);
            var result = await _sender.SendAsync(m);

            responce.Result = result switch
            {
                NoticeSendResult.TryOnceAgain => SendResult.Inprogress,
                NoticeSendResult.MessageIncorrect => SendResult.IncorrectRecipient,
                NoticeSendResult.SendingImpossible => SendResult.Impossible,
                _ => SendResult.OK,
            };

            return responce;
        }
        catch (Exception e)
        {
            return new SendResponse(message, _senderName, e);
        }
    }
}

[Scope]
public class EmailSenderSinkMessageCreator : SinkMessageCreator
{
    private readonly TenantManager _tenantManager;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly ILogger _logger;

    public EmailSenderSinkMessageCreator(TenantManager tenantManager, CoreConfiguration coreConfiguration, ILoggerProvider options)
    {
        _tenantManager = tenantManager;
        _coreConfiguration = coreConfiguration;
        _logger = options.CreateLogger("ASC.Notify");
    }

    public override async Task<NotifyMessage> CreateNotifyMessageAsync(INoticeMessage message, string senderName)
    {
        var m = new NotifyMessage
        {
            Subject = message.Subject.Trim(' ', '\t', '\n', '\r'),
            ContentType = message.ContentType,
            Content = message.Body,
            SenderType = senderName,
            CreationDate = DateTime.UtcNow,
        };

        var tenant = await _tenantManager.GetCurrentTenantAsync(false);
        m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

        var settings = await _coreConfiguration.GetDefaultSmtpSettingsAsync();
        var from = MailAddressUtils.Create(settings.SenderAddress, settings.SenderDisplayName);
        var fromTag = message.Arguments.FirstOrDefault(x => x.Tag.Equals("MessageFrom"));
        if ((settings.IsDefaultSettings || string.IsNullOrEmpty(settings.SenderDisplayName)) &&
            fromTag != null && fromTag.Value != null)
        {
            try
            {
                from = MailAddressUtils.Create(from.Address, fromTag.Value.ToString());
            }

            catch { }
        }
        m.Sender = from.ToString();

        var to = new List<string>();
        foreach (var address in message.Recipient.Addresses)
        {
            to.Add(MailAddressUtils.Create(address, message.Recipient.Name).ToString());
        }
        m.Reciever = string.Join("|", to.ToArray());

        var replyTag = message.Arguments.FirstOrDefault(x => x.Tag == "replyto");
        if (replyTag != null && replyTag.Value is string value)
        {
            try
            {
                m.ReplyTo = MailAddressUtils.Create(value).ToString();
            }
            catch (Exception e)
            {
                _logger.ErrorCreatingTag(replyTag.Value, e);
            }
        }

        var priority = message.Arguments.FirstOrDefault(a => a.Tag == "Priority");
        if (priority != null)
        {
            m.Priority = Convert.ToInt32(priority.Value);
        }

        var attachmentTag = message.Arguments.FirstOrDefault(x => x.Tag == "EmbeddedAttachments");
        if (attachmentTag != null && attachmentTag.Value != null)
        {
            m.Attachments = attachmentTag.Value as NotifyMessageAttachment[];
        }

        var autoSubmittedTag = message.Arguments.FirstOrDefault(x => x.Tag == "AutoSubmitted");
        if (autoSubmittedTag != null && autoSubmittedTag.Value is string)
        {
            try
            {
                m.AutoSubmitted = autoSubmittedTag.Value.ToString();
            }
            catch (Exception e)
            {
                _logger.ErrorCreatingAutoSubmitted(replyTag.Value, e);
            }
        }

        return m;
    }
}
