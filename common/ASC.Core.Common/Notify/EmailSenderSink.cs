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

namespace ASC.Core.Notify;

public class EmailSenderSink : Sink
{
    private static readonly string _senderName = Configuration.Constants.NotifyEMailSenderSysName;
    private readonly INotifySender _sender;


    public EmailSenderSink(INotifySender sender, IServiceProvider serviceProvider, IOptionsMonitor<ILog> options)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _serviceProvider = serviceProvider;
        _logger = options.Get("ASC.Notify");
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILog _logger;

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        if (message.Recipient.Addresses == null || message.Recipient.Addresses.Length == 0)
        {
            return new SendResponse(message, _senderName, SendResult.IncorrectRecipient);
        }

        var responce = new SendResponse(message, _senderName, default(SendResult));
        try
        {
            var m = CreateNotifyMessage(message);
            var result = _sender.Send(m);

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


    private NotifyMessage CreateNotifyMessage(INoticeMessage message)
    {
        var m = new NotifyMessage
        {
            Subject = message.Subject.Trim(' ', '\t', '\n', '\r'),
            ContentType = message.ContentType,
            Content = message.Body,
            SenderType = _senderName,
            CreationDate = DateTime.UtcNow.Ticks,
        };

        using var scope = _serviceProvider.CreateScope();

        var scopeClass = scope.ServiceProvider.GetService<EmailSenderSinkScope>();
        var (tenantManager, configuration, options) = scopeClass;

        var tenant = tenantManager.GetCurrentTenant(false);
        m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

        var from = MailAddressUtils.Create(configuration.SmtpSettings.SenderAddress, configuration.SmtpSettings.SenderDisplayName);
        var fromTag = message.Arguments.FirstOrDefault(x => x.Tag.Equals("MessageFrom"));
        if ((configuration.SmtpSettings.IsDefaultSettings || string.IsNullOrEmpty(configuration.SmtpSettings.SenderDisplayName)) &&
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
                _serviceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC.Notify").Error("Error creating reply to tag for: " + replyTag.Value, e);
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
            m.Attachments.AddRange(attachmentTag.Value as NotifyMessageAttachment[]);
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
                _logger.Error("Error creating AutoSubmitted tag for: " + autoSubmittedTag.Value, e);
            }
        }

        return m;
    }
}

[Scope]
public class EmailSenderSinkScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly IOptionsMonitor<ILog> _options;

    public EmailSenderSinkScope(TenantManager tenantManager, CoreConfiguration coreConfiguration, IOptionsMonitor<ILog> options)
    {
        _tenantManager = tenantManager;
        _coreConfiguration = coreConfiguration;
        _options = options;
    }

    public void Deconstruct(out TenantManager tenantManager, out CoreConfiguration coreConfiguration, out IOptionsMonitor<ILog> optionsMonitor)
    {
        (tenantManager, coreConfiguration, optionsMonitor) = (_tenantManager, _coreConfiguration, _options);
    }
}
