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
using System.Linq;
using System.Net;
using System.Threading;
using ASC.Core.Common.Caching;
using ASC.Mail.Core;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using MailKit.Security;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using ILog = ASC.Common.Logging.ILog;
using System.ServiceProcess;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ASC.Mail.Models;
using Microsoft.Extensions.Options;

namespace ASC.Mail.ImapSync
{
    public class ImapSyncService : IHostedService
    {
        private readonly ILog _log;
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly ConcurrentDictionary<string,MailImapClient> clients;

        private readonly SemaphoreSlim CreateClientSemaphore;
        private ManualResetEvent _resetEvent;

        public ImapSyncService(IOptionsMonitor<ILog> options)
        {
            CreateClientSemaphore = new SemaphoreSlim(1, 1);
            try
            {
                _log = options.Get("ASC.Mail.ImapSyncService");

                clients = new ConcurrentDictionary<string, MailImapClient>();

                _cancelTokenSource = new CancellationTokenSource();

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

            try
            {
                var cache = new RedisClient();

                if (cache == null)
                {
                    return StopAsync(cancellationToken);
                }

                cache.SubscribeQueueKey<CashedTenantUserMailBox>(CreateNewClient);

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
            string clientKey = cashedTenantUserMailBox.MailBoxId.ToString();

            if (clients.Keys.Contains(clientKey))
            {
                if(clients[clientKey]!=null)
                {
                    clients[clientKey]?.CheckRedis(cashedTenantUserMailBox.Folder, cashedTenantUserMailBox.tags);

                    _log.Info($"ImapSyncService. User Activity -> {cashedTenantUserMailBox.MailBoxId}, clients.Count={clients.Count} ");

                    return;
                }
                else
                {
                    if(clients.TryRemove(clientKey, out MailImapClient trashValue))
                    {
                        _log.Info($"ImapSyncService. MailImapClient {clientKey} died and was remove.");
                    }
                    else
                    {
                        _log.Info($"ImapSyncService. MailImapClient {clientKey} died, bud wasn`t remove");
                    }
                }
            }

            try
            {
                CreateClientSemaphore.Wait();

                if (clients.Keys.Contains(clientKey)) return;

                var _engineFactory = new EngineFactory(-1);

                var userMailboxesExp = new UserMailboxesExp(cashedTenantUserMailBox.Tenant, cashedTenantUserMailBox.UserName, onlyTeamlab: true);

                var mailboxes = _engineFactory.MailboxEngine.GetMailboxDataList(userMailboxesExp);

                var mailbox = mailboxes.FirstOrDefault(x => x.MailBoxId == cashedTenantUserMailBox.MailBoxId);

                if (mailbox == null) return;

                clients.TryAdd(clientKey, null);

                Thread thread = new Thread(() => CreateMailClient(mailbox, clientKey));

                thread.Start();
            }
            finally
            {
                CreateClientSemaphore.Release();
            }
        }

        private void CreateMailClient(MailBoxData mailbox, string clientKey)
        {
            var log = LogManager.GetLogger($"ASC.Mail.ImapSyncService.Mbox_{mailbox.MailBoxId}_{Thread.CurrentThread.ManagedThreadId}");

            MailImapClient client = null;

            var connectError = false;

            try
            {
                client = new MailImapClient(mailbox, _cancelTokenSource.Token, _tasksConfig,
                   mailbox.IsTeamlab || _tasksConfig.SslCertificateErrorsPermit, _tasksConfig.ProtocolLogPath, log);

                log.DebugFormat("MailClient.LoginImapPop(Tenant = {0}, MailboxId = {1} Address = '{2}')",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);

                if (client == null)
                {
                    log.InfoFormat("ReleaseMailbox(Tenant = {0} MailboxId = {1}, Address = '{2}')",
                               mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);
                    //ReleaseMailbox(mailbox);
                }
                else
                {
                    client.DeleteClient += Client_DeleteClient;
                    clients.TryUpdate(clientKey, client, null);
                }
            }
            catch (System.TimeoutException exTimeout)
            {
                log.WarnFormat(
                    "[TIMEOUT] CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, exTimeout.ToString());

                connectError = true;
            }
            catch (OperationCanceledException)
            {
                log.InfoFormat(
                    "[CANCEL] CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail);
            }
            catch (AuthenticationException authEx)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, authEx.ToString());

                connectError = true;
            }
            catch (WebException webEx)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, webEx.ToString());

                connectError = true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat(
                    "CreateTasks->client.LoginImapPop(Tenant = {0}, MailboxId = {1}, Address = '{2}')\r\nException: {3}\r\n",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail,
                    ex is ImapProtocolException || ex is Pop3ProtocolException ? ex.Message : ex.ToString());
            }
            finally
            {
                if (connectError)
                {
                    SetMailboxAuthError(mailbox, log);
                }
            }
        }

        private void Client_DeleteClient(object sender, EventArgs e)
        {
            var client = sender as MailImapClient;

            if (client == null) return;

            var clientKey=clients.FirstOrDefault(x => x.Value == client).Key;

            if (clients.TryRemove(clientKey, out MailImapClient trashValue))
            {
                _log.Info($"ImapSyncService. MailImapClient {clientKey} died and was remove.");
            }
            else
            {
                _log.Info($"ImapSyncService. MailImapClient {clientKey} died, bud wasn`t remove");
            }
        }

        private static void SetMailboxAuthError(MailBoxData mailbox, ILog log)
        {
            try
            {
                if (mailbox.AuthErrorDate.HasValue)
                    return;

                mailbox.AuthErrorDate = DateTime.UtcNow;

                var engine = new EngineFactory(mailbox.TenantId);
                engine.MailboxEngine.SetMaiboxAuthError(mailbox.MailBoxId, mailbox.AuthErrorDate.Value);
            }
            catch (Exception ex)
            {
                log.ErrorFormat(
                    "CreateTasks->SetMailboxAuthError(Tenant = {0}, MailboxId = {1}, Address = '{2}') Exception: {3}",
                    mailbox.TenantId, mailbox.MailBoxId, mailbox.EMail, ex.Message);
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