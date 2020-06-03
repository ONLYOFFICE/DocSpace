using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ASC.Api.Collections;
using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Contracts;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Backup;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PortalController : ControllerBase
    {
        public BackupAjaxHandler BackupHandler { get; }

        public Tenant Tenant { get { return ApiContext.Tenant; } }

        public ApiContext ApiContext { get; }
        public UserManager UserManager { get; }
        public TenantManager TenantManager { get; }
        public PaymentManager PaymentManager { get; }
        public CommonLinkUtility CommonLinkUtility { get; }
        public UrlShortener UrlShortener { get; }
        public AuthContext AuthContext { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public ILog Log { get; }


        public PortalController(
            IOptionsMonitor<ILog> options,
            ApiContext apiContext,
            UserManager userManager,
            TenantManager tenantManager,
            PaymentManager paymentManager,
            CommonLinkUtility commonLinkUtility,
            UrlShortener urlShortener,
            AuthContext authContext,
            WebItemSecurity webItemSecurity
            )
        {
            Log = options.CurrentValue;
            ApiContext = apiContext;
            UserManager = userManager;
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            CommonLinkUtility = commonLinkUtility;
            UrlShortener = urlShortener;
            AuthContext = authContext;
            WebItemSecurity = webItemSecurity;
        }

        [Read("")]
        public Tenant Get()
        {
            return Tenant;
        }

        [Read("users/{userID}")]
        public UserInfo GetUser(Guid userID)
        {
            return UserManager.GetUsers(userID);
        }

        [Read("users/invite/{employeeType}")]
        public string GeInviteLink(EmployeeType employeeType)
        {
            if (!WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, AuthContext.CurrentAccount.ID))
            {
                throw new SecurityException("Method not available");
            }

            return CommonLinkUtility.GetConfirmationUrl(string.Empty, ConfirmType.LinkInvite, (int)employeeType)
                   + $"&emplType={employeeType:d}";
        }

        [Update("getshortenlink")]
        public string GetShortenLink(string link)
        {
            try
            {
                return UrlShortener.Instance.GetShortenLink(link);
            }
            catch (Exception ex)
            {
                Log.Error("getshortenlink", ex);
                return link;
            }
        }


        [Read("usedspace")]
        public double GetUsedSpace()
        {
            return Math.Round(
                TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant.TenantId))
                           .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                           .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
        }


        [Read("userscount")]
        public long GetUsersCount()
        {
            return UserManager.GetUserNames(EmployeeStatus.Active).Count();
        }

        [Read("tariff")]
        public Tariff GetTariff()
        {
            return PaymentManager.GetTariff(Tenant.TenantId);
        }

        [Read("quota")]
        public TenantQuota GetQuota()
        {
            return TenantManager.GetTenantQuota(Tenant.TenantId);
        }

        [Read("quota/right")]
        public TenantQuota GetRightQuota()
        {
            var usedSpace = GetUsedSpace();
            var needUsersCount = GetUsersCount();

            return TenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                              .FirstOrDefault(quota =>
                                              quota.ActiveUsers > needUsersCount
                                              && quota.MaxTotalSize > usedSpace
                                              && !quota.Year);
        }


        [Read("path")]
        public string GetFullAbsolutePath(string virtualPath)
        {
            return CommonLinkUtility.GetFullAbsolutePath(virtualPath);
        }

        /// <summary>
        /// Returns the backup schedule of the current portal
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup Schedule</returns>
        [Read("getbackupschedule")]
        public BackupAjaxHandler.Schedule GetBackupSchedule()
        {
            return BackupHandler.GetSchedule();
        }

        /// <summary>
        /// Create the backup schedule of the current portal
        /// </summary>
        /// <param name="storageType">Storage type</param>
        /// <param name="storageParams">Storage parameters</param>
        /// <param name="backupsStored">Max of the backup's stored copies</param>
        /// <param name="cronParams">Cron parameters</param>
        /// <param name="backupMail">Include mail in the backup</param>
        /// <category>Backup</category>
        [Create("createbackupschedule")]
        public void CreateBackupSchedule(BackupStorageType storageType, [FromQuery] Dictionary<string, string> storageParams, int backupsStored, [FromBody] BackupAjaxHandler.CronParams cronParams, bool backupMail)
        {
            BackupHandler.CreateSchedule(storageType, storageParams, backupsStored, cronParams, backupMail);
        }

        /// <summary>
        /// Delete the backup schedule of the current portal
        /// </summary>
        /// <category>Backup</category>
        [Delete("deletebackupschedule")]
        public void DeleteBackupSchedule()
        {
            BackupHandler.DeleteSchedule();
        }

        /// <summary>
        /// Start a backup of the current portal
        /// </summary>
        /// <param name="storageType">Storage Type</param>
        /// <param name="storageParams">Storage Params</param>
        /// <param name="backupMail">Include mail in the backup</param>
        /// <category>Backup</category>
        /// <returns>Backup Progress</returns>
        [Create("startbackup")]
        public BackupProgress StartBackup(BackupStorageType storageType, Dictionary<string,string> storageParams, bool backupMail)
        {
            return BackupHandler.StartBackup(storageType, storageParams, backupMail);
        }

        /// <summary>
        /// Returns the progress of the started backup
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup Progress</returns>
        [Read("getbackupprogress")]
        public BackupProgress GetBackupProgress()
        {
            return BackupHandler.GetBackupProgress();
        }

        /// <summary>
        /// Returns the backup history of the started backup
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup History</returns>
        [Read("getbackuphistory")]
        public List<BackupHistoryRecord> GetBackupHistory()
        {
            return BackupHandler.GetBackupHistory();
        }

        /// <summary>
        /// Delete the backup with the specified id
        /// </summary>
        /// <category>Backup</category>
        [Delete("deletebackup/{id}")]
        public void DeleteBackup(Guid id)
        {
            BackupHandler.DeleteBackup(id);
        }

        /// <summary>
        /// Delete all backups of the current portal
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Backup History</returns>
        [Delete("deletebackuphistory")]
        public void DeleteBackupHistory()
        {
            BackupHandler.DeleteAllBackups();
        }

        /// <summary>
        /// Start a data restore of the current portal
        /// </summary>
        /// <param name="backupId">Backup Id</param>
        /// <param name="storageType">Storage Type</param>
        /// <param name="storageParams">Storage Params</param>
        /// <param name="notify">Notify about backup to users</param>
        /// <category>Backup</category>
        /// <returns>Restore Progress</returns>
        [Create("startrestore")]
        public BackupProgress StartBackupRestore(string backupId, BackupStorageType storageType, IEnumerable<ItemKeyValuePair<string, string>> storageParams, bool notify)
        {
            return BackupHandler.StartRestore(backupId, storageType, storageParams.ToDictionary(r => r.Key, r => r.Value), notify);
        }

        /// <summary>
        /// Returns the progress of the started restore
        /// </summary>
        /// <category>Backup</category>
        /// <returns>Restore Progress</returns>
        [Read("getrestoreprogress", true)]  //NOTE: this method doesn't check payment!!!
        public BackupProgress GetRestoreProgress()
        {
            return BackupHandler.GetRestoreProgress();
        }

        ///<visible>false</visible>
        [Read("backuptmp")]
        public string GetTempPath(string alias)
        {
            return BackupHandler.GetTmpFolder();
        }
    }

    public static class PortalControllerExtension
    {
        public static DIHelper AddPortalController(this DIHelper services)
        {
            return services
                .AddUrlShortener()
                .AddMessageServiceService()
                .AddStudioNotifyServiceService()
                .AddApiContextService()
                .AddUserManagerService()
                .AddAuthContextService()
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddEmailValidationKeyProviderService()
                .AddPaymentManagerService()
                .AddCommonLinkUtilityService()
                .AddAuthContextService()
                .AddWebItemSecurity()
                .AddBackupAjaxHandler();
        }
    }
}