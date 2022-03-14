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


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Core.Notify.Senders
{
    [Singletone(Additional = typeof(JabberSenderExtension))]
    public class JabberSender : INotifySender
    {
        private readonly ILog log;

        private IServiceProvider ServiceProvider { get; }

        public JabberSender(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            log = ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
        }

        public void Init(IDictionary<string, string> properties)
        {
        }

        public NoticeSendResult Send(NotifyMessage m)
        {
            var text = m.Content;
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace("\r\n", "\n").Trim('\n', '\r');
                text = Regex.Replace(text, "\n{3,}", "\n\n");
            }
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetService<JabberServiceClient>();
                service.SendMessage(m.Tenant, null, m.To, text, m.Subject);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Unexpected error, {0}, {1}, {2}",
                       e.Message, e.StackTrace, e.InnerException != null ? e.InnerException.Message : string.Empty);
            }
            return NoticeSendResult.OK;
        }
    }

    public static class JabberSenderExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<JabberServiceClient>();
        }
    }
}
