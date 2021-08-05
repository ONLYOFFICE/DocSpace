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
        public MessageWrapper UpdateProjectMessage(int messageid, MessageStatus status)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandEdit(discussion);

            discussion.Status = status;
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

        [Create(@"message/{messageid:int}/files")]
        public MessageWrapper UploadFilesToMessage(int messageid, IEnumerable<int> files)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetMessageEngine().AttachFile(discussion, file.ID, true);
            }

            MessageService.Send(MessageAction.DiscussionAttachedFiles, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Delete(@"message/{messageid:int}/files")]
        public MessageWrapper DetachFileFromMessage(int messageid, int fileid)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();

            EngineFactory.GetMessageEngine().DetachFile(discussion, fileid);
            MessageService.Send(MessageAction.DiscussionDetachedFile, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, file.Title);

            return ModelHelper.GetMessageWrapper(discussion);
        }

        [Delete(@"message/{messageid:int}/filesmany")]
        public MessageWrapper DetachFileFromMessage(int messageid, IEnumerable<int> files)
        {
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            ProjectSecurity.DemandReadFiles(discussion.Project);

            var filesList = files.ToList();
            var attachments = new List<Files.Core.File<int>>();
            foreach (var fileid in filesList)
            {
                var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
                attachments.Add(file);
                EngineFactory.GetMessageEngine().DetachFile(discussion, fileid);
            }

            MessageService.Send(MessageAction.DiscussionDetachedFile, MessageTarget.Create(discussion.ID), discussion.Project.Title, discussion.Title, attachments.Select(x => x.Title));

            return ModelHelper.GetMessageWrapper(discussion);
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

        ///<summary>
        ///Get preview
        ///</summary>
        ///<short>
        ///Get preview
        ///</short>
        ///<category>Discussions</category>
        ///<param name="htmltext">html to create preview</param>
        [Create(@"message/discussion/preview")]
        public string GetPreview(string htmltext)
        {
            return HtmlUtility.GetFull(htmltext);
        }
    }
}
