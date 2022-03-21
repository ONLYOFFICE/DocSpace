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
