/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security;
using System.ServiceModel.Security;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using ASC.Api.Collections;
using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Notify;
using ASC.Core.Common.Settings;
using ASC.Core.Encryption;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Backup;
using ASC.Data.Backup.Contracts;
using ASC.Data.Storage;
using ASC.Data.Storage.Configuration;
using ASC.Data.Storage.Encryption;
using ASC.Data.Storage.Migration;
using ASC.FederatedLogin.LoginProviders;
using ASC.IPSecurity;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Sms;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.WebZones;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Quota;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Core.Statistic;
using ASC.Web.Studio.Core.TFA;
using ASC.Web.Studio.UserControls.CustomNavigation;
using ASC.Web.Studio.UserControls.FirstTime;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using ASC.Webhooks.Core;
using ASC.Webhooks.Core.Dao.Models;

using Google.Authenticator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Api.Settings
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public partial class SettingsController : ControllerBase
    {
        //private const int ONE_THREAD = 1;

        //private static readonly DistributedTaskQueue quotaTasks = new DistributedTaskQueue("quotaOperations", ONE_THREAD);
        //private static DistributedTaskQueue LDAPTasks { get; } = new DistributedTaskQueue("ldapOperations");
        //private static DistributedTaskQueue SMTPTasks { get; } = new DistributedTaskQueue("smtpOperations");
        public Tenant Tenant { get { return ApiContext.Tenant; } }
        public ApiContext ApiContext { get; }
        private MessageService MessageService { get; }
        private StudioNotifyService StudioNotifyService { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }
        private IServiceProvider ServiceProvider { get; }
        private EmployeeWraperHelper EmployeeWraperHelper { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private SmsProviderManager SmsProviderManager { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        private CustomNamingPeople CustomNamingPeople { get; }
        private IPSecurity.IPSecurity IpSecurity { get; }
        private IMemoryCache MemoryCache { get; }
        private ProviderManager ProviderManager { get; }
        private FirstTimeTenantSettings FirstTimeTenantSettings { get; }
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private TenantExtra TenantExtra { get; }
        private TenantStatisticsProvider TenantStatisticsProvider { get; }
        private AuthContext AuthContext { get; }
        private CookiesManager CookiesManager { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private LicenseReader LicenseReader { get; }
        private PermissionContext PermissionContext { get; }
        private SettingsManager SettingsManager { get; }
        private TfaManager TfaManager { get; }
        private WebItemManager WebItemManager { get; }
        private WebItemManagerSecurity WebItemManagerSecurity { get; }
        private TenantInfoSettingsHelper TenantInfoSettingsHelper { get; }
        private TenantWhiteLabelSettingsHelper TenantWhiteLabelSettingsHelper { get; }
        private StorageHelper StorageHelper { get; }
        private TenantLogoManager TenantLogoManager { get; }
        private TenantUtil TenantUtil { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private ColorThemesSettingsHelper ColorThemesSettingsHelper { get; }
        private IConfiguration Configuration { get; }
        private SetupInfo SetupInfo { get; }
        private BuildVersion BuildVersion { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private StatisticManager StatisticManager { get; }
        private IPRestrictionsService IPRestrictionsService { get; }
        private CoreConfiguration CoreConfiguration { get; }
        private MessageTarget MessageTarget { get; }
        private StudioSmsNotificationSettingsHelper StudioSmsNotificationSettingsHelper { get; }
        private CoreSettings CoreSettings { get; }
        private StorageSettingsHelper StorageSettingsHelper { get; }
        private ServiceClient ServiceClient { get; }
        private StorageFactory StorageFactory { get; }
        private UrlShortener UrlShortener { get; }
        private EncryptionServiceClient EncryptionServiceClient { get; }
        private EncryptionSettingsHelper EncryptionSettingsHelper { get; }
        private BackupAjaxHandler BackupAjaxHandler { get; }
        private ICacheNotify<DeleteSchedule> CacheDeleteSchedule { get; }
        private EncryptionWorker EncryptionWorker { get; }
        private PasswordHasher PasswordHasher { get; }
        private ILog Log { get; set; }
        private TelegramHelper TelegramHelper { get; }
        private PaymentManager PaymentManager { get; }
        private Constants Constants { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private Signature Signature { get; }
        private DbWorker WebhookDbWorker { get; }
        public IHttpClientFactory ClientFactory { get; }

        public SettingsController(
            IOptionsMonitor<ILog> option,
            MessageService messageService,
            StudioNotifyService studioNotifyService,
            ApiContext apiContext,
            UserManager userManager,
            TenantManager tenantManager,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticsProvider,
            AuthContext authContext,
            CookiesManager cookiesManager,
            WebItemSecurity webItemSecurity,
            StudioNotifyHelper studioNotifyHelper,
            LicenseReader licenseReader,
            PermissionContext permissionContext,
            SettingsManager settingsManager,
            TfaManager tfaManager,
            WebItemManager webItemManager,
            WebItemManagerSecurity webItemManagerSecurity,
            TenantInfoSettingsHelper tenantInfoSettingsHelper,
            TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
            StorageHelper storageHelper,
            TenantLogoManager tenantLogoManager,
            TenantUtil tenantUtil,
            CoreBaseSettings coreBaseSettings,
            CommonLinkUtility commonLinkUtility,
            ColorThemesSettingsHelper colorThemesSettingsHelper,
            IConfiguration configuration,
            SetupInfo setupInfo,
            BuildVersion buildVersion,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            StatisticManager statisticManager,
            IPRestrictionsService iPRestrictionsService,
            CoreConfiguration coreConfiguration,
            MessageTarget messageTarget,
            StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
            CoreSettings coreSettings,
            StorageSettingsHelper storageSettingsHelper,
            IWebHostEnvironment webHostEnvironment,
            IServiceProvider serviceProvider,
            EmployeeWraperHelper employeeWraperHelper,
            ConsumerFactory consumerFactory,
            SmsProviderManager smsProviderManager,
            TimeZoneConverter timeZoneConverter,
            CustomNamingPeople customNamingPeople,
            IPSecurity.IPSecurity ipSecurity,
            IMemoryCache memoryCache,
            ProviderManager providerManager,
            FirstTimeTenantSettings firstTimeTenantSettings,
            ServiceClient serviceClient,
            TelegramHelper telegramHelper,
            StorageFactory storageFactory,
            UrlShortener urlShortener,
            EncryptionServiceClient encryptionServiceClient,
            EncryptionSettingsHelper encryptionSettingsHelper,
            BackupAjaxHandler backupAjaxHandler,
            ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
            EncryptionWorker encryptionWorker,
            PasswordHasher passwordHasher,
            PaymentManager paymentManager,
            Constants constants,
            InstanceCrypto instanceCrypto,
            Signature signature,
            DbWorker dbWorker,
            IHttpClientFactory clientFactory)
        {
            Log = option.Get("ASC.Api");
            WebHostEnvironment = webHostEnvironment;
            ServiceProvider = serviceProvider;
            EmployeeWraperHelper = employeeWraperHelper;
            ConsumerFactory = consumerFactory;
            SmsProviderManager = smsProviderManager;
            TimeZoneConverter = timeZoneConverter;
            CustomNamingPeople = customNamingPeople;
            IpSecurity = ipSecurity;
            MemoryCache = memoryCache;
            ProviderManager = providerManager;
            FirstTimeTenantSettings = firstTimeTenantSettings;
            MessageService = messageService;
            StudioNotifyService = studioNotifyService;
            ApiContext = apiContext;
            UserManager = userManager;
            TenantManager = tenantManager;
            TenantExtra = tenantExtra;
            TenantStatisticsProvider = tenantStatisticsProvider;
            AuthContext = authContext;
            CookiesManager = cookiesManager;
            WebItemSecurity = webItemSecurity;
            StudioNotifyHelper = studioNotifyHelper;
            LicenseReader = licenseReader;
            PermissionContext = permissionContext;
            SettingsManager = settingsManager;
            TfaManager = tfaManager;
            WebItemManager = webItemManager;
            WebItemManagerSecurity = webItemManagerSecurity;
            TenantInfoSettingsHelper = tenantInfoSettingsHelper;
            TenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
            StorageHelper = storageHelper;
            TenantLogoManager = tenantLogoManager;
            TenantUtil = tenantUtil;
            CoreBaseSettings = coreBaseSettings;
            CommonLinkUtility = commonLinkUtility;
            ColorThemesSettingsHelper = colorThemesSettingsHelper;
            Configuration = configuration;
            SetupInfo = setupInfo;
            BuildVersion = buildVersion;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            StatisticManager = statisticManager;
            IPRestrictionsService = iPRestrictionsService;
            CoreConfiguration = coreConfiguration;
            MessageTarget = messageTarget;
            StudioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
            CoreSettings = coreSettings;
            StorageSettingsHelper = storageSettingsHelper;
            ServiceClient = serviceClient;
            EncryptionServiceClient = encryptionServiceClient;
            EncryptionSettingsHelper = encryptionSettingsHelper;
            BackupAjaxHandler = backupAjaxHandler;
            CacheDeleteSchedule = cacheDeleteSchedule;
            EncryptionWorker = encryptionWorker;
            PasswordHasher = passwordHasher;
            StorageFactory = storageFactory;
            UrlShortener = urlShortener;
            TelegramHelper = telegramHelper;
            PaymentManager = paymentManager;
            WebhookDbWorker = dbWorker;
            Constants = constants;
            InstanceCrypto = instanceCrypto;
            Signature = signature;
            ClientFactory = clientFactory;
        }

        [Read("", Check = false)]
        [AllowAnonymous]
        public SettingsWrapper GetSettings(bool? withpassword)
        {
            var settings = new SettingsWrapper
            {
                Culture = Tenant.GetCulture().ToString(),
                GreetingSettings = Tenant.Name,
                Personal = CoreBaseSettings.Personal,
                Version = Configuration["version:number"] ?? ""
            };

            if (AuthContext.IsAuthenticated)
            {
                settings.TrustedDomains = Tenant.TrustedDomains;
                settings.TrustedDomainsType = Tenant.TrustedDomainsType;
                var timeZone = Tenant.TimeZone;
                settings.Timezone = timeZone;
                settings.UtcOffset = TimeZoneConverter.GetTimeZone(timeZone).GetUtcOffset(DateTime.UtcNow);
                settings.UtcHoursOffset = settings.UtcOffset.TotalHours;
                settings.OwnerId = Tenant.OwnerId;
                settings.NameSchemaId = CustomNamingPeople.Current.Id;

                settings.SocketUrl = Configuration["web:hub:url"] ?? "";

                settings.Firebase = new FirebaseWrapper
                {
                    ApiKey = Configuration["firebase:apiKey"] ?? "",
                    AuthDomain = Configuration["firebase:authDomain"] ?? "",
                    ProjectId = Configuration["firebase:projectId"] ?? "",
                    StorageBucket = Configuration["firebase:storageBucket"] ?? "",
                    MessagingSenderId = Configuration["firebase:messagingSenderId"] ?? "",
                    AppId = Configuration["firebase:appId"] ?? "",
                    MeasurementId = Configuration["firebase:measurementId"] ?? ""
                };

                bool debugInfo;
                if (bool.TryParse(Configuration["debug-info:enabled"], out debugInfo))
                {
                    settings.DebugInfo = debugInfo;
                }
            }
            else
            {
                if (!SettingsManager.Load<WizardSettings>().Completed)
                {
                    settings.WizardToken = CommonLinkUtility.GetToken(Tenant.TenantId, "", ConfirmType.Wizard, userId: Tenant.OwnerId);
                }

                settings.EnabledJoin =
                    (Tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom &&
                    Tenant.TrustedDomains.Count > 0) ||
                    Tenant.TrustedDomainsType == TenantTrustedDomainsType.All;

                if (settings.EnabledJoin.GetValueOrDefault(false))
                {
                    settings.TrustedDomainsType = Tenant.TrustedDomainsType;
                    settings.TrustedDomains = Tenant.TrustedDomains;
                }

                var studioAdminMessageSettings = SettingsManager.Load<StudioAdminMessageSettings>();

                settings.EnableAdmMess = studioAdminMessageSettings.Enable || TenantExtra.IsNotPaid();

                settings.ThirdpartyEnable = SetupInfo.ThirdPartyAuthEnabled && ProviderManager.IsNotEmpty;

                settings.RecaptchaPublicKey = SetupInfo.RecaptchaPublicKey;
            }

            if (!AuthContext.IsAuthenticated || (withpassword.HasValue && withpassword.Value))
            {
                settings.PasswordHash = PasswordHasher;
            }

            return settings;
        }

        [Create("messagesettings")]
        public object EnableAdminMessageSettingsFromBody([FromBody] AdminMessageSettingsModel model)
        {
            return EnableAdminMessageSettings(model);
        }

        [Create("messagesettings")]
        [Consumes("application/x-www-form-urlencoded")]
        public object EnableAdminMessageSettingsFromForm([FromForm] AdminMessageSettingsModel model)
        {
            return EnableAdminMessageSettings(model);
        }

        private object EnableAdminMessageSettings(AdminMessageSettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            SettingsManager.Save(new StudioAdminMessageSettings { Enable = model.TurnOn });

            MessageService.Send(MessageAction.AdministratorMessageSettingsUpdated);

            return Resource.SuccessfullySaveSettingsMessage;
        }

        [AllowAnonymous]
        [Create("sendadmmail")]
        public object SendAdmMailFromBody([FromBody] AdminMessageSettingsModel model)
        {
            return SendAdmMail(model);
        }

        [AllowAnonymous]
        [Create("sendadmmail")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendAdmMailFromForm([FromForm] AdminMessageSettingsModel model)
        {
            return SendAdmMail(model);
        }

        private object SendAdmMail(AdminMessageSettingsModel model)
        {
            var studioAdminMessageSettings = SettingsManager.Load<StudioAdminMessageSettings>();
            var enableAdmMess = studioAdminMessageSettings.Enable || TenantExtra.IsNotPaid();

            if (!enableAdmMess)
                throw new MethodAccessException("Method not available");

            if (!model.Email.TestEmailRegex())
                throw new Exception(Resource.ErrorNotCorrectEmail);

            if (string.IsNullOrEmpty(model.Message))
                throw new Exception(Resource.ErrorEmptyMessage);

            CheckCache("sendadmmail");

            StudioNotifyService.SendMsgToAdminFromNotAuthUser(model.Email, model.Message);
            MessageService.Send(MessageAction.ContactAdminMailSent);

            return Resource.AdminMessageSent;
        }

        [Create("maildomainsettings")]
        public object SaveMailDomainSettingsFromBody([FromBody] MailDomainSettingsModel model)
        {
            return SaveMailDomainSettings(model);
        }

        [Create("maildomainsettings")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SaveMailDomainSettingsFromForm([FromForm] MailDomainSettingsModel model)
        {
            return SaveMailDomainSettings(model);
        }

        private object SaveMailDomainSettings(MailDomainSettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (model.Type == TenantTrustedDomainsType.Custom)
            {
                Tenant.TrustedDomains.Clear();
                foreach (var d in model.Domains.Select(domain => (domain ?? "").Trim().ToLower()))
                {
                    if (!(!string.IsNullOrEmpty(d) && new Regex("^[a-z0-9]([a-z0-9-.]){1,98}[a-z0-9]$").IsMatch(d)))
                        return Resource.ErrorNotCorrectTrustedDomain;

                    Tenant.TrustedDomains.Add(d);
                }

                if (Tenant.TrustedDomains.Count == 0)
                    model.Type = TenantTrustedDomainsType.None;
            }

            Tenant.TrustedDomainsType = model.Type;

            SettingsManager.Save(new StudioTrustedDomainSettings { InviteUsersAsVisitors = model.InviteUsersAsVisitors });

            TenantManager.SaveTenant(Tenant);

            MessageService.Send(MessageAction.TrustedMailDomainSettingsUpdated);

            return Resource.SuccessfullySaveSettingsMessage;
        }

        [AllowAnonymous]
        [Create("sendjoininvite")]
        public object SendJoinInviteMailFromBody([FromBody] AdminMessageSettingsModel model)
        {
            return SendJoinInviteMail(model);
        }

        [AllowAnonymous]
        [Create("sendjoininvite")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendJoinInviteMailFromForm([FromForm] AdminMessageSettingsModel model)
        {
            return SendJoinInviteMail(model);
        }

        private object SendJoinInviteMail(AdminMessageSettingsModel model)
        {
            try
            {
                var email = model.Email;
                if (!(
                    (Tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom &&
                    Tenant.TrustedDomains.Count > 0) ||
                    Tenant.TrustedDomainsType == TenantTrustedDomainsType.All))
                    throw new MethodAccessException("Method not available");

                if (!email.TestEmailRegex())
                    throw new Exception(Resource.ErrorNotCorrectEmail);

                CheckCache("sendjoininvite");

                var user = UserManager.GetUserByEmail(email);
                if (!user.ID.Equals(Constants.LostUser.ID))
                    throw new Exception(CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));

                var settings = SettingsManager.Load<IPRestrictionsSettings>();

                if (settings.Enable && !IpSecurity.Verify())
                    throw new Exception(Resource.ErrorAccessRestricted);

                var trustedDomainSettings = SettingsManager.Load<StudioTrustedDomainSettings>();
                var emplType = trustedDomainSettings.InviteUsersAsVisitors ? EmployeeType.Visitor : EmployeeType.User;
                if (!CoreBaseSettings.Personal)
                {
                    var enableInviteUsers = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

                    if (!enableInviteUsers)
                        emplType = EmployeeType.Visitor;
                }

                switch (Tenant.TrustedDomainsType)
                {
                    case TenantTrustedDomainsType.Custom:
                    {
                        var address = new MailAddress(email);
                        if (Tenant.TrustedDomains.Any(d => address.Address.EndsWith("@" + d.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            StudioNotifyService.SendJoinMsg(email, emplType);
                            MessageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                            return Resource.FinishInviteJoinEmailMessage;
                        }

                        throw new Exception(Resource.ErrorEmailDomainNotAllowed);
                    }
                    case TenantTrustedDomainsType.All:
                    {
                        StudioNotifyService.SendJoinMsg(email, emplType);
                        MessageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                        return Resource.FinishInviteJoinEmailMessage;
                    }
                    default:
                        throw new Exception(Resource.ErrorNotCorrectEmail);
                }
            }
            catch (FormatException)
            {
                return Resource.ErrorNotCorrectEmail;
            }
            catch (Exception e)
            {
                return e.Message.HtmlEncode();
            }
        }

        [Read("customschemas")]
        public List<SchemaModel> PeopleSchemas()
        {
            return CustomNamingPeople
                    .GetSchemas()
                    .Select(r =>
                    {
                        var names = CustomNamingPeople.GetPeopleNames(r.Key);

                        return new SchemaModel
                        {
                            Id = names.Id,
                            Name = names.SchemaName,
                            UserCaption = names.UserCaption,
                            UsersCaption = names.UsersCaption,
                            GroupCaption = names.GroupCaption,
                            GroupsCaption = names.GroupsCaption,
                            UserPostCaption = names.UserPostCaption,
                            RegDateCaption = names.RegDateCaption,
                            GroupHeadCaption = names.GroupHeadCaption,
                            GuestCaption = names.GuestCaption,
                            GuestsCaption = names.GuestsCaption,
                        };
                    })
                    .ToList();
        }

        [Create("customschemas")]
        public SchemaModel SaveNamingSettings(SchemaModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            CustomNamingPeople.SetPeopleNames(model.Id);

            TenantManager.SaveTenant(TenantManager.GetCurrentTenant());

            MessageService.Send(MessageAction.TeamTemplateChanged);

            return PeopleSchema(model.Id);
        }

        [Update("customschemas")]
        public SchemaModel SaveCustomNamingSettings(SchemaModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var usrCaption = (model.UserCaption ?? "").Trim();
            var usrsCaption = (model.UsersCaption ?? "").Trim();
            var grpCaption = (model.GroupCaption ?? "").Trim();
            var grpsCaption = (model.GroupsCaption ?? "").Trim();
            var usrStatusCaption = (model.UserPostCaption ?? "").Trim();
            var regDateCaption = (model.RegDateCaption ?? "").Trim();
            var grpHeadCaption = (model.GroupHeadCaption ?? "").Trim();
            var guestCaption = (model.GuestCaption ?? "").Trim();
            var guestsCaption = (model.GuestsCaption ?? "").Trim();

            if (string.IsNullOrEmpty(usrCaption)
             || string.IsNullOrEmpty(usrsCaption)
             || string.IsNullOrEmpty(grpCaption)
             || string.IsNullOrEmpty(grpsCaption)
             || string.IsNullOrEmpty(usrStatusCaption)
             || string.IsNullOrEmpty(regDateCaption)
             || string.IsNullOrEmpty(grpHeadCaption)
             || string.IsNullOrEmpty(guestCaption)
             || string.IsNullOrEmpty(guestsCaption))
            {
                throw new Exception(Resource.ErrorEmptyFields);
            }

            var names = new PeopleNamesItem
            {
                Id = PeopleNamesItem.CustomID,
                UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
                UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
                GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
                GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
                UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
                RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
                GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
                GuestCaption = guestCaption.Substring(0, Math.Min(30, guestCaption.Length)),
                GuestsCaption = guestsCaption.Substring(0, Math.Min(30, guestsCaption.Length)),
            };

            CustomNamingPeople.SetPeopleNames(names);

            TenantManager.SaveTenant(TenantManager.GetCurrentTenant());

            MessageService.Send(MessageAction.TeamTemplateChanged);

            return PeopleSchema(PeopleNamesItem.CustomID);
        }

        [Read("customschemas/{id}")]
        public SchemaModel PeopleSchema(string id)
        {
            var names = CustomNamingPeople.GetPeopleNames(id);
            var schemaItem = new SchemaModel
            {
                Id = names.Id,
                Name = names.SchemaName,
                UserCaption = names.UserCaption,
                UsersCaption = names.UsersCaption,
                GroupCaption = names.GroupCaption,
                GroupsCaption = names.GroupsCaption,
                UserPostCaption = names.UserPostCaption,
                RegDateCaption = names.RegDateCaption,
                GroupHeadCaption = names.GroupHeadCaption,
                GuestCaption = names.GuestCaption,
                GuestsCaption = names.GuestsCaption,
            };
            return schemaItem;
        }

        [Read("quota")]
        public QuotaWrapper GetQuotaUsed()
        {
            return new QuotaWrapper(Tenant, CoreBaseSettings, CoreConfiguration, TenantExtra, TenantStatisticsProvider, AuthContext, SettingsManager, WebItemManager, Constants);
        }

        [AllowAnonymous]
        [Read("cultures", Check = false)]
        public IEnumerable<object> GetSupportedCultures()
        {
            return SetupInfo.EnabledCultures.Select(r => r.Name).ToArray();
        }

        [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard,Administrators")]
        [Read("timezones", Check = false)]
        public List<TimezonesModel> GetTimeZones()
        {
            ApiContext.AuthByClaim();
            var timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

            if (timeZones.All(tz => tz.Id != "UTC"))
            {
                timeZones.Add(TimeZoneInfo.Utc);
            }

            var listOfTimezones = new List<TimezonesModel>();

            foreach (var tz in timeZones.OrderBy(z => z.BaseUtcOffset))
            {
                listOfTimezones.Add(new TimezonesModel
                {
                    Id = tz.Id,
                    DisplayName = TimeZoneConverter.GetTimeZoneDisplayName(tz)
                });
            }

            return listOfTimezones;
        }

        [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
        [Read("machine", Check = false)]
        public object GetMachineName()
        {
            return Dns.GetHostName().ToLowerInvariant();
        }

        [Read("greetingsettings")]
        public ContentResult GetGreetingSettings()
        {
            return new ContentResult { Content = Tenant.Name };
        }

        [Create("greetingsettings")]
        public ContentResult SaveGreetingSettingsFromBody([FromBody] GreetingSettingsModel model)
        {
            return SaveGreetingSettings(model);
        }

        [Create("greetingsettings")]
        [Consumes("application/x-www-form-urlencoded")]
        public ContentResult SaveGreetingSettingsFromForm([FromForm] GreetingSettingsModel model)
        {
            return SaveGreetingSettings(model);
        }

        private ContentResult SaveGreetingSettings(GreetingSettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            Tenant.Name = model.Title;
            TenantManager.SaveTenant(Tenant);

            MessageService.Send(MessageAction.GreetingSettingsUpdated);

            return new ContentResult { Content = Resource.SuccessfullySaveGreetingSettingsMessage };
        }

        [Create("greetingsettings/restore")]
        public ContentResult RestoreGreetingSettings()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            TenantInfoSettingsHelper.RestoreDefaultTenantName();

            return new ContentResult
            {
                Content = Tenant.Name
            };
        }

        //[Read("recalculatequota")]
        //public void RecalculateQuota()
        //{
        //    SecurityContext.DemandPermissions(Tenant, SecutiryConstants.EditPortalSettings);

        //    var operations = quotaTasks.GetTasks()
        //        .Where(t => t.GetProperty<int>(QuotaSync.TenantIdKey) == Tenant.TenantId);

        //    if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
        //    {
        //        throw new InvalidOperationException(Resource.LdapSettingsTooManyOperations);
        //    }

        //    var op = new QuotaSync(Tenant.TenantId, ServiceProvider);

        //    quotaTasks.QueueTask(op.RunJob, op.GetDistributedTask());
        //}

        //[Read("checkrecalculatequota")]
        //public bool CheckRecalculateQuota()
        //{
        //    PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        //    var task = quotaTasks.GetTasks().FirstOrDefault(t => t.GetProperty<int>(QuotaSync.TenantIdKey) == Tenant.TenantId);

        //    if (task != null && task.Status == DistributedTaskStatus.Completed)
        //    {
        //        quotaTasks.RemoveTask(task.Id);
        //        return false;
        //    }

        //    return task != null;
        //}

        [AllowAnonymous]
        [Read("version/build", false)]
        public Task<BuildVersion> GetBuildVersionsAsync()
        {
            return BuildVersion.GetCurrentBuildVersionAsync();
        }

        [Read("version")]
        public TenantVersionWrapper GetVersions()
        {
            return new TenantVersionWrapper(Tenant.Version, TenantManager.GetTenantVersions());
        }

        [Update("version")]
        public TenantVersionWrapper SetVersionFromBody([FromBody] SettingsModel model)
        {
            return SetVersion(model);
        }

        [Update("version")]
        [Consumes("application/x-www-form-urlencoded")]
        public TenantVersionWrapper SetVersionFromForm([FromForm] SettingsModel model)
        {
            return SetVersion(model);
        }

        private TenantVersionWrapper SetVersion(SettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            TenantManager.GetTenantVersions().FirstOrDefault(r => r.Id == model.VersionId).NotFoundIfNull();
            TenantManager.SetTenantVersion(Tenant, model.VersionId);

            return GetVersions();
        }

        [Read("security")]
        public IEnumerable<SecurityWrapper> GetWebItemSecurityInfo([FromQuery] IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                ids = WebItemManager.GetItemsAll().Select(i => i.ID.ToString());
            }

            var subItemList = WebItemManager.GetItemsAll().Where(item => item.IsSubItem()).Select(i => i.ID.ToString());

            return ids.Select(r => WebItemSecurity.GetSecurityInfo(r))
                      .Select(i => new SecurityWrapper
                      {
                          WebItemId = i.WebItemId,
                          Enabled = i.Enabled,
                          Users = i.Users.Select(EmployeeWraperHelper.Get),
                          Groups = i.Groups.Select(g => new GroupWrapperSummary(g, UserManager)),
                          IsSubItem = subItemList.Contains(i.WebItemId),
                      }).ToList();
        }

        [Read("security/{id}")]
        public bool GetWebItemSecurityInfo(Guid id)
        {
            var module = WebItemManager[id];

            return module != null && !module.IsDisabled(WebItemSecurity, AuthContext);
        }

        [Read("security/modules")]
        public object GetEnabledModules()
        {
            var EnabledModules = WebItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.Normal)
                                        .Where(item => !item.IsSubItem() && item.Visible)
                                        .Select(item => new
                                        {
                                            id = item.ProductClassName.HtmlEncode(),
                                            title = item.Name.HtmlEncode()
                                        });

            return EnabledModules;
        }

        [Read("security/password", Check = false)]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Everyone")]
        public object GetPasswordSettings()
        {
            var UserPasswordSettings = SettingsManager.Load<PasswordSettings>();

            return UserPasswordSettings;
        }

        [Update("security")]
        public IEnumerable<SecurityWrapper> SetWebItemSecurityFromBody([FromBody] WebItemSecurityModel model)
        {
            return SetWebItemSecurity(model);
        }

        [Update("security")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<SecurityWrapper> SetWebItemSecurityFromForm([FromForm] WebItemSecurityModel model)
        {
            return SetWebItemSecurity(model);
        }

        private IEnumerable<SecurityWrapper> SetWebItemSecurity(WebItemSecurityModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            WebItemSecurity.SetSecurity(model.Id, model.Enabled, model.Subjects?.ToArray());
            var securityInfo = GetWebItemSecurityInfo(new List<string> { model.Id });

            if (model.Subjects == null) return securityInfo;

            var productName = GetProductName(new Guid(model.Id));

            if (!model.Subjects.Any())
            {
                MessageService.Send(MessageAction.ProductAccessOpened, productName);
            }
            else
            {
                foreach (var info in securityInfo)
                {
                    if (info.Groups.Any())
                    {
                        MessageService.Send(MessageAction.GroupsOpenedProductAccess, productName, info.Groups.Select(x => x.Name));
                    }
                    if (info.Users.Any())
                    {
                        MessageService.Send(MessageAction.UsersOpenedProductAccess, productName, info.Users.Select(x => HttpUtility.HtmlDecode(x.DisplayName)));
                    }
                }
            }

            return securityInfo;
        }

        [Update("security/access")]
        public IEnumerable<SecurityWrapper> SetAccessToWebItemsFromBody([FromBody] WebItemSecurityModel model)
        {
            return SetAccessToWebItems(model);
        }

        [Update("security/access")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<SecurityWrapper> SetAccessToWebItemsFromForm([FromForm] WebItemSecurityModel model)
        {
            return SetAccessToWebItems(model);
        }

        private IEnumerable<SecurityWrapper> SetAccessToWebItems(WebItemSecurityModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var itemList = new ItemDictionary<string, bool>();

            foreach (var item in model.Items)
            {
                if (!itemList.ContainsKey(item.Key))
                    itemList.Add(item.Key, item.Value);
            }

            var defaultPageSettings = SettingsManager.Load<StudioDefaultPageSettings>();

            foreach (var item in itemList)
            {
                Guid[] subjects = null;
                var productId = new Guid(item.Key);

                if (item.Value)
                {
                    if (WebItemManager[productId] is IProduct webItem || productId == WebItemManager.MailProductID)
                    {
                        var productInfo = WebItemSecurity.GetSecurityInfo(item.Key);
                        var selectedGroups = productInfo.Groups.Select(group => group.ID).ToList();
                        var selectedUsers = productInfo.Users.Select(user => user.ID).ToList();
                        selectedUsers.AddRange(selectedGroups);
                        if (selectedUsers.Count > 0)
                        {
                            subjects = selectedUsers.ToArray();
                        }
                    }
                }
                else if (productId == defaultPageSettings.DefaultProductID)
                {
                    SettingsManager.Save((StudioDefaultPageSettings)defaultPageSettings.GetDefault(ServiceProvider));
                }

                WebItemSecurity.SetSecurity(item.Key, item.Value, subjects);
            }

            MessageService.Send(MessageAction.ProductsListUpdated);

            return GetWebItemSecurityInfo(itemList.Keys.ToList());
        }

        [Read("security/administrator/{productid}")]
        public IEnumerable<EmployeeWraper> GetProductAdministrators(Guid productid)
        {
            return WebItemSecurity.GetProductAdministrators(productid)
                                  .Select(EmployeeWraperHelper.Get)
                                  .ToList();
        }

        [Read("security/administrator")]
        public object IsProductAdministrator(Guid productid, Guid userid)
        {
            var result = WebItemSecurity.IsProductAdministrator(productid, userid);
            return new { ProductId = productid, UserId = userid, Administrator = result };
        }

        [Update("security/administrator")]
        public object SetProductAdministratorFromBody([FromBody] SecurityModel model)
        {
            return SetProductAdministrator(model);
        }

        [Update("security/administrator")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SetProductAdministratorFromForm([FromForm] SecurityModel model)
        {
            return SetProductAdministrator(model);
        }

        private object SetProductAdministrator(SecurityModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            WebItemSecurity.SetProductAdministrator(model.ProductId, model.UserId, model.Administrator);

            var admin = UserManager.GetUsers(model.UserId);

            if (model.ProductId == Guid.Empty)
            {
                var messageAction = model.Administrator ? MessageAction.AdministratorOpenedFullAccess : MessageAction.AdministratorDeleted;
                MessageService.Send(messageAction, MessageTarget.Create(admin.ID), admin.DisplayUserName(false, DisplayUserSettingsHelper));
            }
            else
            {
                var messageAction = model.Administrator ? MessageAction.ProductAddedAdministrator : MessageAction.ProductDeletedAdministrator;
                MessageService.Send(messageAction, MessageTarget.Create(admin.ID), GetProductName(model.ProductId), admin.DisplayUserName(false, DisplayUserSettingsHelper));
            }

            return new { model.ProductId, model.UserId, model.Administrator };
        }

        [Read("logo")]
        public object GetLogo()
        {
            return TenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(SettingsManager.Load<TenantInfoSettings>());
        }

        ///<visible>false</visible>
        [Create("whitelabel/save")]
        public bool SaveWhiteLabelSettingsFromBody([FromBody] WhiteLabelModel model, [FromQuery] WhiteLabelQuery query)
        {
            return SaveWhiteLabelSettings(model, query);
        }

        [Create("whitelabel/save")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool SaveWhiteLabelSettingsFromForm([FromForm] WhiteLabelModel model, [FromQuery] WhiteLabelQuery query)
        {
            return SaveWhiteLabelSettings(model, query);
        }

        private bool SaveWhiteLabelSettings(WhiteLabelModel model, WhiteLabelQuery query)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!TenantLogoManager.WhiteLabelEnabled || !TenantLogoManager.WhiteLabelPaid)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }

            if (query.IsDefault)
            {
                DemandRebrandingPermission();
                SaveWhiteLabelSettingsForDefaultTenant(model);
            }
            else
            {
                SaveWhiteLabelSettingsForCurrentTenant(model);
            }
            return true;
        }

        private void SaveWhiteLabelSettingsForCurrentTenant(WhiteLabelModel model)
        {
            var settings = SettingsManager.Load<TenantWhiteLabelSettings>();

            SaveWhiteLabelSettingsForTenant(settings, null, Tenant.TenantId, model);
        }

        private void SaveWhiteLabelSettingsForDefaultTenant(WhiteLabelModel model)
        {
            var settings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
            var storage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            SaveWhiteLabelSettingsForTenant(settings, storage, Tenant.DEFAULT_TENANT, model);
        }

        private void SaveWhiteLabelSettingsForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId, WhiteLabelModel model)
        {
            if (model.Logo != null)
            {
                var logoDict = new Dictionary<int, string>();

                foreach(var l in model.Logo)
                {
                    logoDict.Add(Int32.Parse(l.Key), l.Value);
                }

                TenantWhiteLabelSettingsHelper.SetLogo(settings, logoDict, storage);
            }

            settings.SetLogoText(model.LogoText);
            TenantWhiteLabelSettingsHelper.Save(settings, tenantId, TenantLogoManager);

        }

        ///<visible>false</visible>
        [Create("whitelabel/savefromfiles")]
        public bool SaveWhiteLabelSettingsFromFiles([FromQuery] WhiteLabelQuery query)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!TenantLogoManager.WhiteLabelEnabled || !TenantLogoManager.WhiteLabelPaid)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }

            if (HttpContext.Request.Form?.Files == null || HttpContext.Request.Form.Files.Count == 0)
            {
                throw new InvalidOperationException("No input files");
            }

            if (query.IsDefault)
            {
                DemandRebrandingPermission();
                SaveWhiteLabelSettingsFromFilesForDefaultTenant();
            }
            else
            {
                SaveWhiteLabelSettingsFromFilesForCurrentTenant();
            }
            return true;
        }

        private void SaveWhiteLabelSettingsFromFilesForCurrentTenant()
        {
            var settings = SettingsManager.Load<TenantWhiteLabelSettings>();

            SaveWhiteLabelSettingsFromFilesForTenant(settings, null, Tenant.TenantId);
        }

        private void SaveWhiteLabelSettingsFromFilesForDefaultTenant()
        {
            var settings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
            var storage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            SaveWhiteLabelSettingsFromFilesForTenant(settings, storage, Tenant.DEFAULT_TENANT);
        }

        private void SaveWhiteLabelSettingsFromFilesForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId)
        {
            foreach (var f in HttpContext.Request.Form.Files)
            {
                var parts = f.FileName.Split('.');
                var logoType = (WhiteLabelLogoTypeEnum)Convert.ToInt32(parts[0]);
                var fileExt = parts[1];
                TenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), storage);
            }

            SettingsManager.SaveForTenant(settings, tenantId);
        }


        ///<visible>false</visible>
        [Read("whitelabel/sizes")]
        public object GetWhiteLabelSizes()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!TenantLogoManager.WhiteLabelEnabled)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }

            return
            new[]
            {
                new {type = (int)WhiteLabelLogoTypeEnum.LightSmall, name = nameof(WhiteLabelLogoTypeEnum.LightSmall), height = TenantWhiteLabelSettings.logoLightSmallSize.Height, width = TenantWhiteLabelSettings.logoLightSmallSize.Width},
                new {type = (int)WhiteLabelLogoTypeEnum.Dark, name = nameof(WhiteLabelLogoTypeEnum.Dark), height = TenantWhiteLabelSettings.logoDarkSize.Height, width = TenantWhiteLabelSettings.logoDarkSize.Width},
                new {type = (int)WhiteLabelLogoTypeEnum.Favicon, name = nameof(WhiteLabelLogoTypeEnum.Favicon), height = TenantWhiteLabelSettings.logoFaviconSize.Height, width = TenantWhiteLabelSettings.logoFaviconSize.Width},
                new {type = (int)WhiteLabelLogoTypeEnum.DocsEditor, name = nameof(WhiteLabelLogoTypeEnum.DocsEditor), height = TenantWhiteLabelSettings.logoDocsEditorSize.Height, width = TenantWhiteLabelSettings.logoDocsEditorSize.Width},
                new {type = (int)WhiteLabelLogoTypeEnum.DocsEditorEmbed, name =  nameof(WhiteLabelLogoTypeEnum.DocsEditorEmbed), height = TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Height, width = TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Width}

            };
        }



        ///<visible>false</visible>
        [Read("whitelabel/logos")]
        public Dictionary<string, string> GetWhiteLabelLogos([FromQuery] WhiteLabelQuery query)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!TenantLogoManager.WhiteLabelEnabled)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }

            Dictionary<string, string> result;

            if (query.IsDefault)
            {
                result = new Dictionary<string, string>
                {
                    { ((int)WhiteLabelLogoTypeEnum.LightSmall).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.Dark).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Dark, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.Favicon).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.DocsEditor).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.DocsEditorEmbed).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !query.IsRetina)) }
                };
            }
            else
            {
                var _tenantWhiteLabelSettings = SettingsManager.Load<TenantWhiteLabelSettings>();

                result = new Dictionary<string, string>
                    {
                        { ((int)WhiteLabelLogoTypeEnum.LightSmall).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, !query.IsRetina)) },
                        { ((int)WhiteLabelLogoTypeEnum.Dark).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, !query.IsRetina)) },
                        { ((int)WhiteLabelLogoTypeEnum.Favicon).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, !query.IsRetina)) },
                        { ((int)WhiteLabelLogoTypeEnum.DocsEditor).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, !query.IsRetina)) },
                        { ((int)WhiteLabelLogoTypeEnum.DocsEditorEmbed).ToString(), CommonLinkUtility.GetFullAbsolutePath(TenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings,WhiteLabelLogoTypeEnum.DocsEditorEmbed, !query.IsRetina)) }
                    };
            }

            return result;
        }

        ///<visible>false</visible>
        [Read("whitelabel/logotext")]
        public object GetWhiteLabelLogoText([FromQuery] WhiteLabelQuery query)
        {
            if (!TenantLogoManager.WhiteLabelEnabled)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }

            var settings = query.IsDefault ? SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>() : SettingsManager.Load<TenantWhiteLabelSettings>();

            return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
        }


        ///<visible>false</visible>
        [Update("whitelabel/restore")]
        public bool RestoreWhiteLabelOptions(WhiteLabelQuery query)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!TenantLogoManager.WhiteLabelEnabled || !TenantLogoManager.WhiteLabelPaid)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
            }
            if (query.IsDefault)
            {
                DemandRebrandingPermission();
                RestoreWhiteLabelOptionsForDefaultTenant();
            }
            else
            {
                RestoreWhiteLabelOptionsForCurrentTenant();
            }
            return true;
        }

        private void RestoreWhiteLabelOptionsForCurrentTenant()
        {
            var settings = SettingsManager.Load<TenantWhiteLabelSettings>();

            RestoreWhiteLabelOptionsForTenant(settings, null, Tenant.TenantId);

            var tenantInfoSettings = SettingsManager.Load<TenantInfoSettings>();
            TenantInfoSettingsHelper.RestoreDefaultLogo(tenantInfoSettings, TenantLogoManager);
            SettingsManager.Save(tenantInfoSettings);
        }

        private void RestoreWhiteLabelOptionsForDefaultTenant()
        {
            var settings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
            var storage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            RestoreWhiteLabelOptionsForTenant(settings, storage, Tenant.DEFAULT_TENANT);
        }

        private void RestoreWhiteLabelOptionsForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId)
        {
            TenantWhiteLabelSettingsHelper.RestoreDefault(settings, TenantLogoManager, tenantId, storage);
        }


        [Read("iprestrictions")]
        public IEnumerable<IPRestriction> GetIpRestrictions()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            return IPRestrictionsService.Get(Tenant.TenantId);
        }

        [Update("iprestrictions")]
        public IEnumerable<string> SaveIpRestrictionsFromBody([FromBody] IpRestrictionsModel model)
        {
            return SaveIpRestrictions(model);
        }

        [Update("iprestrictions")]
        [Consumes("application/x-www-form-urlencoded")]
        public IEnumerable<string> SaveIpRestrictionsFromForm([FromForm] IpRestrictionsModel model)
        {
            return SaveIpRestrictions(model);
        }

        private IEnumerable<string> SaveIpRestrictions(IpRestrictionsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            return IPRestrictionsService.Save(model.Ips, Tenant.TenantId);
        }

        [Update("iprestrictions/settings")]
        public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromBody([FromBody] IpRestrictionsModel model)
        {
            return UpdateIpRestrictionsSettings(model);
        }

        [Update("iprestrictions/settings")]
        [Consumes("application/x-www-form-urlencoded")]
        public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromForm([FromForm] IpRestrictionsModel model)
        {
            return UpdateIpRestrictionsSettings(model);
        }

        private IPRestrictionsSettings UpdateIpRestrictionsSettings(IpRestrictionsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var settings = new IPRestrictionsSettings { Enable = model.Enable };
            SettingsManager.Save(settings);

            return settings;
        }

        [Update("tips")]
        public TipsSettings UpdateTipsSettingsFromBody([FromBody] SettingsModel model)
        {
            return UpdateTipsSettings(model);
        }

        [Update("tips")]
        [Consumes("application/x-www-form-urlencoded")]
        public TipsSettings UpdateTipsSettingsFromForm([FromForm] SettingsModel model)
        {
            return UpdateTipsSettings(model);
        }

        private TipsSettings UpdateTipsSettings(SettingsModel model)
        {
            var settings = new TipsSettings { Show = model.Show };
            SettingsManager.SaveForCurrentUser(settings);

            if (!model.Show && !string.IsNullOrEmpty(SetupInfo.TipsAddress))
            {
                try
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri($"{SetupInfo.TipsAddress}/tips/deletereaded");

                    var data = new NameValueCollection
                    {
                        ["userId"] = AuthContext.CurrentAccount.ID.ToString(),
                        ["tenantId"] = Tenant.TenantId.ToString(CultureInfo.InvariantCulture)
                    };
                    var body = JsonSerializer.Serialize(data);//todo check
                    request.Content = new StringContent(body);

                    var httpClient = ClientFactory.CreateClient();
                    using var response = httpClient.Send(request);

                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                }
            }

            return settings;
        }

        [Update("tips/change/subscription")]
        public bool UpdateTipsSubscription()
        {
            return StudioPeriodicNotify.ChangeSubscription(AuthContext.CurrentAccount.ID, StudioNotifyHelper);
        }

        [Read("tips/subscription")]
        public bool GetTipsSubscription()
        {
            return StudioNotifyHelper.IsSubscribedToNotify(AuthContext.CurrentAccount.ID, Actions.PeriodicNotify);
        }

        [Update("wizard/complete", Check = false)]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
        public WizardSettings CompleteWizardFromBody([FromBody] WizardModel wizardModel)
        {
            return CompleteWizard(wizardModel);
        }

        [Update("wizard/complete", Check = false)]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
        [Consumes("application/x-www-form-urlencoded")]
        public WizardSettings CompleteWizardFromForm([FromForm] WizardModel wizardModel)
        {
            return CompleteWizard(wizardModel);
        }

        private WizardSettings CompleteWizard(WizardModel wizardModel)
        {
            ApiContext.AuthByClaim();

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            return FirstTimeTenantSettings.SaveData(wizardModel);
        }

        [Read("tfaapp")]
        public IEnumerable<TfaSettings> GetTfaSettings()
        {
            var result = new List<TfaSettings>();

            var SmsVisible = StudioSmsNotificationSettingsHelper.IsVisibleSettings();
            var SmsEnable = SmsVisible && SmsProviderManager.Enabled();
            var TfaVisible = TfaAppAuthSettings.IsVisibleSettings;

            if (SmsVisible)
            {
                result.Add(new TfaSettings
                {
                    Enabled = StudioSmsNotificationSettingsHelper.Enable,
                    Id = "sms",
                    Title = Resource.ButtonSmsEnable,
                    Avaliable = SmsEnable
                });
            }

            if (TfaVisible)
            {
                result.Add(new TfaSettings
                {
                    Enabled = SettingsManager.Load<TfaAppAuthSettings>().EnableSetting,
                    Id = "app",
                    Title = Resource.ButtonTfaAppEnable,
                    Avaliable = true
                });
            }

            return result;
        }

        [Create("tfaapp/validate")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation,Everyone")]
        public bool TfaValidateAuthCode(TfaValidateModel model)
        {
            ApiContext.AuthByClaim();
            var user = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
            return TfaManager.ValidateAuthCode(user, model.Code);
        }

        [Read("tfaapp/confirm")]
        public object TfaConfirmUrl()
        {
            var user = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
            if (StudioSmsNotificationSettingsHelper.IsVisibleSettings() && StudioSmsNotificationSettingsHelper.Enable)// && smsConfirm.ToLower() != "true")
            {
                var confirmType = string.IsNullOrEmpty(user.MobilePhone) ||
                               user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                                   ? ConfirmType.PhoneActivation
                                   : ConfirmType.PhoneAuth;

                return CommonLinkUtility.GetConfirmationUrl(user.Email, confirmType);
            }

            if (TfaAppAuthSettings.IsVisibleSettings && SettingsManager.Load<TfaAppAuthSettings>().EnableSetting)
            {
                var confirmType = TfaAppUserSettings.EnableForUser(SettingsManager, AuthContext.CurrentAccount.ID)
                    ? ConfirmType.TfaAuth
                    : ConfirmType.TfaActivation;

                return CommonLinkUtility.GetConfirmationUrl(user.Email, confirmType);
            }

            return string.Empty;
        }

        [Update("tfaapp")]
        public bool TfaSettingsFromBody([FromBody] TfaModel model)
        {
            return TfaSettingsUpdate(model);
        }

        [Update("tfaapp")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool TfaSettingsFromForm([FromForm] TfaModel model)
        {
            return TfaSettingsUpdate(model);
        }

        private bool TfaSettingsUpdate(TfaModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var result = false;

            MessageAction action;
            var settings = SettingsManager.Load<TfaAppAuthSettings>();

            switch (model.Type)
            {
                case "sms":
                    if (!StudioSmsNotificationSettingsHelper.IsVisibleSettings())
                        throw new Exception(Resource.SmsNotAvailable);

                    if (!SmsProviderManager.Enabled())
                        throw new MethodAccessException();

                    StudioSmsNotificationSettingsHelper.Enable = true;
                    action = MessageAction.TwoFactorAuthenticationEnabledBySms;

                    if (settings.EnableSetting)
                    {
                        settings.EnableSetting = false;
                        SettingsManager.Save(settings);
                    }

                    result = true;

                    break;

                case "app":
                    if (!TfaAppAuthSettings.IsVisibleSettings)
                    {
                        throw new Exception(Resource.TfaAppNotAvailable);
                    }

                    settings.EnableSetting = true;
                    SettingsManager.Save(settings);

                    action = MessageAction.TwoFactorAuthenticationEnabledByTfaApp;

                    if (StudioSmsNotificationSettingsHelper.IsVisibleSettings() && StudioSmsNotificationSettingsHelper.Enable)
                    {
                        StudioSmsNotificationSettingsHelper.Enable = false;
                    }

                    result = true;

                    break;

                default:
                    if (settings.EnableSetting)
                    {
                        settings.EnableSetting = false;
                        SettingsManager.Save(settings);
                    }

                    if (StudioSmsNotificationSettingsHelper.IsVisibleSettings() && StudioSmsNotificationSettingsHelper.Enable)
                    {
                        StudioSmsNotificationSettingsHelper.Enable = false;
                    }

                    action = MessageAction.TwoFactorAuthenticationDisabled;

                    break;
            }

            if (result)
            {
                CookiesManager.ResetTenantCookie();
            }

            MessageService.Send(action);
            return result;
        }

        [Read("tfaapp/setup")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "TfaActivation")]
        public SetupCode TfaAppGenerateSetupCode()
        {
            ApiContext.AuthByClaim();
            var currentUser = UserManager.GetUsers(AuthContext.CurrentAccount.ID);

            if (!TfaAppAuthSettings.IsVisibleSettings ||
                !SettingsManager.Load<TfaAppAuthSettings>().EnableSetting ||
                TfaAppUserSettings.EnableForUser(SettingsManager, currentUser.ID))
                throw new Exception(Resource.TfaAppNotAvailable);

            if (currentUser.IsVisitor(UserManager) || currentUser.IsOutsider(UserManager))
                throw new NotSupportedException("Not available.");

            return TfaManager.GenerateSetupCode(currentUser);
        }

        [Read("tfaappcodes")]
        public IEnumerable<object> TfaAppGetCodes()
        {
            var currentUser = UserManager.GetUsers(AuthContext.CurrentAccount.ID);

            if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(SettingsManager, currentUser.ID))
                throw new Exception(Resource.TfaAppNotAvailable);

            if (currentUser.IsVisitor(UserManager) || currentUser.IsOutsider(UserManager))
                throw new NotSupportedException("Not available.");

            return SettingsManager.LoadForCurrentUser<TfaAppUserSettings>().CodesSetting.Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(InstanceCrypto, Signature) }).ToList();
        }

        [Update("tfaappnewcodes")]
        public IEnumerable<object> TfaAppRequestNewCodes()
        {
            var currentUser = UserManager.GetUsers(AuthContext.CurrentAccount.ID);

            if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(SettingsManager, currentUser.ID))
                throw new Exception(Resource.TfaAppNotAvailable);

            if (currentUser.IsVisitor(UserManager) || currentUser.IsOutsider(UserManager))
                throw new NotSupportedException("Not available.");

            var codes = TfaManager.GenerateBackupCodes().Select(r => new { r.IsUsed, Code = r.GetEncryptedCode(InstanceCrypto, Signature) }).ToList();
            MessageService.Send(MessageAction.UserConnectedTfaApp, MessageTarget.Create(currentUser.ID), currentUser.DisplayUserName(false, DisplayUserSettingsHelper));
            return codes;
        }

        [Update("tfaappnewapp")]
        public object TfaAppNewAppFromBody([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TfaModel model)
        {
            return TfaAppNewApp(model);
        }

        [Update("tfaappnewapp")]
        [Consumes("application/x-www-form-urlencoded")]
        public object TfaAppNewAppFromForm([FromForm] TfaModel model)
        {
            return TfaAppNewApp(model);
        }

        private object TfaAppNewApp(TfaModel model)
        {
            var id = model?.Id ?? Guid.Empty;
            var isMe = id.Equals(Guid.Empty);
            var user = UserManager.GetUsers(isMe ? AuthContext.CurrentAccount.ID : id);

            if (!isMe && !PermissionContext.CheckPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser))
                throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);

            if (!TfaAppAuthSettings.IsVisibleSettings || !TfaAppUserSettings.EnableForUser(SettingsManager, user.ID))
                throw new Exception(Resource.TfaAppNotAvailable);

            if (user.IsVisitor(UserManager) || user.IsOutsider(UserManager))
                throw new NotSupportedException("Not available.");

            TfaAppUserSettings.DisableForUser(ServiceProvider, SettingsManager, user.ID);
            MessageService.Send(MessageAction.UserDisconnectedTfaApp, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            if (isMe)
            {
                return CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaActivation);
            }

            StudioNotifyService.SendMsgTfaReset(user);
            return string.Empty;
        }


        ///<visible>false</visible>
        [Update("welcome/close")]
        public void CloseWelcomePopup()
        {
            var currentUser = UserManager.GetUsers(AuthContext.CurrentAccount.ID);

            var collaboratorPopupSettings = SettingsManager.LoadForCurrentUser<CollaboratorSettings>();

            if (!(currentUser.IsVisitor(UserManager) && collaboratorPopupSettings.FirstVisit && !currentUser.IsOutsider(UserManager)))
                throw new NotSupportedException("Not available.");

            collaboratorPopupSettings.FirstVisit = false;
            SettingsManager.SaveForCurrentUser(collaboratorPopupSettings);
        }

        ///<visible>false</visible>
        [Update("colortheme")]
        public void SaveColorThemeFromBody([FromBody] SettingsModel model)
        {
            SaveColorTheme(model);
        }

        [Update("colortheme")]
        [Consumes("application/x-www-form-urlencoded")]
        public void SaveColorThemeFromForm([FromForm] SettingsModel model)
        {
            SaveColorTheme(model);
        }

        private void SaveColorTheme(SettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            ColorThemesSettingsHelper.SaveColorTheme(model.Theme);
            MessageService.Send(MessageAction.ColorThemeChanged);
        }

        ///<visible>false</visible>
        [Update("timeandlanguage")]
        public object TimaAndLanguageFromBody([FromBody] SettingsModel model)
        {
            return TimaAndLanguage(model);
        }

        [Update("timeandlanguage")]
        [Consumes("application/x-www-form-urlencoded")]
        public object TimaAndLanguageFromForm([FromForm] SettingsModel model)
        {
            return TimaAndLanguage(model);
        }

        private object TimaAndLanguage(SettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var culture = CultureInfo.GetCultureInfo(model.Lng);

            var changelng = false;
            if (SetupInfo.EnabledCultures.Find(c => string.Equals(c.Name, culture.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                if (!string.Equals(Tenant.Language, culture.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    Tenant.Language = culture.Name;
                    changelng = true;
                }
            }

            var oldTimeZone = Tenant.TimeZone;
            var timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();
            if (timeZones.All(tz => tz.Id != "UTC"))
            {
                timeZones.Add(TimeZoneInfo.Utc);
            }
            Tenant.TimeZone = timeZones.FirstOrDefault(tz => tz.Id == model.TimeZoneID)?.Id ?? TimeZoneInfo.Utc.Id;

            TenantManager.SaveTenant(Tenant);

            if (!Tenant.TimeZone.Equals(oldTimeZone) || changelng)
            {
                if (!Tenant.TimeZone.Equals(oldTimeZone))
                {
                    MessageService.Send(MessageAction.TimeZoneSettingsUpdated);
                }
                if (changelng)
                {
                    MessageService.Send(MessageAction.LanguageSettingsUpdated);
                }
            }

            return Resource.SuccessfullySaveSettingsMessage;
        }

        [Create("owner")]
        public object SendOwnerChangeInstructionsFromBody([FromBody] SettingsModel model)
        {
            return SendOwnerChangeInstructions(model);
        }

        [Create("owner")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SendOwnerChangeInstructionsFromForm([FromForm] SettingsModel model)
        {
            return SendOwnerChangeInstructions(model);
        }

        private object SendOwnerChangeInstructions(SettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var curTenant = TenantManager.GetCurrentTenant();
            var owner = UserManager.GetUsers(curTenant.OwnerId);
            var newOwner = UserManager.GetUsers(model.OwnerId);

            if (newOwner.IsVisitor(UserManager)) throw new System.Security.SecurityException("Collaborator can not be an owner");

            if (!owner.ID.Equals(AuthContext.CurrentAccount.ID) || Guid.Empty.Equals(newOwner.ID))
            {
                return new { Status = 0, Message = Resource.ErrorAccessDenied };
            }

            var confirmLink = CommonLinkUtility.GetConfirmationUrl(owner.Email, ConfirmType.PortalOwnerChange, newOwner.ID, newOwner.ID);
            StudioNotifyService.SendMsgConfirmChangeOwner(owner, newOwner, confirmLink);

            MessageService.Send(MessageAction.OwnerSentChangeOwnerInstructions, MessageTarget.Create(owner.ID), owner.DisplayUserName(false, DisplayUserSettingsHelper));

            var emailLink = $"<a href=\"mailto:{owner.Email}\">{owner.Email}</a>";
            return new { Status = 1, Message = Resource.ChangePortalOwnerMsg.Replace(":email", emailLink) };
        }

        [Update("owner")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
        public void OwnerFromBody([FromBody] SettingsModel model)
        {
            Owner(model);
        }

        [Update("owner")]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
        [Consumes("application/x-www-form-urlencoded")]
        public void OwnerFromForm([FromForm] SettingsModel model)
        {
            Owner(model);
        }

        private void Owner(SettingsModel model)
        {
            var newOwner = Constants.LostUser;
            try
            {
                newOwner = UserManager.GetUsers(model.OwnerId);
            }
            catch
            {
            }
            if (Constants.LostUser.Equals(newOwner))
            {
                throw new Exception(Resource.ErrorUserNotFound);
            }

            if (UserManager.IsUserInGroup(newOwner.ID, Constants.GroupVisitor.ID))
            {
                throw new Exception(Resource.ErrorUserNotFound);
            }

            var curTenant = TenantManager.GetCurrentTenant();
            curTenant.OwnerId = newOwner.ID;
            TenantManager.SaveTenant(curTenant);

            MessageService.Send(MessageAction.OwnerUpdated, newOwner.DisplayUserName(false, DisplayUserSettingsHelper));
        }

        ///<visible>false</visible>
        [Update("defaultpage")]
        public object SaveDefaultPageSettingsFromBody([FromBody] SettingsModel model)
        {
            return SaveDefaultPageSettings(model);
        }

        [Update("defaultpage")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SaveDefaultPageSettingsFromForm([FromForm] SettingsModel model)
        {
            return SaveDefaultPageSettings(model);
        }

        private object SaveDefaultPageSettings(SettingsModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            SettingsManager.Save(new StudioDefaultPageSettings { DefaultProductID = model.DefaultProductID });

            MessageService.Send(MessageAction.DefaultStartPageSettingsUpdated);

            return Resource.SuccessfullySaveSettingsMessage;
        }


        private string GetProductName(Guid productId)
        {
            var product = WebItemManager[productId];
            return productId == Guid.Empty ? "All" : product != null ? product.Name : productId.ToString();
        }

        [Read("license/refresh", Check = false)]
        public bool RefreshLicense()
        {
            if (!CoreBaseSettings.Standalone) return false;
            LicenseReader.RefreshLicense();
            return true;
        }

        [Create("license/accept", Check = false)]
        public object AcceptLicense()
        {
            if (!CoreBaseSettings.Standalone) return "";

            TariffSettings.SetLicenseAccept(SettingsManager);
            MessageService.Send(MessageAction.LicenseKeyUploaded);

            try
            {
                LicenseReader.RefreshLicense();
            }
            catch (BillingNotFoundException)
            {
                return UserControlsCommonResource.LicenseKeyNotFound;
            }
            catch (BillingNotConfiguredException)
            {
                return UserControlsCommonResource.LicenseKeyNotCorrect;
            }
            catch (BillingException)
            {
                return UserControlsCommonResource.LicenseException;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        ///<visible>false</visible>
        [Create("license/trial")]
        public bool ActivateTrial()
        {
            if (!CoreBaseSettings.Standalone) throw new NotSupportedException();
            if (!UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsAdmin(UserManager)) throw new SecurityException();

            var curQuota = TenantExtra.GetTenantQuota();
            if (curQuota.Id != Tenant.DEFAULT_TENANT) return false;
            if (curQuota.Trial) return false;

            var curTariff = TenantExtra.GetCurrentTariff();
            if (curTariff.DueDate.Date != DateTime.MaxValue.Date) return false;

            var quota = new TenantQuota(-1000)
            {
                Name = "apirequest",
                ActiveUsers = curQuota.ActiveUsers,
                MaxFileSize = curQuota.MaxFileSize,
                MaxTotalSize = curQuota.MaxTotalSize,
                Features = curQuota.Features
            };
            quota.Trial = true;

            TenantManager.SaveTenantQuota(quota);

            const int DEFAULT_TRIAL_PERIOD = 30;

            var tariff = new Tariff
            {
                QuotaId = quota.Id,
                DueDate = DateTime.Today.AddDays(DEFAULT_TRIAL_PERIOD)
            };

            PaymentManager.SetTariff(-1, tariff);

            MessageService.Send(MessageAction.LicenseKeyUploaded);

            return true;
        }

        [AllowAnonymous]
        [Read("license/required", Check = false)]
        public bool RequestLicense()
        {
            return FirstTimeTenantSettings.RequestLicense;
        }


        [Create("license", Check = false)]
        [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard, Administrators")]
        public object UploadLicense([FromForm] UploadLicenseModel model)
        {
            try
            {
                ApiContext.AuthByClaim();
                if (!AuthContext.IsAuthenticated && SettingsManager.Load<WizardSettings>().Completed) throw new SecurityException(Resource.PortalSecurity);
                if (!model.Files.Any()) throw new Exception(Resource.ErrorEmptyUploadFileSelected);



                var licenseFile = model.Files.First();
                var dueDate = LicenseReader.SaveLicenseTemp(licenseFile.OpenReadStream());

                return dueDate >= DateTime.UtcNow.Date
                                     ? Resource.LicenseUploaded
                                     : string.Format(
                                         TenantExtra.GetTenantQuota().Update
                                              ? Resource.LicenseUploadedOverdueSupport
                                              : Resource.LicenseUploadedOverdue,
                                                     "",
                                                     "",
                                                     dueDate.Date.ToLongDateString());
            }
            catch (LicenseExpiredException ex)
            {
                Log.Error("License upload", ex);
                throw new Exception(Resource.LicenseErrorExpired);
            }
            catch (LicenseQuotaException ex)
            {
                Log.Error("License upload", ex);
                throw new Exception(Resource.LicenseErrorQuota);
            }
            catch (LicensePortalException ex)
            {
                Log.Error("License upload", ex);
                throw new Exception(Resource.LicenseErrorPortal);
            }
            catch (Exception ex)
            {
                Log.Error("License upload", ex);
                throw new Exception(Resource.LicenseError);
            }
        }


        [Read("customnavigation/getall")]
        public List<CustomNavigationItem> GetCustomNavigationItems()
        {
            return SettingsManager.Load<CustomNavigationSettings>().Items;
        }

        [Read("customnavigation/getsample")]
        public CustomNavigationItem GetCustomNavigationItemSample()
        {
            return CustomNavigationItem.GetSample();
        }

        [Read("customnavigation/get/{id}")]
        public CustomNavigationItem GetCustomNavigationItem(Guid id)
        {
            return SettingsManager.Load<CustomNavigationSettings>().Items.FirstOrDefault(item => item.Id == id);
        }

        [Create("customnavigation/create")]
        public CustomNavigationItem CreateCustomNavigationItemFromBody([FromBody] CustomNavigationItem item)
        {
            return CreateCustomNavigationItem(item);
        }

        [Create("customnavigation/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public CustomNavigationItem CreateCustomNavigationItemFromForm([FromForm] CustomNavigationItem item)
        {
            return CreateCustomNavigationItem(item);
        }

        private CustomNavigationItem CreateCustomNavigationItem(CustomNavigationItem item)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var settings = SettingsManager.Load<CustomNavigationSettings>();

            var exist = false;

            foreach (var existItem in settings.Items)
            {
                if (existItem.Id != item.Id) continue;

                existItem.Label = item.Label;
                existItem.Url = item.Url;
                existItem.ShowInMenu = item.ShowInMenu;
                existItem.ShowOnHomePage = item.ShowOnHomePage;

                if (existItem.SmallImg != item.SmallImg)
                {
                    StorageHelper.DeleteLogo(existItem.SmallImg);
                    existItem.SmallImg = StorageHelper.SaveTmpLogo(item.SmallImg);
                }

                if (existItem.BigImg != item.BigImg)
                {
                    StorageHelper.DeleteLogo(existItem.BigImg);
                    existItem.BigImg = StorageHelper.SaveTmpLogo(item.BigImg);
                }

                exist = true;
                break;
            }

            if (!exist)
            {
                item.Id = Guid.NewGuid();
                item.SmallImg = StorageHelper.SaveTmpLogo(item.SmallImg);
                item.BigImg = StorageHelper.SaveTmpLogo(item.BigImg);

                settings.Items.Add(item);
            }

            SettingsManager.Save(settings);

            MessageService.Send(MessageAction.CustomNavigationSettingsUpdated);

            return item;
        }

        [Delete("customnavigation/delete/{id}")]
        public void DeleteCustomNavigationItem(Guid id)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var settings = SettingsManager.Load<CustomNavigationSettings>();

            var terget = settings.Items.FirstOrDefault(item => item.Id == id);

            if (terget == null) return;

            StorageHelper.DeleteLogo(terget.SmallImg);
            StorageHelper.DeleteLogo(terget.BigImg);

            settings.Items.Remove(terget);
            SettingsManager.Save(settings);

            MessageService.Send(MessageAction.CustomNavigationSettingsUpdated);
        }

        [Update("emailactivation")]
        public EmailActivationSettings UpdateEmailActivationSettingsFromBody([FromBody] EmailActivationSettings settings)
        {
            SettingsManager.SaveForCurrentUser(settings);
            return settings;
        }

        [Update("emailactivation")]
        [Consumes("application/x-www-form-urlencoded")]
        public EmailActivationSettings UpdateEmailActivationSettingsFromForm([FromForm] EmailActivationSettings settings)
        {
            SettingsManager.SaveForCurrentUser(settings);
            return settings;
        }

        ///<visible>false</visible>
        [Read("companywhitelabel")]
        public List<CompanyWhiteLabelSettings> GetLicensorData()
        {
            var result = new List<CompanyWhiteLabelSettings>();

            var instance = CompanyWhiteLabelSettings.Instance(SettingsManager);

            result.Add(instance);

            if (!instance.IsDefault(CoreSettings) && !instance.IsLicensor)
            {
                result.Add(instance.GetDefault(ServiceProvider) as CompanyWhiteLabelSettings);
            }

            return result;
        }

        [Read("statistics/spaceusage/{id}")]
        public Task<List<UsageSpaceStatItemWrapper>> GetSpaceUsageStatistics(Guid id)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var webitem = WebItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.All)
                                       .FirstOrDefault(item =>
                                                       item != null &&
                                                       item.ID == id &&
                                                       item.Context != null &&
                                                       item.Context.SpaceUsageStatManager != null);

            if (webitem == null) return Task.FromResult(new List<UsageSpaceStatItemWrapper>());

            return InternalGetSpaceUsageStatistics(webitem);
        }

        private async Task<List<UsageSpaceStatItemWrapper>> InternalGetSpaceUsageStatistics(IWebItem webitem)
        {
            var statData = await webitem.Context.SpaceUsageStatManager.GetStatDataAsync();

            return statData.ConvertAll(it => new UsageSpaceStatItemWrapper
            {
                Name = it.Name.HtmlEncode(),
                Icon = it.ImgUrl,
                Disabled = it.Disabled,
                Size = FileSizeComment.FilesSizeToString(it.SpaceUsage),
                Url = it.Url
            });
        }

        [Read("statistics/visit")]
        public List<ChartPointWrapper> GetVisitStatistics(ApiDateTime fromDate, ApiDateTime toDate)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var from = TenantUtil.DateTimeFromUtc(fromDate);
            var to = TenantUtil.DateTimeFromUtc(toDate);

            var points = new List<ChartPointWrapper>();

            if (from.CompareTo(to) >= 0) return points;

            for (var d = new DateTime(from.Ticks); d.Date.CompareTo(to.Date) <= 0; d = d.AddDays(1))
            {
                points.Add(new ChartPointWrapper
                {
                    DisplayDate = d.Date.ToShortDateString(),
                    Date = d.Date,
                    Hosts = 0,
                    Hits = 0
                });
            }

            var hits = StatisticManager.GetHitsByPeriod(Tenant.TenantId, from, to);
            var hosts = StatisticManager.GetHostsByPeriod(Tenant.TenantId, from, to);

            if (hits.Count == 0 || hosts.Count == 0) return points;

            hits.Sort((x, y) => x.VisitDate.CompareTo(y.VisitDate));
            hosts.Sort((x, y) => x.VisitDate.CompareTo(y.VisitDate));

            for (int i = 0, n = points.Count, hitsNum = 0, hostsNum = 0; i < n; i++)
            {
                while (hitsNum < hits.Count && points[i].Date.CompareTo(hits[hitsNum].VisitDate.Date) == 0)
                {
                    points[i].Hits += hits[hitsNum].VisitCount;
                    hitsNum++;
                }
                while (hostsNum < hosts.Count && points[i].Date.CompareTo(hosts[hostsNum].VisitDate.Date) == 0)
                {
                    points[i].Hosts++;
                    hostsNum++;
                }
            }

            return points;
        }

        [Read("storage")]
        public List<StorageWrapper> GetAllStorages()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            TenantExtra.DemandControlPanelPermission();

            var current = SettingsManager.Load<StorageSettings>();
            var consumers = ConsumerFactory.GetAll<DataStoreConsumer>();
            return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
        }

        [Read("storage/progress", false)]
        public double GetStorageProgress()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!CoreBaseSettings.Standalone) return -1;

            return ServiceClient.GetProgress(Tenant.TenantId);
        }

        public readonly object Locker = new object();

        [Create("encryption/start")]
        public bool StartStorageEncryptionFromBody([FromBody] StorageEncryptionModel storageEncryption)
        {
            return StartStorageEncryption(storageEncryption);
        }

        [Create("encryption/start")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool StartStorageEncryptionFromForm([FromForm] StorageEncryptionModel storageEncryption)
        {
            return StartStorageEncryption(storageEncryption);
        }

        private bool StartStorageEncryption(StorageEncryptionModel storageEncryption)
        {
            if (CoreBaseSettings.CustomMode)
            {
                return false;
            }

            lock (Locker)
            {
                var activeTenants = TenantManager.GetTenants();

                if (activeTenants.Count > 0)
                {
                    StartEncryption(storageEncryption.NotifyUsers);
                }
            }
            return true;
        }

        private void StartEncryption(bool notifyUsers)
        {
            if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
            {
                throw new NotSupportedException();
            }

            if (!CoreBaseSettings.Standalone)
            {
                throw new NotSupportedException();
            }

            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            TenantExtra.DemandControlPanelPermission();

            if (!TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).DiscEncryption)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
            }

            var storages = GetAllStorages();

            if (storages.Any(s => s.Current))
            {
                throw new NotSupportedException();
            }

            var cdnStorages = GetAllCdnStorages();

            if (cdnStorages.Any(s => s.Current))
            {
                throw new NotSupportedException();
            }

            var tenants = TenantManager.GetTenants();

            foreach (var tenant in tenants)
            {
                var progress = BackupAjaxHandler.GetBackupProgress(tenant.TenantId);
                if (progress != null && !progress.IsCompleted)
                {
                    throw new Exception();
                }
            }

            foreach (var tenant in tenants)
            {
                CacheDeleteSchedule.Publish(new DeleteSchedule() { TenantId = tenant.TenantId }, CacheNotifyAction.Insert);
            }

            var settings = EncryptionSettingsHelper.Load();

            settings.NotifyUsers = notifyUsers;

            if (settings.Status == EncryprtionStatus.Decrypted)
            {
                settings.Status = EncryprtionStatus.EncryptionStarted;
                settings.Password = EncryptionSettingsHelper.GeneratePassword(32, 16);
            }
            else if (settings.Status == EncryprtionStatus.Encrypted)
            {
                settings.Status = EncryprtionStatus.DecryptionStarted;
            }

            MessageService.Send(settings.Status == EncryprtionStatus.EncryptionStarted ? MessageAction.StartStorageEncryption : MessageAction.StartStorageDecryption);

            var serverRootPath = CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

            foreach (var tenant in tenants)
            {
                TenantManager.SetCurrentTenant(tenant);

                if (notifyUsers)
                {
                    if (settings.Status == EncryprtionStatus.EncryptionStarted)
                    {
                        StudioNotifyService.SendStorageEncryptionStart(serverRootPath);
                    }
                    else
                    {
                        StudioNotifyService.SendStorageDecryptionStart(serverRootPath);
                    }
                }

                tenant.SetStatus(TenantStatus.Encryption);
                TenantManager.SaveTenant(tenant);
            }

            EncryptionSettingsHelper.Save(settings);

            var encryptionSettingsProto = new EncryptionSettingsProto
            {
                NotifyUsers = settings.NotifyUsers,
                Password = settings.Password,
                Status = settings.Status,
                ServerRootPath = serverRootPath
            };
            EncryptionServiceClient.Start(encryptionSettingsProto);
        }

        /// <summary>
        /// Get storage encryption settings
        /// </summary>
        /// <returns>EncryptionSettings</returns>
        /// <visible>false</visible>
        [Read("encryption/settings")]
        public EncryptionSettings GetStorageEncryptionSettings()
        {
            try
            {
                if (CoreBaseSettings.CustomMode)
                {
                    return null;
                }

                if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
                {
                    throw new NotSupportedException();
                }

                if (!CoreBaseSettings.Standalone)
                {
                    throw new NotSupportedException();
                }

                PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                TenantExtra.DemandControlPanelPermission();

                if (!TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).DiscEncryption)
                {
                    throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
                }

                var settings = EncryptionSettingsHelper.Load();

                settings.Password = string.Empty; // Don't show password

                return settings;
            }
            catch (Exception e)
            {
                Log.Error("GetStorageEncryptionSettings", e);
                return null;
            }
        }

        [Read("encryption/progress")]
        public double? GetStorageEncryptionProgress()
        {
            if (CoreBaseSettings.CustomMode)
            {
                return -1;
            }

            if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
            {
                throw new NotSupportedException();
            }

            if (!CoreBaseSettings.Standalone)
            {
                throw new NotSupportedException();
            }

            if (!TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).DiscEncryption)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
            }

            return EncryptionWorker.GetEncryptionProgress();
        }

        [Update("storage")]
        public StorageSettings UpdateStorageFromBody([FromBody] StorageModel model)
        {
            return UpdateStorage(model);
        }

        [Update("storage")]
        [Consumes("application/x-www-form-urlencoded")]
        public StorageSettings UpdateStorageFromForm([FromForm] StorageModel model)
        {
            return UpdateStorage(model);
        }

        private StorageSettings UpdateStorage(StorageModel model)
        {
            try
            {
                PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                if (!CoreBaseSettings.Standalone) return null;

                TenantExtra.DemandControlPanelPermission();

                var consumer = ConsumerFactory.GetByKey(model.Module);
                if (!consumer.IsSet)
                    throw new ArgumentException("module");

                var settings = SettingsManager.Load<StorageSettings>();
                if (settings.Module == model.Module) return settings;

                settings.Module = model.Module;
                settings.Props = model.Props.ToDictionary(r => r.Key, b => b.Value);

                StartMigrate(settings);
                return settings;
            }
            catch (Exception e)
            {
                Log.Error("UpdateStorage", e);
                throw;
            }
        }

        [Delete("storage")]
        public void ResetStorageToDefault()
        {
            try
            {
                PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                if (!CoreBaseSettings.Standalone) return;

                TenantExtra.DemandControlPanelPermission();

                var settings = SettingsManager.Load<StorageSettings>();

                settings.Module = null;
                settings.Props = null;


                StartMigrate(settings);
            }
            catch (Exception e)
            {
                Log.Error("ResetStorageToDefault", e);
                throw;
            }
        }

        [Read("storage/cdn")]
        public List<StorageWrapper> GetAllCdnStorages()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!CoreBaseSettings.Standalone) return null;

            TenantExtra.DemandControlPanelPermission();

            var current = SettingsManager.Load<CdnStorageSettings>();
            var consumers = ConsumerFactory.GetAll<DataStoreConsumer>().Where(r => r.Cdn != null);
            return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
        }

        [Update("storage/cdn")]
        public CdnStorageSettings UpdateCdnFromBody([FromBody] StorageModel model)
        {
            return UpdateCdn(model);
        }

        [Update("storage/cdn")]
        [Consumes("application/x-www-form-urlencoded")]
        public CdnStorageSettings UpdateCdnFromForm([FromForm] StorageModel model)
        {
            return UpdateCdn(model);
        }

        private CdnStorageSettings UpdateCdn(StorageModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!CoreBaseSettings.Standalone) return null;

            TenantExtra.DemandControlPanelPermission();

            var consumer = ConsumerFactory.GetByKey(model.Module);
            if (!consumer.IsSet)
                throw new ArgumentException("module");

            var settings = SettingsManager.Load<CdnStorageSettings>();
            if (settings.Module == model.Module) return settings;

            settings.Module = model.Module;
            settings.Props = model.Props.ToDictionary(r => r.Key, b => b.Value);

            try
            {
                ServiceClient.UploadCdn(Tenant.TenantId, "/", WebHostEnvironment.ContentRootPath, settings);
            }
            catch (Exception e)
            {
                Log.Error("UpdateCdn", e);
                throw;
            }

            return settings;
        }

        [Delete("storage/cdn")]
        public void ResetCdnToDefault()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!CoreBaseSettings.Standalone) return;

            TenantExtra.DemandControlPanelPermission();

            StorageSettingsHelper.Clear(SettingsManager.Load<CdnStorageSettings>());
        }

        [Read("storage/backup")]
        public List<StorageWrapper> GetAllBackupStorages()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (CoreBaseSettings.Standalone)
            {
                TenantExtra.DemandControlPanelPermission();
            }
            var schedule = BackupAjaxHandler.GetSchedule();
            var current = new StorageSettings();

            if (schedule != null && schedule.StorageType == BackupStorageType.ThirdPartyConsumer)
            {
                current = new StorageSettings
                {
                    Module = schedule.StorageParams["module"],
                    Props = schedule.StorageParams.Where(r => r.Key != "module").ToDictionary(r => r.Key, r => r.Value)
                };
            }

            var consumers = ConsumerFactory.GetAll<DataStoreConsumer>();
            return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
        }

        private void StartMigrate(StorageSettings settings)
        {
            ServiceClient.Migrate(Tenant.TenantId, settings);

            Tenant.SetStatus(TenantStatus.Migrating);
            TenantManager.SaveTenant(Tenant);
        }

        [Read("socket")]
        public object GetSocketSettings()
        {
            var hubUrl = Configuration["web:hub"] ?? string.Empty;
            if (hubUrl.Length != 0)
            {
                if (!hubUrl.EndsWith('/'))
                {
                    hubUrl += "/";
                }
            }

            return new { Url = hubUrl };
        }

        ///<visible>false</visible>
        [Read("controlpanel")]
        public TenantControlPanelSettings GetTenantControlPanelSettings()
        {
            return SettingsManager.Load<TenantControlPanelSettings>();
        }

        ///<visible>false</visible>
        [Create("rebranding/company")]
        public bool SaveCompanyWhiteLabelSettingsFromBody([FromBody] CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
        {
            return SaveCompanyWhiteLabelSettings(companyWhiteLabelSettingsWrapper);
        }

        [Create("rebranding/company")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool SaveCompanyWhiteLabelSettingsFromForm([FromForm] CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
        {
            return SaveCompanyWhiteLabelSettings(companyWhiteLabelSettingsWrapper);
        }

        private bool SaveCompanyWhiteLabelSettings(CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
        {
            if (companyWhiteLabelSettingsWrapper.Settings == null) throw new ArgumentNullException("settings");

            DemandRebrandingPermission();

            companyWhiteLabelSettingsWrapper.Settings.IsLicensor = false; //TODO: CoreContext.TenantManager.GetTenantQuota(TenantProvider.CurrentTenantID).Branding && settings.IsLicensor

            SettingsManager.SaveForDefaultTenant(companyWhiteLabelSettingsWrapper.Settings);
            return true;
        }

        ///<visible>false</visible>
        [Read("rebranding/company")]
        public CompanyWhiteLabelSettings GetCompanyWhiteLabelSettings()
        {
            return SettingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>();
        }

        ///<visible>false</visible>
        [Delete("rebranding/company")]
        public CompanyWhiteLabelSettings DeleteCompanyWhiteLabelSettings()
        {
            DemandRebrandingPermission();

            var defaultSettings = (CompanyWhiteLabelSettings)SettingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().GetDefault(CoreSettings);

            SettingsManager.SaveForDefaultTenant(defaultSettings);

            return defaultSettings;
        }

        ///<visible>false</visible>
        [Create("rebranding/additional")]
        public bool SaveAdditionalWhiteLabelSettingsFromBody([FromBody] AdditionalWhiteLabelSettingsWrapper wrapper)
        {
            return SaveAdditionalWhiteLabelSettings(wrapper);
        }

        [Create("rebranding/additional")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool SaveAdditionalWhiteLabelSettingsFromForm([FromForm] AdditionalWhiteLabelSettingsWrapper wrapper)
        {
            return SaveAdditionalWhiteLabelSettings(wrapper);
        }

        private bool SaveAdditionalWhiteLabelSettings(AdditionalWhiteLabelSettingsWrapper wrapper)
        {
            if (wrapper.Settings == null) throw new ArgumentNullException("settings");

            DemandRebrandingPermission();

            SettingsManager.SaveForDefaultTenant(wrapper.Settings);
            return true;
        }

        ///<visible>false</visible>
        [Read("rebranding/additional")]
        public AdditionalWhiteLabelSettings GetAdditionalWhiteLabelSettings()
        {
            return SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
        }

        ///<visible>false</visible>
        [Delete("rebranding/additional")]
        public AdditionalWhiteLabelSettings DeleteAdditionalWhiteLabelSettings()
        {
            DemandRebrandingPermission();

            var defaultSettings = (AdditionalWhiteLabelSettings)SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().GetDefault(Configuration);

            SettingsManager.SaveForDefaultTenant(defaultSettings);

            return defaultSettings;
        }

        ///<visible>false</visible>
        [Create("rebranding/mail")]
        public bool SaveMailWhiteLabelSettingsFromBody([FromBody] MailWhiteLabelSettings settings)
        {
            return SaveMailWhiteLabelSettings(settings);
        }

        ///<visible>false</visible>
        [Create("rebranding/mail")]
        public bool SaveMailWhiteLabelSettingsFromForm([FromForm] MailWhiteLabelSettings settings)
        {
            return SaveMailWhiteLabelSettings(settings);
        }

        private bool SaveMailWhiteLabelSettings(MailWhiteLabelSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            DemandRebrandingPermission();

            SettingsManager.SaveForDefaultTenant(settings);
            return true;
        }

        ///<visible>false</visible>
        [Update("rebranding/mail")]
        public bool UpdateMailWhiteLabelSettingsFromBody([FromBody] MailWhiteLabelSettingsModel model)
        {
            return UpdateMailWhiteLabelSettings(model);
        }

        [Update("rebranding/mail")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool UpdateMailWhiteLabelSettingsFromForm([FromForm] MailWhiteLabelSettingsModel model)
        {
            return UpdateMailWhiteLabelSettings(model);
        }

        private bool UpdateMailWhiteLabelSettings(MailWhiteLabelSettingsModel model)
        {
            DemandRebrandingPermission();

            var settings = SettingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>();

            settings.FooterEnabled = model.FooterEnabled;

            SettingsManager.SaveForDefaultTenant(settings);

            return true;
        }

        ///<visible>false</visible>
        [Read("rebranding/mail")]
        public MailWhiteLabelSettings GetMailWhiteLabelSettings()
        {
            return SettingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>();
        }

        ///<visible>false</visible>
        [Delete("rebranding/mail")]
        public MailWhiteLabelSettings DeleteMailWhiteLabelSettings()
        {
            DemandRebrandingPermission();

            var defaultSettings = (MailWhiteLabelSettings)SettingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>().GetDefault(Configuration);

            SettingsManager.SaveForDefaultTenant(defaultSettings);

            return defaultSettings;
        }

        private void DemandRebrandingPermission()
        {
            TenantExtra.DemandControlPanelPermission();

            if (!TenantManager.GetTenantQuota(Tenant.TenantId).SSBranding)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "SSBranding");
            }

            if (CoreBaseSettings.CustomMode)
            {
                throw new SecurityException();
            }
        }

        [Read("authservice")]
        public IEnumerable<AuthServiceModel> GetAuthServices()
        {
            return ConsumerFactory.GetAll<Consumer>()
                .Where(consumer => consumer.ManagedKeys.Any())
                .OrderBy(services => services.Order)
                .Select(r => new AuthServiceModel(r))
                .ToList();
        }

        [Create("authservice")]
        public bool SaveAuthKeysFromBody([FromBody] AuthServiceModel model)
        {
            return SaveAuthKeys(model);
        }

        [Create("authservice")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool SaveAuthKeysFromForm([FromForm] AuthServiceModel model)
        {
            return SaveAuthKeys(model);
        }

        private bool SaveAuthKeys(AuthServiceModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var saveAvailable = CoreBaseSettings.Standalone || TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).ThirdParty;
            if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.ThirdPartyAuthorization))
                || !saveAvailable)
                throw new BillingException(Resource.ErrorNotAllowedOption, "ThirdPartyAuthorization");

            var changed = false;
            var consumer = ConsumerFactory.GetByKey<Consumer>(model.Name);

            var validateKeyProvider = consumer as IValidateKeysProvider;

            if (validateKeyProvider != null)
            {
                try
                {
                    if (validateKeyProvider is TwilioProvider twilioLoginProvider)
                    {
                        twilioLoginProvider.ClearOldNumbers();
                    }
                    if (validateKeyProvider is BitlyLoginProvider bitly)
                    {
                        UrlShortener.Instance = null;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            if (model.Props.All(r => string.IsNullOrEmpty(r.Value)))
            {
                consumer.Clear();
                changed = true;
            }
            else
            {
                foreach (var authKey in model.Props.Where(authKey => consumer[authKey.Name] != authKey.Value))
                {
                    consumer[authKey.Name] = authKey.Value;
                    changed = true;
                }
            }

            //TODO: Consumer implementation required (Bug 50606)
            var allPropsIsEmpty = consumer.GetType() == typeof(SmscProvider)
                ? consumer.ManagedKeys.All(key => string.IsNullOrEmpty(consumer[key]))
                : consumer.All(r => string.IsNullOrEmpty(r.Value));

            if (validateKeyProvider != null && !validateKeyProvider.ValidateKeys() && !allPropsIsEmpty)
            {
                consumer.Clear();
                throw new ArgumentException(Resource.ErrorBadKeys);
            }

            if (changed)
                MessageService.Send(MessageAction.AuthorizationKeysSetting);

            return changed;
        }

        [Read("payment", Check = false)]
        public object PaymentSettings()
        {
            var settings = SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
            var currentQuota = TenantExtra.GetTenantQuota();
            var currentTariff = TenantExtra.GetCurrentTariff();

            return
                new
                {
                    settings.SalesEmail,
                    settings.FeedbackAndSupportUrl,
                    settings.BuyUrl,
                    CoreBaseSettings.Standalone,
                    currentLicense = new
                    {
                        currentQuota.Trial,
                        currentTariff.DueDate.Date
                    }
                };
        }

        /// <visible>false</visible>
        /// <summary>
        /// Gets a link that will connect TelegramBot to your account
        /// </summary>
        /// <returns>url</returns>
        /// 
        [Read("telegramlink")]
        public object TelegramLink()
        {
            var currentLink = TelegramHelper.CurrentRegistrationLink(AuthContext.CurrentAccount.ID, Tenant.TenantId);

            if (string.IsNullOrEmpty(currentLink))
            {
                var url = TelegramHelper.RegisterUser(AuthContext.CurrentAccount.ID, Tenant.TenantId);
                return url;
            }
            else
            {
                return currentLink;
            }
        }

        /// <summary>
        /// Checks if user has connected TelegramBot
        /// </summary>
        /// <returns>0 - not connected, 1 - connected, 2 - awaiting confirmation</returns>
        [Read("telegramisconnected")]
        public object TelegramIsConnected()
        {
            return (int)TelegramHelper.UserIsConnected(AuthContext.CurrentAccount.ID, Tenant.TenantId);
        }

        /// <summary>
        /// Unlinks TelegramBot from your account
        /// </summary>
        [Delete("telegramdisconnect")]
        public void TelegramDisconnect()
        {
            TelegramHelper.Disconnect(AuthContext.CurrentAccount.ID, Tenant.TenantId);
        }

        /// <summary>
        /// Add new config for webhooks
        /// </summary>
        [Create("webhook")]
        public void CreateWebhook(WebhooksConfig model)
        {
            if (model.Uri == null) throw new ArgumentNullException("Uri");
            if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
            WebhookDbWorker.AddWebhookConfig(model);
        }

        /// <summary>
        /// Update config for webhooks
        /// </summary>
        [Update("webhook")]
        public void UpdateWebhook(WebhooksConfig model)
        {
            if (model.Uri == null) throw new ArgumentNullException("Uri");
            if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
            WebhookDbWorker.UpdateWebhookConfig(model);
        }

        /// <summary>
        /// Remove config for webhooks
        /// </summary>
        [Delete("webhook")]
        public void RemoveWebhook(WebhooksConfig model)
        {
            if (model.Uri == null) throw new ArgumentNullException("Uri");
            if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
            WebhookDbWorker.RemoveWebhookConfig(model);
        }

        /// <summary>
        /// Read Webhooks history for actual tenant
        /// </summary>
        [Read("webhooks")]
        public List<WebhooksLog> TenantWebhooks()
        {
            return WebhookDbWorker.GetTenantWebhooks();
        }


        private readonly int maxCount = 10;
        private readonly int expirationMinutes = 2;
        private void CheckCache(string basekey)
        {
            var key = ApiContext.HttpContextAccessor.HttpContext.Request.GetUserHostAddress() + basekey;
            if (MemoryCache.TryGetValue<int>(key, out var count))
            {
                if (count > maxCount)
                    throw new Exception(Resource.ErrorRequestLimitExceeded);
            }

            MemoryCache.Set(key, count + 1, TimeSpan.FromMinutes(expirationMinutes));
        }
    }
}