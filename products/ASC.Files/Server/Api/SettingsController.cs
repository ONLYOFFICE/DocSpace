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

    public SettingsController(
        FileStorageService<string> fileStorageServiceString,
        FilesSettingsHelper filesSettingsHelper,
        ProductEntryPoint productEntryPoint)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _filesSettingsHelper = filesSettingsHelper;
        _productEntryPoint = productEntryPoint;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("thirdparty")]
    public bool ChangeAccessToThirdparty(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("changedeleteconfrim")]
    public bool ChangeDeleteConfrim(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(inDto.Set);
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
        return _fileStorageServiceString.ChangeDownloadTarGz(inDto.Set);
    }

    [HttpPut("settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(inDto.Set);
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
        return _fileStorageServiceString.DisplayFavorite(inDto.Set);
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
        return _fileStorageServiceString.DisplayRecent(inDto.Set);
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
        return _fileStorageServiceString.DisplayTemplates(inDto.Set);
    }

    [HttpPut("settings/external")]
    public bool ExternalShare(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeExternalShareSettings(inDto.Set);
    }

    [HttpPut("settings/externalsocialmedia")]
    public bool ExternalShareSocialMedia(DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeExternalShareSocialMediaSettings(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("forcesave")]
    public bool Forcesave(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.Forcesave(inDto.Set);
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
        return _fileStorageServiceString.HideConfirmConvert(inDto.Save);
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
    public bool StoreForcesave(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreForcesave(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("storeoriginal")]
    public bool StoreOriginal(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreOriginal(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [HttpPut("updateifexist")]
    public bool UpdateIfExist(SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.UpdateIfExist(inDto.Set);
    }

    [HttpPut("settings/autocleanup")]
    public AutoCleanUpData ChangeAutomaticallyCleanUp(bool set, DateToAutoCleanUp gap)
    {
        return _fileStorageServiceString.ChangeAutomaticallyCleanUp(set, gap);
    }

    [HttpPut("settings/dafaultaccessrights")]
    public List<Core.Security.FileShare> ChangeDafaultAccessRights(List<Core.Security.FileShare> value)
    {
        return _fileStorageServiceString.ChangeDafaultAccessRights(value);
    }
}
