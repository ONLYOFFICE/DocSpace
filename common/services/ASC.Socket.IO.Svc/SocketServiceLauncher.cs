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
using System.Collections.Generic;
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

using SocketIOClient;

namespace ASC.Socket.IO.Svc
{
    [Singletone]
    public class SocketServiceLauncher : IHostedService
    {
        private int PingInterval { get; set; }
        private int ReconnectAttempts { get; set; }
        private Process Proc { get; set; }
        private ProcessStartInfo StartInfo { get; set; }
        private SocketIO SocketClient { get; set; }
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

                var path = settings.Path;
                if (!Path.IsPathRooted(settings.Path))
                {
                    path = Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, settings.Path));
                }

                PingInterval = settings.PingInterval.GetValueOrDefault(10000);
                ReconnectAttempts = settings.ReconnectAttempts.GetValueOrDefault(5);

                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = "node",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"\"{Path.Combine(path, "server.js")}\"",
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
                StartInfo.EnvironmentVariables.Add("logPath", CrossPlatform.PathCombine(LogDir, "socket-io.%DATE%.log"));
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

            CancellationTokenSource = new CancellationTokenSource();

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

        private async void StartPing()
        {
            try
            {
                var settings = ConfigurationExtension.GetSetting<SocketSettings>("socket");

                var uri = new Uri($"ws://localhost:{settings.Port}"); //TODO: replace localhost to variable 

                var token = SignalrServiceClient.CreateAuthToken();

                SocketClient = new SocketIO(uri, new SocketIOOptions
                {
                    ExtraHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", token }
                    },
                    ConnectionTimeout = TimeSpan.FromSeconds(30),
                    Reconnection = true,
                    ReconnectionAttempts = ReconnectAttempts,
                    EIO = 4,
                    Path = "/socket.io",
                    Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                    RandomizationFactor = 0.5

                });

                SocketClient.OnConnected += IOClient_OnConnected;
                SocketClient.OnDisconnected += IOClient_OnDisconnected;
                SocketClient.OnReconnectAttempt += IOClient_OnReconnectAttempt;
                SocketClient.OnError += IOClient_OnError;
                SocketClient.On("pong", response =>
                {
                    Logger.Debug($"pong (server) at {response}");
                });

                Logger.Debug("Try to connect...");

                await SocketClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                if (CancellationTokenSource.IsCancellationRequested) return;

                Logger.Error(ex.Message);

                StopNode();
                Process.GetCurrentProcess().Kill();
            }
        }

        private async void IOClient_OnConnected(object sender, EventArgs e)
        {
            var socket = sender as SocketIO;

            Logger.Info($"Socket_OnConnected Socket.Id: {socket.Id}");

            while (SocketClient.Connected)
            {
                if (CancellationTokenSource.IsCancellationRequested) return;

                await Task.Delay(PingInterval);

                if (!SocketClient.Connected)
                    break;

                await SocketClient.EmitAsync("ping", DateTime.UtcNow.ToString());
            }
        }

        private void IOClient_OnDisconnected(object sender, string e)
        {
            Logger.Debug($"Socket_OnDisconnected {e}");
        }

        private void IOClient_OnReconnectAttempt(object sender, int attempt)
        {
            Logger.Debug($"Try to reconnect... attempt {attempt}");

            if (attempt >= ReconnectAttempts)
            {
                StopPing();
                Process.GetCurrentProcess().Kill();
            }
        }

        private void IOClient_OnError(object sender, string e)
        {
            Logger.Error($"IOClient_OnError {e}");
        }

        private void StopPing()
        {
            try
            {
                if (SocketClient != null)
                {
                    SocketClient.OnConnected -= IOClient_OnConnected;
                    SocketClient.OnDisconnected -= IOClient_OnDisconnected;
                    SocketClient.OnReconnectAttempt -= IOClient_OnReconnectAttempt;
                    SocketClient.OnError -= IOClient_OnError;

                    SocketClient.Dispose();
                    SocketClient = null;
                }

                CancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Logger.Error($"Ping failed stop {ex.Message}");
                StopNode();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}