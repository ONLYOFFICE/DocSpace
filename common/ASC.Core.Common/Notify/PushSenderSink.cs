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

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Common.Notify;

class PushSenderSink : Sink
{
    private readonly ILog _logger;
    private bool _configured = true;

    public PushSenderSink(IServiceProvider serviceProvider, IOptionsMonitor<ILog> options)
    {
        _serviceProvider = serviceProvider;
        _logger = options.CurrentValue;
    }

    private readonly IServiceProvider _serviceProvider;

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

            var notification = new PushNotification
            {
                Module = GetTagValue<PushModule>(message, PushConstants.PushModuleTagName),
                Action = GetTagValue<PushAction>(message, PushConstants.PushActionTagName),
                Item = GetTagValue<PushItem>(message, PushConstants.PushItemTagName),
                ParentItem = GetTagValue<PushItem>(message, PushConstants.PushParentItemTagName),
                Message = message.Body,
                ShortMessage = message.Subject
            };

            if (_configured)
            {
                try
                {
                    using var pushClient = new PushServiceClient();
                    pushClient.EnqueueNotification(
                        tenantManager.GetCurrentTenant().Id,
                        message.Recipient.ID,
                        notification,
                        new List<string>());
                }
                catch (InvalidOperationException)
                {
                    _configured = false;
                    _logger.LogDebug("push sender endpoint is not configured!");
                }
            }
            else
            {
                _logger.LogDebug("push sender endpoint is not configured!");
            }

            return new SendResponse(message, Constants.NotifyPushSenderSysName, SendResult.OK);
        }
        catch (Exception error)
        {
            return new SendResponse(message, Constants.NotifyPushSenderSysName, error);
        }
    }

    private T GetTagValue<T>(INoticeMessage message, string tagName)
    {
        var tag = message.Arguments.FirstOrDefault(arg => arg.Tag == tagName);

        return tag != null ? (T)tag.Value : default;
    }
}
