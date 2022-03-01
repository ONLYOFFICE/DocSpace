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

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Studio.UserControls.FirstTime;

[Transient]
public class FirstTimeTenantSettings
{
    private readonly ILog _log;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly SettingsManager _settingsManager;
    private readonly UserManager _userManager;
    private readonly SetupInfo _setupInfo;
    private readonly SecurityContext _securityContext;
    private readonly PaymentManager _paymentManager;
    private readonly MessageService _messageService;
    private readonly LicenseReader _licenseReader;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IHttpClientFactory _clientFactory;

    public FirstTimeTenantSettings(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        SettingsManager settingsManager,
        UserManager userManager,
        SetupInfo setupInfo,
        SecurityContext securityContext,
        PaymentManager paymentManager,
        MessageService messageService,
        LicenseReader licenseReader,
        StudioNotifyService studioNotifyService,
        TimeZoneConverter timeZoneConverter,
        CoreBaseSettings coreBaseSettings,
        IHttpClientFactory clientFactory)
    {
        _log = options.CurrentValue;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _settingsManager = settingsManager;
        _userManager = userManager;
        _setupInfo = setupInfo;
        _securityContext = securityContext;
        _paymentManager = paymentManager;
        _messageService = messageService;
        _licenseReader = licenseReader;
        _studioNotifyService = studioNotifyService;
        _timeZoneConverter = timeZoneConverter;
        _coreBaseSettings = coreBaseSettings;
        _clientFactory = clientFactory;
    }

    public WizardSettings SaveData(WizardDto wizardModel)
    {
        try
        {
            var (email, passwordHash, lng, timeZone, promocode, amiid, subscribeFromSite) = wizardModel;

            var tenant = _tenantManager.GetCurrentTenant();
            var settings = _settingsManager.Load<WizardSettings>();
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
                Thread.Sleep(TimeSpan.FromSeconds(6)); // wait cache interval
                tenant = _tenantManager.GetTenant(tenant.Id);
                if (tenant.OwnerId == Guid.Empty)
                {
                    _log.Error(tenant.Id + ": owner id is empty.");
                }
            }

            var currentUser = _userManager.GetUsers(_tenantManager.GetCurrentTenant().OwnerId);

            if (!UserManagerWrapper.ValidateEmail(email))
            {
                throw new Exception(Resource.EmailAndPasswordIncorrectEmail);
            }

            if (string.IsNullOrEmpty(passwordHash))
                throw new Exception(Resource.ErrorPasswordEmpty);

            _securityContext.SetUserPasswordHash(currentUser.Id, passwordHash);

            email = email.Trim();
            if (currentUser.Email != email)
            {
                currentUser.Email = email;
                currentUser.ActivationStatus = EmployeeActivationStatus.NotActivated;
            }
            _userManager.SaveUserInfo(currentUser);

            if (!string.IsNullOrWhiteSpace(promocode))
            {
                try
                {
                    _paymentManager.ActivateKey(promocode);
                }
                catch (Exception err)
                {
                    _log.Error("Incorrect Promo: " + promocode, err);
                    throw new Exception(Resource.EmailAndPasswordIncorrectPromocode);
                }
            }

            if (RequestLicense)
            {
                TariffSettings.SetLicenseAccept(_settingsManager);
                _messageService.Send(MessageAction.LicenseKeyUploaded);

                _licenseReader.RefreshLicense();
            }

            settings.Completed = true;
            _settingsManager.Save(settings);

            TrySetLanguage(tenant, lng);

            tenant.TimeZone = _timeZoneConverter.GetTimeZone(timeZone).Id;

            _tenantManager.SaveTenant(tenant);

            _studioNotifyService.SendCongratulations(currentUser);
            _studioNotifyService.SendRegData(currentUser);

            if (subscribeFromSite && _tenantExtra.Opensource && !_coreBaseSettings.CustomMode)
            {
                SubscribeFromSite(currentUser);
            }

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
            _log.Error(ex);
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
        if (string.IsNullOrEmpty(lng)) return;

        try
        {
            var culture = CultureInfo.GetCultureInfo(lng);
            tenant.Language = culture.Name;
        }
        catch (Exception err)
        {
            _log.Error(err);
        }
    }

    private static string _amiId;

    private bool IncorrectAmiId(string customAmiId)
    {
        customAmiId = (customAmiId ?? "").Trim();
        if (string.IsNullOrEmpty(customAmiId)) return true;

        if (string.IsNullOrEmpty(_amiId))
        {
            var getAmiIdUrl = _setupInfo.AmiMetaUrl + "instance-id";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(getAmiIdUrl);

            try
            {
                var httpClient = _clientFactory.CreateClient();
                using (var response = httpClient.Send(request))
                using (var responseStream = response.Content.ReadAsStream())
                using (var reader = new StreamReader(responseStream))
                {
                    _amiId = reader.ReadToEnd();
                }

                _log.Debug("Instance id: " + _amiId);
            }
            catch (Exception e)
            {
                _log.Error("Request AMI id", e);
            }
        }

        return string.IsNullOrEmpty(_amiId) || _amiId != customAmiId;
    }

    private void SubscribeFromSite(UserInfo user)
    {
        try
        {
            var url = (_setupInfo.TeamlabSiteRedirect ?? "").Trim().TrimEnd('/');

            if (string.IsNullOrEmpty(url)) return;

            url += "/post.ashx";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
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

            _log.Debug("Subscribe response: " + response);//toto write

        }
        catch (Exception e)
        {
            _log.Error("Subscribe request", e);
        }
    }
}