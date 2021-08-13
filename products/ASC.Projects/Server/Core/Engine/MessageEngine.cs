/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
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
            Init(NotifyConstants.Event_NewCommentForMessage);
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


            DaoProjectFactory.GetMessageDao().SaveOrUpdate(message);

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

            _ = FactoryIndexer.IndexAsync(DaoProjectFactory.GetMessageDao().ToDbMessage(message));

            return message;
        }

        public Message ChangeStatus(Message message)
        {
            message.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            message.LastModifiedOn = TenantUtil.DateTimeNow();

            ProjectSecurity.DemandEdit(message);
            DaoProjectFactory.GetMessageDao().SaveOrUpdate(message);

            return message;
        }

        public void Delete(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Project == null) throw new Exception("Project");

            ProjectSecurity.DemandEdit(message);

            DaoProjectFactory.GetMessageDao().Delete(message.ID);

            var recipients = GetSubscribers(message);

            if (recipients.Any() && !DisableNotifications)
            {
                NotifyClient.SendAboutMessageDeleting(recipients, message);
            }

            UnSubscribeAll(message);

            _ = FactoryIndexer.DeleteAsync(DaoProjectFactory.GetMessageDao().ToDbMessage(message));  
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