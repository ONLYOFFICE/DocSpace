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

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.Dao;
using ASC.MessagingSystem;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Projects.Model;
using ASC.Projects.Model.Messages;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class MessageController : BaseProjectController
    {
        public MessageController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
        {
        }

        [Read(@"message/filter")]
        public IEnumerable<MessageWrapper> GetMessageByFilter(ModelMessageByFilter model)
        {
            var filter = CreateFilter(EntityType.Message);

            filter.DepartmentId = model.Departament;
            filter.UserId = model.Participant;
            filter.FromDate = model.CreatedStart;
            filter.ToDate = model.CreatedStop;
            filter.TagId = model.Tag;
            filter.MyProjects = model.MyProjects;
            filter.LastId = model.LastId;
            filter.Follow = model.Follow;
            filter.MessageStatus = model.Status;

            if (model.ProjectId != 0)
                filter.ProjectIds.Add(model.ProjectId);

            Context.SetTotalCount(EngineFactory.GetMessageEngine().GetByFilterCount(filter));

            return EngineFactory.GetMessageEngine().GetByFilter(filter).NotFoundIfNull().Select(r=> ModelHelper.GetMessageWrapper(r)).ToList();
        }

        [Read(@"{projectid:int}/message")]
        public IEnumerable<MessageWrapper> GetProjectMessages(int projectid)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();

            if (!ProjectSecurity.CanRead<Message>(project)) throw ProjectSecurity.CreateSecurityException();

            return EngineFactory.GetMessageEngine().GetByProject(projectid).Select(r=> ModelHelper.GetMessageWrapper(r)).ToList();
        }

        [Create(@"{projectid:int}/message")]
        public MessageWrapper AddProjectMessage(int projectid, ModelAddMessage model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");
            if (string.IsNullOrEmpty(model.Content)) throw new ArgumentException(@"description can't be empty", "content");

            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandCreate<Message>(project);

            var discussion = new Message
            {
                Description = model.Content,
                Title = model.Title,
                Project = project,
            };

            EngineFactory.GetMessageEngine().SaveOrUpdate(discussion, model.Notify.HasValue ? model.Notify.Value : true, ToGuidList(model.Participants));
            MessageService.Send(MessageAction.DiscussionCreated, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title);

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Update(@"message/{messageid:int}")]
        public MessageWrapperFull UpdateProjectMessage(int messageid, ModelUpdateMessage model)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            var project = EngineFactory.GetProjectEngine().GetByID(model.ProjectId).NotFoundIfNull();
            ProjectSecurity.DemandEdit(discussion);

            discussion.Project = Update.IfNotEmptyAndNotEquals(discussion.Project, project);
            discussion.Description = Update.IfNotEmptyAndNotEquals(discussion.Description, model.Content);
            discussion.Title = Update.IfNotEmptyAndNotEquals(discussion.Title, model.Title);

            EngineFactory.GetMessageEngine().SaveOrUpdate(discussion, model.Notify.HasValue ? model.Notify.Value : true, ToGuidList(model.Participants));
            MessageService.Send(MessageAction.DiscussionUpdated, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title);

            return ModelHelper.GetMessageWrapperFull(discussion, ModelHelper.GetProjectWrapperFull(discussion.Project, EngineFactory.GetFileEngine().GetRoot(discussion.Project.ID)), GetProjectMessageSubscribers(messageid));
        }

        [Update(@"message/{messageid:int}/status")]
        public MessageWrapper UpdateProjectMessage(int messageid, ModelUpdateStatus model)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandEdit(discussion);

            discussion.Status = model.Status;
            EngineFactory.GetMessageEngine().ChangeStatus(discussion);
            MessageService.Send(MessageAction.DiscussionUpdated, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title);

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Delete(@"message/{messageid:int}")]
        public MessageWrapper DeleteProjectMessage(int messageid)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandEdit(discussion);

            EngineFactory.GetMessageEngine().Delete(discussion);
            MessageService.Send(MessageAction.DiscussionDeleted, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title);

            return ModelHelper.GetMessageWrapper(discussion);
        }

        private static IEnumerable<Guid> ToGuidList(string participants)
        {
            return participants != null ?
                 participants.Equals(string.Empty) ? new List<Guid>() : participants.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x))
                : null;
        }

        [Read(@"message/{messageid:int}")]
        public MessageWrapperFull GetProjectMessage(int messageid)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            var project = ModelHelper.GetProjectWrapperFull(discussion.Project, EngineFactory.GetFileEngine().GetRoot(discussion.Project.ID));
            var subscribers = GetProjectMessageSubscribers(messageid);
            var files = EngineFactory.GetMessageEngine().GetFiles(discussion).Select(f=> FileWrapperHelper.GetFileWrapper(f));
            var comments = EngineFactory.GetCommentEngine().GetComments(discussion);
            return ModelHelper.GetMessageWrapperFull(discussion, project, subscribers, files, comments.Where(r => r.Parent.Equals(Guid.Empty)).Select(x => ModelHelper.GetCommentInfo(comments, x, discussion)).ToList());
        }

        [Read(@"message")]
        public IEnumerable<MessageWrapper> GetProjectRecentMessages()
        {
            return EngineFactory.GetMessageEngine().GetMessages((int)Context.StartIndex, (int)Context.Count).Select(m=> ModelHelper.GetMessageWrapper(m));
        }

        [Read(@"message/{messageid:int}/comment")]
        public IEnumerable<CommentWrapper> GetProjectMessagesComments(int messageid)
        {
            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            return EngineFactory.GetCommentEngine().GetComments(message).Select(x => ModelHelper.GetCommentWrapper(x,message));
        }

        [Create(@"message/{messageid:int}/comment")]
        public CommentWrapper AddProjectMessagesComment(int messageid, ModelContentAndParentId model)
        {
            if (string.IsNullOrEmpty(model.Content)) throw new ArgumentException(@"Comment text is empty", model.Content);
            if (model.ParentId != Guid.Empty && EngineFactory.GetCommentEngine().GetByID(model.ParentId) == null) throw new ItemNotFoundException("parent comment not found");

            var comment = new Comment
            {
                Content = model.Content,
                TargetUniqID = ProjectEntity.BuildUniqId<Message>(messageid),
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow()
            };

            if (model.ParentId != Guid.Empty)
            {
                comment.Parent = model.ParentId;
            }

            var message = EngineFactory.GetCommentEngine().GetEntityByTargetUniqId(comment).NotFoundIfNull();

            EngineFactory.GetCommentEngine().SaveOrUpdateComment(message, comment);

            MessageService.Send(MessageAction.DiscussionCommentCreated, MessageTarget.Create(comment.ID), message.Project.Title, message.Title);

            return ModelHelper.GetCommentWrapper(comment, message);
        }

        [Update(@"message/{messageid:int}/subscribe")]
        public MessageWrapper SubscribeToMessage(int messageid)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            EngineFactory.GetMessageEngine().Follow(discussion);
            MessageService.Send(MessageAction.DiscussionUpdatedFollowing, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title);

            return ModelHelper.GetMessageWrapperFull(discussion, ModelHelper.GetProjectWrapperFull(discussion.Project, EngineFactory.GetFileEngine().GetRoot(discussion.Project.ID)), GetProjectMessageSubscribers(messageid));
        }

        [Read(@"message/{messageid:int}/subscribe")]
        public bool IsSubscribedToMessage(int messageid)
        {

            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            return EngineFactory.GetMessageEngine().IsSubscribed(message);
        }

        [Read(@"message/{messageid:int}/subscribes")]
        public IEnumerable<EmployeeWraperFull> GetProjectMessageSubscribers(int messageid)
        {
            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            return EngineFactory.GetMessageEngine().GetSubscribers(message).Select(r => ModelHelper.GetEmployeeWraperFull(new Guid(r.ID)))
                .OrderBy(r => r.DisplayName).ToList();
        }

        [Create(@"message/discussion/preview")]
        public object GetPreview(ModelPreview model)
        {
            return HtmlUtility.GetFull(model.Htmltext);
        }
    }
}
