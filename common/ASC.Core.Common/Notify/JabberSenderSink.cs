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

    public override async Task<SendResponse> ProcessMessage(INoticeMessage message)
    {
        try
        {
            var result = SendResult.OK;
            await using var scope = _serviceProvider.CreateAsyncScope();
            var m = await scope.ServiceProvider.GetRequiredService<JabberSenderSinkMessageCreator>().CreateNotifyMessageAsync(message, _senderName);

            if (string.IsNullOrEmpty(m.Reciever))
            {
                result = SendResult.IncorrectRecipient;
            }
            else
            {
                await _sender.SendAsync(m);
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
public class JabberSenderSinkMessageCreator : SinkMessageCreator
{
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;

    public JabberSenderSinkMessageCreator(UserManager userManager, TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
    }

    public override async Task<NotifyMessage> CreateNotifyMessageAsync(INoticeMessage message, string senderName)
    {
        var username = (await _userManager.GetUsersAsync(new Guid(message.Recipient.ID))).UserName;

        var m = new NotifyMessage
        {
            Reciever = username,
            Subject = message.Subject,
            ContentType = message.ContentType,
            Content = message.Body,
            SenderType = senderName,
            CreationDate = DateTime.UtcNow,
        };

        var tenant = await _tenantManager.GetCurrentTenantAsync(false);
        m.TenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;
        return m;
    }
}
