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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Notify.Signalr;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using WebSocketSharp;

namespace ASC.Socket.IO.Svc
{
    [Singletone]
    public class SocketServiceLauncher : IHostedService
    {
        private const int PingInterval = 10000;

        private Process Proc { get; set; }
        private ProcessStartInfo StartInfo { get; set; }
        private WebSocket WebSocket { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private ILog Logger { get; set; }
        private string LogDir { get; set; }
        private IConfiguration Configuration { get; set; }
        private ConfigurationExtension ConfigurationExtension { get; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private SignalrServiceClient SignalrServiceClient { get; set; }
        private IHostEnvironment HostEnvironment { get; set; }

        public SocketServiceLauncher(
            IOptionsMonitor<ILog> options,
            IConfiguration configuration,
            ConfigurationExtension configurationExtension,
            CoreBaseSettings coreBaseSettings,
            IOptionsSnapshot<SignalrServiceClient> signalrServiceClient,
            IHostEnvironment hostEnvironment)
        {
            Logger = options.CurrentValue;
            CancellationTokenSource = new CancellationTokenSource();
            Configuration = configuration;
            ConfigurationExtension = configurationExtension;
            CoreBaseSettings = coreBaseSettings;
            SignalrServiceClient = signalrServiceClient.Value;
            HostEnvironment = hostEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var settings = ConfigurationExtension.GetSetting<SocketSettings>("socket");

                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = "node",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"\"{Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, settings.Path, "app.js"))}\"",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                StartInfo.EnvironmentVariables.Add("core.machinekey", Configuration["core:machinekey"]);
                StartInfo.EnvironmentVariables.Add("port", settings.Port);

                if (!string.IsNullOrEmpty(settings.RedisHost) && !string.IsNullOrEmpty(settings.RedisPort))
                {
                    StartInfo.EnvironmentVariables.Add("redis:host", settings.RedisHost);
                    StartInfo.EnvironmentVariables.Add("redis:port", settings.RedisPort);
                }

                if (CoreBaseSettings.Standalone)
                {
                    StartInfo.EnvironmentVariables.Add("portal.internal.url", "http://localhost");
                }

                LogDir = Logger.LogDirectory;
                StartInfo.EnvironmentVariables.Add("logPath", CrossPlatform.PathCombine(LogDir, "web.socketio.log"));
                StartNode();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopPing();
            StopNode();
            return Task.CompletedTask;
        }

        private void StartNode()
        {
            StopNode();
            Proc = Process.Start(StartInfo);

            var task = new Task(StartPing, CancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start(TaskScheduler.Default);
        }

        private void StopNode()
        {
            try
            {
                if (Proc != null && !Proc.HasExited)
                {
                    Proc.Kill();
                    if (!Proc.WaitForExit(10000)) /* wait 10 seconds */
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
                Logger.Error("SocketIO failed stop", e);
            }
        }

        private void StartPing()
        {
            Thread.Sleep(PingInterval);

            var error = false;
            WebSocket = new WebSocket($"ws://127.0.0.1:{StartInfo.EnvironmentVariables["port"]}/socket.io/?EIO=3&transport=websocket");
            WebSocket.SetCookie(new WebSocketSharp.Net.Cookie("authorization", SignalrServiceClient.CreateAuthToken()));
            WebSocket.EmitOnPing = true;

            WebSocket.Log.Level = WebSocketSharp.LogLevel.Trace;

            WebSocket.Log.Output = (logData, filePath) =>
            {
                if (logData.Message.Contains("SocketException"))
                {
                    error = true;
                }

                Logger.Debug(logData.Message);
            };

            WebSocket.OnOpen += (sender, e) =>
            {
                Logger.Info("Open");
                error = false;

                Thread.Sleep(PingInterval);

                Task.Run(() =>
                {
                    while (WebSocket.Ping())
                    {
                        Logger.Debug("Ping " + WebSocket.ReadyState);
                        Thread.Sleep(PingInterval);
                    }
                    Logger.Debug("Reconnect" + WebSocket.ReadyState);

                }, CancellationTokenSource.Token);
            };

            WebSocket.OnClose += (sender, e) =>
            {
                Logger.Info("Close");
                if (CancellationTokenSource.IsCancellationRequested) return;

                if (error)
                {
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    WebSocket.Connect();
                }

            };

            WebSocket.OnMessage += (sender, e) =>
            {
                if (e.Data.Contains("error"))
                {
                    Logger.Error("Auth error");
                    CancellationTokenSource.Cancel();
                }
            };

            WebSocket.OnError += (sender, e) =>
            {
                Logger.Error("Error", e.Exception);
            };

            WebSocket.Connect();
        }

        private void StopPing()
        {
            try
            {
                CancellationTokenSource.Cancel();
                if (WebSocket.IsAlive)
                {
                    WebSocket.Close();
                    WebSocket = null;
                }
            }
            catch (Exception)
            {
                Logger.Error("Ping failed stop");
            }
        }
    }
}