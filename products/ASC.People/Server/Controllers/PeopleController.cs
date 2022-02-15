
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Reassigns;
using ASC.FederatedLogin;
using ASC.FederatedLogin.LoginProviders;
using ASC.FederatedLogin.Profile;
using ASC.MessagingSystem;
using ASC.People;
using ASC.People.Models;
using ASC.People.Resources;
using ASC.Security.Cryptography;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Mobile;
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
using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Employee.Core.Controllers
{
    [Scope(Additional = typeof(BaseLoginProviderExtension))]
    [DefaultRoute]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        public Tenant Tenant { get { return ApiContext.Tenant; } }
        private ApiContext ApiContext { get; }
        private MessageService MessageService { get; }
        private QueueWorkerReassign QueueWorkerReassign { get; }
        private QueueWorkerRemove QueueWorkerRemove { get; }
        private StudioNotifyService StudioNotifyService { get; }
        private UserManagerWrapper UserManagerWrapper { get; }
        private UserManager UserManager { get; }
        private TenantExtra TenantExtra { get; }
        private TenantStatisticsProvider TenantStatisticsProvider { get; }
        private UserPhotoManager UserPhotoManager { get; }
        private SecurityContext SecurityContext { get; }
        private CookiesManager CookiesManager { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private PermissionContext PermissionContext { get; }
        private AuthContext AuthContext { get; }
        private WebItemManager WebItemManager { get; }
        private CustomNamingPeople CustomNamingPeople { get; }
        private TenantUtil TenantUtil { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private SetupInfo SetupInfo { get; }
        private FileSizeComment FileSizeComment { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private Signature Signature { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private WebItemSecurityCache WebItemSecurityCache { get; }
        private MessageTarget MessageTarget { get; }
        private SettingsManager SettingsManager { get; }
        private IOptionsSnapshot<AccountLinker> AccountLinker { get; }
        private EmployeeWraperFullHelper EmployeeWraperFullHelper { get; }
        private EmployeeWraperHelper EmployeeWraperHelper { get; }
        private UserFormatter UserFormatter { get; }
        private PasswordHasher PasswordHasher { get; }
        private UserHelpTourHelper UserHelpTourHelper { get; }
        private PersonalSettingsHelper PersonalSettingsHelper { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private MobileDetector MobileDetector { get; }
        private ProviderManager ProviderManager { get; }
        private Constants Constants { get; }
        private Recaptcha Recaptcha { get; }
        private ILog Log { get; }
        private IHttpClientFactory ClientFactory { get; }

        public PeopleController(
            MessageService messageService,
            QueueWorkerReassign queueWorkerReassign,
            QueueWorkerRemove queueWorkerRemove,
            StudioNotifyService studioNotifyService,
            UserManagerWrapper userManagerWrapper,
            ApiContext apiContext,
            UserManager userManager,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticsProvider,
            UserPhotoManager userPhotoManager,
            SecurityContext securityContext,
            CookiesManager cookiesManager,
            WebItemSecurity webItemSecurity,
            PermissionContext permissionContext,
            AuthContext authContext,
            WebItemManager webItemManager,
            CustomNamingPeople customNamingPeople,
            TenantUtil tenantUtil,
            CoreBaseSettings coreBaseSettings,
            SetupInfo setupInfo,
            FileSizeComment fileSizeComment,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            Signature signature,
            InstanceCrypto instanceCrypto,
            WebItemSecurityCache webItemSecurityCache,
            MessageTarget messageTarget,
            SettingsManager settingsManager,
            IOptionsMonitor<ILog> option,
            IOptionsSnapshot<AccountLinker> accountLinker,
            EmployeeWraperFullHelper employeeWraperFullHelper,
            EmployeeWraperHelper employeeWraperHelper,
            UserFormatter userFormatter,
            PasswordHasher passwordHasher,
            UserHelpTourHelper userHelpTourHelper,
            PersonalSettingsHelper personalSettingsHelper,
            CommonLinkUtility commonLinkUtility,
            MobileDetector mobileDetector,
            ProviderManager providerManager,
            Constants constants,
            Recaptcha recaptcha,
            IHttpClientFactory clientFactory
            )
        {
            Log = option.Get("ASC.Api");
            MessageService = messageService;
            QueueWorkerReassign = queueWorkerReassign;
            QueueWorkerRemove = queueWorkerRemove;
            StudioNotifyService = studioNotifyService;
            UserManagerWrapper = userManagerWrapper;
            ApiContext = apiContext;
            UserManager = userManager;
            TenantExtra = tenantExtra;
            TenantStatisticsProvider = tenantStatisticsProvider;
            UserPhotoManager = userPhotoManager;
            SecurityContext = securityContext;
            CookiesManager = cookiesManager;
            WebItemSecurity = webItemSecurity;
            PermissionContext = permissionContext;
            AuthContext = authContext;
            WebItemManager = webItemManager;
            CustomNamingPeople = customNamingPeople;
            TenantUtil = tenantUtil;
            CoreBaseSettings = coreBaseSettings;
            SetupInfo = setupInfo;
            FileSizeComment = fileSizeComment;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            WebItemSecurityCache = webItemSecurityCache;
            MessageTarget = messageTarget;
            SettingsManager = settingsManager;
            AccountLinker = accountLinker;
            EmployeeWraperFullHelper = employeeWraperFullHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            UserFormatter = userFormatter;
            PasswordHasher = passwordHasher;
            UserHelpTourHelper = userHelpTourHelper;
            PersonalSettingsHelper = personalSettingsHelper;
            CommonLinkUtility = commonLinkUtility;
            MobileDetector = mobileDetector;
            ProviderManager = providerManager;
            Constants = constants;
            Recaptcha = recaptcha;
            ClientFactory = clientFactory;
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new PeopleProduct();
            product.Init();
            return new Module(product);
        }

        [Read]
        public IQueryable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [Read("status/{status}")]
        public IQueryable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreBaseSettings.Personal) throw new Exception("Method not available");
            Guid? groupId = null;
            if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                groupId = new Guid(ApiContext.FilterValue);
                ApiContext.SetDataFiltered();
            }
            return GetFullByFilter(status, groupId, null, null, null);
        }

        [Read("@self")]
        public EmployeeWraper Self()
        {
            return EmployeeWraperFullHelper.GetFull(UserManager.GetUser(SecurityContext.CurrentAccount.ID, EmployeeWraperFullHelper.GetExpression(ApiContext)));
        }

        [Read("email")]
        public EmployeeWraperFull GetByEmail([FromQuery] string email)
        {
            if (CoreBaseSettings.Personal && !UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOwner(Tenant))
                throw new MethodAccessException("Method not available");
            var user = UserManager.GetUserByEmail(email);
            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Read("{username}", order: int.MaxValue)]
        public EmployeeWraperFull GetById(string username)
        {
            if (CoreBaseSettings.Personal) throw new MethodAccessException("Method not available");
            var user = UserManager.GetUserByUserName(username);
            if (user.ID == Constants.LostUser.ID)
            {
                if (Guid.TryParse(username, out var userId))
                {
                    user = UserManager.GetUsers(userId);
                }
                else
                {
                    Log.Error(string.Format("Account {0} сould not get user by name {1}", SecurityContext.CurrentAccount.ID, username));
                }
            }

            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Read("@search/{query}")]
        public IEnumerable<EmployeeWraperFull> GetSearch(string query)
        {
            if (CoreBaseSettings.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var groupId = Guid.Empty;
                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    groupId = new Guid(ApiContext.FilterValue);
                }

                return UserManager.Search(query, EmployeeStatus.Active, groupId).Select(EmployeeWraperFullHelper.GetFull);
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
            return null;
        }

        [Read("search")]
        public IEnumerable<EmployeeWraperFull> GetPeopleSearch([FromQuery] string query)
        {
            return GetSearch(query);
        }

        [Read("status/{status}/search")]
        public IEnumerable<EmployeeWraperFull> GetAdvanced(EmployeeStatus status, [FromQuery] string query)
        {
            if (CoreBaseSettings.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var list = UserManager.GetUsers(status).AsEnumerable();

                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    var groupId = new Guid(ApiContext.FilterValue);
                    //Filter by group
                    list = list.Where(x => UserManager.IsUserInGroup(x.ID, groupId));
                    ApiContext.SetDataFiltered();
                }

                list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                       (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.ContactsList != null && x.ContactsList.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

                return list.Select(EmployeeWraperFullHelper.GetFull);
            }
            catch (Exception error)
            {
                Log.Error(error);
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
        public IQueryable<EmployeeWraperFull> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);
            return users.Select(r => EmployeeWraperFullHelper.GetFull(r));
        }

        [Read("simple/filter")]
        public IEnumerable<EmployeeWraper> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);
            return users.Select(EmployeeWraperHelper.Get);
        }

        private IQueryable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            if (CoreBaseSettings.Personal) throw new MethodAccessException("Method not available");
            var isAdmin = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin(UserManager) ||
                          WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

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

                var products = WebItemManager.GetItemsAll().Where(i => i is IProduct || i.ID == WebItemManager.MailProductID);
                adminGroups.AddRange(products.Select(r => r.ID));

                includeGroups.Add(adminGroups);
            }

            var users = UserManager.GetUsers(isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, ApiContext.FilterValue, ApiContext.SortBy, !ApiContext.SortDescending, ApiContext.Count, ApiContext.StartIndex, out var total, out var count);

            ApiContext.SetTotalCount(total).SetCount(count);

            return users;
        }

        [AllowAnonymous]
        [Create(@"register")]
        public Task<string> RegisterUserOnPersonalAsync(RegisterPersonalUserModel model)
        {
            if (!CoreBaseSettings.Personal) throw new MethodAccessException("Method is only available on personal.onlyoffice.com");

            return InternalRegisterUserOnPersonalAsync(model);
        }

        private async Task<string> InternalRegisterUserOnPersonalAsync(RegisterPersonalUserModel model)
        {
            try
            {
                if (CoreBaseSettings.CustomMode) model.Lang = "ru-RU";

                var cultureInfo = SetupInfo.GetPersonalCulture(model.Lang).Value;

                if (cultureInfo != null)
                {
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                }

                model.Email.ThrowIfNull(new ArgumentException(Resource.ErrorEmailEmpty, "email"));

                if (!model.Email.TestEmailRegex()) throw new ArgumentException(Resource.ErrorNotCorrectEmail, "email");

                if (!SetupInfo.IsSecretEmail(model.Email)
                    && !string.IsNullOrEmpty(SetupInfo.RecaptchaPublicKey) && !string.IsNullOrEmpty(SetupInfo.RecaptchaPrivateKey))
                {
                    var ip = Request.Headers["X-Forwarded-For"].ToString() ?? Request.GetUserHostAddress();

                    if (string.IsNullOrEmpty(model.RecaptchaResponse)
                        || !await Recaptcha.ValidateRecaptchaAsync(model.RecaptchaResponse, ip))
                    {
                        throw new RecaptchaException(Resource.RecaptchaInvalid);
                    }
                }

                var newUserInfo = UserManager.GetUserByEmail(model.Email);

                if (UserManager.UserExists(newUserInfo.ID))
                {
                    if (!SetupInfo.IsSecretEmail(model.Email) || SecurityContext.IsAuthenticated)
                    {
                        StudioNotifyService.SendAlreadyExist(model.Email);
                        return string.Empty;
                    }

                    try
                    {
                        SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                        UserManager.DeleteUser(newUserInfo.ID);
                    }
                    finally
                    {
                        SecurityContext.Logout();
                    }
                }
                if (!model.Spam)
                {
                    try
                    {
                        //TODO
                        //const string _databaseID = "com";
                        //using (var db = DbManager.FromHttpContext(_databaseID))
                        //{
                        //    db.ExecuteNonQuery(new SqlInsert("template_unsubscribe", false)
                        //                           .InColumnValue("email", email.ToLowerInvariant())
                        //                           .InColumnValue("reason", "personal")
                        //        );
                        //    Log.Debug(String.Format("Write to template_unsubscribe {0}", email.ToLowerInvariant()));
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.Debug($"ERROR write to template_unsubscribe {ex.Message}, email:{model.Email.ToLowerInvariant()}");
                    }
                }

                StudioNotifyService.SendInvitePersonal(model.Email);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }

        [Create]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
        public EmployeeWraperFull AddMemberFromBody([FromBody] MemberModel memberModel)
        {
            return AddMember(memberModel);
        }

        [Create]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull AddMemberFromForm([FromForm] MemberModel memberModel)
        {
            return AddMember(memberModel);
        }

        private EmployeeWraperFull AddMember(MemberModel memberModel)
        {
            ApiContext.AuthByClaim();

            PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

            memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
            if (string.IsNullOrEmpty(memberModel.PasswordHash))
            {
                memberModel.Password = (memberModel.Password ?? "").Trim();

                if (string.IsNullOrEmpty(memberModel.Password))
                {
                    memberModel.Password = UserManagerWrapper.GeneratePassword();
                }
                else
                {
                    UserManagerWrapper.CheckPasswordPolicy(memberModel.Password);
                }

                memberModel.PasswordHash = PasswordHasher.GetClientPassword(memberModel.Password);
            }

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

            user.BirthDate = memberModel.Birthday != null && memberModel.Birthday != DateTime.MinValue ? TenantUtil.DateTimeFromUtc(memberModel.Birthday) : null;
            user.WorkFromDate = memberModel.Worksfrom != null && memberModel.Worksfrom != DateTime.MinValue ? TenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : DateTime.UtcNow.Date;

            UpdateContacts(memberModel.Contacts, user);

            user = UserManagerWrapper.AddUser(user, memberModel.PasswordHash, memberModel.FromInviteLink, true, memberModel.IsVisitor, memberModel.FromInviteLink);

            var messageAction = memberModel.IsVisitor ? MessageAction.GuestCreated : MessageAction.UserCreated;
            MessageService.Send(messageAction, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Create("active")]
        public EmployeeWraperFull AddMemberAsActivatedFromBody([FromBody] MemberModel memberModel)
        {
            return AddMemberAsActivated(memberModel);
        }

        [Create("active")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull AddMemberAsActivatedFromForm([FromForm] MemberModel memberModel)
        {
            return AddMemberAsActivated(memberModel);
        }

        private EmployeeWraperFull AddMemberAsActivated(MemberModel memberModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

            var user = new UserInfo();

            memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
            if (string.IsNullOrEmpty(memberModel.PasswordHash))
            {
                memberModel.Password = (memberModel.Password ?? "").Trim();

                if (string.IsNullOrEmpty(memberModel.Password))
                {
                    memberModel.Password = UserManagerWrapper.GeneratePassword();
                }
                else
                {
                    UserManagerWrapper.CheckPasswordPolicy(memberModel.Password);
                }

                memberModel.PasswordHash = PasswordHasher.GetClientPassword(memberModel.Password);
            }

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

            user.BirthDate = memberModel.Birthday != null ? TenantUtil.DateTimeFromUtc(memberModel.Birthday) : null;
            user.WorkFromDate = memberModel.Worksfrom != null ? TenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : DateTime.UtcNow.Date;

            UpdateContacts(memberModel.Contacts, user);

            user = UserManagerWrapper.AddUser(user, memberModel.PasswordHash, false, false, memberModel.IsVisitor);

            user.ActivationStatus = EmployeeActivationStatus.Activated;

            UpdateDepartments(memberModel.Department, user);

            if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
            {
                UpdatePhotoUrl(memberModel.Files, user);
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Update("{userid}/culture")]
        public EmployeeWraperFull UpdateMemberCultureFromBody(string userid, [FromBody] UpdateMemberModel memberModel)
        {
            return UpdateMemberCulture(userid, memberModel);
        }

        [Update("{userid}/culture")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull UpdateMemberCultureFromForm(string userid, [FromForm] UpdateMemberModel memberModel)
        {
            return UpdateMemberCulture(userid, memberModel);
        }

        private EmployeeWraperFull UpdateMemberCulture(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            var curLng = user.CultureName;

            if (SetupInfo.EnabledCultures.Find(c => string.Equals(c.Name, memberModel.CultureName, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                if (curLng != memberModel.CultureName)
                {
                    user.CultureName = memberModel.CultureName;

                    try
                    {
                        UserManager.SaveUserInfo(user);
                    }
                    catch
                    {
                        user.CultureName = curLng;
                        throw;
                    }

                    MessageService.Send(MessageAction.UserUpdatedLanguage, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

                }
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Update("{userid}")]
        public EmployeeWraperFull UpdateMemberFromBody(string userid, [FromBody] UpdateMemberModel memberModel)
        {
            return UpdateMember(userid, memberModel);
        }

        [Update("{userid}")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull UpdateMemberFromForm(string userid, [FromForm] UpdateMemberModel memberModel)
        {
            return UpdateMember(userid, memberModel);
        }

        private EmployeeWraperFull UpdateMember(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

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

            user.BirthDate = memberModel.Birthday != null ? TenantUtil.DateTimeFromUtc(memberModel.Birthday) : user.BirthDate;

            if (user.BirthDate == resetDate)
            {
                user.BirthDate = null;
            }

            user.WorkFromDate = memberModel.Worksfrom != null ? TenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : user.WorkFromDate;

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
                user.TerminatedDate = memberModel.Disable.Value ? DateTime.UtcNow : null;
            }

            if (self && !isAdmin)
            {
                StudioNotifyService.SendMsgToAdminAboutProfileUpdated();
            }

            // change user type
            var canBeGuestFlag = !user.IsOwner(Tenant) && !user.IsAdmin(UserManager) && user.GetListAdminModules(WebItemSecurity).Count == 0 && !user.IsMe(AuthContext);

            if (memberModel.IsVisitor && !user.IsVisitor(UserManager) && canBeGuestFlag)
            {
                UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
                WebItemSecurityCache.ClearCache(Tenant.TenantId);
            }

            if (!self && !memberModel.IsVisitor && user.IsVisitor(UserManager))
            {
                var usersQuota = TenantExtra.GetTenantQuota().ActiveUsers;
                if (TenantStatisticsProvider.GetUsersCount() < usersQuota)
                {
                    UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                    WebItemSecurityCache.ClearCache(Tenant.TenantId);
                }
                else
                {
                    throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", usersQuota));
                }
            }

            UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserUpdated, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            if (memberModel.Disable.HasValue && memberModel.Disable.Value)
            {
                CookiesManager.ResetUserCookie(user.ID);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Delete("{userid}")]
        public EmployeeWraperFull DeleteMember(string userid)
        {
            PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID) || user.IsLDAP())
                throw new SecurityException();

            if (user.Status != EmployeeStatus.Terminated)
                throw new Exception("The user is not suspended");

            CheckReassignProccess(new[] { user.ID });

            var userName = user.DisplayUserName(false, DisplayUserSettingsHelper);

            UserPhotoManager.RemovePhoto(user.ID);
            UserManager.DeleteUser(user.ID);
            QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);

            MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Delete("@self")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "ProfileRemove")]
        public EmployeeWraperFull DeleteProfile()
        {
            ApiContext.AuthByClaim();

            if (UserManager.IsSystemUser(SecurityContext.CurrentAccount.ID))
                throw new SecurityException();

            var user = GetUserInfo(SecurityContext.CurrentAccount.ID.ToString());

            if (!UserManager.UserExists(user))
                throw new Exception(Resource.ErrorUserNotFound);

            if (user.IsLDAP())
                throw new SecurityException();

            SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);

            user.Status = EmployeeStatus.Terminated;

            UserManager.SaveUserInfo(user);

            var userName = user.DisplayUserName(false, DisplayUserSettingsHelper);
            MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(user.ID), userName);

            CookiesManager.ResetUserCookie(user.ID);
            MessageService.Send(MessageAction.CookieSettingsUpdated);

            if (CoreBaseSettings.Personal)
            {
                UserPhotoManager.RemovePhoto(user.ID);
                UserManager.DeleteUser(user.ID);
                MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);
            }
            else
            {
                //StudioNotifyService.Instance.SendMsgProfileHasDeletedItself(user);
                //StudioNotifyService.SendMsgProfileDeletion(Tenant.TenantId, user);
            }

            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Update("{userid}/contacts")]
        public EmployeeWraperFull UpdateMemberContactsFromBody(string userid, [FromBody] UpdateMemberModel memberModel)
        {
            return UpdateMemberContacts(userid, memberModel);
        }

        [Update("{userid}/contacts")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull UpdateMemberContactsFromForm(string userid, [FromForm] UpdateMemberModel memberModel)
        {
            return UpdateMemberContacts(userid, memberModel);
        }

        private EmployeeWraperFull UpdateMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            UpdateContacts(memberModel.Contacts, user);
            UserManager.SaveUserInfo(user);
            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Create("{userid}/contacts")]
        public EmployeeWraperFull SetMemberContactsFromBody(string userid, [FromBody] UpdateMemberModel memberModel)
        {
            return SetMemberContacts(userid, memberModel);
        }

        [Create("{userid}/contacts")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull SetMemberContactsFromForm(string userid, [FromForm] UpdateMemberModel memberModel)
        {
            return SetMemberContacts(userid, memberModel);
        }

        private EmployeeWraperFull SetMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            user.ContactsList.Clear();
            UpdateContacts(memberModel.Contacts, user);
            UserManager.SaveUserInfo(user);
            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Delete("{userid}/contacts")]
        public EmployeeWraperFull DeleteMemberContactsFromBody(string userid, [FromBody] UpdateMemberModel memberModel)
        {
            return DeleteMemberContacts(userid, memberModel);
        }

        [Delete("{userid}/contacts")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull DeleteMemberContactsFromForm(string userid, [FromForm] UpdateMemberModel memberModel)
        {
            return DeleteMemberContacts(userid, memberModel);
        }

        private EmployeeWraperFull DeleteMemberContacts(string userid, UpdateMemberModel memberModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            DeleteContacts(memberModel.Contacts, user);
            UserManager.SaveUserInfo(user);
            return EmployeeWraperFullHelper.GetFull(user);
        }

        [Read("{userid}/photo")]
        public ThumbnailsDataWrapper GetMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            return new ThumbnailsDataWrapper(user.ID, UserPhotoManager);
        }

        [Create("{userid}/photo")]
        public FileUploadResult UploadMemberPhoto(string userid, IFormCollection model)
        {
            var result = new People.Models.FileUploadResult();
            var autosave = bool.Parse(model["Autosave"]);

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

                    PermissionContext.DemandPermissions(new UserSecurityProvider(userId), Constants.Action_EditUser);

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

                    if (autosave)
                    {
                        if (data.Length > SetupInfo.MaxImageUploadSize)
                            throw new ImageSizeLimitException();

                        var mainPhoto = UserPhotoManager.SaveOrUpdatePhoto(userId, data);

                        result.Data =
                            new
                            {
                                main = mainPhoto,
                                retina = UserPhotoManager.GetRetinaPhotoURL(userId),
                                max = UserPhotoManager.GetMaxPhotoURL(userId),
                                big = UserPhotoManager.GetBigPhotoURL(userId),
                                medium = UserPhotoManager.GetMediumPhotoURL(userId),
                                small = UserPhotoManager.GetSmallPhotoURL(userId),
                            };
                    }
                    else
                    {
                        result.Data = UserPhotoManager.SaveTempPhoto(data, SetupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = PeopleResource.ErrorEmptyUploadFileSelected;
                }

            }
            catch (Web.Core.Users.UnknownImageFormatException)
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
        public ThumbnailsDataWrapper UpdateMemberPhotoFromBody(string userid, [FromBody] UpdateMemberModel model)
        {
            return UpdateMemberPhoto(userid, model);
        }

        [Update("{userid}/photo")]
        [Consumes("application/x-www-form-urlencoded")]
        public ThumbnailsDataWrapper UpdateMemberPhotoFromForm(string userid, [FromForm] UpdateMemberModel model)
        {
            return UpdateMemberPhoto(userid, model);
        }

        private ThumbnailsDataWrapper UpdateMemberPhoto(string userid, UpdateMemberModel model)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (model.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(user.ID))
            {
                UpdatePhotoUrl(model.Files, user);
            }

            UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserAddedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            return new ThumbnailsDataWrapper(user.ID, UserPhotoManager);
        }

        [Delete("{userid}/photo")]
        public ThumbnailsDataWrapper DeleteMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            UserPhotoManager.RemovePhoto(user.ID);

            UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserDeletedAvatar, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            return new ThumbnailsDataWrapper(user.ID, UserPhotoManager);
        }


        [Create("{userid}/photo/thumbnails")]
        public ThumbnailsDataWrapper CreateMemberPhotoThumbnailsFromBody(string userid, [FromBody] ThumbnailsModel thumbnailsModel)
        {
            return CreateMemberPhotoThumbnails(userid, thumbnailsModel);
        }

        [Create("{userid}/photo/thumbnails")]
        [Consumes("application/x-www-form-urlencoded")]
        public ThumbnailsDataWrapper CreateMemberPhotoThumbnailsFromForm(string userid, [FromForm] ThumbnailsModel thumbnailsModel)
        {
            return CreateMemberPhotoThumbnails(userid, thumbnailsModel);
        }

        private ThumbnailsDataWrapper CreateMemberPhotoThumbnails(string userid, ThumbnailsModel thumbnailsModel)
        {
            var user = GetUserInfo(userid);

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!string.IsNullOrEmpty(thumbnailsModel.TmpFile))
            {
                var fileName = Path.GetFileName(thumbnailsModel.TmpFile);
                var data = UserPhotoManager.GetTempPhotoData(fileName);

                var settings = new UserPhotoThumbnailSettings(thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height);
                SettingsManager.SaveForUser(settings, user.ID);

                UserPhotoManager.RemovePhoto(user.ID);
                UserPhotoManager.SaveOrUpdatePhoto(user.ID, data);
                UserPhotoManager.RemoveTempPhoto(fileName);
            }
            else
            {
                UserPhotoThumbnailManager.SaveThumbnails(UserPhotoManager, SettingsManager, thumbnailsModel.X, thumbnailsModel.Y, thumbnailsModel.Width, thumbnailsModel.Height, user.ID);
            }

            UserManager.SaveUserInfo(user);
            MessageService.Send(MessageAction.UserUpdatedAvatarThumbnails, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            return new ThumbnailsDataWrapper(user.ID, UserPhotoManager);
        }


        [AllowAnonymous]
        [Create("password", false)]
        public object SendUserPasswordFromBody([FromBody] MemberModel memberModel)
        {
            return SendUserPassword(memberModel);
        }

        [AllowAnonymous]
        [Create("password", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendUserPasswordFromForm([FromForm] MemberModel memberModel)
        {
            return SendUserPassword(memberModel);
        }

        private object SendUserPassword(MemberModel memberModel)
        {
            string error = UserManagerWrapper.SendUserPassword(memberModel.Email);
            if (!string.IsNullOrEmpty(error))
            {
                Log.ErrorFormat("Password recovery ({0}): {1}", memberModel.Email, error);
            }

            return string.Format(Resource.MessageYourPasswordSendedToEmail, memberModel.Email);
        }

        [Update("{userid}/password")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
        public EmployeeWraperFull ChangeUserPasswordFromBody(Guid userid, [FromBody] MemberModel memberModel)
        {
            return ChangeUserPassword(userid, memberModel);
        }

        [Update("{userid}/password")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmployeeWraperFull ChangeUserPasswordFromForm(Guid userid, [FromForm] MemberModel memberModel)
        {
            return ChangeUserPassword(userid, memberModel);
        }

        private EmployeeWraperFull ChangeUserPassword(Guid userid, MemberModel memberModel)
        {
            ApiContext.AuthByClaim();
            PermissionContext.DemandPermissions(new UserSecurityProvider(userid), Constants.Action_EditUser);

            var user = UserManager.GetUsers(userid);

            if (!UserManager.UserExists(user)) return null;

            if (UserManager.IsSystemUser(user.ID))
                throw new SecurityException();

            if (!string.IsNullOrEmpty(memberModel.Email))
            {
                var address = new MailAddress(memberModel.Email);
                if (!string.Equals(address.Address, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    user.Email = address.Address.ToLowerInvariant();
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    UserManager.SaveUserInfo(user);
                }
            }

            memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
            if (string.IsNullOrEmpty(memberModel.PasswordHash))
            {
                memberModel.Password = (memberModel.Password ?? "").Trim();

                if (!string.IsNullOrEmpty(memberModel.Password))
                {
                    memberModel.PasswordHash = PasswordHasher.GetClientPassword(memberModel.Password);
                }
            }

            if (!string.IsNullOrEmpty(memberModel.PasswordHash))
            {
                SecurityContext.SetUserPasswordHash(userid, memberModel.PasswordHash);
                MessageService.Send(MessageAction.UserUpdatedPassword);

                CookiesManager.ResetUserCookie(userid);
                MessageService.Send(MessageAction.CookieSettingsUpdated);
            }

            return EmployeeWraperFullHelper.GetFull(GetUserInfo(userid.ToString()));
        }


        [Create("email", false)]
        public object SendEmailChangeInstructionsFromBody([FromBody] UpdateMemberModel model)
        {
            return SendEmailChangeInstructions(model);
        }

        [Create("email", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendEmailChangeInstructionsFromForm([FromForm] UpdateMemberModel model)
        {
            return SendEmailChangeInstructions(model);
        }

        private object SendEmailChangeInstructions(UpdateMemberModel model)
        {
            Guid.TryParse(model.UserId, out var userid);

            if (userid == Guid.Empty) throw new ArgumentNullException("userid");

            var email = (model.Email ?? "").Trim();

            if (string.IsNullOrEmpty(email)) throw new Exception(Resource.ErrorEmailEmpty);

            if (!email.TestEmailRegex()) throw new Exception(Resource.ErrorNotCorrectEmail);

            var viewer = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            var user = UserManager.GetUsers(userid);

            if (user == null)
                throw new Exception(Resource.ErrorUserNotFound);

            if (viewer == null || (user.IsOwner(Tenant) && viewer.ID != user.ID))
                throw new Exception(Resource.ErrorAccessDenied);

            var existentUser = UserManager.GetUserByEmail(email);

            if (existentUser.ID != Constants.LostUser.ID)
                throw new Exception(CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));

            if (!viewer.IsAdmin(UserManager))
            {
                StudioNotifyService.SendEmailChangeInstructions(user, email);
            }
            else
            {
                if (email == user.Email)
                    throw new Exception(Resource.ErrorEmailsAreTheSame);

                user.Email = email;
                user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                UserManager.SaveUserInfo(user);
                StudioNotifyService.SendEmailActivationInstructions(user, email);
            }

            MessageService.Send(MessageAction.UserSentEmailChangeInstructions, user.DisplayUserName(false, DisplayUserSettingsHelper));

            return string.Format(Resource.MessageEmailChangeInstuctionsSentOnEmail, email);
        }

        private UserInfo GetUserInfo(string userNameOrId)
        {
            UserInfo user;
            try
            {
                var userId = new Guid(userNameOrId);
                user = UserManager.GetUsers(userId);
            }
            catch (FormatException)
            {
                user = UserManager.GetUserByUserName(userNameOrId);
            }
            if (user == null || user.ID == Constants.LostUser.ID)
                throw new ItemNotFoundException("user not found");
            return user;
        }

        [Update("activationstatus/{activationstatus}")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
        public IEnumerable<EmployeeWraperFull> UpdateEmployeeActivationStatusFromBody(EmployeeActivationStatus activationstatus, [FromBody] UpdateMembersModel model)
        {
            return UpdateEmployeeActivationStatus(activationstatus, model);
        }

        [Update("activationstatus/{activationstatus}")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<EmployeeWraperFull> UpdateEmployeeActivationStatusFromForm(EmployeeActivationStatus activationstatus, [FromForm] UpdateMembersModel model)
        {
            return UpdateEmployeeActivationStatus(activationstatus, model);
        }

        private IEnumerable<EmployeeWraperFull> UpdateEmployeeActivationStatus(EmployeeActivationStatus activationstatus, UpdateMembersModel model)
        {
            ApiContext.AuthByClaim();

            var retuls = new List<EmployeeWraperFull>();
            foreach (var id in model.UserIds.Where(userId => !UserManager.IsSystemUser(userId)))
            {
                PermissionContext.DemandPermissions(new UserSecurityProvider(id), Constants.Action_EditUser);
                var u = UserManager.GetUsers(id);
                if (u.ID == Constants.LostUser.ID || u.IsLDAP()) continue;

                u.ActivationStatus = activationstatus;
                UserManager.SaveUserInfo(u);
                retuls.Add(EmployeeWraperFullHelper.GetFull(u));
            }

            return retuls;
        }

        [Update("type/{type}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserTypeFromBody(EmployeeType type, [FromBody] UpdateMembersModel model)
        {
            return UpdateUserType(type, model);
        }

        [Update("type/{type}")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<EmployeeWraperFull> UpdateUserTypeFromForm(EmployeeType type, [FromForm] UpdateMembersModel model)
        {
            return UpdateUserType(type, model);
        }

        private IEnumerable<EmployeeWraperFull> UpdateUserType(EmployeeType type, UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !UserManager.IsSystemUser(userId))
                .Select(userId => UserManager.GetUsers(userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner(Tenant) || user.IsAdmin(UserManager) || user.IsMe(AuthContext) || user.GetListAdminModules(WebItemSecurity).Count > 0)
                    continue;

                switch (type)
                {
                    case EmployeeType.User:
                        if (user.IsVisitor(UserManager))
                        {
                            if (TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers)
                            {
                                UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                                WebItemSecurityCache.ClearCache(Tenant.TenantId);
                            }
                        }
                        break;
                    case EmployeeType.Visitor:
                        if (CoreBaseSettings.Standalone || TenantStatisticsProvider.GetVisitorsCount() < TenantExtra.GetTenantQuota().ActiveUsers * Constants.CoefficientOfVisitors)
                        {
                            UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
                            WebItemSecurityCache.ClearCache(Tenant.TenantId);
                        }
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedType, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

            return users.Select(EmployeeWraperFullHelper.GetFull);
        }

        [Update("status/{status}")]
        public IEnumerable<EmployeeWraperFull> UpdateUserStatusFromBody(EmployeeStatus status, [FromBody] UpdateMembersModel model)
        {
            return UpdateUserStatus(status, model);
        }

        [Update("status/{status}")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<EmployeeWraperFull> UpdateUserStatusFromForm(EmployeeStatus status, [FromForm] UpdateMembersModel model)
        {
            return UpdateUserStatus(status, model);
        }

        private IEnumerable<EmployeeWraperFull> UpdateUserStatus(EmployeeStatus status, UpdateMembersModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            var users = model.UserIds.Select(userId => UserManager.GetUsers(userId))
                .Where(u => !UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            foreach (var user in users)
            {
                if (user.IsOwner(Tenant) || user.IsMe(AuthContext))
                    continue;

                switch (status)
                {
                    case EmployeeStatus.Active:
                        if (user.Status == EmployeeStatus.Terminated)
                        {
                            if (TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers || user.IsVisitor(UserManager))
                            {
                                user.Status = EmployeeStatus.Active;
                                UserManager.SaveUserInfo(user);
                            }
                        }
                        break;
                    case EmployeeStatus.Terminated:
                        user.Status = EmployeeStatus.Terminated;
                        UserManager.SaveUserInfo(user);

                        CookiesManager.ResetUserCookie(user.ID);
                        MessageService.Send(MessageAction.CookieSettingsUpdated);
                        break;
                }
            }

            MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

            return users.Select(EmployeeWraperFullHelper.GetFull);
        }


        [Update("invite")]
        public IEnumerable<EmployeeWraperFull> ResendUserInvitesFromBody([FromBody] UpdateMembersModel model)
        {
            return ResendUserInvites(model);
        }

        [Update("invite")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<EmployeeWraperFull> ResendUserInvitesFromForm([FromForm] UpdateMembersModel model)
        {
            return ResendUserInvites(model);
        }

        private IEnumerable<EmployeeWraperFull> ResendUserInvites(UpdateMembersModel model)
        {
            var users = model.UserIds
                .Where(userId => !UserManager.IsSystemUser(userId))
                .Select(userId => UserManager.GetUsers(userId))
                .ToList();

            foreach (var user in users)
            {
                if (user.IsActive) continue;
                var viewer = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                if (viewer == null) throw new Exception(Resource.ErrorAccessDenied);

                if (viewer.IsAdmin(UserManager) || viewer.ID == user.ID)
                {
                    if (user.ActivationStatus == EmployeeActivationStatus.Activated)
                    {
                        user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                    }
                    if (user.ActivationStatus == (EmployeeActivationStatus.AutoGenerated | EmployeeActivationStatus.Activated))
                    {
                        user.ActivationStatus = EmployeeActivationStatus.AutoGenerated;
                    }
                    UserManager.SaveUserInfo(user);
                }

                if (user.ActivationStatus == EmployeeActivationStatus.Pending)
                {
                    if (user.IsVisitor(UserManager))
                    {
                        StudioNotifyService.GuestInfoActivation(user);
                    }
                    else
                    {
                        StudioNotifyService.UserInfoActivation(user);
                    }
                }
                else
                {
                    StudioNotifyService.SendEmailActivationInstructions(user, user.Email);
                }
            }

            MessageService.Send(MessageAction.UsersSentActivationInstructions, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

            return users.Select(EmployeeWraperFullHelper.GetFull);
        }

        [Update("delete", Order = -1)]
        public IEnumerable<EmployeeWraperFull> RemoveUsersFromBody([FromBody] UpdateMembersModel model)
        {
            return RemoveUsers(model);
        }

        [Update("delete", Order = -1)]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<EmployeeWraperFull> RemoveUsersFromForm([FromForm] UpdateMembersModel model)
        {
            return RemoveUsers(model);
        }

        private IEnumerable<EmployeeWraperFull> RemoveUsers(UpdateMembersModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

            CheckReassignProccess(model.UserIds);

            var users = model.UserIds.Select(userId => UserManager.GetUsers(userId))
                .Where(u => !UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
                .ToList();

            var userNames = users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)).ToList();

            foreach (var user in users)
            {
                if (user.Status != EmployeeStatus.Terminated) continue;

                UserPhotoManager.RemovePhoto(user.ID);
                UserManager.DeleteUser(user.ID);
                QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);
            }

            MessageService.Send(MessageAction.UsersDeleted, MessageTarget.Create(users.Select(x => x.ID)), userNames);

            return users.Select(EmployeeWraperFullHelper.GetFull);
        }


        [Update("self/delete")]
        public object SendInstructionsToDelete()
        {
            var user = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            if (user.IsLDAP())
                throw new SecurityException();

            StudioNotifyService.SendMsgProfileDeletion(user);
            MessageService.Send(MessageAction.UserSentDeleteInstructions);

            return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
        }

        [AllowAnonymous]
        [Read("thirdparty/providers")]
        public ICollection<AccountInfo> GetAuthProviders(bool inviteView, bool settingsView, string clientCallback, string fromOnly)
        {
            ICollection<AccountInfo> infos = new List<AccountInfo>();
            IEnumerable<LoginProfile> linkedAccounts = new List<LoginProfile>();

            if (AuthContext.IsAuthenticated)
            {
                linkedAccounts = AccountLinker.Get("webstudio").GetLinkedProfiles(AuthContext.CurrentAccount.ID.ToString());
            }

            fromOnly = string.IsNullOrWhiteSpace(fromOnly) ? string.Empty : fromOnly.ToLower();

            foreach (var provider in ProviderManager.AuthProviders.Where(provider => string.IsNullOrEmpty(fromOnly) || fromOnly == provider || (provider == "google" && fromOnly == "openid")))
            {
                if (inviteView && provider.Equals("twitter", StringComparison.OrdinalIgnoreCase)) continue;

                var loginProvider = ProviderManager.GetLoginProvider(provider);
                if (loginProvider != null && loginProvider.IsEnabled)
                {

                    var url = VirtualPathUtility.ToAbsolute("~/login.ashx") + $"?auth={provider}";
                    var mode = settingsView || inviteView || (!MobileDetector.IsMobile() && !Request.DesktopApp())
                            ? $"&mode=popup&callback={clientCallback}"
                            : "&mode=Redirect&desktop=true";

                    infos.Add(new AccountInfo
                    {
                        Linked = linkedAccounts.Any(x => x.Provider == provider),
                        Provider = provider,
                        Url = url + mode
                    });
                }
            }

            return infos;
        }


        [Update("thirdparty/linkaccount")]
        public void LinkAccountFromBody([FromBody] LinkAccountModel model)
        {
            LinkAccount(model);
        }

        [Update("thirdparty/linkaccount")]
        [Consumes("application/x-www-form-urlencoded")]
        public void LinkAccountFromForm([FromForm] LinkAccountModel model)
        {
            LinkAccount(model);
        }

        public void LinkAccount(LinkAccountModel model)
        {
            var profile = new LoginProfile(Signature, InstanceCrypto, model.SerializedProfile);

            if (!(CoreBaseSettings.Standalone || TenantExtra.GetTenantQuota().Oauth))
            {
                throw new Exception("ErrorNotAllowedOption");
            }

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

        [AllowAnonymous]
        [Create("thirdparty/signup")]
        public void SignupAccountFromBody([FromBody] SignupAccountModel model)
        {
            SignupAccount(model);
        }

        [AllowAnonymous]
        [Create("thirdparty/signup")]
        [Consumes("application/x-www-form-urlencoded")]
        public void SignupAccountFromForm([FromForm] SignupAccountModel model)
        {
            SignupAccount(model);
        }

        public void SignupAccount(SignupAccountModel model)
        {
            var employeeType = model.EmplType ?? EmployeeType.User;
            var passwordHash = model.PasswordHash;
            var mustChangePassword = false;

            if (string.IsNullOrEmpty(passwordHash))
            {
                passwordHash = UserManagerWrapper.GeneratePassword();
                mustChangePassword = true;
            }

            var thirdPartyProfile = new LoginProfile(Signature, InstanceCrypto, model.SerializedProfile);
            if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
            {
                // ignore cancellation
                if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
                    throw new Exception(thirdPartyProfile.AuthorizationError);

                return;
            }

            if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
            {
                throw new Exception(Resource.ErrorNotCorrectEmail);
            }

            var userID = Guid.Empty;
            try
            {
                SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                var newUser = CreateNewUser(GetFirstName(model, thirdPartyProfile), GetLastName(model, thirdPartyProfile), GetEmailAddress(model, thirdPartyProfile), passwordHash, employeeType, false);

                var messageAction = employeeType == EmployeeType.User ? MessageAction.UserCreatedViaInvite : MessageAction.GuestCreatedViaInvite;
                MessageService.Send(MessageInitiator.System, messageAction, MessageTarget.Create(newUser.ID), newUser.DisplayUserName(false, DisplayUserSettingsHelper));

                userID = newUser.ID;
                if (!string.IsNullOrEmpty(thirdPartyProfile.Avatar))
                {
                    SaveContactImage(userID, thirdPartyProfile.Avatar);
                }

                GetLinker().AddLink(userID.ToString(), thirdPartyProfile);
            }
            finally
            {
                SecurityContext.Logout();
            }

            var user = UserManager.GetUsers(userID);
            var cookiesKey = SecurityContext.AuthenticateMe(user.Email, passwordHash);
            CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            MessageService.Send(MessageAction.LoginSuccess);
            StudioNotifyService.UserHasJoin();

            if (mustChangePassword)
            {
                StudioNotifyService.UserPasswordChange(user);
            }

            UserHelpTourHelper.IsNewUser = true;
            if (CoreBaseSettings.Personal)
                PersonalSettingsHelper.IsNewUser = true;

        }

        [Create("phone")]
        public object SendNotificationToChangeFromBody([FromBody] UpdateMemberModel model)
        {
            return SendNotificationToChange(model.UserId);
        }

        [Create("phone")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendNotificationToChangeFromForm([FromForm] UpdateMemberModel model)
        {
            return SendNotificationToChange(model.UserId);
        }

        public object SendNotificationToChange(string userId)
        {
            var user = UserManager.GetUsers(
                string.IsNullOrEmpty(userId)
                    ? SecurityContext.CurrentAccount.ID
                    : new Guid(userId));

            var canChange =
                user.IsMe(AuthContext)
                || PermissionContext.CheckPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!canChange)
                throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);

            user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
            UserManager.SaveUserInfo(user);

            if (user.IsMe(AuthContext))
            {
                return CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation);
            }

            StudioNotifyService.SendMsgMobilePhoneChange(user);
            return string.Empty;
        }

        protected string GetEmailAddress(SignupAccountModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
                return model.Email.Trim();

            return string.Empty;
        }

        private string GetEmailAddress(SignupAccountModel model, LoginProfile account)
        {
            var value = GetEmailAddress(model);
            return string.IsNullOrEmpty(value) ? account.EMail : value;
        }

        protected string GetFirstName(SignupAccountModel model)
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(model.FirstName)) value = model.FirstName.Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetFirstName(SignupAccountModel model, LoginProfile account)
        {
            var value = GetFirstName(model);
            return string.IsNullOrEmpty(value) ? account.FirstName : value;
        }

        protected string GetLastName(SignupAccountModel model)
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(model.LastName)) value = model.LastName.Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetLastName(SignupAccountModel model, LoginProfile account)
        {
            var value = GetLastName(model);
            return string.IsNullOrEmpty(value) ? account.LastName : value;
        }

        private UserInfo CreateNewUser(string firstName, string lastName, string email, string passwordHash, EmployeeType employeeType, bool fromInviteLink)
        {
            var isVisitor = employeeType == EmployeeType.Visitor;

            if (SetupInfo.IsSecretEmail(email))
            {
                fromInviteLink = false;
            }

            var userInfo = new UserInfo
            {
                FirstName = string.IsNullOrEmpty(firstName) ? UserControlsCommonResource.UnknownFirstName : firstName,
                LastName = string.IsNullOrEmpty(lastName) ? UserControlsCommonResource.UnknownLastName : lastName,
                Email = email,
            };

            if (CoreBaseSettings.Personal)
            {
                userInfo.ActivationStatus = EmployeeActivationStatus.Activated;
                userInfo.CultureName = CoreBaseSettings.CustomMode ? "ru-RU" : Thread.CurrentThread.CurrentUICulture.Name;
            }

            return UserManagerWrapper.AddUser(userInfo, passwordHash, true, true, isVisitor, fromInviteLink);
        }

        private void SaveContactImage(Guid userID, string url)
        {
            using (var memstream = new MemoryStream())
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);

                var httpClient = ClientFactory.CreateClient();
                using (var response = httpClient.Send(request))
                using (var stream = response.Content.ReadAsStream())
                {
                    var buffer = new byte[512];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        memstream.Write(buffer, 0, bytesRead);
                    var bytes = memstream.ToArray();

                    UserPhotoManager.SaveOrUpdatePhoto(userID, bytes);
                }
            }
        }

        private AccountLinker GetLinker()
        {
            return AccountLinker.Get("webstudio");
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
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            return QueueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
        }

        [Update(@"reassign/terminate")]
        public void TerminateReassignFromBody([FromBody] TerminateModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerReassign.Terminate(Tenant.TenantId, model.UserId);
        }

        [Update(@"reassign/terminate")]
        [Consumes("application/x-www-form-urlencoded")]
        public void TerminateReassignFromForm([FromForm] TerminateModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerReassign.Terminate(Tenant.TenantId, model.UserId);
        }

        [Create(@"reassign/start")]
        public ReassignProgressItem StartReassignFromBody([FromBody] StartReassignModel model)
        {
            return StartReassign(model);
        }

        [Create(@"reassign/start")]
        [Consumes("application/x-www-form-urlencoded")]
        public ReassignProgressItem StartReassignFromForm([FromForm] StartReassignModel model)
        {
            return StartReassign(model);
        }

        private ReassignProgressItem StartReassign(StartReassignModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            var fromUser = UserManager.GetUsers(model.FromUserId);

            if (fromUser == null || fromUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + model.FromUserId + " not found");

            if (fromUser.IsOwner(Tenant) || fromUser.IsMe(AuthContext) || fromUser.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + model.FromUserId);

            var toUser = UserManager.GetUsers(model.ToUserId);

            if (toUser == null || toUser.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + model.ToUserId + " not found");

            if (toUser.IsVisitor(UserManager) || toUser.Status == EmployeeStatus.Terminated)
                throw new ArgumentException("Can not reassign data to user with id = " + model.ToUserId);

            return QueueWorkerReassign.Start(Tenant.TenantId, model.FromUserId, model.ToUserId, SecurityContext.CurrentAccount.ID, model.DeleteProfile);
        }

        private void CheckReassignProccess(IEnumerable<Guid> userIds)
        {
            foreach (var userId in userIds)
            {
                var reassignStatus = QueueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
                if (reassignStatus == null || reassignStatus.IsCompleted)
                    continue;

                var userName = UserManager.GetUsers(userId).DisplayUserName(DisplayUserSettingsHelper);
                throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
            }
        }

        //#endregion

        #region Remove user data


        [Read(@"remove/progress")]
        public RemoveProgressItem GetRemoveProgress(Guid userId)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            return QueueWorkerRemove.GetProgressItemStatus(Tenant.TenantId, userId);
        }

        [Update(@"remove/terminate")]
        public void TerminateRemoveFromBody([FromBody] TerminateModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerRemove.Terminate(Tenant.TenantId, model.UserId);
        }

        [Update(@"remove/terminate")]
        [Consumes("application/x-www-form-urlencoded")]
        public void TerminateRemoveFromForm([FromForm] TerminateModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            QueueWorkerRemove.Terminate(Tenant.TenantId, model.UserId);
        }

        [Create(@"remove/start")]
        public RemoveProgressItem StartRemoveFromBody([FromBody] TerminateModel model)
        {
            return StartRemove(model);
        }

        [Create(@"remove/start")]
        [Consumes("application/x-www-form-urlencoded")]
        public RemoveProgressItem StartRemoveFromForm([FromForm] TerminateModel model)
        {
            return StartRemove(model);
        }

        private RemoveProgressItem StartRemove(TerminateModel model)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditUser);

            var user = UserManager.GetUsers(model.UserId);

            if (user == null || user.ID == Constants.LostUser.ID)
                throw new ArgumentException("User with id = " + model.UserId + " not found");

            if (user.IsOwner(Tenant) || user.IsMe(AuthContext) || user.Status != EmployeeStatus.Terminated)
                throw new ArgumentException("Can not delete user with id = " + model.UserId);

            return QueueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, true);
        }

        #endregion

        private void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
        {
            if (!PermissionContext.CheckPermissions(Constants.Action_EditGroups)) return;
            if (department == null) return;

            var groups = UserManager.GetUserGroups(user.ID);
            var managerGroups = new List<Guid>();
            foreach (var groupInfo in groups)
            {
                UserManager.RemoveUserFromGroup(user.ID, groupInfo.ID);
                var managerId = UserManager.GetDepartmentManager(groupInfo.ID);
                if (managerId == user.ID)
                {
                    managerGroups.Add(groupInfo.ID);
                    UserManager.SetDepartmentManager(groupInfo.ID, Guid.Empty);
                }
            }
            foreach (var guid in department)
            {
                var userDepartment = UserManager.GetGroupInfo(guid);
                if (userDepartment != Constants.LostGroupInfo)
                {
                    UserManager.AddUserIntoGroup(user.ID, guid);
                    if (managerGroups.Contains(guid))
                    {
                        UserManager.SetDepartmentManager(guid, user.ID);
                    }
                }
            }
        }

        private void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (contacts == null) return;
            var values = contacts.Where(r => !string.IsNullOrEmpty(r.Value)).Select(r => $"{r.Type}|{r.Value}");
            user.Contacts = string.Join('|', values);
        }

        private void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);
            if (contacts == null) return;

            if (user.ContactsList == null)
            {
                user.ContactsList = new List<string>();
            }

            foreach (var contact in contacts)
            {
                var index = user.ContactsList.IndexOf(contact.Type);
                if (index != -1)
                {
                    //Remove existing
                    user.ContactsList.RemoveRange(index, 2);
                }
            }
        }

        private void UpdatePhotoUrl(string files, UserInfo user)
        {
            if (string.IsNullOrEmpty(files))
            {
                return;
            }

            PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

            if (!files.StartsWith("http://") && !files.StartsWith("https://"))
            {
                files = new Uri(ApiContext.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority) + "/" + files.TrimStart('/');
            }
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(files);

            var httpClient = ClientFactory.CreateClient();
            using var response = httpClient.Send(request);
            using var inputStream = response.Content.ReadAsStream();
            using var br = new BinaryReader(inputStream);
            var imageByteArray = br.ReadBytes((int)inputStream.Length);
            UserPhotoManager.SaveOrUpdatePhoto(user.ID, imageByteArray);
        }

        private static void CheckImgFormat(byte[] data)
        {
            IImageFormat imgFormat;
            try
            {
                using var img = Image.Load(data, out var format);
                imgFormat = format;
            }
            catch (OutOfMemoryException)
            {
                throw new ImageSizeLimitException();
            }
            catch (ArgumentException error)
            {
                throw new Web.Core.Users.UnknownImageFormatException(error);
            }

            if (imgFormat.Name != "PNG" && imgFormat.Name != "JPEG")
            {
                throw new Web.Core.Users.UnknownImageFormatException();
            }
        }
    }
}
