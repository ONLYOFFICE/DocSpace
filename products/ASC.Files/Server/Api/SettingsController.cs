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
    private readonly FileStorageService _fileStorageService;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly ProductEntryPoint _productEntryPoint;
    private readonly SettingsManager _settingsManager;
    private readonly TenantManager _tenantManager;

    public SettingsController(
        FileStorageService fileStorageService,
        FilesSettingsHelper filesSettingsHelper,
        ProductEntryPoint productEntryPoint,
        FolderDtoHelper folderDtoHelper,
        FileDtoHelper fileDtoHelper,
        SettingsManager settingsManager,
        TenantManager tenantManager) : base(folderDtoHelper, fileDtoHelper)
    {
        _fileStorageService = fileStorageService;
        _filesSettingsHelper = filesSettingsHelper;
        _productEntryPoint = productEntryPoint;
        _settingsManager = settingsManager;
        _tenantManager = tenantManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("thirdparty")]
    public async Task<bool> ChangeAccessToThirdpartyAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.ChangeAccessToThirdpartyAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("changedeleteconfrim")]
    public bool ChangeDeleteConfrim(SettingsRequestDto inDto)
    {
        return _fileStorageService.ChangeDeleteConfrim(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [HttpPut("settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageService.ChangeDownloadTarGz(inDto.Set);
    }

    [HttpPut("settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageService.ChangeDownloadTarGz(inDto.Set);
    }

    /// <summary>
    /// Display favorite folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [HttpPut("settings/favorites")]
    public bool DisplayFavorite(DisplayRequestDto inDto)
    {
        return _fileStorageService.DisplayFavorite(inDto.Set);
    }

    /// <summary>
    /// Display recent folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [HttpPut("displayRecent")]
    public bool DisplayRecent(DisplayRequestDto inDto)
    {
        return _fileStorageService.DisplayRecent(inDto.Set);
    }

    /// <summary>
    /// Display template folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [HttpPut("settings/templates")]
    public bool DisplayTemplates(DisplayRequestDto inDto)
    {
        return _fileStorageService.DisplayTemplates(inDto.Set);
    }

    [HttpPut("settings/external")]
    public async Task<bool> ExternalShareAsync(DisplayRequestDto inDto)
    {
        return await _fileStorageService.ChangeExternalShareSettingsAsync(inDto.Set);
    }

    [HttpPut("settings/externalsocialmedia")]
    public async Task<bool> ExternalShareSocialMediaAsync(DisplayRequestDto inDto)
    {
        return await _fileStorageService.ChangeExternalShareSocialMediaSettingsAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("forcesave")]
    public async Task<bool> ForcesaveAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.ForcesaveAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings")]
    public FilesSettingsHelper GetFilesSettings()
    {
        return _filesSettingsHelper;
    }

    [HttpGet("info")]
    public Module GetModule()
    {
        _productEntryPoint.Init();
        return new Module(_productEntryPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="save"></param>
    /// <visible>false</visible>
    /// <returns></returns>
    [HttpPut("hideconfirmconvert")]
    public bool HideConfirmConvert(HideConfirmConvertRequestDto inDto)
    {
        return _fileStorageService.HideConfirmConvert(inDto.Save);
    }

    [HttpGet("@privacy/available")]
    public bool IsAvailablePrivacyRoomSettings()
    {
        return PrivacyRoomSettings.IsAvailable();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("storeforcesave")]
    public async Task<bool> StoreForcesaveAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.StoreForcesaveAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("storeoriginal")]
    public async Task<bool> StoreOriginalAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.StoreOriginalAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("keepnewfilename")]
    public async Task<bool> KeepNewFileNameAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.KeepNewFileNameAsync(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("updateifexist")]
    public async Task<bool> UpdateIfExistAsync(SettingsRequestDto inDto)
    {
        return await _fileStorageService.UpdateIfExistAsync(inDto.Set);
    }

    [HttpPut("settings/autocleanup")]
    public AutoCleanUpData ChangeAutomaticallyCleanUp(AutoCleanupRequestDto inDto)
    {
        return _fileStorageService.ChangeAutomaticallyCleanUp(inDto.Set, inDto.Gap);
    }

    [HttpPut("settings/dafaultaccessrights")]
    public List<FileShare> ChangeDafaultAccessRights(List<FileShare> value)
    {
        return _fileStorageService.ChangeDafaultAccessRights(value);
    }
}
