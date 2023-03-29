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

using Module = ASC.Api.Core.Module;

namespace ASC.Files.Api;

public class SettingsController : ApiControllerBase
{
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly ProductEntryPoint _productEntryPoint;
    private readonly SettingsManager _settingsManager;
    private readonly TenantManager _tenantManager;

    public SettingsController(
        FileStorageService<string> fileStorageServiceString,
        FilesSettingsHelper filesSettingsHelper,
        ProductEntryPoint productEntryPoint,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        SettingsManager settingsManager,
        TenantManager tenantManager) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _filesSettingsHelper = filesSettingsHelper;
        _productEntryPoint = productEntryPoint;
        _settingsManager = settingsManager;
        _tenantManager = tenantManager;
    }

    /// <summary>
    /// Changes the access to the third-party settings.
    /// </summary>
    /// <short>Change the third-party settings access</short>
    /// <category>Settings</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/thirdparty</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("thirdparty")]
    public bool ChangeAccessToThirdparty(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(inDto.Set);
    }

    /// <summary>
    /// Specifies whether to confirm the file deletion or not.
    /// </summary>
    /// <short>Confirm the file deletion</short>
    /// <category>Settings</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/changedeleteconfrim</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("changedeleteconfrim")]
    public bool ChangeDeleteConfrim(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(inDto.Set);
    }

    /// <summary>
    /// Changes the format of the downloaded archive from .zip to .tar.gz. This method uses the body parameters.
    /// </summary>
    /// <short>Change the archive format (using body parameters)</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Archive</returns>
    /// <path>api/2.0/files/settings/downloadtargz</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(inDto.Set);
    }

    /// <summary>
    /// Changes the format of the downloaded archive from .zip to .tar.gz. This method uses the form parameters.
    /// </summary>
    /// <short>Change the archive format (using form parameters)</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Archive</returns>
    /// <path>api/2.0/files/settings/downloadtargz</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(inDto.Set);
    }

    /// <summary>
    /// Displays the "Favorites" folder.
    /// </summary>
    /// <short>Display the "Favorites" folder</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/settings/favorites</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/favorites")]
    public bool DisplayFavorite(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayFavorite(inDto.Set);
    }

    /// <summary>
    /// Displays the "Recent" folder.
    /// </summary>
    /// <short>Display the "Recent" folder</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/displayRecent</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("displayRecent")]
    public bool DisplayRecent(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayRecent(inDto.Set);
    }

    /// <summary>
    /// Displays the "Templates" folder.
    /// </summary>
    /// <short>Display the "Templates" folder</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/settings/templates</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/templates")]
    public bool DisplayTemplates(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayTemplates(inDto.Set);
    }

    /// <summary>
    /// Changes the ability to share a file externally.
    /// </summary>
    /// <short>Change the external sharing ability</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/settings/external</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/external")]
    public bool ExternalShare(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeExternalShareSettings(inDto.Set);
    }

    /// <summary>
    /// Changes the ability to share a file externally on social networks.
    /// </summary>
    /// <short>Change the external sharing ability on social networks</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.DisplayRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/settings/externalsocialmedia</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/externalsocialmedia")]
    public bool ExternalShareSocialMedia(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeExternalShareSocialMediaSettings(inDto.Set);
    }

    /// <summary>
    /// Changes the ability to force save a file.
    /// </summary>
    /// <short>Change the forcasaving ability</short>
    /// <category>Settings</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/forcesave</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("forcesave")]
    public bool Forcesave(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.Forcesave(inDto.Set);
    }

    /// <summary>
    /// Returns all the file settings.
    /// </summary>
    /// <short>Get file settings</short>
    /// <category>Settings</category>
    /// <returns>File settings</returns>
    /// <path>api/2.0/files/settings</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("settings")]
    public FilesSettingsHelper GetFilesSettings()
    {
        return _filesSettingsHelper;
    }

    /// <summary>
    /// Returns the information about the Documents module.
    /// </summary>
    /// <short>Get the Documents information</short>
    /// <category>Settings</category>
    /// <returns>Module information: ID, product class name, title, description, icon URL, large icon URL, start URL, primary or nor, help URL</returns>
    /// <path>api/2.0/files/info</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("info")]
    public Module GetModule()
    {
        _productEntryPoint.Init();
        return new Module(_productEntryPoint);
    }

    /// <summary>
    /// Hides the confirmation dialog for saving the file copy in the original format when converting a file.
    /// </summary>
    /// <short>Hide the confirmation dialog when converting</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.HideConfirmConvertRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Request parameters for hiding the confirmation dialog: Save (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/hideconfirmconvert</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("hideconfirmconvert")]
    public bool HideConfirmConvert(HideConfirmConvertRequestDto inDto)
    {
        return _fileStorageServiceString.HideConfirmConvert(inDto.Save);
    }

    /// <summary>
    /// Checks if the Private Room settings are available or not.
    /// </summary>
    /// <short>Check the Private Room availability</short>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the Private Room settings are available</returns>
    /// <path>api/2.0/files/@privacy/available</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("@privacy/available")]
    public bool IsAvailablePrivacyRoomSettings()
    {
        return PrivacyRoomSettings.IsAvailable();
    }

    /// <summary>
    /// Changes the ability to store the forcesaved file versions.
    /// </summary>
    /// <short>Change the ability to store the forcesaved files</short>
    /// <category>Settings</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/storeforcesave</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("storeforcesave")]
    public bool StoreForcesave(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreForcesave(inDto.Set);
    }

    /// <summary>
    /// Changes the ability to upload documents in the original formats as well.
    /// </summary>
    /// <short>Change the ability to upload original formats</short>
    /// <category>Settings</category>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/files/storeoriginal</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("storeoriginal")]
    public bool StoreOriginal(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreOriginal(inDto.Set);
    }

    /// <summary>
    /// Updates a file version if a file with such a name already exists.
    /// </summary>
    /// <short>Update a file version if it exists</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.SettingsRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Settings request parameters: Set (bool) - specifies whether to set the specified settings or not</param>
    /// <category>Settings</category>
    /// <returns>Boolean value: true if the parameter is enabled</returns>
    /// <path>api/2.0/files/updateifexist</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("keepnewfilename")]
    public bool KeepNewFileName(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.KeepNewFileName(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inDto"></param>
    /// <returns></returns>
    [HttpPut("updateifexist")]
    public bool UpdateIfExist(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.UpdateIfExist(inDto.Set);
    }

    /// <summary>
    /// Updates the trash bin auto-clearing setting.
    /// </summary>
    /// <short>Update the trash bin auto-clearing setting</short>
    /// <param type="ASC.Files.Core.ApiModels.RequestDto.AutoCleanupRequestDto, ASC.Files.Core.ApiModels.RequestDto" name="inDto">Auto-clearing request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>Set</b> (bool) - enables the auto-clearing or not,</li>
    ///     <li><b>Gap</b> (DateToAutoCleanUp) - a time interval when the auto-clearing will be performed (OneWeek, TwoWeeks, OneMonth, TwoMonths, ThreeMonths).</li>
    /// </ul>
    /// ]]></param>
    /// <category>Settings</category>
    /// <returns>The auto-clearing setting properties: auto-clearing or not, a time interval when the auto-clearing will be performed</returns>
    /// <path>api/2.0/files/settings/autocleanup</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/autocleanup")]
    public AutoCleanUpData ChangeAutomaticallyCleanUp(AutoCleanupRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeAutomaticallyCleanUp(inDto.Set, inDto.Gap);
    }

    /// <summary>
    /// Returns the auto-clearing setting properties.
    /// </summary>
    /// <short>Get the auto-clearing setting properties</short>
    /// <category>Settings</category>
    /// <returns>The auto-clearing setting properties: auto-clearing or not, a time interval when the auto-clearing will be performed</returns>
    /// <path>api/2.0/files/settings/autocleanup</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("settings/autocleanup")]
    public AutoCleanUpData GetAutomaticallyCleanUp()
    {
        return _fileStorageServiceString.GetSettingsAutomaticallyCleanUp();
    }

    /// <summary>
    /// Changes the default access rights in the sharing settings.
    /// </summary>
    /// <short>Change the default access rights</short>
    /// <param type="System.Collections.Generic.List{ASC.Files.Core.Security.FileShare}, System.Collections.Generic" name="value">Sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing)</param>
    /// <category>Settings</category>
    /// <returns>Updated sharing rights (None, ReadWrite, Read, Restrict, Varies, Review, Comment, FillForms, CustomFilter, RoomAdmin, Editing)</returns>
    /// <path>api/2.0/files/settings/dafaultaccessrights</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("settings/dafaultaccessrights")]
    public List<FileShare> ChangeDafaultAccessRights(List<FileShare> value)
    {
        return _fileStorageServiceString.ChangeDafaultAccessRights(value);
    }
}
