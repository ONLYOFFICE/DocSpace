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

namespace ASC.Web.Studio.UserControls.FirstTime;

[Transient]
public class FirstTimeTenantSettings
{
    private readonly ILogger<FirstTimeTenantSettings> _log;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly SettingsManager _settingsManager;
    private readonly UserManager _userManager;
    private readonly SetupInfo _setupInfo;
    private readonly SecurityContext _securityContext;
    private readonly MessageService _messageService;
    private readonly LicenseReader _licenseReader;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IHttpClientFactory _clientFactory;
    private readonly CookiesManager _cookiesManager;

    public FirstTimeTenantSettings(
        ILogger<FirstTimeTenantSettings> logger,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        SettingsManager settingsManager,
        UserManager userManager,
        SetupInfo setupInfo,
        SecurityContext securityContext,
        MessageService messageService,
        LicenseReader licenseReader,
        StudioNotifyService studioNotifyService,
        TimeZoneConverter timeZoneConverter,
        CoreBaseSettings coreBaseSettings,
        IHttpClientFactory clientFactory,
        CookiesManager cookiesManager)
    {
        _log = logger;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _settingsManager = settingsManager;
        _userManager = userManager;
        _setupInfo = setupInfo;
        _securityContext = securityContext;
        _messageService = messageService;
        _licenseReader = licenseReader;
        _studioNotifyService = studioNotifyService;
        _timeZoneConverter = timeZoneConverter;
        _coreBaseSettings = coreBaseSettings;
        _clientFactory = clientFactory;
        _cookiesManager = cookiesManager;
    }

    public async Task<WizardSettings> SaveDataAsync(WizardRequestsDto inDto)
    {
        try
        {
            var (email, passwordHash, lng, timeZone, amiid, subscribeFromSite) = inDto;

            var tenant = await _tenantManager.GetCurrentTenantAsync();
            var settings = await _settingsManager.LoadAsync<WizardSettings>();
            if (settings.Completed)
            {
                throw new Exception("Wizard passed.");
            }

            if (!string.IsNullOrEmpty(_setupInfo.AmiMetaUrl) && IncorrectAmiId(amiid))
            {
                //throw new Exception(Resource.EmailAndPasswordIncorrectAmiId); TODO
            }

            if (tenant.OwnerId == Guid.Empty)
            {
                await Task.Delay(TimeSpan.FromSeconds(6));// wait cache interval
                tenant = await _tenantManager.GetTenantAsync(tenant.Id);
                if (tenant.OwnerId == Guid.Empty)
                {
                    _log.ErrorOwnerEmpty(tenant.Id);
                }
            }

            var currentUser = await _userManager.GetUsersAsync((await _tenantManager.GetCurrentTenantAsync()).OwnerId);

            if (!UserManagerWrapper.ValidateEmail(email))
            {
                throw new Exception(Resource.EmailAndPasswordIncorrectEmail);
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new Exception(Resource.ErrorPasswordEmpty);
            }

            await _securityContext.SetUserPasswordHashAsync(currentUser.Id, passwordHash);

            email = email.Trim();
            if (currentUser.Email != email)
            {
                currentUser.Email = email;
                currentUser.ActivationStatus = EmployeeActivationStatus.NotActivated;
            }

            await _userManager.UpdateUserInfoAsync(currentUser);

            if (_tenantExtra.EnableTariffSettings && _tenantExtra.Enterprise)
            {
                await TariffSettings.SetLicenseAcceptAsync(_settingsManager);
                await _messageService.SendAsync(MessageAction.LicenseKeyUploaded);

                await _licenseReader.RefreshLicenseAsync();
            }

            settings.Completed = true;
            await _settingsManager.SaveAsync(settings);

            TrySetLanguage(tenant, lng);

            tenant.TimeZone = _timeZoneConverter.GetTimeZone(timeZone).Id;

            await _tenantManager.SaveTenantAsync(tenant);

            await _studioNotifyService.SendCongratulationsAsync(currentUser);
            await _studioNotifyService.SendRegDataAsync(currentUser);

            if (subscribeFromSite && _tenantExtra.Opensource && !_coreBaseSettings.CustomMode)
            {
                SubscribeFromSite(currentUser);
            }

            await _cookiesManager.AuthenticateMeAndSetCookiesAsync(currentUser.TenantId, currentUser.Id, MessageAction.LoginSuccess);

            return settings;
        }
        catch (BillingNotFoundException)
        {
            throw new Exception(UserControlsCommonResource.LicenseKeyNotFound);
        }
        catch (BillingNotConfiguredException)
        {
            throw new Exception(UserControlsCommonResource.LicenseKeyNotCorrect);
        }
        catch (BillingException)
        {
            throw new Exception(UserControlsCommonResource.LicenseException);
        }
        catch (Exception ex)
        {
            _log.ErrorFirstTimeTenantSettings(ex);
            throw;
        }
    }

    public bool RequestLicense
    {
        get
        {
            return _tenantExtra.EnableTariffSettings && _tenantExtra.Enterprise
                && !File.Exists(_licenseReader.LicensePath);
        }
    }

    private void TrySetLanguage(Tenant tenant, string lng)
    {
        if (string.IsNullOrEmpty(lng))
        {
            return;
        }

        try
        {
            var culture = CultureInfo.GetCultureInfo(lng);
            tenant.Language = culture.Name;
        }
        catch (Exception err)
        {
            _log.ErrorTrySetLanguage(err);
        }
    }

    private static string _amiId;

    private bool IncorrectAmiId(string customAmiId)
    {
        customAmiId = (customAmiId ?? "").Trim();
        if (string.IsNullOrEmpty(customAmiId))
        {
            return true;
        }

        if (string.IsNullOrEmpty(_amiId))
        {
            var getAmiIdUrl = _setupInfo.AmiMetaUrl + "instance-id";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(getAmiIdUrl)
            };

            try
            {
                var httpClient = _clientFactory.CreateClient();
                using (var response = httpClient.Send(request))
                using (var responseStream = response.Content.ReadAsStream())
                using (var reader = new StreamReader(responseStream))
                {
                    _amiId = reader.ReadToEnd();
                }

                _log.DebugInstanceId(_amiId);
            }
            catch (Exception e)
            {
                _log.ErrorRequestAMIId(e);
            }
        }

        return string.IsNullOrEmpty(_amiId) || _amiId != customAmiId;
    }

    private void SubscribeFromSite(UserInfo user)
    {
        try
        {
            var url = (_setupInfo.TeamlabSiteRedirect ?? "").Trim().TrimEnd('/');

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            url += "/post.ashx";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url)
            };
            var values = new NameValueCollection
                    {
                        { "type", "sendsubscription" },
                        { "subscr_type", "Opensource" },
                        { "email", user.Email }
                    };
            var data = JsonSerializer.Serialize(values);
            request.Content = new StringContent(data);

            var httpClient = _clientFactory.CreateClient();
            using var response = httpClient.Send(request);

            _log.DebugSubscribeResponse(response);//toto write

        }
        catch (Exception e)
        {
            _log.ErrorSubscribeRequest(e);
        }
    }
}