using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using ASC.People;
using ASC.People.Models;
using ASC.People.Resources;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public Tenant Tenant { get { return ApiContext.Tenant; } }

        public ApiContext ApiContext { get; }
        public MessageService MessageService { get; }
        public QueueWorkerReassign QueueWorkerReassign { get; }
        public QueueWorkerRemove QueueWorkerRemove { get; }
        public StudioNotifyService StudioNotifyService { get; }
        public UserManagerWrapper UserManagerWrapper { get; }

        public PeopleController(Common.Logging.LogManager logManager,
            MessageService messageService,
            QueueWorkerReassign queueWorkerReassign,
            QueueWorkerRemove queueWorkerRemove,
            StudioNotifyService studioNotifyService,
            UserManagerWrapper userManagerWrapper,
            ApiContext apiContext)
        {
            LogManager = logManager;
            MessageService = messageService;
            QueueWorkerReassign = queueWorkerReassign;
            QueueWorkerRemove = queueWorkerRemove;
            StudioNotifyService = studioNotifyService;
            UserManagerWrapper = userManagerWrapper;
            ApiContext = apiContext;
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new PeopleProduct();
            product.Init();
            return new Module(product, true);
        }

        [Read]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [Read("status/{status}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(Tenant, status).AsEnumerable();
            if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                var groupId = new Guid(ApiContext.FilterValue);
                //Filter by group
                query = query.Where(x => CoreContext.UserManager.IsUserInGroup(Tenant, x.ID, groupId));
                ApiContext.SetDataFiltered();
            }
            return query.Select(x => new EmployeeWraperFull(x, ApiContext));
        }

        [Read("@self")]
        public EmployeeWraper Self()
        {
            return new EmployeeWraperFull(CoreContext.UserManager.GetUsers(Tenant.TenantId, SecurityContext.CurrentAccount.ID), ApiContext);
        }

        [Read("email")]
        public EmployeeWraperFull GetByEmail([FromQuery]string email)
        {
            if (CoreContext.Configuration.Personal && !CoreContext.UserManager.GetUsers(Tenant.TenantId, SecurityContext.CurrentAccount.ID).IsOwner(Tenant))
                throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByEmail(Tenant.TenantId, email);
            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user, ApiContext);
        }

        [Read("{username}", order: int.MaxValue)]
        public EmployeeWraperFull GetById(string username)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByUserName(Tenant.TenantId, username);
            if (user.ID == Constants.LostUser.ID)
            {
                if (Guid.TryParse(username, out var userId))
                {
                    user = CoreContext.UserManager.GetUsers(Tenant.TenantId, userId);
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

            return new EmployeeWraperFull(user, ApiContext);
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

                return CoreContext.UserManager.Search(Tenant, query, EmployeeStatus.Active, groupId).Select(x => new EmployeeWraperFull(x, ApiContext));
            }
            catch (Exception error)
            {
                LogManager.Get("ASC.Api").Error(error);
            }
            return null;
        }

        [Read("search")]
        public IEnumerable<EmployeeWraperFull> GetPeopleSearch([FromQuery]string query)
        {
            return GetSearch(query);
        }

        [Read("status/{status}/search")]
        public IEnumerable<EmployeeWraperFull> GetAdvanced(EmployeeStatus status, [FromQuery]string query)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var list = CoreContext.UserManager.GetUsers(Tenant, status).AsEnumerable();

                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    var groupId = new Guid(ApiContext.FilterValue);
                    //Filter by group
                    list = list.Where(x => CoreContext.UserManager.IsUserInGroup(Tenant, x.ID, groupId));
                    ApiContext.SetDataFiltered();
                }

                list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                       (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Contacts != null && x.Contacts.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

                return list.Select(x => new EmployeeWraperFull(x, ApiContext));
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


        [Read("filter")]
        public IEnumerable<EmployeeWraperFull> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraperFull(u, ApiContext)).ToList();
        }

        [Read("simple/filter")]
        public IEnumerable<EmployeeWraper> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraper(u, ApiContext));
        }

        private IEnumerable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var isAdmin = CoreContext.UserManager.GetUsers(Tenant.TenantId, SecurityContext.CurrentAccount.ID).IsAdmin(Tenant) ||
                          WebItemSecurity.IsProductAdministrator(Tenant, WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

            var includeGroups = new List<List<Guid>>();
            if (groupId.HasValue)
            {
                includeGroups.Add(new List<Guid> { groupId.Value });
            }

            var excludeGroups = new List<Guid>();

            if (employeeType != null)
            {
                switch (employeeType)
                {
                    case EmployeeType.User:
                        excludeGroups.Add(Constants.GroupVisitor.ID);
                        break;
                    case EmployeeType.Visitor:
                        includeGroups.Add(new List<Guid> { Constants.GroupVisitor.ID });
                        break;
                }
            }

            if (isAdministrator.HasValue && isAdministrator.Value)
            {
                var adminGroups = new List<Guid>
                {
                    Constants.GroupAdmin.ID
                };

                var products = WebItemManager.Instance.GetItemsAll().Where(i => i is IProduct || i.ID == WebItemManager.MailProductID);
                adminGroups.AddRange(products.Select(r => r.ID));

                includeGroups.Add(adminGroups);
            }

            var users = CoreContext.UserManager.GetUsers(Tenant.TenantId, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, ApiContext.FilterValue, ApiContext.SortBy, !ApiContext.SortDescending, ApiContext.Count, ApiContext.StartIndex, out var total);

            ApiContext.SetTotalCount(total);

            return users;
        }

        [Create]
        public EmployeeWraperFull AddMember(MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_AddRemoveUser);

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

            user = UserManagerWrapper.AddUser(Tenant, user, memberModel.Password, false, true, memberModel.IsVisitor);

            var messageAction = memberModel.IsVisitor ? MessageAction.GuestCreated : MessageAction.UserCreated;
            MessageService.Send(messageAction, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return new EmployeeWraperFull(user, ApiContext);
        }

        [Create("active")]
        public EmployeeWraperFull AddMemberAsActivated(MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_AddRemoveUser);

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

            user = UserManagerWrapper.AddUser(Tenant, user, memberModel.Password, false, false, memberModel.IsVisitor);

            user.ActivationStatus = EmployeeActivationStatus.Activated;

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return new EmployeeWraperFull(user, ApiContext);
        }

        [Update("{userid}")]
        public EmployeeWraperFull UpdateMember(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            var self = SecurityContext.CurrentAccount.ID.Equals(user.ID);
            var resetDate = new DateTime(1900, 01, 01);

            //Update it

            var isLdap = user.IsLDAP();
            var isSso = user.IsSSO();
            var isAdmin = WebItemSecurity.IsProductAdministrator(Tenant, WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

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

            if (memberModel.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(Tenant, user.ID))
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
                StudioNotifyService.SendMsgToAdminAboutProfileUpdated(Tenant.TenantId);
            }

            // change user type
            var canBeGuestFlag = !user.IsOwner(Tenant) && !user.IsAdmin(Tenant) && !user.GetListAdminModules(Tenant).Any() && !user.IsMe();

            if (memberModel.IsVisitor && !user.IsVisitor(Tenant) && canBeGuestFlag)
            {
                CoreContext.UserManager.AddUserIntoGroup(Tenant, user.ID, Constants.GroupVisitor.ID);
                WebItemSecurity.ClearCache();
            }

            if (!self && !memberModel.IsVisitor && user.IsVisitor(Tenant))
            {
                var usersQuota = TenantExtra.GetTenantQuota().ActiveUsers;
                if (TenantStatisticsProvider.GetUsersCount(Tenant) < usersQuota)
                {
                    CoreContext.UserManager.RemoveUserFromGroup(Tenant, user.ID, Constants.GroupVisitor.ID);
                    WebItemSecurity.ClearCache();
                }
                else
                {
                    throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", usersQuota));
                }
            }

            CoreContext.UserManager.SaveUserInfo(Tenant, user, memberModel.IsVisitor);
            MessageService.Send(MessageAction.UserUpdated, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            if (memberModel.Disable.HasValue && memberModel.Disable.Value)
            {
                HttpContext.ResetUserCookie(Tenant.TenantId, user.ID);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return new EmployeeWraperFull(user, ApiContext);
        }

        [Delete("{userid}")]
        public EmployeeWraperFull DeleteMember(string userid)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_AddRemoveUser);

            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID) || user.IsLDAP())
                throw new SecurityException();

            if (user.Status != EmployeeStatus.Terminated)
                throw new Exception("The user is not suspended");

            CheckReassignProccess(new[] { user.ID });

            var userName = user.DisplayUserName(false);

            UserPhotoManager.RemovePhoto(Tenant, user.ID);
            CoreContext.UserManager.DeleteUser(Tenant, user.ID);
            QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);

            MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);

            return new EmployeeWraperFull(user, ApiContext);
        }

        [Update("{userid}/contacts")]
        public EmployeeWraperFull UpdateMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            UpdateContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            return new EmployeeWraperFull(user, ApiContext);
        }

        [Create("{userid}/contacts")]
        public EmployeeWraperFull SetMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            user.Contacts.Clear();
            UpdateContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            return new EmployeeWraperFull(user, ApiContext);
        }

        [Delete("{userid}/contacts")]
        public EmployeeWraperFull DeleteMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            DeleteContacts(memberModel.Contacts, user);
            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            return new EmployeeWraperFull(user, ApiContext);
        }

        [Read("{userid}/photo")]
        public ThumbnailsDataWrapper GetMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            return new ThumbnailsDataWrapper(Tenant, user.ID);
        }

        [Create("{userid}/photo")]
        public FileUploadResult UploadMemberPhoto(string userid, UploadPhotoModel model)
        {
            var result = new FileUploadResult();
            try
            {
                if (model.Files.Count != 0)
                {
                    Guid userId;
                    try
                    {
                        userId = new Guid(userid);
                    }
                    catch
                    {
                        userId = SecurityContext.CurrentAccount.ID;
                    }

                    SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(userId), Constants.Action_EditUser);

                    var userPhoto = model.Files[0];

                    if (userPhoto.Length > SetupInfo.MaxImageUploadSize)
                    {
                        result.Success = false;
                        result.Message = FileSizeComment.FileImageSizeExceptionString;
                        return result;
                    }

                    var data = new byte[userPhoto.Length];
                    using var inputStream = userPhoto.OpenReadStream();

                    var br = new BinaryReader(inputStream);
                    br.Read(data, 0, (int)userPhoto.Length);
                    br.Close();

                    CheckImgFormat(data);

                    if (model.Autosave)
                    {
                        if (data.Length > SetupInfo.MaxImageUploadSize)
                            throw new ImageSizeLimitException();

                        var mainPhoto = UserPhotoManager.SaveOrUpdatePhoto(Tenant, userId, data);

                        result.Data =
                            new
                            {
                                main = mainPhoto,
                                retina = UserPhotoManager.GetRetinaPhotoURL(Tenant.TenantId, userId),
                                max = UserPhotoManager.GetMaxPhotoURL(Tenant.TenantId, userId),
                                big = UserPhotoManager.GetBigPhotoURL(Tenant.TenantId, userId),
                                medium = UserPhotoManager.GetMediumPhotoURL(Tenant.TenantId, userId),
                                small = UserPhotoManager.GetSmallPhotoURL(Tenant.TenantId, userId),
                            };
                    }
                    else
                    {
                        result.Data = UserPhotoManager.SaveTempPhoto(Tenant.TenantId, data, SetupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = PeopleResource.ErrorEmptyUploadFileSelected;
                }

            }
            catch (UnknownImageFormatException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorUnknownFileImageType;
            }
            catch (ImageWeightLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageWeightLimit;
            }
            catch (ImageSizeLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageSizetLimit;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.HtmlEncode();
            }

            return result;
        }

        [Update("{userid}/photo")]
        public ThumbnailsDataWrapper UpdateMemberPhoto(string userid, UpdateMemberModel model)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (model.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(Tenant, user.ID))
            {
                UpdatePhotoUrl(model.Files, user);
            }

            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            MessageService.Send(MessageAction.UserAddedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(Tenant, user.ID);
        }

        [Delete("{userid}/photo")]
        public ThumbnailsDataWrapper DeleteMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            UserPhotoManager.RemovePhoto(Tenant, user.ID);

            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            MessageService.Send(MessageAction.UserDeletedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(Tenant, user.ID);
        }


        [Create("{userid}/photo/thumbnails")]
        public ThumbnailsDataWrapper CreateMemberPhotoThumbnails(string userid, ThumbnailsModel thumbnailsModel)
        {
            var user = GetUserInfo(userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!string.IsNullOrEmpty(thumbnailsModel.TmpFile))
            {
                var fileName = Path.GetFileName(thumbnailsModel.TmpFile);
                var data = UserPhotoManager.GetTempPhotoData(Tenant.TenantId, fileName);

                var settings = new UserPhotoThumbnailSettings(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height);
                settings.SaveForUser(user.ID);

                UserPhotoManager.SaveOrUpdatePhoto(Tenant, user.ID, data);
                UserPhotoManager.RemoveTempPhoto(Tenant.TenantId, fileName);
            }
            else
            {
                UserPhotoThumbnailManager.SaveThumbnails(Tenant.TenantId, thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height, user.ID);
            }

            CoreContext.UserManager.SaveUserInfo(Tenant, user);
            MessageService.Send(MessageAction.UserUpdatedAvatarThumbnails, MessageTarget.Create(user.ID), user.DisplayUserName(false));

            return new ThumbnailsDataWrapper(Tenant, user.ID);
        }


        [AllowAnonymous]
        [Create("password", false)]
        public string SendUserPassword(MemberModel memberModel)
        {
            var userInfo = UserManagerWrapper.SendUserPassword(Tenant.TenantId, memberModel.Email, MessageService, HttpContext);

            return string.Format(Resource.MessageYourPasswordSuccessfullySendedToEmail, userInfo.Email);
        }

        [Update("{userid}/password")]
        public EmployeeWraperFull ChangeUserPassword(Guid userid, MemberModel memberModel)
        {
            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(userid), Constants.Action_EditUser);

            if (!CoreContext.UserManager.UserExists(Tenant.TenantId, userid)) return null;

            var user = CoreContext.UserManager.GetUsers(Tenant.TenantId, userid);

            if (CoreContext.UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (!string.IsNullOrEmpty(memberModel.Email))
            {
                var address = new MailAddress(memberModel.Email);
                if (!string.Equals(address.Address, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    user.Email = address.Address.ToLowerInvariant();
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    CoreContext.UserManager.SaveUserInfo(Tenant, user);
                }
            }

            if (!string.IsNullOrEmpty(memberModel.Password))
            {
                SecurityContext.SetUserPassword(Tenant.TenantId, userid, memberModel.Password);
                MessageService.Send(MessageAction.UserUpdatedPassword);

                HttpContext.ResetUserCookie(Tenant.TenantId, userid);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return new EmployeeWraperFull(GetUserInfo(userid.ToString()), ApiContext);
        }

        private UserInfo GetUserInfo(string userNameOrId)
        {
            UserInfo user;
            try
            {
                var userId = new Guid(userNameOrId);
                user = CoreContext.UserManager.GetUsers(Tenant.TenantId, userId);
            }
            catch (FormatException)
            {
                user = CoreContext.UserManager.GetUserByUserName(Tenant.TenantId, userNameOrId);
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
                SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(id), Constants.Action_EditUser);
                var u = CoreContext.UserManager.GetUsers(Tenant.TenantId, id);
                if (u.ID == Constants.LostUser.ID || u.IsLDAP()) continue;

                u.ActivationStatus = activationstatus;
                CoreContext.UserManager.SaveUserInfo(Tenant, u);
                retuls.Add(new EmployeeWraperFull(u, ApiContext));
            }

            return retuls;
        }


        [Update("type/{type}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserType(EmployeeType type, UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !CoreContext.UserManager.IsSystemUser(userId))
                .Select(userId => CoreContext.UserManager.GetUsers(Tenant.TenantId, userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner(Tenant) || user.IsAdmin(Tenant) || user.IsMe() || user.GetListAdminModules(Tenant).Any())
                    continue;

                switch (type)
                {
                    case EmployeeType.User:
                        if (user.IsVisitor(Tenant))
                        {
                            if (TenantStatisticsProvider.GetUsersCount(Tenant) < TenantExtra.GetTenantQuota().ActiveUsers)
                            {
                                CoreContext.UserManager.RemoveUserFromGroup(Tenant, user.ID, Constants.GroupVisitor.ID);
                                WebItemSecurity.ClearCache();
                            }
                        }
                        break;
                    case EmployeeType.Visitor:
                        CoreContext.UserManager.AddUserIntoGroup(Tenant, user.ID, Constants.GroupVisitor.ID);
                        WebItemSecurity.ClearCache();
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedType, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user, ApiContext));
        }

        [Update("status/{status}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserStatus(EmployeeStatus status, UpdateMembersModel model)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            var users = model.UserIds.Select(userId => CoreContext.UserManager.GetUsers(Tenant.TenantId, userId))
                .Where(u => !CoreContext.UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner(Tenant) || user.IsMe())
                    continue;

                switch (status)
                {
                    case EmployeeStatus.Active:
                        if (user.Status == EmployeeStatus.Terminated)
                        {
                            if (TenantStatisticsProvider.GetUsersCount(Tenant) < TenantExtra.GetTenantQuota().ActiveUsers || user.IsVisitor(Tenant))
                            {
                                user.Status = EmployeeStatus.Active;
                                CoreContext.UserManager.SaveUserInfo(Tenant, user);
                            }
                        }
                        break;
                    case EmployeeStatus.Terminated:
                        user.Status = EmployeeStatus.Terminated;
                        CoreContext.UserManager.SaveUserInfo(Tenant, user);

                        HttpContext.ResetUserCookie(Tenant.TenantId, user.ID);
                        MessageService.Send(MessageAction.CookieSettingsUpdated);
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user, ApiContext));
        }


        [Update("invite")]
        public IEnumerable<EmployeeWraperFull> ResendUserInvites(UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !CoreContext.UserManager.IsSystemUser(userId))
                .Select(userId => CoreContext.UserManager.GetUsers(Tenant.TenantId, userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsActive) continue;

                if (user.ActivationStatus == EmployeeActivationStatus.Pending)
                {
                    if (user.IsVisitor(Tenant))
                    {
                        StudioNotifyService.GuestInfoActivation(Tenant.TenantId, user);
                    }
                    else
                    {
                        StudioNotifyService.UserInfoActivation(Tenant.TenantId, user);
                    }
                }
                else
                {
                    StudioNotifyService.SendEmailActivationInstructions(user, user.Email);
                }
            }

            MessageService.Send(MessageAction.UsersSentActivationInstructions, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false)));

            return users.Select(user => new EmployeeWraperFull(user, ApiContext));
        }

        [Update("delete")]
        public IEnumerable<EmployeeWraperFull> RemoveUsers(UpdateMembersModel model)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_AddRemoveUser);

            CheckReassignProccess(model.UserIds);

            var users = model.UserIds.Select(userId => CoreContext.UserManager.GetUsers(Tenant.TenantId, userId))
                .Where(u => !CoreContext.UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            var userNames = users.Select(x => x.DisplayUserName(false)).ToList();

            foreach (var user in users)
            {
                if (user.Status != EmployeeStatus.Terminated) continue;

                UserPhotoManager.RemovePhoto(Tenant, user.ID);
                CoreContext.UserManager.DeleteUser(Tenant, user.ID);
                QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);
            }

            MessageService.Send(MessageAction.UsersDeleted, MessageTarget.Create(users.Select(x => x.ID)), userNames);

            return users.Select(user => new EmployeeWraperFull(user, ApiContext));
        }


        [Update("self/delete")]
        public string SendInstructionsToDelete()
        {
            var user = CoreContext.UserManager.GetUsers(Tenant.TenantId, SecurityContext.CurrentAccount.ID);

            if (user.IsLDAP())
                throw new SecurityException();

            StudioNotifyService.SendMsgProfileDeletion(user);
            MessageService.Send(MessageAction.UserSentDeleteInstructions);

            return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
        }


        [Update("thirdparty/linkaccount")]
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

        [Delete("thirdparty/unlinkaccount")]
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


        [Read(@"reassign/progress")]
        public ReassignProgressItem GetReassignProgress(Guid userId)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            return QueueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
        }

        [Update(@"reassign/terminate")]
        public void TerminateReassign(Guid userId)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            QueueWorkerReassign.Terminate(Tenant.TenantId, userId);
        }

        [Create(@"reassign/start")]
        public ReassignProgressItem StartReassign(Guid fromUserId, Guid toUserId, bool deleteProfile)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            var fromUser = CoreContext.UserManager.GetUsers(Tenant.TenantId, fromUserId);

            if (fromUser == null || fromUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + fromUserId + " not found");

            if (fromUser.IsOwner(Tenant) || fromUser.IsMe() || fromUser.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + fromUserId);

            var toUser = CoreContext.UserManager.GetUsers(Tenant.TenantId, toUserId);

            if (toUser == null || toUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + toUserId + " not found");

            if (toUser.IsVisitor(Tenant) || toUser.Status == EmployeeStatus.Terminated)
                throw new ArgumentException("Can not reassign data to user with id = " + toUserId);

            return QueueWorkerReassign.Start(Tenant.TenantId, fromUserId, toUserId, SecurityContext.CurrentAccount.ID, deleteProfile);
        }

        private void CheckReassignProccess(IEnumerable<Guid> userIds)
        {
            foreach (var userId in userIds)
            {
                var reassignStatus = QueueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
                if (reassignStatus == null || reassignStatus.IsCompleted)
                    continue;

                var userName = CoreContext.UserManager.GetUsers(Tenant.TenantId, userId).DisplayUserName();
                throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
            }
        }

        //#endregion


        #region Remove user data


        [Read(@"remove/progress")]
        public RemoveProgressItem GetRemoveProgress(Guid userId)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            return QueueWorkerRemove.GetProgressItemStatus(Tenant.TenantId, userId);
        }

        [Update(@"remove/terminate")]
        public void TerminateRemove(Guid userId)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            QueueWorkerRemove.Terminate(Tenant.TenantId, userId);
        }

        [Create(@"remove/start")]
        public RemoveProgressItem StartRemove(Guid userId)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditUser);

            var user = CoreContext.UserManager.GetUsers(Tenant.TenantId, userId);

            if (user == null || user.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + userId + " not found");

            if (user.IsOwner(Tenant) || user.IsMe() || user.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + userId);

            return QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, true);
        }

        #endregion

        private void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
        {
            if (!SecurityContext.CheckPermissions(Tenant, Constants.Action_EditGroups)) return;
            if (department == null) return;

            var groups = CoreContext.UserManager.GetUserGroups(Tenant, user.ID);
            var managerGroups = new List<Guid>();
            foreach (var groupInfo in groups)
            {
                CoreContext.UserManager.RemoveUserFromGroup(Tenant, user.ID, groupInfo.ID);
                var managerId = CoreContext.UserManager.GetDepartmentManager(Tenant.TenantId, groupInfo.ID);
                if (managerId == user.ID)
                {
                    managerGroups.Add(groupInfo.ID);
                    CoreContext.UserManager.SetDepartmentManager(Tenant.TenantId, groupInfo.ID, Guid.Empty);
                }
            }
            foreach (var guid in department)
            {
                var userDepartment = CoreContext.UserManager.GetGroupInfo(Tenant.TenantId, guid);
                if (userDepartment != Constants.LostGroupInfo)
                {
                    CoreContext.UserManager.AddUserIntoGroup(Tenant, user.ID, guid);
                    if (managerGroups.Contains(guid))
                    {
                        CoreContext.UserManager.SetDepartmentManager(Tenant.TenantId, guid, user.ID);
                    }
                }
            }
        }

        private void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);
            user.Contacts.Clear();
            if (contacts == null) return;

            foreach (var contact in contacts)
            {
                user.Contacts.Add(contact.Type);
                user.Contacts.Add(contact.Value);
            }
        }

        private void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);
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

            SecurityContext.DemandPermissions(Tenant, new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!files.StartsWith("http://") && !files.StartsWith("https://"))
            {
                files = new Uri(ApiContext.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Scheme | UriPartial.Authority) + "/" + files.TrimStart('/');
            }
            var request = WebRequest.Create(files);
            using var response = (HttpWebResponse)request.GetResponse();
            using var inputStream = response.GetResponseStream();
            using var br = new BinaryReader(inputStream);
            var imageByteArray = br.ReadBytes((int)response.ContentLength);
            UserPhotoManager.SaveOrUpdatePhoto(Tenant, user.ID, imageByteArray);
        }

        private static void CheckImgFormat(byte[] data)
        {
            ImageFormat imgFormat;

            try
            {
                using var stream = new MemoryStream(data);
                using var img = new Bitmap(stream);
                imgFormat = img.RawFormat;
            }
            catch (OutOfMemoryException)
            {
                throw new ImageSizeLimitException();
            }
            catch (ArgumentException error)
            {
                throw new UnknownImageFormatException(error);
            }

            if (!imgFormat.Equals(ImageFormat.Png) && !imgFormat.Equals(ImageFormat.Jpeg))
            {
                throw new UnknownImageFormatException();
            }
        }
    }
}
