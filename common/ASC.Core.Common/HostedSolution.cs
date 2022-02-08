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
using System.Linq;
using System.Security;

using ASC.Common;
using ASC.Core.Billing;
using ASC.Core.Caching;
using ASC.Core.Data;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;

using Microsoft.Extensions.Options;

namespace ASC.Core
{
    [Scope]
    class ConfigureHostedSolution : IConfigureNamedOptions<HostedSolution>
    {
        private readonly UserFormatter _userFormatter;
        private readonly IOptionsSnapshot<CachedTenantService> _tenantService;
        private readonly IOptionsSnapshot<CachedUserService> _userService;
        private readonly IOptionsSnapshot<CachedQuotaService> _quotaService;
        private readonly IOptionsSnapshot<TariffService> _tariffService;
        private readonly IOptionsSnapshot<TenantManager> _tenantManager;
        private readonly IOptionsSnapshot<TenantUtil> _tenantUtil;
        private readonly IOptionsSnapshot<DbSettingsManager> _dbSettingsManager;
        private readonly IOptionsSnapshot<CoreSettings> _coreSettings;

        public ConfigureHostedSolution(
            UserFormatter userFormatter,
            IOptionsSnapshot<CachedTenantService> tenantService,
            IOptionsSnapshot<CachedUserService> userService,
            IOptionsSnapshot<CachedQuotaService> quotaService,
            IOptionsSnapshot<TariffService> tariffService,
            IOptionsSnapshot<TenantManager> tenantManager,
            IOptionsSnapshot<TenantUtil> tenantUtil,
            IOptionsSnapshot<DbSettingsManager> dbSettingsManager,
            IOptionsSnapshot<CoreSettings> coreSettings
            )
        {
            _userFormatter = userFormatter;
            _tenantService = tenantService;
            _userService = userService;
            _quotaService = quotaService;
            _tariffService = tariffService;
            _tenantManager = tenantManager;
            _tenantUtil = tenantUtil;
            _dbSettingsManager = dbSettingsManager;
            _coreSettings = coreSettings;
        }

        public void Configure(HostedSolution hostedSolution)
        {
            hostedSolution.UserFormatter = _userFormatter;
            hostedSolution.TenantService = _tenantService.Value;
            hostedSolution.UserService = _userService.Value;
            hostedSolution.QuotaService = _quotaService.Value;
            hostedSolution.TariffService = _tariffService.Value;
            hostedSolution.ClientTenantManager = _tenantManager.Value;
            hostedSolution.TenantUtil = _tenantUtil.Value;
            hostedSolution.SettingsManager = _dbSettingsManager.Value;
            hostedSolution.CoreSettings = _coreSettings.Value;
        }

        public void Configure(string name, HostedSolution hostedSolution)
        {
            Configure(hostedSolution);
            hostedSolution.Region = name;
            hostedSolution.TenantService = _tenantService.Get(name);
            hostedSolution.UserService = _userService.Get(name);
            hostedSolution.QuotaService = _quotaService.Get(name);
            hostedSolution.TariffService = _tariffService.Get(name);
            hostedSolution.ClientTenantManager = _tenantManager.Get(name);
            hostedSolution.TenantUtil = _tenantUtil.Get(name);
            hostedSolution.SettingsManager = _dbSettingsManager.Get(name);
            hostedSolution.CoreSettings = _coreSettings.Get(name);
        }
    }

    [Scope(typeof(ConfigureHostedSolution))]
    public class HostedSolution
    {
        public string Region { get; set; }
        internal ITenantService TenantService { get; set; }
        internal IUserService UserService { get; set; }
        internal IQuotaService QuotaService { get; set; }
        internal ITariffService TariffService { get; set; }
        internal UserFormatter UserFormatter { get; set; }
        internal TenantManager ClientTenantManager { get; set; }
        internal TenantUtil TenantUtil { get; set; }
        internal DbSettingsManager SettingsManager { get; set; }
        internal CoreSettings CoreSettings { get; set; }

        public HostedSolution() { }

        public List<Tenant> GetTenants(DateTime from)
        {
            return TenantService.GetTenants(from).Select(AddRegion).ToList();
        }

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }

        public List<Tenant> FindTenants(string login, string passwordHash)
        {
            return !string.IsNullOrEmpty(passwordHash) && UserService.GetUserByPasswordHash(Tenant.DefaultTenant, login, passwordHash) == null
                ? throw new SecurityException("Invalid login or password.")
                : TenantService.GetTenants(login, passwordHash).Select(AddRegion).ToList();
        }

        public Tenant GetTenant(string domain)
        {
            return AddRegion(TenantService.GetTenant(domain));
        }

        public Tenant GetTenant(int id)
        {
            return AddRegion(TenantService.GetTenant(id));
        }

        public void CheckTenantAddress(string address)
        {
            TenantService.ValidateDomain(address);
        }

        public void RegisterTenant(TenantRegistrationInfo registrationInfo, out Tenant tenant)
        {
            if (registrationInfo == null)
            {
                throw new ArgumentNullException(nameof(registrationInfo));
            }
            if (string.IsNullOrEmpty(registrationInfo.Address))
            {
                throw new Exception("Address can not be empty");
            }
            if (string.IsNullOrEmpty(registrationInfo.Email))
            {
                throw new Exception("Account email can not be empty");
            }
            if (registrationInfo.FirstName == null)
            {
                throw new Exception("Account firstname can not be empty");
            }
            if (registrationInfo.LastName == null)
            {
                throw new Exception("Account lastname can not be empty");
            }
            if (!UserFormatter.IsValidUserName(registrationInfo.FirstName, registrationInfo.LastName))
            {
                throw new Exception("Incorrect firstname or lastname");
            }
            if (string.IsNullOrEmpty(registrationInfo.PasswordHash))
            {
                registrationInfo.PasswordHash = Guid.NewGuid().ToString();
            }

            // create tenant
            tenant = new Tenant(registrationInfo.Address.ToLowerInvariant())
            {
                Name = registrationInfo.Name,
                Language = registrationInfo.Culture.Name,
                TimeZone = registrationInfo.TimeZoneInfo.Id,
                HostedRegion = registrationInfo.HostedRegion,
                PartnerId = registrationInfo.PartnerId,
                AffiliateId = registrationInfo.AffiliateId,
                Campaign = registrationInfo.Campaign,
                Industry = registrationInfo.Industry,
                Spam = registrationInfo.Spam,
                Calls = registrationInfo.Calls
            };

            tenant = TenantService.SaveTenant(CoreSettings, tenant);

            // create user
            var user = new UserInfo
            {
                UserName = registrationInfo.Email.Substring(0, registrationInfo.Email.IndexOf('@')),
                LastName = registrationInfo.LastName,
                FirstName = registrationInfo.FirstName,
                Email = registrationInfo.Email,
                MobilePhone = registrationInfo.MobilePhone,
                WorkFromDate = TenantUtil.DateTimeNow(tenant.TimeZone),
                ActivationStatus = registrationInfo.ActivationStatus
            };
            user = UserService.SaveUser(tenant.Id, user);
            UserService.SetUserPasswordHash(tenant.Id, user.Id, registrationInfo.PasswordHash);
            UserService.SaveUserGroupRef(tenant.Id, new UserGroupRef(user.Id, Constants.GroupAdmin.ID, UserGroupRefType.Contains));

            // save tenant owner
            tenant.OwnerId = user.Id;
            tenant = TenantService.SaveTenant(CoreSettings, tenant);

            SettingsManager.SaveSettings(new TenantControlPanelSettings { LimitedAccess = registrationInfo.LimitedControlPanel }, tenant.Id);
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            return TenantService.SaveTenant(CoreSettings, tenant);
        }

        public void RemoveTenant(Tenant tenant)
        {
            TenantService.RemoveTenant(tenant.Id);
        }

        public string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, Guid userId)
        {
            var u = UserService.GetUser(tenantId, userId);

            return CreateAuthenticationCookie(cookieStorage, tenantId, u);
        }

        public Tariff GetTariff(int tenant, bool withRequestToPaymentSystem = true)
        {
            return TariffService.GetTariff(tenant, withRequestToPaymentSystem);
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            return ClientTenantManager.GetTenantQuota(tenant);
        }

        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return ClientTenantManager.GetTenantQuotas();
        }

        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            return ClientTenantManager.SaveTenantQuota(quota);
        }

        public void SetTariff(int tenant, bool paid)
        {
            var quota = QuotaService.GetTenantQuotas().FirstOrDefault(q => paid ? q.NonProfit : q.Trial);
            if (quota != null)
            {
                TariffService.SetTariff(tenant, new Tariff { QuotaId = quota.Tenant, DueDate = DateTime.MaxValue, });
            }
        }

        public void SetTariff(int tenant, Tariff tariff)
        {
            TariffService.SetTariff(tenant, tariff);
        }

        public void SaveButton(int tariffId, string partnerId, string buttonUrl)
        {
            TariffService.SaveButton(tariffId, partnerId, buttonUrl);
        }

        public IEnumerable<UserInfo> FindUsers(IEnumerable<Guid> userIds)
        {
            return UserService.GetUsersAllTenants(userIds);
        }

        private Tenant AddRegion(Tenant tenant)
        {
            if (tenant != null)
            {
                tenant.HostedRegion = Region;
            }

            return tenant;
        }

        private string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, UserInfo user)
        {
            if (user == null)
            {
                return null;
            }

            var tenantSettings = SettingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, Guid.Empty);
            var expires = tenantSettings.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(tenantSettings.LifeTime);
            var userSettings = SettingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, user.Id);

            return cookieStorage.EncryptCookie(tenantId, user.Id, tenantSettings.Index, expires, userSettings.Index);
        }
    }
}