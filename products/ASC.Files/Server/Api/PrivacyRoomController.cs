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

namespace ASC.Api.Documents;

[ConstraintRoute("int")]
internal class PrivacyRoomControllerInternal : PrivacyRoomController<int>
{
    public PrivacyRoomControllerInternal(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService<int> fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

internal class PrivacyRoomControllerThirdparty : PrivacyRoomController<string>
{
    public PrivacyRoomControllerThirdparty(SettingsManager settingsManager, EncryptionKeyPairDtoHelper encryptionKeyPairHelper, FileStorageService<string> fileStorageService) : base(settingsManager, encryptionKeyPairHelper, fileStorageService)
    {
    }
}

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("privacyroom")]
internal abstract class PrivacyRoomController<T> : ControllerBase
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
    /// 
    /// </summary>
    /// <visible>false</visible>
    [Read("access/{fileId}")]
    public Task<IEnumerable<EncryptionKeyPairDto>> GetPublicKeysWithAccess(T fileId)
    {
        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new System.Security.SecurityException();
        }

        return _encryptionKeyPairHelper.GetKeyPairAsync(fileId, _fileStorageService);
    }
}

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("privacyroom")]
public abstract class PrivacyRoomControllerCommon : ControllerBase
{
    private readonly AuthContext _authContext;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly ILog _logger;
    private readonly MessageService _messageService;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly TenantManager _tenantManager;

    public PrivacyRoomControllerCommon(
        AuthContext authContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        TenantManager tenantManager,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        MessageService messageService,
        IOptionsMonitor<ILog> option)
    {
        _authContext = authContext;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tenantManager = tenantManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _messageService = messageService;
        _logger = option.Get("ASC.Api.Documents");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [Read("keys")]
    public EncryptionKeyPairDto GetKeys()
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new System.Security.SecurityException();
        }

        return _encryptionKeyPairHelper.GetKeyPair();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <visible>false</visible>
    [Read("")]
    public bool PrivacyRoom()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return PrivacyRoomSettings.GetEnabled(_settingsManager);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [Update("keys")]
    public object SetKeysFromBody([FromBody] PrivacyRoomRequestDto inDto)
    {
        return SetKeys(inDto);
    }

    [Update("keys")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SetKeysFromForm([FromForm] PrivacyRoomRequestDto inDto)
    {
        return SetKeys(inDto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    /// <visible>false</visible>
    [Update("")]
    public bool SetPrivacyRoomFromBody([FromBody] PrivacyRoomRequestDto inDto)
    {
        return SetPrivacyRoom(inDto);
    }

    [Update("")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SetPrivacyRoomFromForm([FromForm] PrivacyRoomRequestDto inDto)
    {
        return SetPrivacyRoom(inDto);
    }

    private object SetKeys(PrivacyRoomRequestDto inDto)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            throw new System.Security.SecurityException();
        }

        var keyPair = _encryptionKeyPairHelper.GetKeyPair();
        if (keyPair != null)
        {
            if (!string.IsNullOrEmpty(keyPair.PublicKey) && !inDto.Update)
            {
                return new { isset = true };
            }

            _logger.InfoFormat("User {0} updates address", _authContext.CurrentAccount.ID);
        }

        _encryptionKeyPairHelper.SetKeyPair(inDto.PublicKey, inDto.PrivateKeyEnc);

        return new
        {
            isset = true
        };
    }

    private bool SetPrivacyRoom(PrivacyRoomRequestDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (inDto.Enable)
        {
            if (!PrivacyRoomSettings.IsAvailable(_tenantManager))
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
            }
        }

        PrivacyRoomSettings.SetEnabled(_tenantManager, _settingsManager, inDto.Enable);

        _messageService.Send(inDto.Enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

        return inDto.Enable;
    }
}
