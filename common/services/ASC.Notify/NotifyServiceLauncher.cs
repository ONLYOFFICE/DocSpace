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

namespace ASC.Notify
{
    [Singletone]
    public class NotifyServiceLauncher : IHostedService
    {
        private readonly ILog _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly NotifyCleaner _notifyCleaner;
        private readonly NotifyConfiguration _notifyConfiguration;
        private readonly NotifySender _notifySender;
        private readonly NotifyService _notifyService;
        private readonly NotifyServiceCfg _notifyServiceCfg;
        private readonly WebItemManager _webItemManager;

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
            _notifyServiceCfg = notifyServiceCfg.Value;
            _notifyService = notifyService;
            _notifySender = notifySender;
            _notifyCleaner = notifyCleaner;
            _webItemManager = webItemManager;
            _serviceProvider = serviceProvider;
            _notifyConfiguration = notifyConfiguration;
            _logger = options.Get("ASC.Notify");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _notifyService.Start();
            _notifySender.StartSending();

            if (0 < _notifyServiceCfg.Schedulers.Count)
            {
                InitializeNotifySchedulers();
            }

            _notifyCleaner.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _notifyService.Stop();

            if (_notifySender != null)
            {
                _notifySender.StopSending();
            }

            if (_notifyCleaner != null)
            {
                _notifyCleaner.Stop();
            }

            return Task.CompletedTask;
        }

        private void InitializeNotifySchedulers()
        {
            _notifyConfiguration.Configure();
            foreach (var pair in _notifyServiceCfg.Schedulers.Where(r => r.MethodInfo != null))
            {
                _logger.DebugFormat("Start scheduler {0} ({1})", pair.Name, pair.MethodInfo);
                pair.MethodInfo.Invoke(null, null);
            }
        }
    }
}
