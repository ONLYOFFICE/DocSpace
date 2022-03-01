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

class JabberSenderSink : Sink
{
    private static readonly string _senderName = Configuration.Constants.NotifyMessengerSenderSysName;
    private readonly INotifySender _sender;

    public JabberSenderSink(INotifySender sender, IServiceProvider serviceProvider)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _serviceProvider = serviceProvider;
    }

    private readonly IServiceProvider _serviceProvider;

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<JabberSenderSinkScope>();
            (var userManager, var tenantManager) = scopeClass;
            var result = SendResult.OK;
            var username = userManager.GetUsers(new Guid(message.Recipient.ID)).UserName;
            if (string.IsNullOrEmpty(username))
            {
                result = SendResult.IncorrectRecipient;
            }
            else
            {
                var m = new NotifyMessage
                {
                    Reciever = username,
                    Subject = message.Subject,
                    ContentType = message.ContentType,
                    Content = message.Body,
                    SenderType = _senderName,
                    CreationDate = DateTime.UtcNow.Ticks,
                };

                var tenant = tenantManager.GetCurrentTenant(false);
                m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

                _sender.Send(m);
            }

            return new SendResponse(message, _senderName, result);
        }
        catch (Exception ex)
        {
            return new SendResponse(message, _senderName, ex);
        }
    }
}

[Scope]
public class JabberSenderSinkScope
{
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;

    public JabberSenderSinkScope(UserManager userManager, TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
    }

    public void Deconstruct(out UserManager userManager, out TenantManager tenantManager)
    {
        (userManager, tenantManager) = (_userManager, _tenantManager);
    }
}
