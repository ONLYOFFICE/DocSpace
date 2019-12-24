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
using System.Configuration;
using System.Linq;
using System.Security;

using ASC.Common.Logging;
using ASC.Core.Billing;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Data;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Core
{
    public class HostedSolution
    {
        private readonly ITenantService tenantService;
        private readonly IUserService userService;
        private readonly IQuotaService quotaService;
        private readonly ITariffService tariffService;
        private readonly TenantManager clientTenantManager;
        private readonly DbSettingsManager settingsManager;
        private readonly CoreSettings coreSettings;

        public TenantUtil TenantUtil { get; }
        public string Region
        {
            get;
            private set;
        }

        public HostedSolution(
            IConfiguration configuration,
            ConnectionStringSettings connectionString,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options,
            TenantUtil tenantUtil,
            TenantDomainValidator tenantDomainValidator,
            TenantDbContext tenantDbContext,
            UserDbContext userDbContext,
            CoreDbContext coreDbContext)
            : this(configuration, connectionString, tariffServiceStorage, options, tenantUtil, tenantDomainValidator, tenantDbContext, userDbContext, coreDbContext, null)
        {
        }

        public HostedSolution(
            IConfiguration configuration,
            ConnectionStringSettings connectionString,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options,
            TenantUtil tenantUtil,
            TenantDomainValidator tenantDomainValidator,
            TenantDbContext tenantDbContext,
            UserDbContext userDbContext,
            CoreDbContext coreDbContext,
            string region)
        {
            tenantService = new DbTenantService(tenantDbContext, tenantDomainValidator);
            var baseSettings = new CoreBaseSettings(configuration);
            coreSettings = new CoreSettings(tenantService, baseSettings, configuration);

            userService = new EFUserService(userDbContext);
            quotaService = new DbQuotaService(coreDbContext);
            tariffService = new TariffService(quotaService, tenantService, baseSettings, coreSettings, configuration, coreDbContext, tariffServiceStorage, options);
            clientTenantManager = new TenantManager(tenantService, quotaService, tariffService, null, baseSettings, coreSettings);
            settingsManager = new DbSettingsManager(connectionString);
            TenantUtil = tenantUtil;
            Region = region ?? string.Empty;
        }

        public List<Tenant> GetTenants(DateTime from)
        {
            return tenantService.GetTenants(from).Select(AddRegion).ToList();
        }

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }

        public List<Tenant> FindTenants(string login, string password)
        {
            var hash = !string.IsNullOrEmpty(password) ? Hasher.Base64Hash(password, HashAlg.SHA256) : null;
            if (hash != null && userService.GetUser(Tenant.DEFAULT_TENANT, login, hash) == null)
            {
                throw new SecurityException("Invalid login or password.");
            }
            return tenantService.GetTenants(login, hash).Select(AddRegion).ToList();
        }

        public Tenant GetTenant(string domain)
        {
            return AddRegion(tenantService.GetTenant(domain));
        }

        public Tenant GetTenant(int id)
        {
            return AddRegion(tenantService.GetTenant(id));
        }

        public void CheckTenantAddress(string address)
        {
            tenantService.ValidateDomain(address);
        }

        public void RegisterTenant(TenantRegistrationInfo ri, out Tenant tenant)
        {
            if (ri == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(ri.Address)) throw new Exception("Address can not be empty");

            if (string.IsNullOrEmpty(ri.Email)) throw new Exception("Account email can not be empty");
            if (ri.FirstName == null) throw new Exception("Account firstname can not be empty");
            if (ri.LastName == null) throw new Exception("Account lastname can not be empty");
            if (!UserFormatter.IsValidUserName(ri.FirstName, ri.LastName)) throw new Exception("Incorrect firstname or lastname");

            if (string.IsNullOrEmpty(ri.Password)) ri.Password = Crypto.GeneratePassword(6);

            // create tenant
            tenant = new Tenant(ri.Address.ToLowerInvariant())
            {
                Name = ri.Name,
                Language = ri.Culture.Name,
                TimeZone = ri.TimeZoneInfo.Id,
                HostedRegion = ri.HostedRegion,
                PartnerId = ri.PartnerId,
                AffiliateId = ri.AffiliateId,
                Industry = ri.Industry,
                Spam = ri.Spam,
                Calls = ri.Calls
            };

            tenant = tenantService.SaveTenant(coreSettings, tenant);

            // create user
            var user = new UserInfo
            {
                UserName = ri.Email.Substring(0, ri.Email.IndexOf('@')),
                LastName = ri.LastName,
                FirstName = ri.FirstName,
                Email = ri.Email,
                MobilePhone = ri.MobilePhone,
                WorkFromDate = TenantUtil.DateTimeNow(tenant.TimeZone),
                ActivationStatus = ri.ActivationStatus
            };
            user = userService.SaveUser(tenant.TenantId, user);
            userService.SetUserPassword(tenant.TenantId, user.ID, ri.Password);
            userService.SaveUserGroupRef(tenant.TenantId, new UserGroupRef(user.ID, Constants.GroupAdmin.ID, UserGroupRefType.Contains));

            // save tenant owner
            tenant.OwnerId = user.ID;
            tenant = tenantService.SaveTenant(coreSettings, tenant);

            settingsManager.SaveSettings(new TenantAnalyticsSettings() { Analytics = ri.Analytics }, tenant.TenantId);
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            return tenantService.SaveTenant(coreSettings, tenant);
        }

        public void RemoveTenant(Tenant tenant)
        {
            tenantService.RemoveTenant(tenant.TenantId);
        }

        public string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, string login, string password)
        {
            var passwordhash = Hasher.Base64Hash(password, HashAlg.SHA256);
            var u = userService.GetUser(tenantId, login, passwordhash);
            return u != null ? CreateAuthenticationCookie(cookieStorage, tenantId, u.ID, login, passwordhash) : null;
        }

        public string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, Guid userId)
        {
            var u = userService.GetUser(tenantId, userId);
            var password = userService.GetUserPassword(tenantId, userId);
            var passwordhash = Hasher.Base64Hash(password, HashAlg.SHA256);
            return u != null ? CreateAuthenticationCookie(cookieStorage, tenantId, userId, u.Email, passwordhash) : null;
        }

        private string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, Guid userId, string login, string passwordhash)
        {
            var tenantSettings = settingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, Guid.Empty);
            var expires = tenantSettings.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(tenantSettings.LifeTime);
            var userSettings = settingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, userId);
            return cookieStorage.EncryptCookie(tenantId, userId, login, passwordhash, tenantSettings.Index, expires, userSettings.Index);
        }

        public Tariff GetTariff(int tenant, bool withRequestToPaymentSystem = true)
        {
            return tariffService.GetTariff(tenant, withRequestToPaymentSystem);
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            return clientTenantManager.GetTenantQuota(tenant);
        }

        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return clientTenantManager.GetTenantQuotas();
        }

        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            return clientTenantManager.SaveTenantQuota(quota);
        }

        public void SetTariff(int tenant, bool paid)
        {
            var quota = quotaService.GetTenantQuotas().FirstOrDefault(q => paid ? q.NonProfit : q.Trial);
            if (quota != null)
            {
                tariffService.SetTariff(tenant, new Tariff { QuotaId = quota.Id, DueDate = DateTime.MaxValue, });
            }
        }

        public void SetTariff(int tenant, Tariff tariff)
        {
            tariffService.SetTariff(tenant, tariff);
        }

        public void SaveButton(int tariffId, string partnerId, string buttonUrl)
        {
            tariffService.SaveButton(tariffId, partnerId, buttonUrl);
        }


        private Tenant AddRegion(Tenant tenant)
        {
            if (tenant != null)
            {
                tenant.HostedRegion = Region;
            }
            return tenant;
        }
    }
}