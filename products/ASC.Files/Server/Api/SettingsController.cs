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
    public bool ChangeAccessToThirdpartyFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(model.Set);
    }

    [Update(@"thirdparty")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeAccessToThirdpartyFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.ChangeAccessToThirdparty(model.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"changedeleteconfrim")]
    public bool ChangeDeleteConfrimFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(model.Set);
    }

    [Update(@"changedeleteconfrim")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ChangeDeleteConfrimFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.ChangeDeleteConfrim(model.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromBody([FromBody] DisplayModel model)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(model.Set);
    }

    [Update(@"settings/downloadtargz")]
    public ICompress ChangeDownloadZipFromForm([FromForm] DisplayModel model)
    {
        return _fileStorageServiceString.ChangeDownloadTarGz(model.Set);
    }

    /// <summary>
    /// Display favorite folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/favorites")]
    public bool DisplayFavoriteFromBody([FromBody] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayFavorite(model.Set);
    }

    [Update(@"settings/favorites")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayFavoriteFromForm([FromForm] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayFavorite(model.Set);
    }

    /// <summary>
    /// Display recent folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"displayRecent")]
    public bool DisplayRecentFromBody([FromBody] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayRecent(model.Set);
    }

    [Update(@"displayRecent")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayRecentFromForm([FromForm] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayRecent(model.Set);
    }

    /// <summary>
    /// Display template folder
    /// </summary>
    /// <param name="set"></param>
    /// <category>Settings</category>
    /// <returns></returns>
    [Update(@"settings/templates")]
    public bool DisplayTemplatesFromBody([FromBody] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayTemplates(model.Set);
    }

    [Update(@"settings/templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool DisplayTemplatesFromForm([FromForm] DisplayModel model)
    {
        return _fileStorageServiceString.DisplayTemplates(model.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"forcesave")]
    public bool ForcesaveFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.Forcesave(model.Set);
    }

    [Update(@"forcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool ForcesaveFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.Forcesave(model.Set);
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
    public bool HideConfirmConvertFromBody([FromBody] HideConfirmConvertModel model)
    {
        return _fileStorageServiceString.HideConfirmConvert(model.Save);
    }

    [Update(@"hideconfirmconvert")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool HideConfirmConvertFromForm([FromForm] HideConfirmConvertModel model)
    {
        return _fileStorageServiceString.HideConfirmConvert(model.Save);
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
    public bool StoreForcesaveFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.StoreForcesave(model.Set);
    }

    [Update(@"storeforcesave")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreForcesaveFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.StoreForcesave(model.Set);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"storeoriginal")]
    public bool StoreOriginalFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.StoreOriginal(model.Set);
    }

    [Update(@"storeoriginal")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StoreOriginalFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.StoreOriginal(model.Set);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    [Update(@"updateifexist")]
    public bool UpdateIfExistFromBody([FromBody] SettingsModel model)
    {
        return _fileStorageServiceString.UpdateIfExist(model.Set);
    }

    [Update(@"updateifexist")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool UpdateIfExistFromForm([FromForm] SettingsModel model)
    {
        return _fileStorageServiceString.UpdateIfExist(model.Set);
    }
}
