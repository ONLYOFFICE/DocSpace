/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Caching;
using ASC.Mail.Configuration;

using MailKit.Security;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Mail.ImapSync
{
    [Singletone]
    public class ImapSyncService : IHostedService
    {
        private readonly ILog _log;
        private readonly IOptionsMonitor<ILog> _options;

        private readonly CancellationTokenSource _cancelTokenSource;

        private readonly ConcurrentDictionary<string, MailImapClient> clients;

        private readonly MailSettings _mailSettings;
        private readonly RedisClient _redisClient;

        private readonly IServiceProvider _serviceProvider;

        public ImapSyncService(IOptionsMonitor<ILog> options,
            RedisClient redisClient,
            MailSettings mailSettings,
            IServiceProvider serviceProvider)
        {
            _options = options;
            _redisClient = redisClient;
            _mailSettings = mailSettings;
            _serviceProvider = serviceProvider;

            clients = new ConcurrentDictionary<string, MailImapClient>();

            _cancelTokenSource = new CancellationTokenSource();

            try
            {
                _log = _options.Get("ASC.Mail.ImapSyncService");

                _log.Info("Service is ready.");
            }
            catch (Exception ex)
            {
                _log.FatalFormat("ImapSyncService error under construct: {0}", ex.ToString());

                throw;
            }
        }

        public Task RedisSubscribe(CancellationToken cancellationToken)
        {
            _log.Info("Try to subscribe redis...");

            if (_redisClient == null)
            {
                return StopAsync(cancellationToken);
            }

            try
            {
                _redisClient.SubscribeQueueKey<CashedTenantUserMailBox>(CreateNewClient);

                _log.Info("Success redis subscribe!");
            }
            catch (Exception ex)
            {
                _log.Error($"Didn`t subscribe to redis. Message: {ex.Message}");

                return StopAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public void CreateNewClient(CashedTenantUserMailBox cashedTenantUserMailBox)
        {
            if (clients.ContainsKey(cashedTenantUserMailBox.UserName))
            {
                clients[cashedTenantUserMailBox.UserName]?.CheckRedis(cashedTenantUserMailBox.Folder, cashedTenantUserMailBox.tags);

                _log.Info($"User Activity -> {cashedTenantUserMailBox.UserName}, folder={cashedTenantUserMailBox.Folder}. ");

                return;
            }
            else
            {
                if (clients.TryAdd(cashedTenantUserMailBox.UserName, null))
                {
                    MailImapClient client;

                    try
                    {
                        client = new MailImapClient(cashedTenantUserMailBox.UserName, cashedTenantUserMailBox.Tenant, _cancelTokenSource.Token, _mailSettings, _serviceProvider);

                        if (client == null)
                        {
                            _log.Info($"Can`t create Mail client for user {cashedTenantUserMailBox.UserName}.");
                        }
                        else
                        {
                            clients.TryUpdate(cashedTenantUserMailBox.UserName, client, null);

                            client.OnCriticalError += Client_DeleteClient;
                        }
                    }
                    catch (TimeoutException exTimeout)
                    {
                        _log.Warn($"[TIMEOUT] Create mail client for user {cashedTenantUserMailBox.UserName}. {exTimeout}");
                    }
                    catch (OperationCanceledException)
                    {
                        _log.Info("[CANCEL] Create mail client for user {userName}.");
                    }
                    catch (AuthenticationException authEx)
                    {
                        _log.Error($"[AuthenticationException] Create mail client for user {cashedTenantUserMailBox.UserName}. {authEx}");
                    }
                    catch (WebException webEx)
                    {
                        _log.Error($"[WebException] Create mail client for user {cashedTenantUserMailBox.UserName}. {webEx}");
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Create mail client for user {cashedTenantUserMailBox.UserName}. {ex}");
                    }

                }
            }
        }

        private void Client_DeleteClient(object sender, EventArgs e)
        {
            if (sender is MailImapClient client)
            {
                var clientKey = client?.UserName;

                if (clients.TryRemove(clientKey, out MailImapClient trashValue))
                {
                    trashValue.OnCriticalError -= Client_DeleteClient;
                    trashValue?.Dispose();

                    _log.Info($"ImapSyncService. MailImapClient {clientKey} died and was remove.");
                }
                else
                {
                    _log.Info($"ImapSyncService. MailImapClient {clientKey} died, bud wasn`t remove.");
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("Start service\r\n");

                return RedisSubscribe(cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);

                return StopAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("Stoping service\r\n");

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Stop service Error: {0}\r\n", ex.ToString());
            }
            finally
            {
                _log.Info("Stop service\r\n");
            }

            return Task.CompletedTask;
        }
    }
}