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

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Common.Notify;

class PushSenderSink : Sink
{
    private readonly ILog _logger;
    private bool _configured = true;

    public PushSenderSink(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
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
                    _logger.Debug("push sender endpoint is not configured!");
                }
            }
            else
            {
                _logger.Debug("push sender endpoint is not configured!");
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
