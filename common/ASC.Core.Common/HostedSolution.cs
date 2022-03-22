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
        hostedSolution._userFormatter = _userFormatter;
        hostedSolution._tenantService = _tenantService.Value;
        hostedSolution._userService = _userService.Value;
        hostedSolution._quotaService = _quotaService.Value;
        hostedSolution._tariffService = _tariffService.Value;
        hostedSolution._clientTenantManager = _tenantManager.Value;
        hostedSolution._tenantUtil = _tenantUtil.Value;
        hostedSolution._settingsManager = _dbSettingsManager.Value;
        hostedSolution._coreSettings = _coreSettings.Value;
    }

    public void Configure(string name, HostedSolution hostedSolution)
    {
        Configure(hostedSolution);
        hostedSolution.Region = name;
        hostedSolution._tenantService = _tenantService.Get(name);
        hostedSolution._userService = _userService.Get(name);
        hostedSolution._quotaService = _quotaService.Get(name);
        hostedSolution._tariffService = _tariffService.Get(name);
        hostedSolution._clientTenantManager = _tenantManager.Get(name);
        hostedSolution._tenantUtil = _tenantUtil.Get(name);
        hostedSolution._settingsManager = _dbSettingsManager.Get(name);
        hostedSolution._coreSettings = _coreSettings.Get(name);
    }
}

[Scope(typeof(ConfigureHostedSolution))]
public class HostedSolution
{
    internal ITenantService _tenantService;
    internal IUserService _userService;
    internal IQuotaService _quotaService;
    internal ITariffService _tariffService;
    internal UserFormatter _userFormatter;
    internal TenantManager _clientTenantManager;
    internal TenantUtil _tenantUtil;
    internal DbSettingsManager _settingsManager;
    internal CoreSettings _coreSettings;

    public string Region { get; set; }

    public HostedSolution() { }

    public List<Tenant> GetTenants(DateTime from)
    {
        return _tenantService.GetTenants(from).Select(AddRegion).ToList();
    }

    public List<Tenant> FindTenants(string login)
    {
        return FindTenants(login, null);
    }

    public List<Tenant> FindTenants(string login, string passwordHash)
    {
        if (!string.IsNullOrEmpty(passwordHash) && _userService.GetUserByPasswordHash(Tenant.DefaultTenant, login, passwordHash) == null)
        {
            throw new SecurityException("Invalid login or password.");
        }

        return _tenantService.GetTenants(login, passwordHash).Select(AddRegion).ToList();
    }

    public Tenant GetTenant(string domain)
    {
        return AddRegion(_tenantService.GetTenant(domain));
    }

    public Tenant GetTenant(int id)
    {
        return AddRegion(_tenantService.GetTenant(id));
    }

    public void CheckTenantAddress(string address)
    {
        _tenantService.ValidateDomain(address);
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
        if (!_userFormatter.IsValidUserName(registrationInfo.FirstName, registrationInfo.LastName))
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

        tenant = _tenantService.SaveTenant(_coreSettings, tenant);

        // create user
        var user = new UserInfo
        {
            UserName = registrationInfo.Email.Substring(0, registrationInfo.Email.IndexOf('@')),
            LastName = registrationInfo.LastName,
            FirstName = registrationInfo.FirstName,
            Email = registrationInfo.Email,
            MobilePhone = registrationInfo.MobilePhone,
            WorkFromDate = _tenantUtil.DateTimeNow(tenant.TimeZone),
            ActivationStatus = registrationInfo.ActivationStatus
        };

        user = _userService.SaveUser(tenant.Id, user);
        _userService.SetUserPasswordHash(tenant.Id, user.Id, registrationInfo.PasswordHash);
        _userService.SaveUserGroupRef(tenant.Id, new UserGroupRef(user.Id, Constants.GroupAdmin.ID, UserGroupRefType.Contains));

        // save tenant owner
        tenant.OwnerId = user.Id;
        tenant = _tenantService.SaveTenant(_coreSettings, tenant);

        _settingsManager.SaveSettings(new TenantControlPanelSettings { LimitedAccess = registrationInfo.LimitedControlPanel }, tenant.Id);
    }

    public Tenant SaveTenant(Tenant tenant)
    {
        return _tenantService.SaveTenant(_coreSettings, tenant);
    }

    public void RemoveTenant(Tenant tenant)
    {
        _tenantService.RemoveTenant(tenant.Id);
    }

    public string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, Guid userId)
    {
        var u = _userService.GetUser(tenantId, userId);

        return CreateAuthenticationCookie(cookieStorage, tenantId, u);
    }

    private string CreateAuthenticationCookie(CookieStorage cookieStorage, int tenantId, UserInfo user)
    {
        if (user == null)
        {
            return null;
        }

        var tenantSettings = _settingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, Guid.Empty);
        var expires = tenantSettings.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(tenantSettings.LifeTime);
        var userSettings = _settingsManager.LoadSettingsFor<TenantCookieSettings>(tenantId, user.Id);

        return cookieStorage.EncryptCookie(tenantId, user.Id, tenantSettings.Index, expires, userSettings.Index);
    }

    public Tariff GetTariff(int tenant, bool withRequestToPaymentSystem = true)
    {
        return _tariffService.GetTariff(tenant, withRequestToPaymentSystem);
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        return _clientTenantManager.GetTenantQuota(tenant);
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return _clientTenantManager.GetTenantQuotas();
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        return _clientTenantManager.SaveTenantQuota(quota);
    }

    public void SetTariff(int tenant, bool paid)
    {
        var quota = _quotaService.GetTenantQuotas().FirstOrDefault(q => paid ? q.NonProfit : q.Trial);
        if (quota != null)
        {
            _tariffService.SetTariff(tenant, new Tariff { QuotaId = quota.Tenant, DueDate = DateTime.MaxValue, });
        }
    }

    public void SetTariff(int tenant, Tariff tariff)
    {
        _tariffService.SetTariff(tenant, tariff);
    }

    public void SaveButton(int tariffId, string partnerId, string buttonUrl)
    {
        _tariffService.SaveButton(tariffId, partnerId, buttonUrl);
    }
    public IEnumerable<UserInfo> FindUsers(IEnumerable<Guid> userIds)
    {
        return _userService.GetUsersAllTenants(userIds);
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
