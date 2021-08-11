using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Projects.Wrappers;
using ASC.Collections;
using ASC.Common;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Web.Api.Models;
using ASC.Web.Core;
using ASC.Web.Core.Calendars;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;

namespace ASC.Projects.Model
{
    [Scope]
    public class ModelHelper
    {
        private HttpRequestDictionary<EmployeeWraperFull> EmployeeFullCache { get; set; }
        private HttpRequestDictionary<EmployeeWraper> EmployeeCache { get; set; }
        private EmployeeWraperFullHelper EmployeeWraperFullHelper { get; set; }
        private EmployeeWraperHelper EmployeeWraperHelper { get; set; }
        private UserManager UserManager { get; set; }
        private ProjectSecurity ProjectSecurity { get; set; }
        private TenantManager TenantManager { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; set; }
        private UserPhotoManager UserPhotoManager { get; set; }
        private CommonLinkUtility CommonLinkUtility { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private TimeZoneConverter TimeZoneConverter { get; set; }
        private FolderContentWrapperHelper FolderContentWrapperHelper { get; set; }
        private ApiContext Context { get; set; }
        private FileStorageService<int> FileStorageService { get; set; }
        private EngineFactory EngineFactory { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private HtmlUtility HtmlUtility { get; set; }
        private ApiDateTimeHelper ApiDateTimeHelper { get; }
        private WebItemSecurity WebItemSecurity { get; }

        public ModelHelper(IHttpContextAccessor accessor, UserManager userManager, ProjectSecurity projectSecurity, EmployeeWraperHelper employeeWraperHelper, EmployeeWraperFullHelper employeeWraperFullHelper, IHttpContextAccessor httpContextAccessor, EngineFactory engineFactory, DisplayUserSettingsHelper displayUserSettingsHelper, UserPhotoManager userPhotoManager, CommonLinkUtility commonLinkUtility, SecurityContext securityContext, TenantManager tenantManager, TimeZoneConverter timeZoneConverter, FolderContentWrapperHelper folderContentWrapperHelper, ApiContext context, FileStorageService<int> fileStorageService, TenantUtil tenantUtil, HtmlUtility htmlUtility, ApiDateTimeHelper apiDateTimeHelper, WebItemSecurity webItemSecurity)
        {
            EmployeeFullCache = new HttpRequestDictionary<EmployeeWraperFull>(accessor?.HttpContext, "employeeFullCache");
            EmployeeCache = new HttpRequestDictionary<EmployeeWraper>(accessor?.HttpContext, "employeeCache");
            UserManager = userManager;
            ProjectSecurity = projectSecurity;
            EmployeeWraperHelper = employeeWraperHelper;
            EmployeeWraperFullHelper = employeeWraperFullHelper;
            HttpContextAccessor = httpContextAccessor;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            UserPhotoManager = userPhotoManager;
            CommonLinkUtility = commonLinkUtility;
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            TimeZoneConverter = timeZoneConverter;
            FolderContentWrapperHelper = folderContentWrapperHelper;
            Context = context;
            FileStorageService = fileStorageService;
            EngineFactory = engineFactory;
            TenantUtil = tenantUtil;
            HtmlUtility = htmlUtility;
            ApiDateTimeHelper = apiDateTimeHelper;
            WebItemSecurity = webItemSecurity;
        }

        public EmployeeWraperFull GetEmployeeWraperFull(Guid userId)
        {
            return EmployeeFullCache.Get(userId.ToString(), () => EmployeeWraperFullHelper.GetFull(UserManager.GetUsers(userId)));
        }

        public EmployeeWraper GetEmployeeWraper(Guid userId)
        {
            var employee = EmployeeFullCache.Get(userId.ToString());
            var q = UserManager.GetUsers(userId);
            var qq = EmployeeWraperHelper.Get(UserManager.GetUsers(userId));
            return employee ??
                   EmployeeCache.Get(userId.ToString(), () => EmployeeWraperHelper.Get(UserManager.GetUsers(userId)));
        }

        public CommentWrapper GetCommentWrapper(Comment comment, ProjectEntity entity)
        {
            var model = new CommentWrapper()
            {
                Id = comment.OldGuidId,
                ParentId = comment.Parent,
                Text = comment.Content,
                Created = new ApiDateTime(comment.CreateOn, TimeSpan.Zero),
                Updated = new ApiDateTime(comment.CreateOn, TimeSpan.Zero),
                CreatedBy = GetEmployeeWraper(comment.CreateBy),
                Inactive = comment.Inactive,
                CanEdit = ProjectSecurity.CanEditComment(entity, comment)
            };
            return model;
        }
        public SearchWrapper GetSearchWrapper(SearchItem searchItem)
        {
            var model = new SearchWrapper();
            model.Item = GetSearchItemWrapper(searchItem);
            if (searchItem.Container != null)
            {
                model.Owner = GetSearchItemWrapper(searchItem.Container);
            }
            return model;
        }

        public SearchItemWrapper GetSearchItemWrapper(SearchItem searchItem)
        {
            var model = new SearchItemWrapper();
            model.Id = searchItem.ID;
            model.Title = searchItem.Title;
            model.EntityType = searchItem.EntityType;
            model.Created = new ApiDateTime(searchItem.CreateOn, TimeSpan.Zero);
            model.Description = searchItem.Description;
            return model;
        }

        public MessageWrapper GetMessageWrapper(Message message)
        {
            var model = new MessageWrapper();
            model.Id = message.ID;
            if (message.Project != null)
            {
                model.ProjectOwner = GetSimpleProjectWrapper(message.Project);
            }
            model.Title = message.Title;
            model.Text = message.Description;
            model.Created = new ApiDateTime(message.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(message.LastModifiedOn, TimeSpan.Zero);

            if(HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.CreatedById = message.CreateBy;
                model.UpdatedById = message.LastModifiedBy;
            }
            else
            {
                model.CreatedBy = GetEmployeeWraper(message.CreateBy);
                if (message.CreateBy != message.LastModifiedBy)
                {
                    model.UpdatedBy = GetEmployeeWraper(message.LastModifiedBy);
                }
            }

            model.CanEdit = ProjectSecurity.CanEdit(message);
            model.CommentsCount = message.CommentsCount;
            model.Status = (int)message.Status;
            model.CanCreateComment = ProjectSecurity.CanCreateComment(message);
            return model;
        }

        public SimpleProjectWrapper GetSimpleProjectWrapper(Project project)
        {
            return new SimpleProjectWrapper()
            {
                Id = project.ID,
                Title = project.Title,
                Status = (int)project.Status,
                IsPrivate = project.Private,
            };
        }

        public CommentInfo GetCommentInfo(IEnumerable<Comment> allComments, Comment comment, ProjectEntity entity)
        {
            var creator = EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo;
            var oCommentInfo = new CommentInfo
            {
                TimeStamp = comment.CreateOn,
                TimeStampStr = comment.CreateOn.Ago(TenantUtil),
                CommentBody = HtmlUtility.GetFull(comment.Content),
                CommentID = comment.OldGuidId.ToString(),
                UserID = comment.CreateBy,
                UserFullName = creator.DisplayUserName(DisplayUserSettingsHelper),
                UserProfileLink = creator.GetUserProfilePageURL(CommonLinkUtility),
                Inactive = comment.Inactive,
                IsEditPermissions = ProjectSecurity.CanEditComment(entity, comment),
                IsResponsePermissions = ProjectSecurity.CanCreateComment(entity),
                IsRead = true,
                UserAvatarPath = creator.GetBigPhotoURL(UserPhotoManager),
                UserPost = creator.Title,
                CommentList = new List<CommentInfo>()
            };

            if (allComments != null)
                foreach (var com in allComments.Where(com => com.Parent == comment.OldGuidId))
                {
                    oCommentInfo.CommentList.Add(GetCommentInfo(allComments, comment, entity));
                }

            return oCommentInfo;
        }

        public MessageWrapperFull GetMessageWrapperFull(Message message, ProjectWrapperFull project, IEnumerable<EmployeeWraperFull> subscribers)
        {
            var model = new MessageWrapperFull();
            model.Id = message.ID;
            if (message.Project != null)
            {
                model.ProjectOwner = GetSimpleProjectWrapper(message.Project);
            }
            model.Title = message.Title;
            model.Text = message.Description;
            model.Created = new ApiDateTime(message.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(message.LastModifiedOn, TimeSpan.Zero);

            if (HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.CreatedById = message.CreateBy;
                model.UpdatedById = message.LastModifiedBy;
            }
            else
            {
                model.CreatedBy = GetEmployeeWraper(message.CreateBy);
                if (message.CreateBy != message.LastModifiedBy)
                {
                    model.UpdatedBy = GetEmployeeWraper(message.LastModifiedBy);
                }
            }

            model.CanEdit = ProjectSecurity.CanEdit(message);
            model.CommentsCount = message.CommentsCount;
            model.Status = (int)message.Status;
            model.CanCreateComment = ProjectSecurity.CanCreateComment(message);
            model.CanEditFiles = ProjectSecurity.CanEditFiles(message);
            model.CanReadFiles = ProjectSecurity.CanReadFiles(message.Project);
            model.Text = HtmlUtility.GetFull(model.Text);
            model.Project = project;
            model.Subscribers = subscribers.ToList();
            return model;
        }

        public MessageWrapperFull GetMessageWrapperFull(Message message, ProjectWrapperFull project, IEnumerable<EmployeeWraperFull> subscribers, IEnumerable<FileWrapper<int>> files, IEnumerable<CommentInfo> comments)
        {
            var model = new MessageWrapperFull();
            model.Id = message.ID;
            if (message.Project != null)
            {
                model.ProjectOwner = GetSimpleProjectWrapper(message.Project);
            }
            model.Title = message.Title;
            model.Text = message.Description;
            model.Created = new ApiDateTime(message.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(message.LastModifiedOn, TimeSpan.Zero);

            if (HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.CreatedById = message.CreateBy;
                model.UpdatedById = message.LastModifiedBy;
            }
            else
            {
                model.CreatedBy = GetEmployeeWraper(message.CreateBy);
                if (message.CreateBy != message.LastModifiedBy)
                {
                    model.UpdatedBy = GetEmployeeWraper(message.LastModifiedBy);
                }
            }

            model.CanEdit = ProjectSecurity.CanEdit(message);
            model.CommentsCount = message.CommentsCount;
            model.Status = (int)message.Status;
            model.CanCreateComment = ProjectSecurity.CanCreateComment(message);
            model.CanEditFiles = ProjectSecurity.CanEditFiles(message);
            model.CanReadFiles = ProjectSecurity.CanReadFiles(message.Project);
            model.Text = HtmlUtility.GetFull(model.Text);
            model.Project = project;
            model.Subscribers = subscribers.ToList();
            model.Files = files.ToList();
            var creator = UserManager.GetUsers(message.CreateBy);
            model.Comments = new List<CommentInfo>(comments.Count() + 1)
            {
                new CommentInfo
                {
                    TimeStamp = message.CreateOn,
                   TimeStampStr = message.CreateOn.Ago(TenantUtil),
                    CommentBody = HtmlUtility.GetFull(message.Description),
                    CommentID = SecurityContext.CurrentAccount.ID.ToString() + "1",
                    UserID = message.CreateBy,
                    UserFullName = creator.DisplayUserName(DisplayUserSettingsHelper),
                    UserProfileLink = creator.GetUserProfilePageURL(CommonLinkUtility),
                    Inactive = false,
                    IsEditPermissions = false,
                    IsResponsePermissions = false,
                    IsRead = true,
                    UserAvatarPath = creator.GetBigPhotoURL(UserPhotoManager),
                    UserPost = creator.Title,
                    CommentList = new List<CommentInfo>()
                }
            };
            model.Comments.AddRange(comments);
            return model;
        }

        public ProjectWrapperFull GetProjectWrapperFull(Project project, object filesRoot = null, bool isFollow = false, IEnumerable<string> tags = null)
        {
            var model = new ProjectWrapperFull();
            model.Id = project.ID;
            model.Title = project.Title;
            model.Description = project.Description;
            model.Status = (int)project.Status;
            if (HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.ResponsibleId = project.Responsible;
                model.CreatedById = project.CreateBy;
                model.UpdatedById = project.LastModifiedBy;
            }
            else
            {
                model.Responsible = GetEmployeeWraperFull(project.Responsible);
                model.CreatedBy = GetEmployeeWraper(project.CreateBy);
                if (project.CreateBy != project.LastModifiedBy)
                {
                    model.UpdatedBy = GetEmployeeWraper(project.LastModifiedBy);
                }
            }

            model.Created = new ApiDateTime(project.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(project.LastModifiedOn, TimeSpan.Zero);


            if (project.Security == null)
            {
                ProjectSecurity.GetProjectSecurityInfo(project);
            }
            model.Security = project.Security;
            model.CanEdit = model.Security.CanEdit;
            model.CanDelete = model.Security.CanDelete;
            model.ProjectFolder = filesRoot;
            model.IsPrivate = project.Private;

            model.TaskCount = project.TaskCount;
            model.TaskCountTotal = project.TaskCountTotal;
            model.MilestoneCount = project.MilestoneCount;
            model.DiscussionCount = project.DiscussionCount;
            model.TimeTrackingTotal = project.TimeTrackingTotal ?? "";
            model.DocumentsCount = project.DocumentsCount;
            model.ParticipantCount = project.ParticipantCount;
            model.IsFollow = isFollow;
            model.Tags = tags;
            return model;
        }

        public MilestoneWrapper GetMilestoneWrapper(Milestone milestone)
        {
            var model = new MilestoneWrapper();
            model.Id = milestone.ID;
            model.ProjectOwner = GetSimpleProjectWrapper(milestone.Project);
            model.Title = milestone.Title;
            model.Description = milestone.Description;
            model.Created = new ApiDateTime(milestone.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(milestone.LastModifiedOn, TimeSpan.Zero);
            model.Status = (int)milestone.Status;
            model.Deadline = new ApiDateTime(TenantManager, milestone.DeadLine, TimeZoneInfo.Local, TimeZoneConverter);
            model.IsKey = milestone.IsKey;
            model.IsNotify = milestone.IsNotify;
            model.CanEdit = ProjectSecurity.CanEdit(milestone);
            model.CanDelete = ProjectSecurity.CanDelete(milestone);
            model.ActiveTaskCount = milestone.ActiveTaskCount;
            model.ClosedTaskCount = milestone.ClosedTaskCount;

            if (HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.CreatedById = milestone.CreateBy;
                model.UpdatedById = milestone.LastModifiedBy;
                if (!milestone.Responsible.Equals(Guid.Empty))
                {
                    model.ResponsibleId = milestone.Responsible;
                }
            }
            else
            {
                model.CreatedBy = GetEmployeeWraper(milestone.CreateBy);
                if (milestone.CreateBy != milestone.LastModifiedBy)
                {
                    model.UpdatedBy = GetEmployeeWraper(milestone.LastModifiedBy);
                }
                if (!milestone.Responsible.Equals(Guid.Empty))
                {
                    model.Responsible = GetEmployeeWraper(milestone.Responsible);
                }
            }
            return model;
        }

        public TaskWrapper GetTaskWrapper(Task task, TaskWrapper taskWrapper = null)
        {
            var model = taskWrapper == null ? new TaskWrapper() : taskWrapper;
            model.Id = task.ID;
            model.Title = task.Title;
            model.Description = task.Description;
            model.Status = (int)task.Status;

            if (model.Status > 2)
            {
                model.Status = 1;
            }

            model.CustomTaskStatus = task.CustomTaskStatus;

            model.Deadline = (task.Deadline == DateTime.MinValue ? null : new ApiDateTime(TenantManager,task.Deadline, TimeZoneInfo.Local, TimeZoneConverter));
            model.Priority = task.Priority;
            model.ProjectOwner = GetSimpleProjectWrapper(task.Project);
            model.MilestoneId = task.Milestone;
            model.Created = new ApiDateTime(task.CreateOn, TimeSpan.Zero);
            model.Updated = new ApiDateTime(task.LastModifiedOn, TimeSpan.Zero);
            model.StartDate = task.StartDate.Equals(DateTime.MinValue) ? null : new ApiDateTime(task.StartDate, TimeSpan.Zero);

            if (task.SubTasks != null)
            {
                model.Subtasks = task.SubTasks.Select(x => GetSubtaskWrapper(x, task)).ToList();
            }

            model.Progress = task.Progress;

            if (task.Milestone != 0 && task.MilestoneDesc != null)
            {
                model.Milestone = GetSimpleMilestoneWrapper(task.MilestoneDesc);
            }

            if (task.Links.Any())
            {
                model.Links = task.Links.Select(r => new TaskLinkWrapper(r));
            }

            if (task.Security == null)
            {
                ProjectSecurity.GetTaskSecurityInfo(task);
            }

            if (HttpContextAccessor.HttpContext.Request.Query.GetRequestValue("simple") != null)
            {
                model.CreatedById = task.CreateBy;
                model.UpdatedById = task.LastModifiedBy;
                if (task.Responsibles != null)
                {
                    model.ResponsibleIds = task.Responsibles;
                }
            }
            else
            {
                model.CreatedBy = GetEmployeeWraper(task.CreateBy);
                if (task.CreateBy != task.LastModifiedBy && task.LastModifiedBy != new Guid())
                {
                    model.UpdatedBy = GetEmployeeWraper(task.LastModifiedBy);
                }
                if (task.Responsibles != null)
                {
                    model.Responsibles = task.Responsibles.Select(GetEmployeeWraper).OrderBy(r => r.DisplayName).ToList();
                }
            }

            model.CanEdit = task.Security.CanEdit;
            model.CanCreateSubtask = task.Security.CanCreateSubtask;
            model.CanCreateTimeSpend = task.Security.CanCreateTimeSpend;
            model.CanDelete = task.Security.CanDelete;
            model.CanReadFiles = task.Security.CanReadFiles;
            return model;
        }

        public TaskWrapper GetTaskWrapper(Task task, Milestone milestone, TaskWrapper taskWrapper = null)
        {
            var model = taskWrapper == null ? new TaskWrapper() : taskWrapper;
            model = GetTaskWrapper(task, model);
            if (milestone != null && task.Milestone != 0)
                model.Milestone = GetSimpleMilestoneWrapper(milestone);
            return model;
        }

        public SimpleMilestoneWrapper GetSimpleMilestoneWrapper(Milestone milestone)
        {
            var model = new SimpleMilestoneWrapper()
            {
                Id = milestone.ID,
                Title = milestone.Title,
                Deadline = new ApiDateTime(milestone.DeadLine, TimeSpan.Zero)
            };
            return model;
        }

        public SubtaskWrapper GetSubtaskWrapper(Subtask subtask, Task task)
        {
            var model = new SubtaskWrapper();
            model.Id = subtask.ID;
            model.Title = subtask.Title;
            model.Status = (int)subtask.Status;
            if (subtask.Responsible != Guid.Empty)
            {
                model.Responsible = GetEmployeeWraper(subtask.Responsible);
            }
            model.Created = new ApiDateTime(subtask.CreateOn, TimeSpan.Zero);
            model.CreatedBy = GetEmployeeWraper(subtask.CreateBy);
            model.Updated = new ApiDateTime(subtask.LastModifiedOn, TimeSpan.Zero);
            if (subtask.CreateBy != subtask.LastModifiedBy)
            {
                model.UpdatedBy = GetEmployeeWraper(subtask.LastModifiedBy);
            }
            model.CanEdit = ProjectSecurity.CanEdit(task, subtask);

            model.TaskId = task.ID;
            return model;
        }

        public ProjectWrapper GetProjectWrapper(Project project)
        {
            var model = new ProjectWrapper();
            model.Id = project.ID;
            model.Title = project.Title;
            model.Description = project.Description;
            model.Responsible = GetEmployeeWraper(project.Responsible);
            model.Status = (int)project.Status;
            model.CanEdit = ProjectSecurity.CanEdit(project);
            model.IsPrivate = project.Private;
            return model;
        }

        public TimeWrapper GetTimeWrapper(TimeSpend timeSpend)
        {
            var model = new TimeWrapper()
            {
                Date = new ApiDateTime(timeSpend.Date, TimeSpan.Zero),
                Hours = timeSpend.Hours,
                Id = timeSpend.ID,
                Note = timeSpend.Note,
                CreatedBy = GetEmployeeWraper(timeSpend.CreateBy),
                RelatedProject = timeSpend.Task.Project.ID,
                RelatedTask = timeSpend.Task.ID,
                RelatedTaskTitle = timeSpend.Task.Title,
                CanEdit = ProjectSecurity.CanEdit(timeSpend),
                PaymentStatus = timeSpend.PaymentStatus,
                StatusChanged = new ApiDateTime(timeSpend.StatusChangedOn.GetValueOrDefault(), TimeSpan.Zero),
                CanEditPaymentStatus = ProjectSecurity.CanEditPaymentStatus(timeSpend),
                Task = GetTaskWrapper(timeSpend.Task),
                CreateOn = new ApiDateTime(timeSpend.CreateOn, TimeSpan.Zero)
            };
            if (timeSpend.CreateBy != timeSpend.Person)
            {
                model.Person = GetEmployeeWraper(timeSpend.Person);
            }
            return model;
        }

        private EmployeeWraperFull InitEmployeeWraperFull(EmployeeWraperFull result, UserInfo userInfo)
        {
            result.UserName = userInfo.UserName;
            result.FirstName = userInfo.FirstName;
            result.LastName = userInfo.LastName;
            result.Birthday = ApiDateTimeHelper.Get(userInfo.BirthDate);
            result.Status = userInfo.Status;
            result.ActivationStatus = userInfo.ActivationStatus & ~EmployeeActivationStatus.AutoGenerated;
            result.Terminated = ApiDateTimeHelper.Get(userInfo.TerminatedDate);
            result.WorkFrom = ApiDateTimeHelper.Get(userInfo.WorkFromDate);
            result.Email = userInfo.Email;
            result.IsVisitor = userInfo.IsVisitor(UserManager);
            result.IsAdmin = userInfo.IsAdmin(UserManager);
            result.IsOwner = userInfo.IsOwner(Context.Tenant);
            result.IsLDAP = userInfo.IsLDAP();
            result.IsSSO = userInfo.IsSSO();

            result = (EmployeeWraperFull)EmployeeWraperFullHelper.Init(result, userInfo);

            if (userInfo.Sex.HasValue)
            {
                result.Sex = userInfo.Sex.Value ? "male" : "female";
            }

            if (!string.IsNullOrEmpty(userInfo.Location))
            {
                result.Location = userInfo.Location;
            }

            if (!string.IsNullOrEmpty(userInfo.Notes))
            {
                result.Notes = userInfo.Notes;
            }

            if (!string.IsNullOrEmpty(userInfo.MobilePhone))
            {
                result.MobilePhone = userInfo.MobilePhone;
            }

            result.MobilePhoneActivationStatus = userInfo.MobilePhoneActivationStatus;

            if (!string.IsNullOrEmpty(userInfo.CultureName))
            {
                result.CultureName = userInfo.CultureName;
            }

            EmployeeWraperFullHelper.FillConacts(result, userInfo);

            if (Context.Check("groups") || Context.Check("department"))
            {
                var groups = UserManager.GetUserGroups(userInfo.ID)
                    .Select(x => new GroupWrapperSummary(x, UserManager))
                    .ToList();

                if (groups.Count > 0)
                {
                    result.Groups = groups;
                    result.Department = string.Join(", ", result.Groups.Select(d => d.Name.HtmlEncode()));
                }
                else
                {
                    result.Department = "";
                }
            }

            var userInfoLM = userInfo.LastModified.GetHashCode();

            if (Context.Check("avatarMax"))
            {
                result.AvatarMax = UserPhotoManager.GetMaxPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("avatarMedium"))
            {
                result.AvatarMedium = UserPhotoManager.GetMediumPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("avatar"))
            {
                result.Avatar = UserPhotoManager.GetBigPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("listAdminModules"))
            {
                var listAdminModules = userInfo.GetListAdminModules(WebItemSecurity);

                if (listAdminModules.Any())
                    result.ListAdminModules = listAdminModules;
            }
            return result;
        }

        public ParticipantWrapper GetParticipantWrapper(Participant participant)
        {
            ParticipantWrapper model = (ParticipantWrapper)InitEmployeeWraperFull(new ParticipantWrapper(), participant.UserInfo);
            model.CanReadFiles = participant.CanReadFiles;
            model.CanReadMilestones = participant.CanReadMilestones;
            model.CanReadMessages = participant.CanReadMessages;
            model.CanReadTasks = participant.CanReadTasks;
            model.CanReadContacts = participant.CanReadContacts;
            model.IsAdministrator = ProjectSecurity.IsAdministrator(participant.ID);
            model.IsRemovedFromTeam = participant.IsRemovedFromTeam;
            return model;
        }

        public CommonSecurityInfo GetCommonSecurityInfo()
        {
            var filter = new TaskFilter
            {
                SortBy = "title",
                SortOrder = true,
                ProjectStatuses = new List<ProjectStatus> { ProjectStatus.Open }
            };

            var projects = EngineFactory.GetProjectEngine().GetByFilter(filter).ToList();
            var commonSecurityInfo = new CommonSecurityInfo()
            {
                CanCreateProject = ProjectSecurity.CanCreate<Project>(null),
                CanCreateTask = projects.Any(ProjectSecurity.CanCreate<Task>),
                CanCreateMilestone = projects.Any(ProjectSecurity.CanCreate<Milestone>),
                CanCreateMessage = projects.Any(ProjectSecurity.CanCreate<Message>),
                CanCreateTimeSpend = projects.Any(ProjectSecurity.CanCreate<TimeSpend>),
            };
            return commonSecurityInfo;
        }

        public ReportTemplateWrapper GetReportTemplateWrapper(ReportTemplate reportTemplate)
        {
            var model = new ReportTemplateWrapper()
            {
                Id = reportTemplate.Id,
                Title = reportTemplate.Name,
                Created = new ApiDateTime(reportTemplate.CreateOn, TimeSpan.Zero),
                CreatedBy = EmployeeWraperHelper.Get(reportTemplate.CreateBy),
                AutoGenerated = reportTemplate.AutoGenerated,
                Cron = reportTemplate.Cron,
                ReportType = reportTemplate.ReportType,
                Filter = reportTemplate.Filter.ToUri()
            };
            return model;
        }

        public TaskWrapperFull GetTaskWrapperFull(Task task, Milestone milestone, ProjectWrapperFull project, IEnumerable<FileWrapper<int>> files, IEnumerable<CommentInfo> comments, int commentsCount, bool isSubscribed, float timeSpend)
        {
            TaskWrapperFull model = new TaskWrapperFull();
            model = (TaskWrapperFull)GetTaskWrapper(task, milestone, model);
            model.Files = files.ToList();
            model.CommentsCount = commentsCount;
            model.IsSubscribed = isSubscribed;
            model.Project = project;
            model.CanEditFiles = ProjectSecurity.CanEditFiles(task);
            model.CanCreateComment = ProjectSecurity.CanCreateComment(task);
            model.TimeSpend = timeSpend;
            model.Comments = comments.ToList();
            return model;
        }

        public SimpleTaskWrapper GetSimpleTaskWrapper(Task task)
        {
            var model = new SimpleTaskWrapper();
            model.ID = task.ID;
            model.Title = task.Title;
            model.Description = task.Description;
            model.Status = (int)task.Status;
            model.CustomTaskStatus = task.CustomTaskStatus;

            if (task.Responsibles != null)
            {
                model.Responsibles = task.Responsibles;
            }

            model.Deadline = (task.Deadline == DateTime.MinValue ? null : new ApiDateTime(TenantManager, task.Deadline, TimeZoneInfo.Local, TimeZoneConverter));
            model.Priority = task.Priority;
            model.ProjectOwner = task.Project.ID;
            model.MilestoneId = task.Milestone;
            model.Created = new ApiDateTime(task.CreateOn, TimeSpan.Zero);
            model.CreatedBy = task.CreateBy;
            model.Updated = new ApiDateTime(task.LastModifiedOn, TimeSpan.Zero);
            model.StartDate = task.StartDate.Equals(DateTime.MinValue) ? null : new ApiDateTime(task.StartDate, TimeSpan.Zero);

            if (task.CreateBy != task.LastModifiedBy)
            {
                model.UpdatedBy = task.LastModifiedBy;
            }

            if (task.SubTasks != null)
            {
                model.SubtasksCount = task.SubTasks.Count(r => r.Status == TaskStatus.Open); // somehow don't work :(
            }

            model.Progress = task.Progress;
            model.MilestoneId = task.Milestone;

            if (task.Links.Any())
            {
                model.Links = task.Links.Select(r => new TaskLinkWrapper(r));
            }

            model.CanEdit = ProjectSecurity.CanEdit(task);
            model.CanCreateSubtask = ProjectSecurity.CanCreateSubtask(task);
            model.CanCreateTimeSpend = ProjectSecurity.CanCreateTimeSpend(task);
            model.CanDelete = ProjectSecurity.CanDelete(task);
            return model;
        }
    }
}
