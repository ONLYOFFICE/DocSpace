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

namespace ASC.People.Api;

public class ThirdpartyController : ApiControllerBase
{
    private readonly AccountLinker _accountLinker;
    private readonly CookiesManager _cookiesManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly MobileDetector _mobileDetector;
    private readonly PersonalSettingsHelper _personalSettingsHelper;
    private readonly ProviderManager _providerManager;
    private readonly Signature _signature;
    private readonly UserHelpTourHelper _userHelpTourHelper;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly UserPhotoManager _userPhotoManager;
    private readonly AuthContext _authContext;
    private readonly SecurityContext _securityContext;
    private readonly MessageService _messageService;
    private readonly UserManager _userManager;
    private readonly MessageTarget _messageTarget;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly TenantManager _tenantManager;

    public ThirdpartyController(
        AccountLinker accountLinker,
        CookiesManager cookiesManager,
        CoreBaseSettings coreBaseSettings,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        IHttpClientFactory httpClientFactory,
        InstanceCrypto instanceCrypto,
        MobileDetector mobileDetector,
        PersonalSettingsHelper personalSettingsHelper,
        ProviderManager providerManager,
        Signature signature,
        UserHelpTourHelper userHelpTourHelper,
        UserManagerWrapper userManagerWrapper,
        UserPhotoManager userPhotoManager,
        AuthContext authContext,
        SecurityContext securityContext,
        MessageService messageService,
        UserManager userManager,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        TenantManager tenantManager)
    {
        _accountLinker = accountLinker;
        _cookiesManager = cookiesManager;
        _coreBaseSettings = coreBaseSettings;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _httpClientFactory = httpClientFactory;
        _instanceCrypto = instanceCrypto;
        _mobileDetector = mobileDetector;
        _personalSettingsHelper = personalSettingsHelper;
        _providerManager = providerManager;
        _signature = signature;
        _userHelpTourHelper = userHelpTourHelper;
        _userManagerWrapper = userManagerWrapper;
        _userPhotoManager = userPhotoManager;
        _authContext = authContext;
        _securityContext = securityContext;
        _messageService = messageService;
        _userManager = userManager;
        _messageTarget = messageTarget;
        _studioNotifyService = studioNotifyService;
        _tenantManager = tenantManager;
    }

    [AllowAnonymous, AllowNotPayment]
    [HttpGet("thirdparty/providers")]
    public async Task<ICollection<AccountInfoDto>> GetAuthProvidersAsync(bool inviteView, bool settingsView, string clientCallback, string fromOnly)
    {
        ICollection<AccountInfoDto> infos = new List<AccountInfoDto>();
        IEnumerable<LoginProfile> linkedAccounts = new List<LoginProfile>();

        if (_authContext.IsAuthenticated)
        {
            linkedAccounts = await _accountLinker.GetLinkedProfilesAsync(_authContext.CurrentAccount.ID.ToString());
        }

        fromOnly = string.IsNullOrWhiteSpace(fromOnly) ? string.Empty : fromOnly.ToLower();

        foreach (var provider in ProviderManager.AuthProviders.Where(provider => string.IsNullOrEmpty(fromOnly) || fromOnly == provider || (provider == "google" && fromOnly == "openid")))
        {
            if (inviteView && ProviderManager.InviteExceptProviders.Contains(provider))
            {
                continue;
            }
            var loginProvider = _providerManager.GetLoginProvider(provider);
            if (loginProvider != null && loginProvider.IsEnabled)
            {

                var url = VirtualPathUtility.ToAbsolute("~/login.ashx") + $"?auth={provider}";
                var mode = settingsView || inviteView || (!_mobileDetector.IsMobile() && !Request.DesktopApp())
                        ? $"&mode=popup&callback={clientCallback}"
                        : "&mode=Redirect&desktop=true";

                infos.Add(new AccountInfoDto
                {
                    Linked = linkedAccounts.Any(x => x.Provider == provider),
                    Provider = provider,
                    Url = url + mode
                });
            }
        }

        return infos;
    }

    [HttpPut("thirdparty/linkaccount")]
    public async Task LinkAccountAsync(LinkAccountRequestDto inDto)
    {
        var profile = new LoginProfile(_signature, _instanceCrypto, inDto.SerializedProfile);

        if (!(_coreBaseSettings.Standalone || (await _tenantManager.GetCurrentTenantQuotaAsync()).Oauth))
        {
            throw new Exception("ErrorNotAllowedOption");
        }

        if (string.IsNullOrEmpty(profile.AuthorizationError))
        {
            await _accountLinker.AddLinkAsync(_securityContext.CurrentAccount.ID.ToString(), profile);
            await _messageService.SendAsync(MessageAction.UserLinkedSocialAccount, GetMeaningfulProviderName(profile.Provider));
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

    [AllowAnonymous]
    [HttpPost("thirdparty/signup")]
    public async Task SignupAccountAsync(SignupAccountRequestDto inDto)
    {
        var employeeType = inDto.EmployeeType ?? EmployeeType.RoomAdmin;
        var passwordHash = inDto.PasswordHash;
        var mustChangePassword = false;
        if (string.IsNullOrEmpty(passwordHash))
        {
            passwordHash = UserManagerWrapper.GeneratePassword();
            mustChangePassword = true;
        }

        var thirdPartyProfile = new LoginProfile(_signature, _instanceCrypto, inDto.SerializedProfile);
        if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
        {
            // ignore cancellation
            if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
            {
                throw new Exception(thirdPartyProfile.AuthorizationError);
            }

            return;
        }

        if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        var userID = Guid.Empty;
        try
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(Core.Configuration.Constants.CoreSystem);
            var newUser = await CreateNewUser(GetFirstName(inDto, thirdPartyProfile), GetLastName(inDto, thirdPartyProfile), GetEmailAddress(inDto, thirdPartyProfile), passwordHash, employeeType, false);
            var messageAction = employeeType == EmployeeType.RoomAdmin ? MessageAction.UserCreatedViaInvite : MessageAction.GuestCreatedViaInvite;
            await _messageService.SendAsync(MessageInitiator.System, messageAction, _messageTarget.Create(newUser.Id), newUser.DisplayUserName(false, _displayUserSettingsHelper));
            userID = newUser.Id;
            if (!string.IsNullOrEmpty(thirdPartyProfile.Avatar))
            {
                await SaveContactImage(userID, thirdPartyProfile.Avatar);
            }

            await _accountLinker.AddLinkAsync(userID.ToString(), thirdPartyProfile);
        }
        finally
        {
            _securityContext.Logout();
        }

        var user = await _userManager.GetUsersAsync(userID);

        await _cookiesManager.AuthenticateMeAndSetCookiesAsync(user.Tenant, user.Id, MessageAction.LoginSuccess);

        await _studioNotifyService.UserHasJoinAsync();

        if (mustChangePassword)
        {
            await _studioNotifyService.UserPasswordChangeAsync(user);
        }

        _userHelpTourHelper.IsNewUser = true;
        if (_coreBaseSettings.Personal)
        {
            _personalSettingsHelper.IsNewUser = true;
        }
    }

    [HttpDelete("thirdparty/unlinkaccount")]
    public async Task UnlinkAccountAsync(string provider)
    {
        await _accountLinker.RemoveProviderAsync(_securityContext.CurrentAccount.ID.ToString(), provider);

        await _messageService.SendAsync(MessageAction.UserUnlinkedSocialAccount, GetMeaningfulProviderName(provider));
    }

    private async Task<UserInfo> CreateNewUser(string firstName, string lastName, string email, string passwordHash, EmployeeType employeeType, bool fromInviteLink)
    {
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

        if (_coreBaseSettings.Personal)
        {
            userInfo.ActivationStatus = EmployeeActivationStatus.Activated;
            userInfo.CultureName = _coreBaseSettings.CustomMode ? "ru-RU" : CultureInfo.CurrentUICulture.Name;
        }

        return await _userManagerWrapper.AddUserAsync(userInfo, passwordHash, true, true, employeeType, fromInviteLink);
    }

    private async Task SaveContactImage(Guid userID, string url)
    {
        using (var memstream = new MemoryStream())
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url)
            };

            var httpClient = _httpClientFactory.CreateClient();
            using (var response = httpClient.Send(request))
            using (var stream = response.Content.ReadAsStream())
            {
                var buffer = new byte[512];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memstream.Write(buffer, 0, bytesRead);
                }

                var bytes = memstream.ToArray();

                await _userPhotoManager.SaveOrUpdatePhoto(userID, bytes);
            }
        }
    }

    private string GetEmailAddress(SignupAccountRequestDto inDto)
    {
        if (!string.IsNullOrEmpty(inDto.Email))
        {
            return inDto.Email.Trim();
        }

        return string.Empty;
    }

    private string GetEmailAddress(SignupAccountRequestDto inDto, LoginProfile account)
    {
        var value = GetEmailAddress(inDto);

        return string.IsNullOrEmpty(value) ? account.EMail : value;
    }

    private string GetFirstName(SignupAccountRequestDto inDto)
    {
        var value = string.Empty;
        if (!string.IsNullOrEmpty(inDto.FirstName))
        {
            value = inDto.FirstName.Trim();
        }

        return HtmlUtil.GetText(value);
    }

    private string GetFirstName(SignupAccountRequestDto inDto, LoginProfile account)
    {
        var value = GetFirstName(inDto);

        return string.IsNullOrEmpty(value) ? account.FirstName : value;
    }

    private string GetLastName(SignupAccountRequestDto inDto)
    {
        var value = string.Empty;
        if (!string.IsNullOrEmpty(inDto.LastName))
        {
            value = inDto.LastName.Trim();
        }

        return HtmlUtil.GetText(value);
    }

    private string GetLastName(SignupAccountRequestDto inDto, LoginProfile account)
    {
        var value = GetLastName(inDto);

        return string.IsNullOrEmpty(value) ? account.LastName : value;
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
}
