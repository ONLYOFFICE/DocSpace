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
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;

namespace ASC.Projects.Engine
{
    [Scope]
    public class ProjectEntityEngine
    {
        public ISubscriptionProvider SubscriptionProvider { get; set; }
        public IRecipientProvider RecipientProvider { get; set; }
        public INotifyAction NotifyAction { get; set; }
        public FileEngine FileEngine { get; set; }
        public ProjectSecurity ProjectSecurity { get; set; }
        public SecurityContext SecurityContext { get; set; }
        public IDaoFactory DaoFactory { get; set; }
        public NotifyClient NotifyClient { get; set; }
        public EngineFactory EngineFactory { get; set; }

        public bool DisableNotifications { get; set; }

        public ProjectEntityEngine(SecurityContext securityContext, IDaoFactory daoFactory, NotifySource notifySource, EngineFactory engineFactory, ProjectSecurity projectSecurity, NotifyClient notifyClient)
        {
            SubscriptionProvider = notifySource.GetSubscriptionProvider();
            RecipientProvider = notifySource.GetRecipientsProvider();
            SecurityContext = securityContext;
            DaoFactory = daoFactory;
            NotifyClient = notifyClient;
            EngineFactory = engineFactory;
            ProjectSecurity = projectSecurity;
        }

        public void Init(INotifyAction notifyAction)
        {
            NotifyAction = notifyAction;
        }

        public virtual ProjectEntity GetEntityByID(int id)
        {
            return null;
        }

        #region Subscription

        public void Subscribe(ProjectEntity entity, Guid recipientID)
        {
            var recipient = RecipientProvider.GetRecipient(recipientID.ToString());

            if (recipient == null) return;

            if (!IsUnsubscribed(entity, recipientID) || ProjectSecurity.CanEdit(entity))
                SubscriptionProvider.Subscribe(NotifyAction, entity.NotifyId, recipient);
        }

        public void UnSubscribe(ProjectEntity entity)
        {
            UnSubscribe(entity, SecurityContext.CurrentAccount.ID);
        }

        public void UnSubscribe(ProjectEntity entity, Guid recipientID)
        {
            var recipient = RecipientProvider.GetRecipient(recipientID.ToString());

            if (recipient == null) return;

            SubscriptionProvider.UnSubscribe(NotifyAction, entity.NotifyId, recipient);
        }

        public void UnSubscribeAll<T>(T entity) where T : ProjectEntity
        {
            SubscriptionProvider.UnSubscribe(NotifyAction, entity.NotifyId);
        }

        public void UnSubscribeAll<T>(List<T> entity) where T : ProjectEntity
        {
            entity.ForEach(UnSubscribeAll);
        }

        public bool IsSubscribed(ProjectEntity entity)
        {
            return IsSubscribed(entity, SecurityContext.CurrentAccount.ID);
        }

        public bool IsSubscribed(ProjectEntity entity, Guid recipientID)
        {
            var recipient = RecipientProvider.GetRecipient(recipientID.ToString());

            var objects = SubscriptionProvider.GetSubscriptions(NotifyAction, recipient);

            return objects.Any(item => string.Compare(item, entity.NotifyId, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public bool IsUnsubscribed(ProjectEntity entity, Guid recipientID)
        {
            var recipient = RecipientProvider.GetRecipient(recipientID.ToString());

            return recipient != null && SubscriptionProvider.IsUnsubscribe((IDirectRecipient)recipient, NotifyAction, entity.NotifyId);
        }

        public void Follow(ProjectEntity entity)
        {
            Follow(entity, SecurityContext.CurrentAccount.ID);
        }

        public void Follow(ProjectEntity entity, Guid recipientID)
        {
            var recipient = RecipientProvider.GetRecipient(recipientID.ToString());

            if (recipient == null) return;

            if (!IsSubscribed(entity, recipientID))
                SubscriptionProvider.Subscribe(NotifyAction, entity.NotifyId, recipient);
            else
                SubscriptionProvider.UnSubscribe(NotifyAction, entity.NotifyId, recipient);
        }

        public List<IRecipient> GetSubscribers(ProjectEntity entity)
        {
            return SubscriptionProvider.GetRecipients(NotifyAction, entity.NotifyId).ToList();
        }

        #endregion

        #region Files

        public IEnumerable<File<int>> GetFiles(ProjectEntity entity)
        {
            if (entity == null) return new List<File<int>>();

            if (!ProjectSecurity.CanReadFiles(entity.Project)) return new List<File<int>>();

            var tagdao = DaoFactory.GetTagDao<int>();
            var filedao = DaoFactory.GetFileDao<int>();
            
            var ids = tagdao.GetTags(entity.GetType().Name + entity.ID, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => Convert.ToInt32(t.EntryId)).ToArray();
            var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File<int>>();

            var rootId = EngineFactory.GetFileEngine().GetRoot(entity.Project.ID);

            //delete tags when file moved from project folder
            files.Where(file => !file.RootFolderId.Equals(rootId)).ToList()
                .ForEach(file =>
                {
                    DetachFile(entity, file.ID);
                    files.Remove(file);
                });

            files.ForEach(r => r.Access = EngineFactory.GetFileEngine().GetFileShare(r, entity.Project.ID));
            return files;
            
        }

        public void AttachFile(ProjectEntity entity, int fileId, bool notify = false)
        {
            if (!ProjectSecurity.CanReadFiles(entity.Project)) return;

            File<int> file;

            var dao = DaoFactory.GetTagDao<int>();
            var tag = new Files.Core.Tag(entity.GetType().Name + entity.ID, TagType.System, Guid.Empty) { EntryType = FileEntryType.File, EntryId = fileId };
            dao.SaveTags(tag);
            file = EngineFactory.GetFileEngine().GetFile(fileId, 0);

            if (notify && !DisableNotifications)
            {
                var senders = GetSubscribers(entity);
                NotifyClient.SendNewFile(senders, entity, file.Title);
            }
        }

        public void DetachFile(ProjectEntity entity, object fileId)
        {
            if (!ProjectSecurity.CanReadFiles(entity.Project)) return;

            var dao = DaoFactory.GetTagDao<int>();
            var tag = new Files.Core.Tag(entity.GetType().Name + entity.ID, TagType.System, Guid.Empty) { EntryType = FileEntryType.File, EntryId = fileId };
            dao.RemoveTags(tag);
        }

        #endregion
    }
}