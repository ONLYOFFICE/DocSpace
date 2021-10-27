using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Mail.Watchdog.Service
{
    [Singletone]
    class WatchdogLauncher : IHostedService
    {
        private ILog Log { get; }
        private WatchdogService WatchdogService { get; }
        private ConsoleParameters ConsoleParameters { get; }
        private CancellationTokenSource Cts { get; set; }

        private Task WatchdogTask { get; set; }
        private ManualResetEvent MreStop { get; set; }

        public WatchdogLauncher(
            WatchdogService watchdogService,
            IOptionsMonitor<ILog> options,
            ConsoleParser consoleParser)
        {
            WatchdogService = watchdogService;
            Log = options.Get("ASC.Mail.WatchdogLauncher");
            ConsoleParameters = consoleParser.GetParsedParameters();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.FatalFormat("Unhandled exception: {0}", e.ExceptionObject.ToString());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Info("Start service\r\n");

            Cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (ConsoleParameters.IsConsole)
            {
                Log.Info("Service Start in console-daemon mode");

                WatchdogTask = WatchdogService.StarService(Cts.Token);

                MreStop = new ManualResetEvent(false);
                Console.CancelKeyPress += async (sender, e) => await StopAsync(cancellationToken);
                MreStop.WaitOne();
            }
            else
            {
                WatchdogTask = WatchdogService.StarService(Cts.Token);
            }

            return WatchdogTask.IsCompleted ? WatchdogTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                WatchdogService.StopService(Cts);
                await Task.WhenAny(WatchdogTask, Task.Delay(TimeSpan.FromSeconds(5), cancellationToken));
            }
            catch (Exception e)
            {

                Log.ErrorFormat($"Failed to terminate the service correctly. The details:\r\n{e}\r\n");
            }

            Log.Info("Service stopped.\r\n");
        }
    }
}
