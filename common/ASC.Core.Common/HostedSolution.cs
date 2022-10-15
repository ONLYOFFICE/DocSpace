// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using Constants = ASC.Core.Users.Constants;

namespace ASC.Core;


[Scope]
public class HostedSolution
{
    internal ITenantService TenantService { get; set; }
    internal IUserService UserService { get; set; }
    internal IQuotaService QuotaService { get; set; }
    internal ITariffService TariffService { get; set; }
    internal UserFormatter UserFormatter { get; set; }
    internal TenantManager ClientTenantManager { get; set; }
    internal TenantUtil TenantUtil { get; set; }
    internal DbSettingsManager SettingsManager { get; set; }
    internal CoreSettings CoreSettings { get; set; }

    public string Region { get; set; }

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
        if (!string.IsNullOrEmpty(passwordHash) && UserService.GetUserByPasswordHash(Tenant.DefaultTenant, login, passwordHash) == null)
        {
            throw new SecurityException("Invalid login or password.");
        }

        return TenantService.GetTenants(login, passwordHash).Select(AddRegion).ToList();
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
        ArgumentNullException.ThrowIfNull(registrationInfo);

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

    private string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, UserInfo user)
    {
        if (user == null)
        {
            return null;
        }

        var tenantSettings = SettingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, Guid.Empty);
        var expires = tenantSettings.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(tenantSettings.LifeTime);
        var userSettings = SettingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, user.Id);

        return cookieStorage.EncryptCookie(tenantId, user.Id, tenantSettings.Index, expires, userSettings.Index, 0);
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
            TariffService.SetTariff(tenant, new Tariff { Quotas = new List<Quota> { new Quota(quota.Tenant, 1) }, DueDate = DateTime.MaxValue, });
        }
    }

    public void SetTariff(int tenant, Tariff tariff)
    {
        TariffService.SetTariff(tenant, tariff);
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
}
