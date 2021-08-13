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
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Calendars;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using ASC.Web.Studio.Core.Notify;

namespace ASC.Api.Projects
{
    public class CommentsController : BaseProjectController
    {
        public CommentsController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
        {
        }

        [Read(@"comment/{commentid}")]
        public CommentWrapper GetComment(Guid commentid)
        {
            var commentEngine = EngineFactory.GetCommentEngine();
            var comment = commentEngine.GetByID(commentid).NotFoundIfNull();
            var entity = commentEngine.GetEntityByTargetUniqId(comment).NotFoundIfNull();

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
                TimeStampStr = comment.CreateOn.Ago(TenantUtil),
                UserPost = creator.Title,
                Inactive = comment.Inactive,
                CommentBody = HtmlUtility.GetFull(comment.Content),
                UserFullName = DisplayUserSettingsHelper.GetFullUserName(creator),
                UserProfileLink = creator.GetUserProfilePageURL(CommonLinkUtility),
                UserAvatarPath = creator.GetBigPhotoURL(UserPhotoManager)
            };

            return info;
        }

        [Delete("comment/{commentid}")]
        public object RemoveProjectComment(string commentid)
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
        public object UpdateComment(string commentid, ModelContent model)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(new Guid(commentid));
            comment.Content = model.Content;

            var entity = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment);
            if (entity == null) throw new Exception("Access denied.");

            EngineFactory.GetCommentEngine().SaveOrUpdateComment(entity, comment);

            MessageService.Send(MessageAction.TaskCommentUpdated, MessageTarget.Create(comment.ID), entity.Project.Title, entity.Title);

            return HtmlUtility.GetFull(model.Content);
        }
    }
}