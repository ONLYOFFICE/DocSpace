using Module = ASC.Api.Core.Module;

namespace ASC.Files.Api;

public class SettingsController : ApiControllerBase
{
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly ProductEntryPoint _productEntryPoint;

    public SettingsController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FileStorageService<string> fileStorageServiceString,
        FilesSettingsHelper filesSettingsHelper,
        TenantManager tenantManager,
        ProductEntryPoint productEntryPoint) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _filesSettingsHelper = filesSettingsHelper;
        _tenantManager = tenantManager;
        _productEntryPoint = productEntryPoint;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"thirdparty")]
    public bool ChangeAccessToThirdpartyFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(inDto.Set);
    }

    [Update(@"thirdparty")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeAccessToThirdpartyFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"changedeleteconfrim")]
    public bool ChangeDeleteConfrimFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(inDto.Set);
    }

    [Update(@"changedeleteconfrim")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeDeleteConfrimFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(inDto.Set);
    }

    [Update(@"settings/downloadtargz")]
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
    [Update(@"settings/favorites")]
    public bool DisplayFavoriteFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayFavorite(inDto.Set);
    }

    [Update(@"settings/favorites")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayFavoriteFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayFavorite(inDto.Set);
    }

    /// <summary>
    /// Display recent folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"displayRecent")]
    public bool DisplayRecentFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayRecent(inDto.Set);
    }

    [Update(@"displayRecent")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayRecentFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayRecent(inDto.Set);
    }

    /// <summary>
    /// Display template folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/templates")]
    public bool DisplayTemplatesFromBody([FromBody] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayTemplates(inDto.Set);
    }

    [Update(@"settings/templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayTemplatesFromForm([FromForm] DisplayRequestDto inDto)
    {
        return _fileStorageServiceString.DisplayTemplates(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"forcesave")]
    public bool ForcesaveFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.Forcesave(inDto.Set);
    }

    [Update(@"forcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ForcesaveFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.Forcesave(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Read(@"settings")]
    public FilesSettingsHelper GetFilesSettings()
    {
        return _filesSettingsHelper;
    }

    [Read("info")]
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
    [Update(@"hideconfirmconvert")]
    public bool HideConfirmConvertFromBody([FromBody] HideConfirmConvertRequestDto inDto)
    {
        return _fileStorageServiceString.HideConfirmConvert(inDto.Save);
    }

    [Update(@"hideconfirmconvert")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool HideConfirmConvertFromForm([FromForm] HideConfirmConvertRequestDto inDto)
    {
        return _fileStorageServiceString.HideConfirmConvert(inDto.Save);
    }

    [Read("@privacy/available")]
    public bool IsAvailablePrivacyRoomSettings()
    {
        return PrivacyRoomSettings.IsAvailable(_tenantManager);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"storeforcesave")]
    public bool StoreForcesaveFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreForcesave(inDto.Set);
    }

    [Update(@"storeforcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreForcesaveFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreForcesave(inDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"storeoriginal")]
    public bool StoreOriginalFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreOriginal(inDto.Set);
    }

    [Update(@"storeoriginal")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreOriginalFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.StoreOriginal(inDto.Set);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"updateifexist")]
    public bool UpdateIfExistFromBody([FromBody] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.UpdateIfExist(inDto.Set);
    }

    [Update(@"updateifexist")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool UpdateIfExistFromForm([FromForm] SettingsRequestDto inDto)
    {
        return _fileStorageServiceString.UpdateIfExist(inDto.Set);
    }
}
