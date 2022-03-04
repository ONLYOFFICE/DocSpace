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
    public bool ChangeAccessToThirdpartyFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(requestDto.Set);
    }

    [Update(@"thirdparty")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeAccessToThirdpartyFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(requestDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"changedeleteconfrim")]
    public bool ChangeDeleteConfrimFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(requestDto.Set);
    }

    [Update(@"changedeleteconfrim")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeDeleteConfrimFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(requestDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromBody([FromBody] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(requestDto.Set);
    }

    [Update(@"settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromForm([FromForm] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(requestDto.Set);
    }

    /// <summary>
    /// Display favorite folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/favorites")]
    public bool DisplayFavoriteFromBody([FromBody] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayFavorite(requestDto.Set);
    }

    [Update(@"settings/favorites")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayFavoriteFromForm([FromForm] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayFavorite(requestDto.Set);
    }

    /// <summary>
    /// Display recent folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"displayRecent")]
    public bool DisplayRecentFromBody([FromBody] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayRecent(requestDto.Set);
    }

    [Update(@"displayRecent")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayRecentFromForm([FromForm] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayRecent(requestDto.Set);
    }

    /// <summary>
    /// Display template folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/templates")]
    public bool DisplayTemplatesFromBody([FromBody] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayTemplates(requestDto.Set);
    }

    [Update(@"settings/templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayTemplatesFromForm([FromForm] DisplayRequestDto requestDto)
    {
        return _fileStorageServiceString.DisplayTemplates(requestDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"forcesave")]
    public bool ForcesaveFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.Forcesave(requestDto.Set);
    }

    [Update(@"forcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ForcesaveFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.Forcesave(requestDto.Set);
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
    public bool HideConfirmConvertFromBody([FromBody] HideConfirmConvertRequestDto requestDto)
    {
        return _fileStorageServiceString.HideConfirmConvert(requestDto.Save);
    }

    [Update(@"hideconfirmconvert")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool HideConfirmConvertFromForm([FromForm] HideConfirmConvertRequestDto requestDto)
    {
        return _fileStorageServiceString.HideConfirmConvert(requestDto.Save);
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
    public bool StoreForcesaveFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.StoreForcesave(requestDto.Set);
    }

    [Update(@"storeforcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreForcesaveFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.StoreForcesave(requestDto.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"storeoriginal")]
    public bool StoreOriginalFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.StoreOriginal(requestDto.Set);
    }

    [Update(@"storeoriginal")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreOriginalFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.StoreOriginal(requestDto.Set);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"updateifexist")]
    public bool UpdateIfExistFromBody([FromBody] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.UpdateIfExist(requestDto.Set);
    }

    [Update(@"updateifexist")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool UpdateIfExistFromForm([FromForm] SettingsRequestDto requestDto)
    {
        return _fileStorageServiceString.UpdateIfExist(requestDto.Set);
    }
}
