using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Mail.Aggregator.CollectionService.Console;
using ASC.Mail.Aggregator.CollectionService.Service;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Mail.Aggregator.CollectionService
{
    [Singletone]
    class AggregatorServiceLauncher : IHostedService
    {
        private ILog _log { get; }
        private AggregatorService _aggregatorService { get; }
        private ConsoleParameters _consoleParameters { get; }

        private ManualResetEvent _resetEvent;

        public AggregatorServiceLauncher(
            IOptionsMonitor<ILog> options,
            AggregatorService aggregatorService,
            ConsoleParser consoleParser)
        {
            _log = options.Get("ASC.Mail.MainThread");
            _aggregatorService = aggregatorService;
            _consoleParameters = consoleParser.GetParsedParameters();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.FatalFormat("Unhandled exception: {0}", e.ExceptionObject.ToString());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("Start service\r\n");

                if (Environment.UserInteractive || _consoleParameters.IsConsole)
                {
                    _log.Info("Service Start in console-daemon mode");
                    _aggregatorService.startTimer(true);
                    _resetEvent = new ManualResetEvent(false);
                    System.Console.CancelKeyPress += (sender, e) => StopAsync(cancellationToken);
                    _resetEvent.WaitOne();
                }
                else
                {
                    _aggregatorService.startTimer(true);
                }
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Unhandled exception: {0}", ex.ToString());
                StopAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("Stoping service\r\n");

                _aggregatorService.StopService();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Stop service Error: {0}\r\n", ex.ToString());
            }
            finally
            {
                _log.Info("Stop service\r\n");

                if (_resetEvent != null)
                    _resetEvent.Set();
            }

            return Task.CompletedTask;
        }
    }
}
