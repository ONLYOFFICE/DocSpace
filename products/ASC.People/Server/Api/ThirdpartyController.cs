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
    private readonly InvitationLinkService _invitationLinkService;
    private readonly FileSecurity _fileSecurity;
    private readonly UsersInRoomChecker _usersInRoomChecker;

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
        TenantManager tenantManager,
        InvitationLinkService invitationLinkService,
        FileSecurity fileSecurity,
        UsersInRoomChecker usersInRoomChecker)
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
        _invitationLinkService = invitationLinkService;
        _fileSecurity = fileSecurity;
        _usersInRoomChecker = usersInRoomChecker;
    }

    /// <summary>
    /// Returns a list of the available third-party accounts.
    /// </summary>
    /// <short>Get third-party accounts</short>
    /// <category>Third-party accounts</category>
    /// <param type="System.Boolean, System" name="inviteView">Specifies whether to return providers that are available for invitation links, i.e. the user can login or register through these providers</param>
    /// <param type="System.Boolean, System" name="settingsView">Specifies whether to return URLs in the format that is used on the Settings page</param>
    /// <param type="System.String, System" name="clientCallback">Method that is called after authorization</param>
    /// <param type="System.String, System" name="fromOnly">Provider name if the response only from this provider is needed</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.AccountInfoDto, ASC.People">List of third-party accounts</returns>
    /// <path>api/2.0/people/thirdparty/providers</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <collection>list</collection>
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

    /// <summary>
    /// Links a third-party account specified in the request to the user profile.
    /// </summary>
    /// <short>
    /// Link a third-pary account
    /// </short>
    /// <category>Third-party accounts</category>
    /// <param type="ASC.People.ApiModels.RequestDto.LinkAccountRequestDto, ASC.People" name="inDto">Request parameters for linking accounts</param>
    /// <path>api/2.0/people/thirdparty/linkaccount</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a third-party account with the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Create a third-pary account
    /// </short>
    /// <category>Third-party accounts</category>
    /// <param type="ASC.People.ApiModels.RequestDto.SignupAccountRequestDto, ASC.People" name="inDto">Request parameters for creating a third-party account</param>
    /// <path>api/2.0/people/thirdparty/signup</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns></returns>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpPost("thirdparty/signup")]
    public async Task SignupAccountAsync(SignupAccountRequestDto inDto)
    {
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

        var linkData = await _invitationLinkService.GetProcessedLinkDataAsync(inDto.Key, inDto.Email, inDto.EmployeeType ?? EmployeeType.RoomAdmin);

        if (!linkData.IsCorrect)
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        var employeeType = linkData.EmployeeType;

        var userID = Guid.Empty;
        try
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(Core.Configuration.Constants.CoreSystem);

            var invitedByEmail = linkData.LinkType == InvitationLinkType.Individual;

            var newUser = await CreateNewUser(GetFirstName(inDto, thirdPartyProfile), GetLastName(inDto, thirdPartyProfile), GetEmailAddress(inDto, thirdPartyProfile), passwordHash, employeeType, true, invitedByEmail);
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

        await _cookiesManager.AuthenticateMeAndSetCookiesAsync(user.TenantId, user.Id);

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

        if (linkData is { LinkType: InvitationLinkType.CommonWithRoom })
        {
            var success = int.TryParse(linkData.RoomId, out var id);

            if (success)
            {
                await _usersInRoomChecker.CheckAppend();
                await _fileSecurity.ShareAsync(id, FileEntryType.Folder, user.Id, linkData.Share);
            }
            else
            {
                await _usersInRoomChecker.CheckAppend();
                await _fileSecurity.ShareAsync(linkData.RoomId, FileEntryType.Folder, user.Id, linkData.Share);
            }
        }
    }

    /// <summary>
    /// Unlinks a third-party account specified in the request from the user profile.
    /// </summary>
    /// <short>
    /// Unlink a third-pary account
    /// </short>
    /// <category>Third-party accounts</category>
    /// <param type="System.String, System" name="provider">Provider name</param>
    /// <path>api/2.0/people/thirdparty/unlinkaccount</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <returns></returns>
    [HttpDelete("thirdparty/unlinkaccount")]
    public async Task UnlinkAccountAsync(string provider)
    {
        await _accountLinker.RemoveProviderAsync(_securityContext.CurrentAccount.ID.ToString(), provider);

        await _messageService.SendAsync(MessageAction.UserUnlinkedSocialAccount, GetMeaningfulProviderName(provider));
    }

    private async Task<UserInfo> CreateNewUser(string firstName, string lastName, string email, string passwordHash, EmployeeType employeeType, bool fromInviteLink, bool inviteByEmail)
    {
        if (SetupInfo.IsSecretEmail(email))
        {
            fromInviteLink = false;
        }

        var user = new UserInfo();

        if (inviteByEmail)
        {
            user = await _userManager.GetUserByEmailAsync(email);

            if (user.Equals(Constants.LostUser) || user.ActivationStatus != EmployeeActivationStatus.Pending)
            {
                throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
            }
        }

        user.FirstName = string.IsNullOrEmpty(firstName) ? UserControlsCommonResource.UnknownFirstName : firstName;
        user.LastName = string.IsNullOrEmpty(lastName) ? UserControlsCommonResource.UnknownLastName : lastName;
        user.Email = email;

        if (_coreBaseSettings.Personal)
        {
            user.ActivationStatus = EmployeeActivationStatus.Activated;
            user.CultureName = _coreBaseSettings.CustomMode ? "ru-RU" : CultureInfo.CurrentUICulture.Name;
        }

        return await _userManagerWrapper.AddUserAsync(user, passwordHash, true, true, employeeType, fromInviteLink, updateExising: inviteByEmail);
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
            await using (var stream = response.Content.ReadAsStream())
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
