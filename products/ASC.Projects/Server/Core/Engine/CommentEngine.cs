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

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;

namespace ASC.Projects.Engine
{
    [Scope]
    public class CommentEngine
    {
        public bool DisableNotifications { get; set; }
        private ProjectSecurity ProjectSecurity { get; set; }
        private FactoryIndexer<DbComment> FactoryIndexer { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private NotifyClient NotifyClient { get; set; }
        private EngineFactory EngineFactory { get; set; }
        private IDaoFactory DaoFactory { get; set; }

        public CommentEngine(FactoryIndexer<DbComment> factoryIndexer, TenantUtil tenantUtil, IDaoFactory daoFactory, SecurityContext securityContext, EngineFactory engineFactory, NotifyClient notifyClient)
        {
            FactoryIndexer = factoryIndexer;
            TenantUtil = tenantUtil;
            SecurityContext = securityContext;
            NotifyClient = notifyClient;
            EngineFactory = engineFactory;
            DaoFactory = daoFactory;
        }

        public CommentEngine Init(bool disableNotifications)
        {
            DisableNotifications = disableNotifications;
            return this;
        }

        public List<Comment> GetComments(DomainObject<int> targetObject)
        {
            return targetObject != null ? DaoFactory.GetCommentDao().GetAll(targetObject) : new List<Comment>();
        }

        public Comment GetByID(Guid id)
        {
            return DaoFactory.GetCommentDao().GetById(id);
        }

        public int Count(DomainObject<int> targetObject)
        {
            return targetObject == null ? 0 : DaoFactory.GetCommentDao().Count(targetObject);
        }

        public List<int> Count(List<ProjectEntity> targets)
        {
            return DaoFactory.GetCommentDao().Count(targets);
        }

        public int Count(ProjectEntity target)
        {
            return DaoFactory.GetCommentDao().Count(target);
        }

        public void SaveOrUpdate(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException("comment");

            if (comment.CreateBy == default(Guid)) comment.CreateBy = SecurityContext.CurrentAccount.ID;

            var now = TenantUtil.DateTimeNow();
            if (comment.CreateOn == default(DateTime)) comment.CreateOn = now;

            DaoFactory.GetCommentDao().Save(comment);

            if (!comment.Inactive)
            {
                _ = FactoryIndexer.IndexAsync(DaoFactory.GetCommentDao().ToDbComment(comment));
            }
            else
            {
                _ = FactoryIndexer.DeleteAsync(DaoFactory.GetCommentDao().ToDbComment(comment));
            }
        }

        public ProjectEntity GetEntityByTargetUniqId(Comment comment)
        {
            var engine = GetProjectEntityEngine(comment);
            if (engine == null) return null;

            return engine.GetEntityByID(comment.TargetID);
        }

        public Comment SaveOrUpdateComment(ProjectEntity entity, Comment comment)
        {
            var isNew = comment.OldGuidId.Equals(Guid.Empty);

            if (isNew)
            {
                ProjectSecurity.DemandCreateComment(entity);
            }
            else
            {
                var message = entity as Message;
                if (message != null)
                {
                    ProjectSecurity.DemandEditComment(message, comment);
                }
                else
                {
                    ProjectSecurity.DemandEditComment(entity.Project, comment);
                }
            }

            SaveOrUpdate(comment);

            NotifyNewComment(entity, comment, isNew);

            GetProjectEntityEngine(comment).Subscribe(entity, SecurityContext.CurrentAccount.ID);

            return comment;
        }

        private void NotifyNewComment(ProjectEntity entity, Comment comment, bool isNew)
        {
            if (DisableNotifications) return;

            var senders = GetProjectEntityEngine(comment).GetSubscribers(entity);

            NotifyClient.SendNewComment(senders, entity, comment, isNew);
        }

        private ProjectEntityEngine GetProjectEntityEngine(Comment comment)
        {
            switch (comment.TargetType)
            {
                case "Task":
                    return EngineFactory.GetTaskEngine();
                case "Message":
                    return EngineFactory.GetMessageEngine();
                default:
                    return null;
            }
        }
    }
}