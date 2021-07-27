/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.SsoAuth.Svc
{
    [Singletone]
    public class Launcher : IHostedService
    {
        private ProcessStartInfo startInfo;
        private Process proc;

        private string LogDir { get; set; }
        private ILog Logger { get; set; }
        private IConfiguration Configuration { get; }
        private ConfigurationExtension ConfigurationExtension { get; }
        private IHostEnvironment HostEnvironment { get; }

        public Launcher(
            IOptionsMonitor<ILog> options,
            IConfiguration configuration,
            ConfigurationExtension configurationExtension,
            IHostEnvironment hostEnvironment)
        {
            Logger = options.CurrentValue;
            Configuration = configuration;
            ConfigurationExtension = configurationExtension;
            HostEnvironment = hostEnvironment;
        }



        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var cfg = ConfigurationExtension.GetSetting<SsoAuthSettings>("ssoauth");

                startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = "node",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Format("\"{0}\"", Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, cfg.Path, "app.js"))),
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };


                startInfo.EnvironmentVariables.Add("core.machinekey", Configuration["core:machinekey"]);
                startInfo.EnvironmentVariables.Add("port", cfg.Port);

                LogDir = Logger.LogDirectory;
                startInfo.EnvironmentVariables.Add("logPath", LogDir);

                await StartNode(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Error("Start", e);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (proc != null && !proc.HasExited)
                {
                    proc.Kill();
                    proc.WaitForExit(10000);

                    proc.Close();
                    proc.Dispose();
                    proc = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Stop", e);
            }

            return Task.CompletedTask;
        }

        private async Task StartNode(CancellationToken cancellationToken)
        {
            await StopAsync(cancellationToken);
            proc = Process.Start(startInfo);
        }
    }
}