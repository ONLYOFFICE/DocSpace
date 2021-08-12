
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Mail.Configuration;
using ASC.Mail.Core.Engine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using NLog;

namespace ASC.Mail.Watchdog.Service
{
    [Singletone]
    public class WatchdogService
    {
        private ILog Log { get; }

        private Timer WorkTimer;
        private MailboxEngine MailboxEngine { get; }

        readonly TimeSpan TsInterval;
        readonly TimeSpan TsTasksTimeoutInterval;

        public WatchdogService(
            IOptionsMonitor<ILog> options,
            MailboxEngine mailboxEngine,
            MailSettings settings,
            IConfiguration configuration,
            ConfigurationExtension configurationExtension)
        {
            ConfigureNLog(configuration, configurationExtension);

            Log = options.Get("ASC.Mail.WatchdogService");
            MailboxEngine = mailboxEngine;

            TsInterval = TimeSpan.FromMinutes(Convert.ToInt32(settings.Watchdog.TimerIntervalInMinutes));
            TsTasksTimeoutInterval = TimeSpan.FromMinutes(Convert.ToInt32(settings.Watchdog.TasksTimeoutInMinutes));

            Log.InfoFormat("\r\nConfiguration:\r\n" +
                      "\t- check locked mailboxes in every {0} minutes;\r\n" +
                      "\t- locked mailboxes timeout {1} minutes;\r\n",
                      TsInterval.TotalMinutes,
                      TsTasksTimeoutInterval.TotalMinutes);
        }

        internal Task StarService(CancellationToken token)
        {
            if (WorkTimer == null)
                WorkTimer = new Timer(WorkTimerElapsed, token, 0, Timeout.Infinite);

            return Task.CompletedTask;
        }

        internal void StopService(CancellationTokenSource tokenSource)
        {
            if (tokenSource != null) tokenSource.Cancel();

            Log.Info("Try stop service...");

            if (WorkTimer == null) return;

            WorkTimer.Change(Timeout.Infinite, Timeout.Infinite);
            WorkTimer.Dispose();
            WorkTimer = null;
        }

        private void WorkTimerElapsed(object state)
        {
            try
            {
                WorkTimer.Change(Timeout.Infinite, Timeout.Infinite);

                Log.InfoFormat("ReleaseLockedMailboxes(timeout is {0} minutes)", TsTasksTimeoutInterval.TotalMinutes);

                var freeMailboxIds = MailboxEngine.ReleaseMailboxes((int)TsTasksTimeoutInterval.TotalMinutes);

                if (freeMailboxIds.Any())
                {
                    Log.InfoFormat("Released next locked mailbox's ids: {0}", string.Join(",", freeMailboxIds));
                }
                else
                {
                    Log.Info("Nothing to do!");
                }

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("IntervalTimer_Elapsed() Exception:\r\n{0}", ex.ToString());
            }
            finally
            {
                Log.InfoFormat("Waiting for {0} minutes for next check...", TsInterval.TotalMinutes);
                WorkTimer.Change(TsInterval, TsInterval);
            }
        }

        private void ConfigureNLog(IConfiguration configuration, ConfigurationExtension configurationExtension)
        {
            var fileName = CrossPlatform.PathCombine(configuration["pathToNlogConf"], "nlog.config");

            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(fileName);
            LogManager.ThrowConfigExceptions = false;

            var settings = configurationExtension.GetSetting<NLogSettings>("log");
            if (!string.IsNullOrEmpty(settings.Name))
            {
                LogManager.Configuration.Variables["name"] = settings.Name;
            }

            if (!string.IsNullOrEmpty(settings.Dir))
            {
                LogManager.Configuration.Variables["dir"] = settings.Dir.TrimEnd('/').TrimEnd('\\') + Path.DirectorySeparatorChar;
            }

            NLog.Targets.Target.Register<SelfCleaningTarget>("SelfCleaning");
        }
    }
}
