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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Notify.Config;
using ASC.Web.Core;
using ASC.Web.Studio.Core.Notify;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Notify
{
    [Singletone]
    public class NotifyServiceLauncher : IHostedService
    {
        private NotifyServiceCfg NotifyServiceCfg { get; }
        private NotifyService NotifyService { get; }
        private NotifySender NotifySender { get; }
        private NotifyCleaner NotifyCleaner { get; }
        private WebItemManager WebItemManager { get; }
        private IServiceProvider ServiceProvider { get; }
        private NotifyConfiguration NotifyConfiguration { get; }
        private ILog Log { get; }

        public NotifyServiceLauncher(
            IOptions<NotifyServiceCfg> notifyServiceCfg,
            NotifySender notifySender,
            NotifyService notifyService,
            NotifyCleaner notifyCleaner,
            WebItemManager webItemManager,
            IServiceProvider serviceProvider,
            NotifyConfiguration notifyConfiguration,
            IOptionsMonitor<ILog> options)
        {
            NotifyServiceCfg = notifyServiceCfg.Value;
            NotifyService = notifyService;
            NotifySender = notifySender;
            NotifyCleaner = notifyCleaner;
            WebItemManager = webItemManager;
            ServiceProvider = serviceProvider;
            NotifyConfiguration = notifyConfiguration;
            Log = options.Get("ASC.Notify");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            NotifyService.Start();
            NotifySender.StartSending();

            if (0 < NotifyServiceCfg.Schedulers.Count)
            {
                InitializeNotifySchedulers();
            }

            NotifyCleaner.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            NotifyService.Stop();

            if (NotifySender != null)
            {
                NotifySender.StopSending();
            }

            if (NotifyCleaner != null)
            {
                NotifyCleaner.Stop();
            }

            return Task.CompletedTask;
        }

        private void InitializeNotifySchedulers()
        {
            NotifyConfiguration.Configure();
            WebItemManager.LoadItems();
            foreach (var pair in NotifyServiceCfg.Schedulers.Where(r => r.MethodInfo != null))
            {
                Log.DebugFormat("Start scheduler {0} ({1})", pair.Name, pair.MethodInfo);
                pair.MethodInfo.Invoke(null, null);
            }
        }
    }
}
