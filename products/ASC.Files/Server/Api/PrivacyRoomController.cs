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

namespace ASC.Api.Documents;

[ConstraintRoute("int")]
public class PrivacyRoomControllerInternal : PrivacyRoomController<int>
{
    public PrivacyRoomControllerInternal(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

public class PrivacyRoomControllerThirdparty : PrivacyRoomController<string>
{
    public PrivacyRoomControllerThirdparty(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("privacyroom")]
public abstract class PrivacyRoomController<T> : ControllerBase
{
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly FileStorageService _fileStorageService;
    private readonly SettingsManager _settingsManager;

    public PrivacyRoomController(
        SettingsManager settingsManager,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        FileStorageService fileStorageService)
    {
        _settingsManager = settingsManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [HttpGet("access/{fileId}")]
    public async Task<IEnumerable<EncryptionKeyPairDto>> GetPublicKeysWithAccess(T fileId)
    {
        if (!await PrivacyRoomSettings.GetEnabledAsync(_settingsManager))
        {
            throw new SecurityException();
        }

        return await _encryptionKeyPairHelper.GetKeyPairAsync(fileId, _fileStorageService);
    }
}

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("privacyroom")]
public class PrivacyRoomControllerCommon : ControllerBase
{
    private readonly AuthContext _authContext;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly ILogger _logger;
    private readonly MessageService _messageService;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;

    public PrivacyRoomControllerCommon(
        AuthContext authContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        MessageService messageService,
        ILoggerProvider option)
    {
        _authContext = authContext;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _messageService = messageService;
        _logger = option.CreateLogger("ASC.Api.Documents");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [HttpGet("keys")]
    public async Task<EncryptionKeyPairDto> GetKeysAsync()
    {
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!await PrivacyRoomSettings.GetEnabledAsync(_settingsManager))
        {
            throw new SecurityException();
        }

        return await _encryptionKeyPairHelper.GetKeyPairAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <visible>false</visible>
    [HttpGet("")]
    public async Task<bool> PrivacyRoomAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        return await PrivacyRoomSettings.GetEnabledAsync(_settingsManager);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [HttpPut("keys")]
    public async Task<object> SetKeysAsync(PrivacyRoomRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!await PrivacyRoomSettings.GetEnabledAsync(_settingsManager))
        {
            throw new SecurityException();
        }

        var keyPair = await _encryptionKeyPairHelper.GetKeyPairAsync();
        if (keyPair != null)
        {
            if (!string.IsNullOrEmpty(keyPair.PublicKey) && !inDto.Update)
            {
                return new { isset = true };
            }

            _logger.InformationUpdateAddress(_authContext.CurrentAccount.ID);
        }

        await _encryptionKeyPairHelper.SetKeyPairAsync(inDto.PublicKey, inDto.PrivateKeyEnc);

        return new
        {
            isset = true
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    /// <visible>false</visible>
    [HttpPut("")]
    public async Task<bool> SetPrivacyRoomAsync(PrivacyRoomRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (inDto.Enable)
        {
            if (!PrivacyRoomSettings.IsAvailable())
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
            }
        }

        await PrivacyRoomSettings.SetEnabledAsync(_settingsManager, inDto.Enable);

        await _messageService.SendAsync(inDto.Enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

        return inDto.Enable;
    }
}
