using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Mail.StorageCleaner.Service
{
    [Singletone]
    class StorageCleanerLauncher : IHostedService
    {
        private ILog Log { get; }
        private StorageCleanerService StorageCleanerService { get; }
        private Task CleanerTask { get; set; }
        private CancellationTokenSource Cts { get; set; }
        private ConsoleParameters ConsoleParameters { get; }
        private ManualResetEvent MreStop { get; set; }
        public StorageCleanerLauncher(
            IOptionsMonitor<ILog> options,
            StorageCleanerService cleanerService,
            ConsoleParser consoleParser)
        {
            Log = options.Get("ASC.Mail.Cleaner");
            StorageCleanerService = cleanerService;
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

            try
            {
                Cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                if (ConsoleParameters.IsConsole)
                {
                    Log.Info("Service Start in console-daemon mode");

                    CleanerTask = StorageCleanerService.StartTimer(Cts.Token, true);

                    MreStop = new ManualResetEvent(false);
                    Console.CancelKeyPress += async (sender, e) => await StopAsync(cancellationToken);
                    MreStop.WaitOne();
                }
                else
                {
                    CleanerTask = StorageCleanerService.StartTimer(Cts.Token, true);
                }

                return CleanerTask.IsCompleted ? CleanerTask : Task.CompletedTask;
            }
            catch (Exception)
            {
                return StopAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Info("Stop service\r\n");
                StorageCleanerService.StopService(Cts, MreStop);
                await Task.WhenAny(CleanerTask, Task.Delay(TimeSpan.FromSeconds(5), cancellationToken));
            }
            catch (Exception ex)
            {
                Log.ErrorFormat($"Failed to terminate the service correctly. The details:\r\n{ex}\r\n");
            }
        }
    }
}
