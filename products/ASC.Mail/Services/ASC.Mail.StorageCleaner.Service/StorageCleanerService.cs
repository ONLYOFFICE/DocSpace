using System;
using System.IO;
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

namespace ASC.Mail.StorageCleaner.Service
{
    [Singletone]
    public class StorageCleanerService
    {
        private ILog Log { get; }
        internal Timer WorkTimer { get; private set; }
        readonly TimeSpan TsInterval;
        private MailGarbageEngine Eraser { get; set; }

        public StorageCleanerService(
            IOptionsMonitor<ILog> options,
            MailGarbageEngine mailGarbageEngine,
            MailSettings settings,
            IConfiguration configuration,
            ConfigurationExtension configurationExtension)
        {
            ConfigureNLog(configuration, configurationExtension);

            Log = options.Get("ASC.Mail.Cleaner");
            Eraser = mailGarbageEngine;
            TsInterval = TimeSpan.FromMinutes(Convert.ToInt32(settings.Cleaner.TimerWaitMinutes));

            Log.InfoFormat("Service will clear mail storage every {0} minutes\r\n", TsInterval.TotalMinutes);
        }

        internal Task StartTimer(CancellationToken token, bool immediately = false)
        {
            if (WorkTimer == null)
                WorkTimer = new Timer(WorkTimerElapsed, token, Timeout.Infinite, Timeout.Infinite);

            if (immediately)
            {
                Log.Debug("Setup WorkTimer to start immediately");

                WorkTimer.Change(0, Timeout.Infinite);
            }
            else
            {
                Log.DebugFormat($"Setup WorkTimer to {TsInterval.TotalMinutes} minutes");

                WorkTimer.Change(TsInterval, TsInterval);
            }

            return Task.CompletedTask;
        }

        private void StopTimer()
        {
            if (WorkTimer == null)
                return;

            Log.Debug("Setup WorkTimer to Timeout.Infinite");

            WorkTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        internal void StopService(CancellationTokenSource tokenSource, ManualResetEvent resetEvent)
        {
            if (tokenSource != null)
                tokenSource.Cancel();

            StopTimer();

            if (WorkTimer != null)
            {
                WorkTimer.Dispose();
                WorkTimer = null;
            }

            if (resetEvent != null)
                resetEvent.Set();

            if (Eraser != null)
            {
                Eraser.Dispose();
                Eraser = null;
            }
        }

        private void WorkTimerElapsed(object state)
        {
            Log.Debug("Timer -> WorkTimerElapsed");

            var cancelToken = state as CancellationToken? ?? new CancellationToken();

            try
            {
                Eraser.ClearMailGarbage(cancelToken);

                Log.InfoFormat("All mailboxes were processed. Go back to timer. Next start after {0} minutes.\r\n",
                    TsInterval.TotalMinutes);

            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    ex = ((AggregateException)ex).GetBaseException();
                }

                if (ex is TaskCanceledException || ex is OperationCanceledException)
                {
                    Log.Info("Execution was canceled.");

                    return;
                }

                Log.ErrorFormat("Timer -> WorkTimerElapsed. Exception:\r\n{0}\r\n", ex.ToString());
            }

            StartTimer(cancelToken);
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
