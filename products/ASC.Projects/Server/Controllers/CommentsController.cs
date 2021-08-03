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

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.MessagingSystem;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Projects.Model;
using ASC.Projects.Model.Comments;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class CommentsController : BaseProjectController
    {
        public CommentsController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager)
        {
        }

        [Read(@"comment/{commentid}")]
        public CommentWrapper GetComment(Guid commentid)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull();
            var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment).NotFoundIfNull();

            return ModelHelper.GetCommentWrapper(comment, entity);
        }

        [Create(@"comment/preview")]
        public CommentInfo GetProjectCommentPreview(ModelComment model)
        {
            ProjectSecurity.DemandAuthentication();

            Comment comment;
            if (!string.IsNullOrEmpty(model.CommentId))
            {
                comment = EngineFactory.GetCommentEngine().GetByID(new Guid(model.CommentId));
                comment.Content = model.HtmlText;
            }
            else
            {
                comment = new Comment
                {
                    Content = model.HtmlText,
                    CreateOn = TenantUtil.DateTimeNow(),
                    CreateBy = SecurityContext.CurrentAccount.ID
                };
            }

            var creator = EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo;
            var info = new CommentInfo
            {
                CommentID = comment.OldGuidId.ToString(),
                UserID = comment.CreateBy,
                TimeStamp = comment.CreateOn,
                //TimeStampStr = comment.CreateOn.Ago(),
                UserPost = creator.Title,
                Inactive = comment.Inactive,
                //CommentBody = HtmlUtility.GetFull(comment.Content),//todo
                UserFullName = DisplayUserSettingsHelper.GetFullUserName(creator),
                UserProfileLink = creator.GetUserProfilePageURL(CommonLinkUtility),
                UserAvatarPath = creator.GetBigPhotoURL(UserPhotoManager)
            };

            return info;
        }

        [Delete("comment/{commentid}")]
        public string RemoveProjectComment(string commentid)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(new Guid(commentid)).NotFoundIfNull();
            comment.Inactive = true;

            var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment);
            if (entity == null) return "";

            ProjectSecurity.DemandEditComment(entity.Project, comment);

            EngineFactory.GetCommentEngine().SaveOrUpdate(comment);
            MessageService.Send(MessageAction.TaskCommentDeleted, MessageTarget.Create(comment.ID), entity.Project.Title, entity.Title);

            return commentid;
        }

        [Create("comment")]
        public CommentInfo AddProjectComment(ModelAddComment model)
        {
            if (string.IsNullOrEmpty(model.Type) || !(new List<string> { "message", "task" }).Contains(model.Type.ToLower()))
                throw new ArgumentException();

            var isMessageComment = model.Type.ToLower().Equals("message");
            var comment = isMessageComment
                ? new Comment { Content = model.Content, TargetUniqID = ProjectEntity.BuildUniqId<Message>(model.EntityId) }
                : new Comment { Content = model.Content, TargetUniqID = ProjectEntity.BuildUniqId<Task>(model.EntityId) };


            if (!string.IsNullOrEmpty(model.ParentCommentId))
                comment.Parent = new Guid(model.ParentCommentId);

            var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment).NotFoundIfNull();

            comment = EngineFactory.GetCommentEngine().SaveOrUpdateComment(entity, comment);

            MessageService.Send(isMessageComment ? MessageAction.DiscussionCommentCreated : MessageAction.TaskCommentCreated, MessageTarget.Create(comment.ID), entity.Project.Title, entity.Title);
            return ModelHelper.GetCommentInfo(null, comment, entity);
        }

        [Update("comment/{commentid}")]
        public string UpdateComment(string commentid, string content)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(new Guid(commentid));
            comment.Content = content;

            var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment);
            if (entity == null) throw new Exception("Access denied.");

            EngineFactory.GetCommentEngine().SaveOrUpdateComment(entity, comment);

            MessageService.Send(MessageAction.TaskCommentUpdated, MessageTarget.Create(comment.ID), entity.Project.Title, entity.Title);

            return content;//todo HtmlUtility.GetFull(content);
        }
    }
}