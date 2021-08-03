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
using System.Security;
using System.Web;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Files.Core;
using ASC.MessagingSystem;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Projects.Model;
using ASC.Projects.Model.Projects;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Comment = ASC.Projects.Core.Domain.Comment;
using Task = ASC.Projects.Core.Domain.Task;

namespace ASC.Api.Projects
{
    public class ProjectController : MessageController
    {
        public ProjectController(ASC.Core.SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager)
        {
        }

        [Read("")]
        public IEnumerable<ProjectWrapper> GetAllProjects()
        {
            return EngineFactory.GetProjectEngine().GetAll().Select(p=> ModelHelper.GetProjectWrapper(p)).ToList();
        }

        [Read(@"@self")]
        public IEnumerable<ProjectWrapper> GetMyProjects()
        {
            return EngineFactory.GetProjectEngine()
                .GetByParticipant(SecurityContext.CurrentAccount.ID)
                .Select(p => ModelHelper.GetProjectWrapper(p))
                .ToList();
        }

        [Read(@"@follow")]
        public IEnumerable<ProjectWrapper> GetFollowProjects()
        {
            return EngineFactory.GetProjectEngine()
                .GetFollowing(SecurityContext.CurrentAccount.ID)
                .Select(p => ModelHelper.GetProjectWrapper(p))
                .ToList();
        }

        [Read("{status:regex(open|paused|closed)}")]
        public IEnumerable<ProjectWrapper> GetProjects(ProjectStatus status)
        {
            return EngineFactory.GetProjectEngine().GetAll(status, 0).Select(p => ModelHelper.GetProjectWrapper(p)).ToList();
        }

        [Read(@"{id:int}")]
        public ProjectWrapperFull GetProject(int id)
        {
            var isFollow = EngineFactory.GetProjectEngine().IsFollow(id, SecurityContext.CurrentAccount.ID);
            var tags = EngineFactory.GetTagEngine().GetProjectTags(id).Select(r => r.Value).ToList();
            return ModelHelper.GetProjectWrapperFull(EngineFactory.GetProjectEngine().GetFullProjectByID(id).NotFoundIfNull(), EngineFactory.GetFileEngine().GetRoot(id), isFollow, tags);
        }

        [Read(@"filter")]
        public IEnumerable<ProjectWrapperFull> GetProjectsByFilter(ModelProjectsByFilter model)
        {
            var filter = CreateFilter(EntityType.Project);
            filter.ParticipantId = model.Participant;
            filter.UserId = model.Manager;
            filter.TagId = model.Tag;
            filter.Follow = model.Follow;
            filter.DepartmentId = model.Departament;

            if (model.Status != null)
                filter.ProjectStatuses.Add((ProjectStatus)model.Status);

            Context.SetTotalCount(EngineFactory.GetProjectEngine().GetByFilterCount(filter));

            var projects = EngineFactory.GetProjectEngine().GetByFilter(filter).NotFoundIfNull();
            var projectIds = projects.Select(p => p.ID).ToList();
            var projectRoots = EngineFactory.GetFileEngine().GetRoots(projectIds).ToList();
            ProjectSecurity.GetProjectSecurityInfo(projects);

            return projects.Select((t, i) => ModelHelper.GetProjectWrapperFull(t, projectRoots[i])).ToList();
        }

        [Read(@"{id:int}/@search/{query}")]
        public IEnumerable<SearchWrapper> SearchProject(int id, string query)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetSearchEngine().Search(query, id).Select(x => ModelHelper.GetSearchWrapper(x));
        }

        [Read(@"@search/{query}")]
        public IEnumerable<SearchWrapper> SearchProjects(string query)
        {
            return EngineFactory.GetSearchEngine().Search(query).Select(x => ModelHelper.GetSearchWrapper(x));
        }
        
        [Create("")]
        public ProjectWrapperFull CreateProject(ModelCreateProject model)
        {
            if (model.ResponsibleId == Guid.Empty) throw new ArgumentException(@"responsible can't be empty", "responsibleId");
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            if (model.Private && ProjectSecurity.IsPrivateDisabled) throw new ArgumentException(@"private", "private");

            ProjectSecurity.DemandCreate<Project>(null);

            var project = new Project
            {
                Title = model.Title,
                Status = ProjectStatus.Open,
                Responsible = model.ResponsibleId,
                Description = model.Description,
                Private = model.Private
            };

            if (!ProjectSecurity.IsAdministrator())
            {
                project.Responsible = SecurityContext.CurrentAccount.ID;
            }

            EngineFactory.GetProjectEngine().SaveOrUpdate(project, model.Notify ?? true);
            EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(model.ResponsibleId), model.Notify ?? true);
            EngineFactory.GetTagEngine().SetProjectTags(project.ID, model.Tags);


            var participantsList = model.Participants.ToList();
            foreach (var participant in participantsList)
            {
                EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(participant), model.NotifyResponsibles ?? false);
            }

            foreach (var milestone in model.Milestones)
            {
                milestone.Description = string.Empty;
                milestone.Project = project;
                EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, model.NotifyResponsibles ?? false);
            }
            var ml = model.Milestones.ToArray();
            foreach (var task in model.Tasks)
            {
                task.Description = string.Empty;
                task.Project = project;
                task.Status = TaskStatus.Open;
                if (task.Milestone != 0)
                {
                    task.Milestone = ml[task.Milestone - 1].ID;
                }
                EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, model.NotifyResponsibles ?? false);
            }

            if (!ProjectSecurity.IsAdministrator())
            {
                project.Responsible = model.ResponsibleId;
                EngineFactory.GetProjectEngine().SaveOrUpdate(project, model.Notify ?? true);
            }

            if (model.Tasks.Any() || model.Milestones.Any())
            {
                var order = JsonConvert.SerializeObject(
                        new
                        {
                            tasks = model.Tasks.Select(r => r.ID).ToArray(),
                            milestones = model.Milestones.Select(r => r.ID).ToArray()
                        });

                EngineFactory.GetProjectEngine().SetTaskOrder(project, order);
            }

            MessageService.Send(MessageAction.ProjectCreated, MessageTarget.Create(project.ID), project.Title);

            var wrapper = ModelHelper.GetProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(project.ID));
             wrapper.ParticipantCount = participantsList.Count() + 1;
            return wrapper;
        }

        [Create("withSecurity")]
        public ProjectWrapperFull CreateProject(ModelCreateProjectwithSecurity model)
        {
            var model1 = new ModelCreateProject()
            {
                Title = model.Title,
                Description = model.Description,
                ResponsibleId = model.ResponsibleId,
                Tags = model.Tags,
                Private = model.Private,
                Participants = model.Participants.Select(r => r.ID).ToList(),
                Notify = model.Notify,
                Tasks = model.Tasks,
                Milestones = model.Milestones,
                NotifyResponsibles = model.NotifyResponsibles
            };
            var project = CreateProject(model1);

            foreach (var participant in model.Participants.Where(r => !ProjectSecurity.IsAdministrator(r.ID)))
            {
                EngineFactory.GetProjectEngine().SetTeamSecurity(project.Id, participant);
            }

            return project;
        }

        [Update(@"{id:int}")]
        public ProjectWrapperFull UpdateProject(int id, ModelUpdateProject model)
        {
            if (model.ResponsibleId == Guid.Empty) throw new ArgumentException(@"responsible can't be empty", "responsibleId");
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);
            
            if (!EngineFactory.GetProjectEngine().IsInTeam(project.ID, model.ResponsibleId))
            {
                EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(model.ResponsibleId), false);
            }

            project.Title = Update.IfNotEmptyAndNotEquals(project.Title, model.Title);
            project.StatusChangedOn = DateTime.Now;

            if (model.Status.HasValue)
            {
                project.Status = model.Status.Value;
            }

            project.Responsible = Update.IfNotEmptyAndNotEquals(project.Responsible, model.ResponsibleId);
            project.Description = Update.IfNotEmptyAndNotEquals(project.Description, model.Description);

            if (model.Private.HasValue)
            {
                project.Private = model.Private.Value;
            }

            EngineFactory.GetProjectEngine().SaveOrUpdate(project, model.Notify);
            if (model.Tags != null)
            {
                EngineFactory.GetTagEngine().SetProjectTags(project.ID, model.Tags);
            }
            EngineFactory.GetProjectEngine().UpdateTeam(project, model.Participants, true);

            project.ParticipantCount = model.Participants.Count();

            MessageService.Send(MessageAction.ProjectUpdated, MessageTarget.Create(project.ID), project.Title);

            return ModelHelper.GetProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

        [Update(@"{id:int}/withSecurityInfo")]
        public ProjectWrapperFull UpdateProject(int id, ModelUpdateProjectWithSecurity model)
        {
            var model1 = new ModelUpdateProject()
            {
                Title = model.Title,
                Description = model.Description,
                ResponsibleId = model.ResponsibleId,
                Tags = model.Tags,
                Participants = model.Participants.Select(r => r.ID),
                Status = model.Status,
                Private = model.Private,
                Notify = model.Notify
            };
            var project = UpdateProject(id, model1);

            foreach (var participant in model.Participants)
            {
                EngineFactory.GetProjectEngine().SetTeamSecurity(project.Id, participant);
            }

            return project;
        }

        [Update(@"{id:int}/status")]
        public ProjectWrapperFull UpdateProject(int id, ProjectStatus status)
        {
            var project = EngineFactory.GetProjectEngine().GetFullProjectByID(id).NotFoundIfNull();

            EngineFactory.GetProjectEngine().ChangeStatus(project, status);
            MessageService.Send(MessageAction.ProjectUpdatedStatus, MessageTarget.Create(project.ID), project.Title, LocalizedEnumConverter.ConvertToString(project.Status));

            return ModelHelper.GetProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        } 
        [Delete(@"{id:int}")]
        public ProjectWrapperFull DeleteProject(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);

            var folderId = EngineFactory.GetFileEngine().GetRoot(id);
            EngineFactory.GetProjectEngine().Delete(id);
            MessageService.Send(MessageAction.ProjectDeleted, MessageTarget.Create(project.ID), project.Title);

            return ModelHelper.GetProjectWrapperFull(project, folderId);
        }

        [Delete(@"")]
        public IEnumerable<ProjectWrapperFull> DeleteProjects(int[] projectids)
        {
            var result = new List<ProjectWrapperFull>(projectids.Length);

            foreach (var id in projectids)
            {
                try
                {
                    result.Add(DeleteProject(id));
                }
                catch (Exception e)
                {
                    Log.Error("DeleteProjects " + id, e);
                }
            }

            return result;
        }

        [Update(@"{projectid:int}/follow")]
        public ProjectWrapper FollowToProject(int projectId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectId).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            if (EngineFactory.GetParticipantEngine().GetFollowingProjects(SecurityContext.CurrentAccount.ID).Contains(projectId))
            {
                EngineFactory.GetParticipantEngine().RemoveFromFollowingProjects(projectId, SecurityContext.CurrentAccount.ID);
                MessageService.Send(MessageAction.ProjectUnfollowed, MessageTarget.Create(project.ID), project.Title);
            }
            else
            {
                EngineFactory.GetParticipantEngine().AddToFollowingProjects(projectId, SecurityContext.CurrentAccount.ID);
                MessageService.Send(MessageAction.ProjectFollowed, MessageTarget.Create(project.ID), project.Title);
            }

            return ModelHelper.GetProjectWrapper(project);
        }

        [Update(@"{id:int}/tag")]
        public ProjectWrapperFull UpdateProjectTags(int id, string tags)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);

            EngineFactory.GetTagEngine().SetProjectTags(id, tags);

            return ModelHelper.GetProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

        [Update(@"{id:int}/tags")]
        public ProjectWrapperFull UpdateProjectTags(int id, IEnumerable<int> tags)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);

            EngineFactory.GetTagEngine().SetProjectTags(id, tags);

            return ModelHelper.GetProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

        [Read(@"{id:int}/time")]
        public IEnumerable<TimeWrapper> GetProjectTime(int id)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetTimeTrackingEngine().GetByProject(id).Select(tt=> ModelHelper.GetTimeWrapper(tt));
        }

        [Read(@"{id:int}/time/total")]
        public string GetTotalProjectTime(int id)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetTimeTrackingEngine().GetTotalByProject(id);
        }

        [Create(@"{id:int}/milestone")]
        public MilestoneWrapper AddProjectMilestone(int id, ModelAddMilestone model)
        {
            if (model.Title == null) throw new ArgumentNullException("title");
            if (model.Deadline == DateTime.MinValue) throw new ArgumentNullException("deadline");

            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandCreate<Milestone>(project);

            var milestone = new Milestone
            {
                Description = model.Description ?? "",
                Project = project,
                Title = model.Title.Trim(),
                DeadLine = model.Deadline,
                IsKey = model.IsKey,
                Status = MilestoneStatus.Open,
                IsNotify = model.IsNotify,
                Responsible = model.Responsible
            };
            EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, model.NotifyResponsible);
            MessageService.Send(MessageAction.MilestoneCreated, MessageTarget.Create(milestone.ID), milestone.Project.Title, milestone.Title);

            return ModelHelper.GetMilestoneWrapper(milestone);
        }

        [Read(@"{id:int}/milestone")]
        public IEnumerable<MilestoneWrapper> GetProjectMilestones(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            //NOTE: move to engine
            if (!ProjectSecurity.CanRead<Milestone>(project)) throw ProjectSecurity.CreateSecurityException();

            var milestones = EngineFactory.GetMilestoneEngine().GetByProject(id);

            return milestones.Select(m=> ModelHelper.GetMilestoneWrapper(m));
        }

        [Read(@"{id:int}/milestone/{status:regex(open|closed|late|disable)}")]
        public IEnumerable<MilestoneWrapper> GetProjectMilestones(int id, MilestoneStatus status)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            if (!ProjectSecurity.CanRead<Milestone>(project)) throw ProjectSecurity.CreateSecurityException();

            var milestones = EngineFactory.GetMilestoneEngine().GetByStatus(id, status);

            return milestones.Select(m=> ModelHelper.GetMilestoneWrapper(m));
        }
        [Read(@"{projectid:int}/team")]
        public IEnumerable<ParticipantWrapper> GetProjectTeam(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetProjectEngine().GetTeam(projectid)
                                .Select(x => ModelHelper.GetParticipantWrapper(x))
                                .OrderBy(r => r.DisplayName).ToList();
        }

        [Read(@"{projectid:int}/teamExcluded")]
        public IEnumerable<ParticipantWrapper> GetProjectTeamExcluded(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetProjectEngine().GetProjectTeamExcluded(projectid)
                                .Select(x => ModelHelper.GetParticipantWrapper(x))
                                .OrderBy(r => r.DisplayName).ToList();
        }

        [Create(@"team")]
        public IEnumerable<ParticipantWrapper> GetProjectTeam(List<int> ids)
        {
            return EngineFactory.GetProjectEngine().GetTeam(ids)
                                .Select(x => ModelHelper.GetParticipantWrapper(x))
                                .OrderBy(r => r.DisplayName).ToList();
        }


        [Create(@"{projectid:int}/team")]
        public IEnumerable<ParticipantWrapper> AddToProjectTeam(int projectid, Guid userId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandEditTeam(project);

            EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(userId), true);

            return GetProjectTeam(projectid);
        }

        [Update(@"{projectid:int}/team/security")]
        public IEnumerable<ParticipantWrapper> SetProjectTeamSecurity(int projectid, ModelSetTeamSecurity model)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandEditTeam(project);

            if (!EngineFactory.GetProjectEngine().IsInTeam(projectid, model.UserId))
            {
                throw new ArgumentOutOfRangeException("userId", "Not a project memeber");
            }

            EngineFactory.GetProjectEngine().SetTeamSecurity(project, EngineFactory.GetParticipantEngine().GetByID(model.UserId), model.Security, model.Visible);

            var team = GetProjectTeam(projectid);
            var user = team.SingleOrDefault(t => t.Id == model.UserId);
            if (user != null)
            {
                MessageService.Send(MessageAction.ProjectUpdatedMemberRights, MessageTarget.Create(project.ID), project.Title, HttpUtility.HtmlDecode(user.DisplayName));
            }

            return team;
        }

        [Delete(@"{projectid:int}/team")]
        public IEnumerable<ParticipantWrapper> DeleteFromProjectTeam(int projectid, Guid userId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandEditTeam(project);

            var particapant = EngineFactory.GetParticipantEngine().GetByID(userId);
            EngineFactory.GetProjectEngine().RemoveFromTeam(project, particapant, true);

            MessageService.Send(MessageAction.ProjectDeletedMember, MessageTarget.Create(project.ID), project.Title, particapant.UserInfo.DisplayUserName(DisplayUserSettingsHelper));

            return GetProjectTeam(projectid);
        }

        [Update(@"{projectid:int}/team")]
        public IEnumerable<ParticipantWrapper> UpdateProjectTeam(int projectId, ModelUpdateTeam model)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectId).NotFoundIfNull();
            ProjectSecurity.DemandEditTeam(project);

            var participantsList = model.Participants.ToList();
            EngineFactory.GetProjectEngine().UpdateTeam(project, participantsList, model.Notify);

            var team = GetProjectTeam(projectId);
            MessageService.Send(MessageAction.ProjectUpdatedTeam, MessageTarget.Create(project.ID), project.Title, team.Select(t => t.DisplayName));

            return team;
        }

        [Read(@"{projectid:int}/task")]
        public IEnumerable<TaskWrapper> GetProjectTasks(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetTaskEngine().GetByProject(projectid, TaskStatus.Open, Guid.Empty)
                .Select(t=> ModelHelper.GetTaskWrapper(t))
                .ToList();
        }

        [Create(@"{projectid:int}/task")]
        public TaskWrapper AddProjectTask(int projectid, ModelAddTask model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();


            if (model.Milestoneid > 0 && !EngineFactory.GetMilestoneEngine().IsExists(model.Milestoneid))
            {
                throw new ItemNotFoundException("Milestone not found");
            }

            var task = new Task
            {
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow(),
                Deadline = model.Deadline,
                Description = model.Description ?? "",
                Priority = model.Priority,
                Status = TaskStatus.Open,
                Title = model.Title,
                Project = project,
                Milestone = model.Milestoneid,
                Responsibles = new List<Guid>(model.Responsibles.Distinct()),
                StartDate = model.StartDate
            };
            EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, model.Notify);

            MessageService.Send(MessageAction.TaskCreated, MessageTarget.Create(task.ID), project.Title, task.Title);

            return GetTask(task);
        }

        [Create(@"{projectid:int}/task/{messageid:int}")]
        public TaskWrapper AddProjectTaskByMessage(int projectid, int messageid)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            var discussion = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandCreate<Task>(project);

            var task = new Task
            {
                CreateBy = SecurityContext.CurrentAccount.ID,
                CreateOn = TenantUtil.DateTimeNow(),
                Status = TaskStatus.Open,
                Title = discussion.Title,
                Project = project
            };

            EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, true);

            EngineFactory.GetCommentEngine().SaveOrUpdate(new Comment
            {
                OldGuidId = Guid.NewGuid(),
                TargetUniqID = ProjectEntity.BuildUniqId<Task>(task.ID),
                Content = discussion.Description
            });
            //copy comments
            var comments = EngineFactory.GetCommentEngine().GetComments(discussion);
            var newOldComments = new Dictionary<Guid, Guid>();

            var i = 1;
            foreach (var comment in comments)
            {
                var newID = Guid.NewGuid();
                newOldComments.Add(comment.OldGuidId, newID);

                comment.OldGuidId = newID;
                comment.CreateOn = TenantUtil.DateTimeNow().AddSeconds(i);
                comment.TargetUniqID = ProjectEntity.BuildUniqId<Task>(task.ID);

                if (!comment.Parent.Equals(Guid.Empty))
                {
                    comment.Parent = newOldComments[comment.Parent];
                }

                EngineFactory.GetCommentEngine().SaveOrUpdate(comment);
                i++;
            }

            //copy files
            var files = EngineFactory.GetMessageEngine().GetFiles(discussion);

            foreach (var file in files)
            {
                EngineFactory.GetTaskEngine().AttachFile(task, file.ID);
            }

            //copy recipients

            foreach (var participiant in EngineFactory.GetMessageEngine().GetSubscribers(discussion))
            {
                EngineFactory.GetTaskEngine().Subscribe(task, new Guid(participiant.ID));
            }

            MessageService.Send(MessageAction.TaskCreatedFromDiscussion, MessageTarget.Create(task.ID), project.Title, discussion.Title, task.Title);

            return ModelHelper.GetTaskWrapper(task);
        }

        [Read(@"{projectid:int}/task/{status:regex(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetProjectTasks(int projectid, TaskStatus status)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();
            return EngineFactory.GetTaskEngine().GetByProject(projectid, status, Guid.Empty)
                .Select(t=> ModelHelper.GetTaskWrapper(t)).ToList();
        }

        [Read(@"{projectid:int}/task/@self/{status:regex(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetProjectMyTasks(int projectid, TaskStatus status)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetTaskEngine().GetByProject(projectid, status, SecurityContext.CurrentAccount.ID)
                .Select(t => ModelHelper.GetTaskWrapper(t))
                .ToList();
        }

        [Read(@"{id:int}/files")]
        public FolderContentWrapper<int> GetProjectFiles(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            if (ProjectSecurity.CanReadFiles(project))
                return ModelHelper.GetFolderContentWrapper(EngineFactory.GetFileEngine().GetRoot(id), Guid.Empty, FilterType.None);

            throw new SecurityException("Access to files is denied");
        }

        [Read(@"{entityID:int}/entityfiles")]
        public IEnumerable<FileWrapper<int>> GetEntityFiles(EntityType entityType, int entityID)
        {
            switch (entityType)
            {
                case EntityType.Message:
                    return GetMessageFiles(entityID);

                case EntityType.Task:
                    return GetTaskFiles(entityID);
            }

            return new List<FileWrapper<int>>();
        }

        [Create(@"{entityID:int}/entityfiles")]
        public IEnumerable<FileWrapper<int>> UploadFilesToEntity(EntityType entityType, int entityID, IEnumerable<int> files)
        {
            switch (entityType)
            {
                case EntityType.Message:
                    UploadFilesToMessage(entityID, files.ToList());
                    break;

                case EntityType.Task:
                    UploadFilesToTask(entityID, files);
                    break;
            }

            var listFiles = files.Select(r => EngineFactory.GetFileEngine().GetFile(r).NotFoundIfNull()).ToList();

            return listFiles.Select(f=> FileWrapperHelper.GetFileWrapper(f));
        }

        [Delete(@"{entityID:int}/entityfiles")]
        public FileWrapper<int> DetachFileFromEntity(EntityType entityType, int entityID, int fileid)
        {
            switch (entityType)
            {
                case EntityType.Message:
                    DetachFileFromMessage(entityID, fileid);
                    break;

                case EntityType.Task:
                    DetachFileFromTask(entityID, fileid);
                    break;
            }

            var file = EngineFactory.GetFileEngine().GetFile(fileid).NotFoundIfNull();
            return FileWrapperHelper.GetFileWrapper(file);
        }

        [Delete(@"{entityID:int}/entityfilesmany")]
        public IEnumerable<FileWrapper<int>> DetachFileFromEntity(EntityType entityType, int entityID, IEnumerable<int> files)
        {
            var filesList = files.ToList();

            switch (entityType)
            {
                case EntityType.Message:
                    DetachFileFromMessage(entityID, filesList);
                    break;

                case EntityType.Task:
                    DetachFileFromTask(entityID, filesList);
                    break;
            }

            var listFiles = filesList.Select(r => EngineFactory.GetFileEngine().GetFile(r).NotFoundIfNull()).ToList();

            return listFiles.Select(f=> FileWrapperHelper.GetFileWrapper(f));
        }

        [Create(@"{entityID:int}/entityfiles/upload")]
        public object UploadFilesToEntity(int entityID, ModelUploadFiles model)
        {
            if (!model.Files.Any()) return new object();
            var fileWrappers = FileWrapperHelper.UploadFile(model.Folderid, model.File, model.ContentType, model.ContentDisposition, model.Files, model.CreateNewIfExist, model.StoreOriginalFileFlag, false);

            if (fileWrappers == null) return null;

            var fileIDs = new List<int>();

            var wrappers = fileWrappers as IEnumerable<FileWrapper<int>>;
            if (wrappers != null)
            {
                fileIDs.AddRange(wrappers.Select(r => r.Id));
            }

            switch (model.EntityType)
            {
                case EntityType.Message:
                    UploadFilesToMessage(entityID, fileIDs);
                    break;

                case EntityType.Task:
                    UploadFilesToTask(entityID, fileIDs);
                    break;
            }

            return fileWrappers;
        }

        [Read("contact/{contactid:int}")]
        public IEnumerable<ProjectWrapperFull> GetProjectsByContactID(int contactid)
        {
            if (contactid <= 0) throw new ArgumentException();

            return EngineFactory.GetProjectEngine().GetProjectsByContactID(contactid)
                .Select(x => ModelHelper.GetProjectWrapperFull(x, EngineFactory.GetFileEngine().GetRoot(x.ID))).ToList();
        }

        [Create(@"{projectid:int}/contact")]
        public ProjectWrapperFull AddProjectContact(int projectid, int contactid)
        {
            var contact = CRMDaoFactory.GetContactDao().GetByID(contactid);
            if (contact == null) throw new ArgumentException();

            var project = EngineFactory.GetProjectEngine().GetFullProjectByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandLinkContact(project);

            EngineFactory.GetProjectEngine().AddProjectContact(projectid, contactid);

            var messageAction = contact is Company ? MessageAction.CompanyLinkedProject : MessageAction.PersonLinkedProject;
            MessageService.Send(messageAction, MessageTarget.Create(project.ID), contact.GetTitle(), project.Title);

            return ModelHelper.GetProjectWrapperFull(project, null);
        }

        [Delete("{projectid:int}/contact")]
        public ProjectWrapperFull DeleteProjectContact(int projectid, int contactid)
        {
            var contact = CRMDaoFactory.GetContactDao().GetByID(contactid);
            if (contact == null) throw new ArgumentException();

            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);

            EngineFactory.GetProjectEngine().DeleteProjectContact(projectid, contactid);

            var messageAction = contact is Company ? MessageAction.CompanyUnlinkedProject : MessageAction.PersonUnlinkedProject;
            MessageService.Send(messageAction, MessageTarget.Create(project.ID), contact.GetTitle(), project.Title);

            return ModelHelper.GetProjectWrapperFull(project, null);
        }

        [Read("template")]
        public IEnumerable<object> GetAllTemplates()
        {
            return EngineFactory.GetTemplateEngine().GetAll().Select(x => new { x.Id, x.Title, x.Description, CanEdit = ProjectSecurity.CanEditTemplate(x) });
        }

        [Read(@"template/{id:int}")]
        public ObjectWrapperBase GetTemplate(int id)
        {
            var template = EngineFactory.GetTemplateEngine().GetByID(id).NotFoundIfNull();
            return new ObjectWrapperBase { Id = template.Id, Title = template.Title, Description = template.Description };
        }
        
        [Create("template")]
        public ObjectWrapperBase CreateTemplate(ModelCreateTemplate model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            ProjectSecurity.DemandCreate<Project>(null);

            var template = new Template
            {
                Title = model.Title,
                Description = model.Description
            };

            template = EngineFactory.GetTemplateEngine().SaveOrUpdate(template).NotFoundIfNull();
            MessageService.Send(MessageAction.ProjectTemplateCreated, MessageTarget.Create(template.Id), template.Title);

            return new ObjectWrapperBase { Id = template.Id, Title = template.Title, Description = template.Description };
        }

        [Update(@"template/{id:int}")]
        public ObjectWrapperBase UpdateTemplate(int id, ModelCreateTemplate model)
        {
            if (string.IsNullOrEmpty(model.Title)) throw new ArgumentException(@"title can't be empty", "title");

            var template = EngineFactory.GetTemplateEngine().GetByID(id).NotFoundIfNull();

            template.Title = Update.IfNotEmptyAndNotEquals(template.Title, model.Title);
            template.Description = Update.IfNotEmptyAndNotEquals(template.Description, model.Description);

            EngineFactory.GetTemplateEngine().SaveOrUpdate(template);
            MessageService.Send(MessageAction.ProjectTemplateUpdated, MessageTarget.Create(template.Id), template.Title);

            return new ObjectWrapperBase { Id = template.Id, Title = template.Title, Description = template.Description };
        }

        [Delete(@"template/{id:int}")]
        public ObjectWrapperBase DeleteTemplate(int id)
        {
            var template = EngineFactory.GetTemplateEngine().GetByID(id).NotFoundIfNull();

            EngineFactory.GetTemplateEngine().Delete(id);
            MessageService.Send(MessageAction.ProjectTemplateDeleted, MessageTarget.Create(template.Id), template.Title);

            return new ObjectWrapperBase { Id = template.Id, Title = template.Title, Description = template.Description };
        }

        [Read("securityinfo")]
        public CommonSecurityInfo GetProjectSecurityInfo()
        {
            return ModelHelper.GetCommonSecurityInfo();
        }

        [Read("maxlastmodified")]
        public string GetProjectMaxLastModified()
        {
            var maxModified = EngineFactory.GetProjectEngine().GetMaxLastModified();
            var maxTeamModified = EngineFactory.GetProjectEngine().GetTeamMaxLastModified();
            var result = DateTime.Compare(maxModified, maxTeamModified) > 0 ? maxModified : maxTeamModified;
            return result + EngineFactory.GetProjectEngine().Count().ToString();
        }

        [Read(@"{id:int}/order")]
        public string GetTaskOrder(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            return EngineFactory.GetProjectEngine().GetTaskOrder(project);
        }

        [Update(@"{id:int}/order")]
        public void SetTaskOrder(int id, string order)
        {
            if (string.IsNullOrEmpty(order)) throw new ArgumentException(@"order can't be empty", "order");

            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            EngineFactory.GetProjectEngine().SetTaskOrder(project, order);
        }
    }
}