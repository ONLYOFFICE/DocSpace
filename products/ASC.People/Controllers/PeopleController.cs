using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using ASC.Api.Core;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Reassigns;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.MessagingSystem;
using ASC.People.Models;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Employee.Core.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        public Common.Logging.LogManager LogManager { get; }
        public ApiContext ApiContext { get; }
        public MessageService MessageService { get; }
        public QueueWorkerReassign QueueWorkerReassign { get; }
        public QueueWorkerRemove QueueWorkerRemove { get; }

        public PeopleController(Common.Logging.LogManager logManager, MessageService messageService, QueueWorkerReassign queueWorkerReassign, QueueWorkerRemove queueWorkerRemove)
        {
            LogManager = logManager;
            ApiContext = HttpContext;
            MessageService = messageService;
            QueueWorkerReassign = queueWorkerReassign;
            QueueWorkerRemove = queueWorkerRemove;
        }

        [Read, Read(false)]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [Read("status/{status}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(status).AsEnumerable();
            if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                var groupId = new Guid(ApiContext.FilterValue);
                //Filter by group
                query = query.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID, groupId));
                ApiContext.SetDataFiltered();
            }
            return query.Select(x => new EmployeeWraperFull(x, ApiContext));
        }

        [Read("@self"), Read("@self", false)]
        public EmployeeWraper Self()
        {
            return new EmployeeWraperFull(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID), ApiContext);
        }

        [Read("email"), Read("email", false)]
        public EmployeeWraperFull GetByEmail([FromQuery]string email)
        {
            if (CoreContext.Configuration.Personal && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOwner())
                throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByEmail(email);
            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }

        [Read("{username}", Order = int.MaxValue)]
        public EmployeeWraperFull GetById(string username)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByUserName(username);
            if (user.ID == Constants.LostUser.ID)
            {
                if (Guid.TryParse(username, out var userId))
                {
                    user = CoreContext.UserManager.GetUsers(userId);
                }
                else
                {
                    LogManager.Get("ASC.Api").Error(string.Format("Account {0} сould not get user by name {1}", SecurityContext.CurrentAccount.ID, username));
                }
            }

            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }

        [Read("@search/{query}")]
        public IEnumerable<EmployeeWraperFull> GetSearch(string query)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var groupId = Guid.Empty;
                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    groupId = new Guid(ApiContext.FilterValue);
                }

                return CoreContext.UserManager.Search(query, EmployeeStatus.Active, groupId).Select(x => new EmployeeWraperFull(x));
            }
            catch (Exception error)
            {
                LogManager.Get("ASC.Api").Error(error);
            }
            return null;
        }

        [Read("search"), Read("search", false)]
        public IEnumerable<EmployeeWraperFull> GetPeopleSearch([FromQuery]string query)
        {
            return GetSearch(query);
        }

        [Read("status/{status}/search"), Read("status/{status}/search", false)]
        public IEnumerable<EmployeeWraperFull> GetAdvanced(EmployeeStatus status, [FromQuery]string query)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var list = CoreContext.UserManager.GetUsers(status).AsEnumerable();

                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    var groupId = new Guid(ApiContext.FilterValue);
                    //Filter by group
                    list = list.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID, groupId));
                    ApiContext.SetDataFiltered();
                }

                list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                       (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Contacts != null && x.Contacts.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

                return list.Select(x => new EmployeeWraperFull(x));
            }
            catch (Exception error)
            {
                LogManager.Get("ASC.Api").Error(error);
            }
            return null;
        }

        ///// <summary>
        ///// Adds a new portal user from import with the first and last name, email address
        ///// </summary>
        ///// <short>
        ///// Add new import user
        ///// </short>
        ///// <param name="userList">The list of users to add</param>
        ///// <param name="importUsersAsCollaborators" optional="true">Add users as guests (bool type: false|true)</param>
        ///// <returns>Newly created users</returns>
        //[Create("import/save")]
        //public void SaveUsers(string userList, bool importUsersAsCollaborators)
        //{
        //    lock (progressQueue.SynchRoot)
        //    {
        //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
        //        if (task != null && task.IsCompleted)
        //        {
        //            progressQueue.Remove(task);
        //            task = null;
        //        }
        //        if (task == null)
        //        {
        //            progressQueue.Add(new ImportUsersTask(userList, importUsersAsCollaborators, GetHttpHeaders(HttpContext.Current.Request))
        //            {
        //                Id = TenantProvider.CurrentTenantID,
        //                UserId = SecurityContext.CurrentAccount.ID,
        //                Percentage = 0
        //            });
        //        }
        //    }
        //}

        //[Read("import/status")]
        //public object GetStatus()
        //{
        //    lock (progressQueue.SynchRoot)
        //    {
        //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
        //        if (task == null) return null;

        //        return new
        //        {
        //            Completed = task.IsCompleted,
        //            Percents = (int)task.Percentage,
        //            UserCounter = task.GetUserCounter,
        //            Status = (int)task.Status,
        //            Error = (string)task.Error,
        //            task.Data
        //        };
        //    }
        //}


        [Read("filter"), Read("filter", false)]
        public IEnumerable<EmployeeWraperFull> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraperFull(u, ApiContext));
        }

        [Read("simple/filter"), Read("simple/filter", false)]
        public IEnumerable<EmployeeWraper> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraper(u));
        }

        private IEnumerable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var isAdmin = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin() ||
                          WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);
            var status = isAdmin ? EmployeeStatus.All : EmployeeStatus.Default;

            if (employeeStatus != null)
            {
                switch (employeeStatus)
                {
                    case EmployeeStatus.Terminated:
                    case EmployeeStatus.All:
                        status = isAdmin ? (EmployeeStatus)employeeStatus : EmployeeStatus.Default;
                        break;
                    default:
                        status = (EmployeeStatus)employeeStatus;
                        break;
                }
            }

            var users = string.IsNullOrEmpty(ApiContext.FilterValue) ?
                            CoreContext.UserManager.GetUsers(status).AsEnumerable() :
                            CoreContext.UserManager.Search(ApiContext.FilterValue, status).AsEnumerable();

            if (groupId != null && !groupId.Equals(Guid.Empty))
            {
                users = users.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID, (Guid)groupId));
            }
            if (activationStatus != null)
            {
                users = activationStatus == EmployeeActivationStatus.Activated ?
                            users.Where(x => x.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated)) :
                            users.Where(x => x.ActivationStatus == EmployeeActivationStatus.NotActivated ||
                                             x.ActivationStatus == EmployeeActivationStatus.Pending ||
                                             x.ActivationStatus == EmployeeActivationStatus.AutoGenerated);
            }
            if (employeeType != null)
            {
                switch (employeeType)
                {
                    case EmployeeType.User:
                        users = users.Where(x => !x.IsVisitor());
                        break;
                    case EmployeeType.Visitor:
                        users = users.Where(x => x.IsVisitor());
                        break;
                }
            }

            if (isAdministrator.HasValue && isAdministrator.Value)
            {
                users = users.Where(x => x.IsAdmin() || x.GetListAdminModules().Any());
            }

            ApiContext.TotalCount = users.Count();

            switch (ApiContext.SortBy)
            {
                case "firstname":
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.FirstName) : users.OrderBy(r => r, UserInfoComparer.FirstName);
                    break;
                case "lastname":
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.LastName) : users.OrderBy(r => r, UserInfoComparer.LastName);
                    break;
                default:
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.Default) : users.OrderBy(r => r, UserInfoComparer.Default);
                    break;
            }

            users = users.Skip((int)ApiContext.StartIndex).Take((int)ApiContext.Count - 1);

            ApiContext.SetDataSorted();
            ApiContext.SetDataPaginated();

            return users;
        }

        [Create, Create(false)]
        public EmployeeWraperFull AddMember(MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);

            if (string.IsNullOrEmpty(memberModel.Password))
                memberModel.Password = UserManagerWrapper.GeneratePassword();

            memberModel.Password = memberModel.Password.Trim();

            var user = new UserInfo();

            //Validate email
            var address = new MailAddress(memberModel.Email);
            user.Email = address.Address;
            //Set common fields
            user.FirstName = memberModel.Firstname;
            user.LastName = memberModel.Lastname;
            user.Title = memberModel.Title;
            user.Location = memberModel.Location;
            user.Notes = memberModel.Comment;
            user.Sex = "male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
                           ? true
                           : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

            user.BirthDate = memberModel.Birthday != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Birthday)) : (DateTime?)null;
            user.WorkFromDate = memberModel.Worksfrom != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Worksfrom)) : DateTime.UtcNow.Date;

            UpdateContacts(memberModel.Contacts, user);

            user = UserManagerWrapper.AddUser(user, memberModel.Password, false, true, memberModel.IsVisitor);

            var messageAction = memberModel.IsVisitor ? MessageAction.GuestCreated : MessageAction.UserCreated;
            MessageService.Send(messageAction, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return new EmployeeWraperFull(user);
        }

        [Create("active"), Create("active", false)]
        public EmployeeWraperFull AddMemberAsActivated(MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);

            var user = new UserInfo();

            if (string.IsNullOrEmpty(memberModel.Password))
                memberModel.Password = UserManagerWrapper.GeneratePassword();

            //Validate email
            var address = new MailAddress(memberModel.Email);
            user.Email = address.Address;
            //Set common fields
            user.FirstName = memberModel.Firstname;
            user.LastName = memberModel.Lastname;
            user.Title = memberModel.Title;
            user.Location = memberModel.Location;
            user.Notes = memberModel.Comment;
            user.Sex = "male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
                           ? true
                           : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

            user.BirthDate = memberModel.Birthday != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Birthday)) : (DateTime?)null;
            user.WorkFromDate = memberModel.Worksfrom != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Worksfrom)) : DateTime.UtcNow.Date;

            UpdateContacts(memberModel.Contacts, user);

            user = UserManagerWrapper.AddUser(user, memberModel.Password, false, false, memberModel.IsVisitor);

            user.ActivationStatus = EmployeeActivationStatus.Activated;

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return new EmployeeWraperFull(user);
        }

        [Update("{userid}")]
        public EmployeeWraperFull UpdateMember(UpdateMemberModel memberModel)
        {
            SecurityContext.DemandPermissions(new UserSecurityProvider(new Guid(memberModel.UserId)), Constants.Action_EditUser);

            var user = GetUserInfo(memberModel.UserId);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            var self = SecurityContext.CurrentAccount.ID.Equals(user.ID);
            var resetDate = new DateTime(1900, 01, 01);

            //Update it

            var isLdap = user.IsLDAP();
            var isSso = user.IsSSO();
            var isAdmin = WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

            if (!isLdap && !isSso)
            {
                //Set common fields

                user.FirstName = memberModel.Firstname ?? user.FirstName;
                user.LastName = memberModel.Lastname ?? user.LastName;
                user.Location = memberModel.Location ?? user.Location;

                if (isAdmin)
                {
                    user.Title = memberModel.Title ?? user.Title;
                }
            }

            if (!UserFormatter.IsValidUserName(user.FirstName, user.LastName))
                throw new Exception(Resource.ErrorIncorrectUserName);

            user.Notes = memberModel.Comment ?? user.Notes;
            user.Sex = ("male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
                            ? true
                            : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null)) ?? user.Sex;

            user.BirthDate = memberModel.Birthday != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Birthday)) : user.BirthDate;

            if (user.BirthDate == resetDate)
            {
                user.BirthDate = null;
            }

            user.WorkFromDate = memberModel.Worksfrom != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(memberModel.Worksfrom)) : user.WorkFromDate;

            if (user.WorkFromDate == resetDate)
            {
                user.WorkFromDate = null;
            }

            //Update contacts
            UpdateContacts(memberModel.Contacts, user);
            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(user.ID))
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }
            if (memberModel.Disable.HasValue)
            {
                user.Status = memberModel.Disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
                user.TerminatedDate = memberModel.Disable.Value ? DateTime.UtcNow : (DateTime?)null;
            }

            if (self && !isAdmin)
            {
                StudioNotifyService.Instance.SendMsgToAdminAboutProfileUpdated();
            }

            // change user type
            var canBeGuestFlag = !user.IsOwner() && !user.IsAdmin() && !user.GetListAdminModules().Any() && !user.IsMe();

            if (memberModel.IsVisitor && !user.IsVisitor() && canBeGuestFlag)
            {
                CoreContext.UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
                WebItemSecurity.ClearCache();
            }

            if (!self && !memberModel.IsVisitor && user.IsVisitor())
            {
                var usersQuota = TenantExtra.GetTenantQuota().ActiveUsers;
                if (TenantStatisticsProvider.GetUsersCount() < usersQuota)
                {
                    CoreContext.UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                    WebItemSecurity.ClearCache();
                }
                else
                {
                    throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", usersQuota));
                }
            }

            CoreContext.UserManager.SaveUserInfo(user, memberModel.IsVisitor);
            MessageService.Send(MessageAction.UserUpdated, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            if (memberModel.Disable.HasValue && memberModel.Disable.Value)
            {
                CookiesManager.ResetUserCookie(user.ID);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return new EmployeeWraperFull(user);
        }

        [Delete("{userid}")]
        public EmployeeWraperFull DeleteMember(string userid)
        {
            SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);

            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID) || user.IsLDAP())
                throw new SecurityException();

            if (user.Status != EmployeeStatus.Terminated)
                throw new Exception("The user is not suspended");

            CheckReassignProccess(new[] { user.ID });

            var userName = user.DisplayUserName(false);

            UserPhotoManager.RemovePhoto(user.ID);
            CoreContext.UserManager.DeleteUser(user.ID);
            QueueWorkerRemove.Start(TenantProvider.CurrentTenantID, user, SecurityContext.CurrentAccount.ID, false);

            MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);

            return new EmployeeWraperFull(user);
        }

        [Update("{userid}/contacts"), Update("{userid}/contacts", false)]
        public EmployeeWraperFull UpdateMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            UpdateContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        [Create("{userid}/contacts"), Create("{userid}/contacts", false)]
        public EmployeeWraperFull SetMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            user.Contacts.Clear();
            UpdateContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        [Delete("{userid}/contacts"), Delete("{userid}/contacts", false)]
        public EmployeeWraperFull DeleteMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            DeleteContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        [Read("{userid}/photo"), Read("{userid}/photo", false)]
        public ThumbnailsDataWrapper GetMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            return new ThumbnailsDataWrapper(user.ID);
        }

        [Update("{userid}/photo"), Update("{userid}/photo", false)]
        public ThumbnailsDataWrapper UpdateMemberPhoto(string userid, UpdateMemberModel model)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (model.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(user.ID))
            {
                UpdatePhotoUrl(model.Files, user);
            }

            CoreContext.UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserAddedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(user.ID);
        }

        [Delete("{userid}/photo"), Delete("{userid}/photo", false)]
        public ThumbnailsDataWrapper DeleteMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            SecurityContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            UserPhotoManager.RemovePhoto(user.ID);

            CoreContext.UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserDeletedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(user.ID);
        }


        [Create("{userid}/photo/thumbnails"), Create("{userid}/photo/thumbnails", false)]
        public ThumbnailsDataWrapper CreateMemberPhotoThumbnails(string userid, ThumbnailsModel thumbnailsModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            SecurityContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!string.IsNullOrEmpty(thumbnailsModel.TmpFile))
            {
                var fileName = Path.GetFileName(thumbnailsModel.TmpFile);
                var data = UserPhotoManager.GetTempPhotoData(fileName);

                var settings = new UserPhotoThumbnailSettings(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height);
                settings.SaveForUser(user.ID);

                UserPhotoManager.SaveOrUpdatePhoto(user.ID, data);
                UserPhotoManager.RemoveTempPhoto(fileName);
            }
            else
            {
                UserPhotoThumbnailManager.SaveThumbnails(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height, user.ID);
            }

            CoreContext.UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserUpdatedAvatarThumbnails, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(user.ID);
        }


        [AllowAnonymous]
        [Create("password", check: false), Create("password", false, check: false)]
        public string SendUserPassword(string email)
        {
            var userInfo = UserManagerWrapper.SendUserPassword(email, MessageService);

            return string.Format(Resource.MessageYourPasswordSuccessfullySendedToEmail, userInfo.Email);
        }

        [Update("{userid}/password"), Update("{userid}/password", false)]
        public EmployeeWraperFull ChangeUserPassword(Guid userid, MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(new UserSecurityProvider(userid), Constants.Action_EditUser);

            if (!CoreContext.UserManager.UserExists(userid)) return null;

            var user = CoreContext.UserManager.GetUsers(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (!string.IsNullOrEmpty(memberModel.Email))
            {
                var address = new MailAddress(memberModel.Email);
                if (!string.Equals(address.Address, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    user.Email = address.Address.ToLowerInvariant();
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    CoreContext.UserManager.SaveUserInfo(user);
                }
            }

            if (!string.IsNullOrEmpty(memberModel.Password))
            {
                SecurityContext.SetUserPassword(userid, memberModel.Password);
                MessageService.Send(MessageAction.UserUpdatedPassword);

                CookiesManager.ResetUserCookie(userid);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return new EmployeeWraperFull(GetUserInfo(userid.ToString()));
        }

        private static UserInfo GetUserInfo(string userNameOrId)
        {
            UserInfo user;
            try
            {
                var userId = new Guid(userNameOrId);
                user = CoreContext.UserManager.GetUsers(userId);
            }
            catch (FormatException)
            {
                user = CoreContext.UserManager.GetUserByUserName(userNameOrId);
            }
            if (user == null || user.ID == Constants.LostUser.ID)
                throw new ItemNotFoundException("user not found");
            return user;
        }

        [Update("activationstatus/{activationstatus}")]
        public IEnumerable<EmployeeWraperFull> UpdateEmployeeActivationStatus(EmployeeActivationStatus activationstatus, UpdateMembersModel model)
        {
            var retuls = new List<EmployeeWraperFull>();
            foreach (var id in model.UserIds.Where(userId => !CoreContext.UserManager.IsSystemUser(userId)))
            {
                SecurityContext.DemandPermissions(new UserSecurityProvider(id), Constants.Action_EditUser);
                var u = CoreContext.UserManager.GetUsers(id);
                if (u.ID == Constants.LostUser.ID || u.IsLDAP()) continue;

                u.ActivationStatus = activationstatus;
                CoreContext.UserManager.SaveUserInfo(u);
                retuls.Add(new EmployeeWraperFull(u));
            }

            return retuls;
        }


        [Update("type/{type}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserType(EmployeeType type, UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !CoreContext.UserManager.IsSystemUser(userId))
                .Select(userId => CoreContext.UserManager.GetUsers(userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner() || user.IsAdmin() || user.IsMe() || user.GetListAdminModules().Any())
                    continue;

                switch (type)
                {
                    case EmployeeType.User:
                        if (user.IsVisitor())
                        {
                            if (TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers)
                            {
                                CoreContext.UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                                WebItemSecurity.ClearCache();
                            }
                        }
                        break;
                    case EmployeeType.Visitor:
                        CoreContext.UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
                        WebItemSecurity.ClearCache();
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedType, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user));
        }

        [Update("status/{status}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserStatus(EmployeeStatus status, UpdateMembersModel model)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            var users = model.UserIds.Select(userId => CoreContext.UserManager.GetUsers(userId))
                .Where(u => !CoreContext.UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner() || user.IsMe())
                    continue;

                switch (status)
                {
                    case EmployeeStatus.Active:
                        if (user.Status == EmployeeStatus.Terminated)
                        {
                            if (TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers || user.IsVisitor())
                            {
                                user.Status = EmployeeStatus.Active;
                                CoreContext.UserManager.SaveUserInfo(user);
                            }
                        }
                        break;
                    case EmployeeStatus.Terminated:
                        user.Status = EmployeeStatus.Terminated;
                        CoreContext.UserManager.SaveUserInfo(user);

                        CookiesManager.ResetUserCookie(user.ID);
                        MessageService.Send(MessageAction.CookieSettingsUpdated);
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user));
        }


        [Update("invite"), Update("invite", false)]
        public IEnumerable<EmployeeWraperFull> ResendUserInvites(UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !CoreContext.UserManager.IsSystemUser(userId))
                .Select(userId => CoreContext.UserManager.GetUsers(userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsActive) continue;

                if (user.ActivationStatus == EmployeeActivationStatus.Pending)
                {
                    if (user.IsVisitor())
                    {
                        StudioNotifyService.Instance.GuestInfoActivation(user);
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoActivation(user);
                    }
                }
                else
                {
                    StudioNotifyService.Instance.SendEmailActivationInstructions(user, user.Email);
                }
            }

            MessageService.Send(MessageAction.UsersSentActivationInstructions, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user));
        }

        [Update("delete"), Update("delete", false)]
        public IEnumerable<EmployeeWraperFull> RemoveUsers(UpdateMembersModel model)
        {
            SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);

            CheckReassignProccess(model.UserIds);

            var users = model.UserIds.Select(userId => CoreContext.UserManager.GetUsers(userId))
                .Where(u => !CoreContext.UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            var userNames = users.Select(x => x.DisplayUserName(false)).ToList();

            foreach (var user in users)
            {
                if (user.Status != EmployeeStatus.Terminated) continue;

                UserPhotoManager.RemovePhoto(user.ID);
                CoreContext.UserManager.DeleteUser(user.ID);
                QueueWorkerRemove.Start(TenantProvider.CurrentTenantID, user, SecurityContext.CurrentAccount.ID, false);
            }

            MessageService.Send(MessageAction.UsersDeleted, MessageTarget.Create(users.Select(x => x.ID)), userNames);

            return users.Select(user => new EmployeeWraperFull(user));
        }


        [Update("self/delete"), Update("self/delete", false)]
        public string SendInstructionsToDelete()
        {
            var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            if (user.IsLDAP())
                throw new SecurityException();

            StudioNotifyService.Instance.SendMsgProfileDeletion(user);
            MessageService.Send(MessageAction.UserSentDeleteInstructions);

            return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
        }


        [Update("thirdparty/linkaccount"), Update("thirdparty/linkaccount", false)]
        public void LinkAccount(string serializedProfile)
        {
            var profile = new LoginProfile(serializedProfile);

            if (string.IsNullOrEmpty(profile.AuthorizationError))
            {
                GetLinker().AddLink(SecurityContext.CurrentAccount.ID.ToString(), profile);
                MessageService.Send(MessageAction.UserLinkedSocialAccount, GetMeaningfulProviderName(profile.Provider));
            }
            else
            {
                // ignore cancellation
                if (profile.AuthorizationError != "Canceled at provider")
                {
                    throw new Exception(profile.AuthorizationError);
                }
            }
        }

        [Delete("thirdparty/unlinkaccount"), Delete("thirdparty/unlinkaccount", false)]
        public void UnlinkAccount(string provider)
        {
            GetLinker().RemoveProvider(SecurityContext.CurrentAccount.ID.ToString(), provider);
            MessageService.Send(MessageAction.UserUnlinkedSocialAccount, GetMeaningfulProviderName(provider));
        }

        private static AccountLinker GetLinker()
        {
            return new AccountLinker("webstudio");
        }

        private static string GetMeaningfulProviderName(string providerName)
        {
            switch (providerName)
            {
                case "google":
                case "openid":
                    return "Google";
                case "facebook":
                    return "Facebook";
                case "twitter":
                    return "Twitter";
                case "linkedin":
                    return "LinkedIn";
                default:
                    return "Unknown Provider";
            }
        }


        [Read(@"reassign/progress"), Read(@"reassign/progress", false)]
        public ReassignProgressItem GetReassignProgress(Guid userId)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            return QueueWorkerReassign.GetProgressItemStatus(TenantProvider.CurrentTenantID, userId);
        }

        [Update(@"reassign/terminate"), Update(@"reassign/terminate", false)]
        public void TerminateReassign(Guid userId)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerReassign.Terminate(TenantProvider.CurrentTenantID, userId);
        }

        [Create(@"reassign/start"), Create(@"reassign/start", false)]
        public ReassignProgressItem StartReassign(Guid fromUserId, Guid toUserId, bool deleteProfile)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            var fromUser = CoreContext.UserManager.GetUsers(fromUserId);

            if (fromUser == null || fromUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + fromUserId + " not found");

            if (fromUser.IsOwner() || fromUser.IsMe() || fromUser.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + fromUserId);

            var toUser = CoreContext.UserManager.GetUsers(toUserId);

            if (toUser == null || toUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + toUserId + " not found");

            if (toUser.IsVisitor() || toUser.Status == EmployeeStatus.Terminated)
                throw new ArgumentException("Can not reassign data to user with id = " + toUserId);

            return QueueWorkerReassign.Start(TenantProvider.CurrentTenantID, fromUserId, toUserId, SecurityContext.CurrentAccount.ID, deleteProfile);
        }

        private void CheckReassignProccess(IEnumerable<Guid> userIds)
        {
            foreach (var userId in userIds)
            {
                var reassignStatus = QueueWorkerReassign.GetProgressItemStatus(TenantProvider.CurrentTenantID, userId);
                if (reassignStatus == null || reassignStatus.IsCompleted)
                    continue;

                var userName = CoreContext.UserManager.GetUsers(userId).DisplayUserName();
                throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
            }
        }

        //#endregion


        #region Remove user data


        [Read(@"remove/progress"), Read(@"remove/progress", false)]
        public RemoveProgressItem GetRemoveProgress(Guid userId)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            return QueueWorkerRemove.GetProgressItemStatus(TenantProvider.CurrentTenantID, userId);
        }

        [Update(@"remove/terminate"), Update(@"remove/terminate", false)]
        public void TerminateRemove(Guid userId)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerRemove.Terminate(TenantProvider.CurrentTenantID, userId);
        }

        [Create(@"remove/start"), Create(@"remove/start", false)]
        public RemoveProgressItem StartRemove(Guid userId)
        {
            SecurityContext.DemandPermissions(Constants.Action_EditUser);

            var user = CoreContext.UserManager.GetUsers(userId);

            if (user == null || user.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + userId + " not found");

            if (user.IsOwner() || user.IsMe() || user.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + userId);

            return QueueWorkerRemove.Start(TenantProvider.CurrentTenantID, user, SecurityContext.CurrentAccount.ID, true);
        }

        #endregion

        private static void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
        {
            if (!SecurityContext.CheckPermissions(Constants.Action_EditGroups)) return;
            if (department == null) return;

            var groups = CoreContext.UserManager.GetUserGroups(user.ID);
            var managerGroups = new List<Guid>();
            foreach (var groupInfo in groups)
            {
                CoreContext.UserManager.RemoveUserFromGroup(user.ID, groupInfo.ID);
                var managerId = CoreContext.UserManager.GetDepartmentManager(groupInfo.ID);
                if (managerId == user.ID)
                {
                    managerGroups.Add(groupInfo.ID);
                    CoreContext.UserManager.SetDepartmentManager(groupInfo.ID, Guid.Empty);
                }
            }
            foreach (var guid in department)
            {
                var userDepartment = CoreContext.UserManager.GetGroupInfo(guid);
                if (userDepartment != Constants.LostGroupInfo)
                {
                    CoreContext.UserManager.AddUserIntoGroup(user.ID, guid);
                    if (managerGroups.Contains(guid))
                    {
                        CoreContext.UserManager.SetDepartmentManager(guid, user.ID);
                    }
                }
            }
        }

        private static void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            SecurityContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);
            user.Contacts.Clear();
            if (contacts == null) return;

            foreach (var contact in contacts)
            {
                user.Contacts.Add(contact.Type);
                user.Contacts.Add(contact.Value);
            }
        }

        private static void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            SecurityContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);
            if (contacts == null) return;

            foreach (var contact in contacts)
            {
                var index = user.Contacts.IndexOf(contact.Type);
                if (index != -1)
                {
                    //Remove existing
                    user.Contacts.RemoveRange(index, 2);
                }
            }
        }

        private void UpdatePhotoUrl(string files, UserInfo user)
        {
            if (string.IsNullOrEmpty(files))
            {
                return;
            }

            SecurityContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!files.StartsWith("http://") && !files.StartsWith("https://"))
            {
                files = new Uri(ApiContext.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Scheme | UriPartial.Authority) + "/" + files.TrimStart('/');
            }
            var request = WebRequest.Create(files);
            using var response = (HttpWebResponse)request.GetResponse();
            using var inputStream = response.GetResponseStream();
            using var br = new BinaryReader(inputStream);
            var imageByteArray = br.ReadBytes((int)response.ContentLength);
            UserPhotoManager.SaveOrUpdatePhoto(user.ID, imageByteArray);
        }
    }
}
