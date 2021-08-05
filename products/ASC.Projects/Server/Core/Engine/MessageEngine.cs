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
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Files.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;

namespace ASC.Projects.Engine
{
    [Scope]
    public class MessageEngine : ProjectEntityEngine
    {
        private FactoryIndexer<DbMessage> FactoryIndexer { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private Core.DataInterfaces.IDaoFactory DaoProjectFactory { get; set; }

        public MessageEngine(FactoryIndexer<DbMessage> factoryIndexer, SecurityContext securityContext, TenantUtil tenantUtil, Core.DataInterfaces.IDaoFactory daoProjectFactory, NotifySource notifySource, Files.Core.IDaoFactory daoFactory, EngineFactory engineFactory, ProjectSecurity projectSecurity, NotifyClient notifyClient) : base(securityContext, daoFactory, notifySource, engineFactory, projectSecurity, notifyClient)
        {
            FactoryIndexer = factoryIndexer;
            TenantUtil = tenantUtil;
            DaoFactory = daoFactory;
            DaoProjectFactory = daoProjectFactory;
        }

        public MessageEngine Init(bool disableNotifications)
        {
            Init(NotifyConstants.Event_NewCommentForMessage, disableNotifications);
            return this;
        }

        #region Get Discussion

        public override ProjectEntity GetEntityByID(int id)
        {
            return GetByID(id);
        }

        public Message GetByID(int id)
        {
            return GetByID(id, true);
        }

        public Message GetByID(int id, bool checkSecurity)
        {
            var message = DaoProjectFactory.GetMessageDao().GetById(id);

            if (message != null)
                message.CommentsCount = DaoProjectFactory.GetCommentDao().Count(new List<ProjectEntity> { message }).FirstOrDefault();

            if (!checkSecurity)
                return message;

            return CanRead(message) ? message : null;
        }

        public IEnumerable<Message> GetAll()
        {
            return DaoProjectFactory.GetMessageDao().GetAll().Where(CanRead);
        }

        public IEnumerable<Message> GetByProject(int projectID)
        {
            var messages = DaoProjectFactory.GetMessageDao().GetByProject(projectID)
                .Where(CanRead)
                .ToList();
            var commentsCount = DaoProjectFactory.GetCommentDao().Count(messages.ConvertAll(r => (ProjectEntity)r));

            return messages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            });
        }

        public IEnumerable<Message> GetMessages(int startIndex, int maxResult)
        {
            var messages = DaoProjectFactory.GetMessageDao().GetMessages(startIndex, maxResult)
                .Where(CanRead)
                .ToList();
            var commentsCount = DaoProjectFactory.GetCommentDao().Count(messages.Select(r => (ProjectEntity)r).ToList());

            return messages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            });
        }

        public IEnumerable<Message> GetByFilter(TaskFilter filter)
        {
            var messages = DaoProjectFactory.GetMessageDao().GetByFilter(filter, ProjectSecurity.CurrentUserAdministrator,
                ProjectSecurity.IsPrivateDisabled);

            var commentsCount = DaoProjectFactory.GetCommentDao().Count(messages.Select(r => (ProjectEntity)r).ToList());

            return messages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            });
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            return DaoProjectFactory.GetMessageDao().GetByFilterCount(filter, ProjectSecurity.CurrentUserAdministrator,
                ProjectSecurity.IsPrivateDisabled);
        }

        public List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter)
        {
            return DaoProjectFactory.GetMessageDao().GetByFilterCountForReport(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public bool IsExists(int id)
        {
            return DaoProjectFactory.GetMessageDao().IsExists(id);
        }

        public bool CanRead(Message message)
        {
            return ProjectSecurity.CanRead(message);
        }

        #endregion

        #region Save, Delete, Attach

        public Message SaveOrUpdate(Message message, bool notify, IEnumerable<Guid> participant,
            IEnumerable<int> fileIds = null)
        {
            if (message == null) throw new ArgumentNullException("message");

            var isNew = message.ID == default(int);

            message.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            message.LastModifiedOn = TenantUtil.DateTimeNow();

            if (isNew)
            {
                if (message.CreateBy == default(Guid)) message.CreateBy = SecurityContext.CurrentAccount.ID;
                if (message.CreateOn == default(DateTime)) message.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreate<Message>(message.Project);
            }
            else
            {
                ProjectSecurity.DemandEdit(message);
            }


            var db = DaoProjectFactory.GetMessageDao().Save(message);

            if (fileIds != null)
            {
                foreach (var fileId in fileIds)
                {
                    AttachFile(message, fileId);
                }
            }

            if (participant == null)
                participant = GetSubscribers(message).Select(r => new Guid(r.ID)).ToList();

            NotifyParticipiant(message, isNew, participant, GetFiles(message), notify);

            _ = FactoryIndexer.IndexAsync(db);

            return message;
        }

        public Message ChangeStatus(Message message)
        {
            message.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            message.LastModifiedOn = TenantUtil.DateTimeNow();

            ProjectSecurity.DemandEdit(message);
            DaoProjectFactory.GetMessageDao().Save(message);

            return message;
        }

        public void Delete(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Project == null) throw new Exception("Project");

            ProjectSecurity.DemandEdit(message);

            var db = DaoProjectFactory.GetMessageDao().Delete(message.ID);

            var recipients = GetSubscribers(message);

            if (recipients.Any() && !DisableNotifications)
            {
                NotifyClient.SendAboutMessageDeleting(recipients, message);
            }

            UnSubscribeAll(message);

            _ = FactoryIndexer.DeleteAsync(db);  
        }

        #endregion

        #region Notify

        protected void NotifyParticipiant(Message message, bool isMessageNew, IEnumerable<Guid> participant,
            IEnumerable<File<int>> uploadedFiles, bool sendNotify)
        {
            //Don't send anything if notifications are disabled
            if (DisableNotifications) return;

            var subscriptionRecipients = GetSubscribers(message);

            var recipients = new HashSet<Guid>(participant);

            foreach (var subscriptionRecipient in subscriptionRecipients)
            {
                var subscriptionRecipientId = new Guid(subscriptionRecipient.ID);
                if (!recipients.Contains(subscriptionRecipientId))
                {
                    UnSubscribe(message, subscriptionRecipientId);
                }
            }

            foreach (var subscriber in recipients)
            {
                Subscribe(message, subscriber);
            }

            if (sendNotify && recipients.Any())
            {
                NotifyClient.SendAboutMessageAction(GetSubscribers(message), message, isMessageNew,
                    EngineFactory.GetFileEngine().GetFileListInfoHashtable(uploadedFiles));
            }
        }

        #endregion
    }
}