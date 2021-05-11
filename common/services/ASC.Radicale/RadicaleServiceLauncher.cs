using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Radicale
{
    [Scope]
    public class RadicaleServiceLauncher : IHostedService
    {
        private Process Proc { get; set; }
        private ProcessStartInfo StartInfo { get; set; }
        private ILog Logger { get; set; }
        //private ConfigurationExtension ConfigurationExtension { get; }
        //private IHostEnvironment HostEnvironment { get; set; }

        public RadicaleServiceLauncher(
            IOptionsMonitor<ILog> options,
            ConfigurationExtension configurationExtension,
            IHostEnvironment hostEnvironment)
        {
            Logger = options.CurrentValue;
            //ConfigurationExtension = configurationExtension;
            //HostEnvironment = hostEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pythonName = "python";

                if (WorkContext.IsMono)
                {
                    pythonName = "python3";
                }

                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = pythonName,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Format("-m radicale --config \"{0}\"",
                                                Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "radicale.config"))),
                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
                };

                StartRedicale();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopRadicale();
            return Task.CompletedTask;
        }

        private void StartRedicale()
        {
            StopRadicale();
            Proc = Process.Start(StartInfo);

        }

        private void StopRadicale()
        {
            try
            {
                if (Proc != null && !Proc.HasExited)
                {
                    Proc.Kill();
                    if (!Proc.WaitForExit(10000))
                    {
                        Logger.Warn("The process does not wait for completion.");
                    }
                    Proc.Close();
                    Proc.Dispose();
                    Proc = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Radicale failed stop", e);
            }
        }
    }
}
