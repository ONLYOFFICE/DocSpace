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
    public PrivacyRoomControllerInternal(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService<int> fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

public class PrivacyRoomControllerThirdparty : PrivacyRoomController<string>
{
    public PrivacyRoomControllerThirdparty(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService<string> fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

/// <summary>
/// Provides access to Private Room.
/// </summary>
/// <name>privacyroom</name>
[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("privacyroom")]
public abstract class PrivacyRoomController<T> : ControllerBase
{
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly FileStorageService<T> _fileStorageService;
    private readonly SettingsManager _settingsManager;

    public PrivacyRoomController(
        SettingsManager settingsManager,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        FileStorageService<T> fileStorageService)
    {
        _settingsManager = settingsManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Returns all the key pairs of the users who have access to the file with the ID specified in the request.
    /// </summary>
    /// <short>Get file key pairs</short>
    /// <param type="System.Int32, System" name="fileId">File ID</param>
    /// <returns>List of encryption key pairs: private key, public key, user ID</returns>
    /// <path>api/2.0/privacyroom/access/{fileId}</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("access/{fileId}")]
    public Task<IEnumerable<EncryptionKeyPairDto>> GetPublicKeysWithAccess(T fileId)
    {
        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new SecurityException();
        }

        return _encryptionKeyPairHelper.GetKeyPairAsync(fileId, _fileStorageService);
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
    /// Returns a key pair for the current user.
    /// </summary>
    /// <short>Get encryption keys</short>
    /// <returns>Encryption key pair: private key, public key, user ID</returns>
    /// <path>api/2.0/privacyroom/keys</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("keys")]
    public EncryptionKeyPairDto GetKeys()
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new SecurityException();
        }

        return _encryptionKeyPairHelper.GetKeyPair();
    }


    /// <summary>
    /// Checks if the Private Room settings are enabled or not.
    /// </summary>
    /// <short>Check the Private Room settings</short>
    /// <returns>Boolean value: true - the Private Room settings are enabled, false - the Private Room settings are disabled</returns>
    /// <path>api/2.0/privacyroom</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("")]
    public bool PrivacyRoom()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return PrivacyRoomSettings.GetEnabled(_settingsManager);
    }

    /// <summary>
    /// Sets the key pair for the current user.
    /// </summary>
    /// <short>Set encryption keys</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.PrivacyRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for setting encryption keys: <![CDATA[
    /// <ul>
    ///     <li><b>PublicKey</b> (string) - public key,</li>
    ///     <li><b>PrivateKeyEnc</b> (string) - private key,</li>
    ///     <li><b>Update</b> (bool) - encryption keys need to be updated or not.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true - the key pair is set</returns>
    /// <path>api/2.0/privacyroom/keys</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("keys")]
    public object SetKeys(PrivacyRoomRequestDto inDto)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new SecurityException();
        }

        var keyPair = _encryptionKeyPairHelper.GetKeyPair();
        if (keyPair != null)
        {
            if (!string.IsNullOrEmpty(keyPair.PublicKey) && !inDto.Update)
            {
                return new { isset = true };
            }

            _logger.InformationUpdateAddress(_authContext.CurrentAccount.ID);
        }

        _encryptionKeyPairHelper.SetKeyPair(inDto.PublicKey, inDto.PrivateKeyEnc);

        return new
        {
            isset = true
        };
    }

    /// <summary>
    /// Enables the Private Room settings.
    /// </summary>
    /// <short>Enable the Private Room settings</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.PrivacyRoomRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for setting encryption keys: Enable (bool) - specifies whether to enable the Private Room settings or not</param>
    /// <returns>Boolean value: true - the Private Room settings are enabled, false - the Private Room settings are disabled</returns>
    /// <path>api/2.0/privacyroom</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("")]
    public bool SetPrivacyRoom(PrivacyRoomRequestDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (inDto.Enable)
        {
            if (!PrivacyRoomSettings.IsAvailable())
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
            }
        }

        PrivacyRoomSettings.SetEnabled(_settingsManager, inDto.Enable);

        _messageService.Send(inDto.Enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

        return inDto.Enable;
    }
}
