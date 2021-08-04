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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Caching;
using ASC.Core.Notify.Signalr;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Mail.Clients.Imap;
using ASC.Mail.Configuration;
using ASC.Mail.Core.Dao;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Engine;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Mail.Storage;
using ASC.Mail.Utils;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MimeKit;

using MailFolder = ASC.Mail.Models.MailFolder;

namespace ASC.Mail.ImapSync
{
    public class MailImapClient : IDisposable
    {
        public ConcurrentDictionary<string, List<MailSieveFilterData>> Filters { get; set; }

        private const int SIGNALR_WAIT_SECONDS = 30;

        private readonly MailEnginesFactory _mailEnginesFactory;
        private readonly MailSettings _mailSettings;
        private readonly MailInfoDao _mailInfoDao;
        private readonly StorageFactory _storageFactory;
        private readonly FolderEngine _folderEngine;
        private readonly UserManager _userManager;
        private readonly SignalrServiceClient _signalrServiceClient;
        private readonly RedisClient _redisClient;
        private readonly ILog _log;
        private readonly IndexEngine _indexEngine;

        readonly bool crmAvailable;

        public MailBoxData Account { get; private set; }

        private Dictionary<IMailFolder, MailFolder> foldersDictionary;

        private List<MailInfo> workFolderMails;
        private MailFolder workFolder;

        private CancellationToken CancelToken { get; set; }
        private CancellationTokenSource StopTokenSource { get; set; }

        private SimpleImapClient imapClient;

        public async void CheckRedis(int folderActivity, IEnumerable<int> tags)
        {
            var key = _redisClient.CreateQueueKey(Account.MailBoxId);

            _log.Debug($"ProcessActionFromRedis. Begin read key: {key}.");

            int iterationCount = 0;

            try
            {
                while (true)
                {
                    var actionFromCache = await _redisClient.PopFromQueue<CashedMailUserAction>(key);

                    if (actionFromCache == null) break;

                    imapClient.TrySetFlagsInImap( );
                }
            }
            catch (Exception ex)
            {
                _log.Error($"ProcessActionFromRedis. Error: {ex.Message}.");
            }

            _log.Debug($"ProcessActionFromRedis end. {iterationCount} keys readed.");

            var activityUserFolder = foldersDictionary.Values.FirstOrDefault(x=>x.Folder==(FolderType)folderActivity);

            if (activityUserFolder!=workFolder)
            {
                await ChangeFolder(activityUserFolder);

                _log.Debug($"CheckRedis. WorkFolder change to {activityUserFolder}.");
            }
        }

        public MailImapClient(MailBoxData mailbox, CancellationToken cancelToken, MailSettings mailSettings, IServiceProvider serviceProvider)
        {
            Account = mailbox;
            _mailSettings = mailSettings;

            var clientScope = serviceProvider.CreateScope().ServiceProvider;

            var tenantManager = clientScope.GetService<TenantManager>();
            tenantManager.SetCurrentTenant(Account.TenantId);

            var securityContext = clientScope.GetService<SecurityContext>();
            securityContext.AuthenticateMe(new Guid(Account.UserId));

            _mailInfoDao = clientScope.GetService<MailInfoDao>();
            _mailEnginesFactory = clientScope.GetService<MailEnginesFactory>();
            _indexEngine = clientScope.GetService<IndexEngine>();
            _storageFactory = clientScope.GetService<StorageFactory>();
            _folderEngine = clientScope.GetService<FolderEngine>();
            _userManager = clientScope.GetService<UserManager>();
            _signalrServiceClient = clientScope.GetService<IOptionsSnapshot<SignalrServiceClient>>().Get("mail");
            _redisClient= clientScope.GetService<RedisClient>();
            _log= clientScope.GetService<ILog>();

            _log.Name = $"ASC.Mail.MailImapClient.Mbox_{mailbox.MailBoxId}";

            crmAvailable = false;// mailbox.IsCrmAvailable(_mailSettings.DefaultApiSchema, Log);

            StopTokenSource = new CancellationTokenSource();

            CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, StopTokenSource.Token).Token;

            if (_redisClient == null)
            {
                _log.Error($"Ctor: _redisClient == null.");

                return;
            }


            //if (certificatePermit)
            //    Imap.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            imapClient = new SimpleImapClient(mailbox, cancelToken, mailSettings, clientScope.GetService<ILog>());
            imapClient.ClientIsReady += ImapClient_ClientIsReady;

            Filters = new ConcurrentDictionary<string, List<MailSieveFilterData>>();
        }

        private void ImapClient_ClientIsReady(object sender, EventArgs e)
        {
            DetectFolders(imapClient.foldersList);

            imapClient.NewActionFromImap += ImapClient_NewActionFromImap;
        }

        private void ImapClient_NewActionFromImap(object sender, EventArgs e)
        {
            while (imapClient.actionsFromImap.TryDequeue(out ImapAction newAction))
            {
                string uidl = SplittedUidl.ToUidl(workFolder.Folder, newAction.UniqueId);

                var massageInDB = workFolderMails.FirstOrDefault(x => x.Uidl == uidl);

                if (massageInDB == null) continue;

                switch (newAction.FolderAction)
                {
                    case ImapAction.Action.Nothing:
                        break;
                    case ImapAction.Action.SetAsRead:
                        _mailEnginesFactory.MessageEngine.SetUnread(new List<int>() { massageInDB.Id }, false);
                        break;
                    case ImapAction.Action.SetAsUnread:
                        _mailEnginesFactory.MessageEngine.SetUnread(new List<int>() { massageInDB.Id }, true);
                        break;
                    case ImapAction.Action.SetAsImportant:
                        _mailEnginesFactory.MessageEngine.SetImportant(new List<int>() { massageInDB.Id }, true);
                        break;
                    case ImapAction.Action.SetAsNotImpotant:
                        _mailEnginesFactory.MessageEngine.SetImportant(new List<int>() { massageInDB.Id }, false);
                        break;
                    case ImapAction.Action.SetAsDeleted:
                        break;
                    case ImapAction.Action.RemovedFromFolder:
                        break;
                    case ImapAction.Action.New:
                        break;
                    default:
                        break;
                }
            }
        }

        private async void DetectFolders(List<IMailFolder> foldersIMAP)
        {
            if (foldersDictionary == null)
            {
                foldersDictionary = new Dictionary<IMailFolder, MailFolder>();
            }
            else return;

            foreach (var folderIMAP in foldersIMAP)
            {
                var dbFolder = DetectFolder(folderIMAP);

                foldersDictionary.Add(folderIMAP, dbFolder);

                if (dbFolder == null)
                {
                    _log.Debug($"{folderIMAP.Name}-> skipped");
                    continue;
                }
                else
                {
                    if (workFolder == null) await ChangeFolder(dbFolder);
                    _log.Debug($"{folderIMAP.Name}-> ready to work.");
                }                
            }

            


            _log.Debug($"Loaded {foldersDictionary.Count} folders for work.");
        }

        public void Dispose()
        {
            _log.Info("Dispose.");

            StopTokenSource?.Cancel();

            try
            {
                imapClient?.Dispose();

                StopTokenSource?.Dispose();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Dispose(Mb_Id={0} Mb_Addres: '{1}') Exception: {2}", Account.MailBoxId,
                    Account.EMail.Address, ex.Message);
            }
        }

        public bool SetFlagsFromImap(IMessageSummary imap_message, MailInfo db_message)
        {
            if (imap_message == null || db_message == null) return false;
            try
            {
                _log.Debug("SynchronizeMessageFlagsFromImap begin.");
                _log.Debug($"SynchronizeMessageFlagsFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, flag={imap_message.Flags.Value.ToString()}.");
                _log.Debug($"SynchronizeMessageFlagsFromImap: db_message={db_message.Uidl}, folder={db_message.Folder}, IsRemoved={db_message.IsRemoved}.");
                bool unread = !imap_message.Flags.Value.HasFlag(MessageFlags.Seen);
                bool important = imap_message.Flags.Value.HasFlag(MessageFlags.Flagged);
                bool removed = imap_message.Flags.Value.HasFlag(MessageFlags.Deleted);

                if (db_message.IsNew ^ unread) _mailEnginesFactory.MessageEngine.SetUnread(new List<int>() { db_message.Id }, unread, true);
                if (db_message.Importance ^ important) _mailEnginesFactory.MessageEngine.SetImportant(new List<int>() { db_message.Id }, important);
                if (removed) _mailEnginesFactory.MessageEngine.SetRemoved(new List<int>() { db_message.Id });
                _log.Debug("SynchronizeMessageFlagsFromImap finish.");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"SynchronizeMessageFlagsFromImap({imap_message.UniqueId},{db_message})->{ex.Message}");

                return false;
            }
        }

        public async Task ChangeFolder(MailFolder newFolder)
        {
            if (workFolder == newFolder) return;

            workFolder = newFolder;

            try
            {
                var newImapFolder = foldersDictionary.FirstOrDefault(x => x.Value.Folder == workFolder.Folder).Key;

                if (newImapFolder == null) return;

                var exp = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                    .SetMailboxId(Account.MailBoxId)
                    .SetFolder((int)workFolder.Folder);

                workFolderMails=_mailInfoDao.GetMailInfoList(exp.Build()).ToList();

                Task t= imapClient.ChangeFolder(newImapFolder);

                await t;

                UpdateDbFolder(imapClient.MessagesList);
            }
            catch (Exception ex)
            {
                _log.Error($"ChangeFolder(New folder={newFolder.Name})->{ex.Message}");
            }
        }

        private void UpdateDbFolder(List<IMessageSummary> imapFolderMails)
        {
            if (imapFolderMails == null) return;

            _log.Debug($"UpdateDbFolder.");

            try
            {
                foreach (var imap_message in imapFolderMails)
                {
                    _log.Debug($"UpdateDbFolder: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, folder={imap_message.Folder.Name}.");

                    var db_message = workFolderMails.FirstOrDefault(x => x.Uidl == SplittedUidl.ToUidl(workFolder.Folder, imap_message.UniqueId));

                    if (db_message == null) continue;
                    
                        SetMessageFlagsFromImap(imap_message, db_message);

                    if (db_message.IsRemoved)
                    {
                        var restoreQuery = SimpleMessagesExp.CreateBuilder(Account.TenantId, Account.UserId)
                            .SetMessageId(db_message.Id)
                            .Build();

                        _mailInfoDao.SetFieldValue(restoreQuery, "is_removed", false);

                        _mailEnginesFactory.MessageEngine.Restore(new List<int>() { db_message.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"UpdateDbFolder(IMailFolder->{ex.Message}");
            }
        }

        private void SetMessageFlagsFromImap(IMessageSummary imap_message, MailInfo db_message)
        {
            if (imap_message == null || db_message == null) return;

            try
            {
                    _log.Debug($"SetMessageFlagsFromImap: imap_message_Uidl={imap_message.UniqueId.Id.ToString()}, flag={imap_message.Flags.Value.ToString()}.");
                    _log.Debug($"SetMessageFlagsFromImapp: db_message={db_message.Uidl}, folder={db_message.Folder}, IsRemoved={db_message.IsRemoved}.");
                    bool unread = !imap_message.Flags.Value.HasFlag(MessageFlags.Seen);
                    bool important = imap_message.Flags.Value.HasFlag(MessageFlags.Flagged);
                    bool removed = imap_message.Flags.Value.HasFlag(MessageFlags.Deleted);

                    if (db_message.IsNew ^ unread) _mailEnginesFactory.MessageEngine.SetUnread(new List<int>() { db_message.Id }, unread, true);
                    if (db_message.Importance ^ important) _mailEnginesFactory.MessageEngine.SetImportant(new List<int>() { db_message.Id }, important);
                    if (removed) _mailEnginesFactory.MessageEngine.SetRemoved(new List<int>() { db_message.Id });
                
            }
            catch (Exception ex)
            {
                _log.Error($"SetMessageFlagsFromImap->{ex.Message}");
            }
        }

        private async Task<bool> CreateMessageInDB(IMessageSummary imap_message, IMailFolder imapFolder)
        {
            bool result = true;

            Stopwatch watch = null;

            if (_mailSettings.CollectStatistics)
            {
                watch = new Stopwatch();
                watch.Start();
            }

            try
            {
                var message = await imapFolder.GetMessageAsync(imap_message.UniqueId, CancelToken);

                message.FixDateIssues(imap_message?.InternalDate, _log);

                bool unread = false, important = false;

                if ((imap_message != null) && imap_message.Flags.HasValue)
                {
                    unread = !imap_message.Flags.Value.HasFlag(MessageFlags.Seen);
                    important = imap_message.Flags.Value.HasFlag(MessageFlags.Flagged);
                }

                message.FixEncodingIssues(_log);

                var uid = imap_message.UniqueId.ToString();
                var folder = foldersDictionary[imapFolder];
                var uidl = string.Format("{0}-{1}", uid, (int)folder.Folder);

                _log.Info($"Get message (UIDL: '{uidl}', MailboxId = {Account.MailBoxId}, Address = '{Account.EMail}')");

                var messageDB = _mailEnginesFactory.MessageEngine.Save(Account, message, uidl, folder, null, unread, _log);

                if (messageDB == null || messageDB.Id <= 0)
                {
                    _log.Debug("CreateMessageInDB: failed.");
                    result = false;
                    return result;
                }

                _log.Info($"Message saved (id: {messageDB.Id}, From: '{messageDB.From}', Subject: '{messageDB.Subject}', Unread: {messageDB.IsNew})");

                _log.Info("DoOptionalOperations->START");

                DoOptionalOperations(messageDB, message, Account, folder);

                _log.Info("DoOptionalOperations->END");
            }
            catch (Exception ex)
            {
                _log.Error($"CreateMessageInDB Exception:{ex.Message}");

                result = false;
            }
            finally
            {
                if (_mailSettings.CollectStatistics && watch != null)
                {
                    watch.Stop();

                    LogStat("CreateMessageInDB", watch.Elapsed, result);
                }

                _log.Debug($"CreateMessageInDB time={watch.Elapsed.TotalMilliseconds} ms.");
            }

            return result;
        }

        private MailFolder DetectFolder(IMailFolder folder)
        {
            var folderName = folder.Name.ToLowerInvariant();

            if (_mailSettings.SkipImapFlags != null &&
                _mailSettings.SkipImapFlags.Any() &&
                _mailSettings.SkipImapFlags.Contains(folderName))
            {
                return null;
            }

            FolderType folderId;

            if ((folder.Attributes & FolderAttributes.Inbox) != 0)
            {
                return new MailFolder(FolderType.Inbox, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Sent) != 0)
            {
                return new MailFolder(FolderType.Sent, folder.Name);
            }
            if ((folder.Attributes & FolderAttributes.Junk) != 0)
            {
                return new MailFolder(FolderType.Spam, folder.Name);
            }
            if ((folder.Attributes &
                 (FolderAttributes.All |
                  FolderAttributes.NoSelect |
                  FolderAttributes.NonExistent |
                  FolderAttributes.Trash |
                  FolderAttributes.Archive |
                  FolderAttributes.Drafts |
                  FolderAttributes.Flagged)) != 0)
            {
                return null; // Skip folders
            }

            if (_mailSettings.ImapFlags != null &&
                _mailSettings.ImapFlags.Any() &&
                _mailSettings.ImapFlags.ContainsKey(folderName))
            {
                folderId = (FolderType)_mailSettings.ImapFlags[folderName];
                return new MailFolder(folderId, folder.Name);
            }

            if (_mailSettings.SpecialDomainFolders.Any() &&
                _mailSettings.SpecialDomainFolders.ContainsKey(Account.Server))
            {
                var domainSpecialFolders = _mailSettings.SpecialDomainFolders[Account.Server];

                if (domainSpecialFolders.Any() &&
                    domainSpecialFolders.ContainsKey(folderName))
                {
                    var info = domainSpecialFolders[folderName];
                    return info.skip ? null : new MailFolder(info.folder_id, folder.Name);
                }
            }

            if (_mailSettings.DefaultFolders == null || !_mailSettings.DefaultFolders.ContainsKey(folderName))
                return new MailFolder(FolderType.Inbox, folder.Name, new[] { folder.FullName });

            folderId = (FolderType)_mailSettings.DefaultFolders[folderName];
            return new MailFolder(folderId, folder.Name);
        }



        

        private void LogStat(string method, TimeSpan duration, bool failed)
        {
            if (!_mailSettings.CollectStatistics)
                return;

            _log.DebugWithProps(method,
                new KeyValuePair<string, object>("duration", duration.TotalMilliseconds),
                new KeyValuePair<string, object>("mailboxId", Account.MailBoxId),
                new KeyValuePair<string, object>("address", Account.EMail.ToString()),
                new KeyValuePair<string, object>("isFailed", failed));
        }

        private void DoOptionalOperations(MailMessageData message, MimeMessage mimeMessage, MailBoxData mailbox, MailFolder folder)
        {
            try
            {
                var tagIds = new List<int>();

                if (folder.Tags.Any())
                {
                    _log.Debug("DoOptionalOperations->GetOrCreateTags()");

                    tagIds = _mailEnginesFactory.TagEngine.GetOrCreateTags(mailbox.TenantId, mailbox.UserId, folder.Tags);
                }

                _log.Debug("DoOptionalOperations->IsCrmAvailable()");

                //if (IsCrmAvailable(mailbox, log))
                //{
                //    log.Debug("DoOptionalOperations->GetCrmTags()");

                //    var crmTagIds = mailFactory.TagEngine.GetCrmTags(message.FromEmail);

                //    if (crmTagIds.Any())
                //    {
                //        if (tagIds == null)
                //            tagIds = new List<int>();

                //        tagIds.AddRange(crmTagIds.Select(t => t.Id));
                //    }
                //}

                if (tagIds.Any())
                {
                    if (message.TagIds == null || !message.TagIds.Any())
                        message.TagIds = tagIds;
                    else
                        message.TagIds.AddRange(tagIds);

                    message.TagIds = message.TagIds.Distinct().ToList();
                }

                _log.Debug("DoOptionalOperations->AddMessageToIndex()");

                var wrapper = message.ToMailWrapper(mailbox.TenantId, new Guid(mailbox.UserId));

                _mailEnginesFactory.IndexEngine.Add(wrapper);

                foreach (var tagId in tagIds)
                {
                    try
                    {
                        _log.DebugFormat("DoOptionalOperations->SetMessagesTag(tagId: {0})", tagId);

                        _mailEnginesFactory.TagEngine.SetMessagesTag(new List<int> { message.Id }, tagId);
                    }
                    catch (Exception e)
                    {
                        _log.ErrorFormat(
                            "SetMessagesTag(tenant={0}, userId='{1}', messageId={2}, tagid = {3}) Exception:\r\n{4}\r\n",
                            mailbox.TenantId, mailbox.UserId, message.Id, e.ToString(),
                            tagIds != null ? string.Join(",", tagIds) : "null");
                    }
                }

                _log.Debug("DoOptionalOperations->AddRelationshipEventForLinkedAccounts()");

                _mailEnginesFactory.CrmLinkEngine.AddRelationshipEventForLinkedAccounts(mailbox, message, _mailSettings.DefaultApiSchema);

                _log.Debug("DoOptionalOperations->SaveEmailInData()");

                _mailEnginesFactory.EmailInEngine.SaveEmailInData(mailbox, message, _mailSettings.DefaultApiSchema);

                _log.Debug("DoOptionalOperations->SendAutoreply()");

                _mailEnginesFactory.AutoreplyEngine.SendAutoreply(mailbox, message, _mailSettings.DefaultApiSchema, _log);

                _log.Debug("DoOptionalOperations->UploadIcsToCalendar()");

                if (folder.Folder != Enums.FolderType.Spam)
                {
                    _mailEnginesFactory
                        .CalendarEngine
                        .UploadIcsToCalendar(mailbox, message.CalendarId, message.CalendarUid, message.CalendarEventIcs,
                            message.CalendarEventCharset, message.CalendarEventMimeType, mailbox.EMail.Address,
                            _mailSettings.DefaultApiSchema);
                }

                if (_mailSettings.SaveOriginalMessage)
                {
                    _log.Debug("DoOptionalOperations->StoreMailEml()");
                    StoreMailEml(mailbox.TenantId, mailbox.UserId, message.StreamId, mimeMessage);
                }

                _log.Debug("DoOptionalOperations->ApplyFilters()");

                var filters = GetFilters(_mailEnginesFactory, _log);

                _mailEnginesFactory.FilterEngine.ApplyFilters(message, mailbox, folder, filters);

                _log.Debug("DoOptionalOperations->NotifySignalrIfNeed()");

                SendUnreadUser();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("DoOptionalOperations() Exception:\r\n{0}\r\n", ex.ToString());
            }
        }

        private void SendUnreadUser()
        {
            try
            {
                var mailFolderInfos = _folderEngine.GetFolders();

                var count = (from mailFolderInfo in mailFolderInfos
                             where mailFolderInfo.id == FolderType.Inbox
                             select mailFolderInfo.unreadMessages)
                    .FirstOrDefault();

                if (Account.UserId != Constants.LostUser.ID.ToString())
                {
                    _signalrServiceClient.SendUnreadUser(Account.TenantId, Account.UserId, count);
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat("Unknown Error. {0}, {1}", e.ToString(),
                    e.InnerException != null ? e.InnerException.Message : string.Empty);
            }
        }


        private List<MailSieveFilterData> GetFilters(MailEnginesFactory factory, ILog log)
        {
            var user = factory.UserId;

            if (string.IsNullOrEmpty(user))
                return new List<MailSieveFilterData>();

            try
            {
                if (Filters.ContainsKey(user)) return Filters[user];

                var filters = factory.FilterEngine.GetList();

                Filters.TryAdd(user, filters);

                return filters;
            }
            catch (Exception ex)
            {
                log.Error("GetFilters failed", ex);
            }

            return new List<MailSieveFilterData>();
        }

        public string StoreMailEml(int tenant, string user, string streamId, MimeMessage message)
        {
            if (message == null)
                return string.Empty;

            // Using id_user as domain in S3 Storage - allows not to add quota to tenant.
            var savePath = MailStoragePathCombiner.GetEmlKey(user, streamId);

            var storage = _storageFactory.GetMailStorage(tenant);

            try
            {
                using (var stream = new MemoryStream())
                {
                    message.WriteTo(stream);

                    var res = storage.Save(savePath, stream, MailStoragePathCombiner.EML_FILE_NAME).ToString();

                    _log.InfoFormat("StoreMailEml() tenant='{0}', user_id='{1}', save_eml_path='{2}' Result: {3}", tenant, user, savePath, res);

                    return res;
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("StoreMailEml Exception: {0}", ex.ToString());
            }

            return string.Empty;
        }

        private void SetMailboxAuthError(bool state)
        {
            if (Account.AuthErrorDate.HasValue) return;
            try
            {
                if (state)
                {
                    Account.AuthErrorDate = DateTime.UtcNow;
                    _mailEnginesFactory.MailboxEngine.SetMaiboxAuthError(Account.MailBoxId, DateTime.UtcNow);
                }
                else
                {
                    Account.AuthErrorDate = null;
                    _mailEnginesFactory.MailboxEngine.SetMaiboxAuthError(Account.MailBoxId, null);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"SetMailboxAuthError({Account.EMail}): Exception: {ex.Message}");
            }
        }
    }
}