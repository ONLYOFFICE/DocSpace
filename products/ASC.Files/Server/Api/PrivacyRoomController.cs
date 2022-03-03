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

[Scope]
[DefaultRoute]
[ApiController]
public class PrivacyRoomController : ControllerBase
{
    private readonly AuthContext _authContext;
    private readonly EncryptionKeyPairHelper _encryptionKeyPairHelper;
    private readonly FileStorageService<string> _fileStorageService;
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly ILog _logger;
    private readonly MessageService _messageService;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly TenantManager _tenantManager;

    public PrivacyRoomController(
        AuthContext authContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        TenantManager tenantManager,
        EncryptionKeyPairHelper encryptionKeyPairHelper,
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageService,
        MessageService messageService,
        IOptionsMonitor<ILog> option)
    {
        _authContext = authContext;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tenantManager = tenantManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageService = fileStorageService;
        _messageService = messageService;
        _logger = option.Get("ASC.Api.Documents");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [Read("keys")]
    public EncryptionKeyPair GetKeys()
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager)) throw new System.Security.SecurityException();

        return _encryptionKeyPairHelper.GetKeyPair();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <visible>false</visible>
    [Read("access/{fileId}")]
    public Task<IEnumerable<EncryptionKeyPair>> GetPublicKeysWithAccess(string fileId)
    {
        if (!PrivacyRoomSettings.GetEnabled(_settingsManager)) throw new System.Security.SecurityException();

        return _encryptionKeyPairHelper.GetKeyPairAsync(fileId, _fileStorageService);
    }

    [Read("access/{fileId:int}")]
    public Task<IEnumerable<EncryptionKeyPair>> GetPublicKeysWithAccess(int fileId)
    {
        if (!PrivacyRoomSettings.GetEnabled(_settingsManager)) throw new System.Security.SecurityException();

        return _encryptionKeyPairHelper.GetKeyPairAsync(fileId, _fileStorageServiceInt);
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
    public object SetKeysFromBody([FromBody] PrivacyRoomModel model)
    {
        return SetKeys(model);
    }

    [Update("keys")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SetKeysFromForm([FromForm] PrivacyRoomModel model)
    {
        return SetKeys(model);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    /// <visible>false</visible>
    [Update("")]
    public bool SetPrivacyRoomFromBody([FromBody] PrivacyRoomModel model)
    {
        return SetPrivacyRoom(model);
    }

    [Update("")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SetPrivacyRoomFromForm([FromForm] PrivacyRoomModel model)
    {
        return SetPrivacyRoom(model);
    }

    private object SetKeys(PrivacyRoomModel model)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(_authContext.CurrentAccount.ID), Constants.Action_EditUser);

        if (!PrivacyRoomSettings.GetEnabled(_settingsManager)) throw new System.Security.SecurityException();

        var keyPair = _encryptionKeyPairHelper.GetKeyPair();
        if (keyPair != null)
        {
            if (!string.IsNullOrEmpty(keyPair.PublicKey) && !model.Update)
            {
                return new { isset = true };
            }

            _logger.InfoFormat("User {0} updates address", _authContext.CurrentAccount.ID);
        }

        _encryptionKeyPairHelper.SetKeyPair(model.PublicKey, model.PrivateKeyEnc);

        return new
        {
            isset = true
        };
    }

    private bool SetPrivacyRoom(PrivacyRoomModel model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (model.Enable)
        {
            if (!PrivacyRoomSettings.IsAvailable(_tenantManager))
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
            }
        }

        PrivacyRoomSettings.SetEnabled(_tenantManager, _settingsManager, model.Enable);

        _messageService.Send(model.Enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

        return model.Enable;
    }
}
