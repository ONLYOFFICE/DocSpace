using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Files.Helpers;

public class EditorControllerHelper<T> : FilesHelperBase<T>
{
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceTrackerHelper _documentServiceTrackerHelper;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairDtoHelper;
    private readonly SettingsManager _settingsManager;
    private readonly EntryManager _entryManager;

    public EditorControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        DocumentServiceHelper documentServiceHelper,
        DocumentServiceTrackerHelper documentServiceTrackerHelper,
        EncryptionKeyPairDtoHelper encryptionKeyPairDtoHelper,
        SettingsManager settingsManager,
        EntryManager entryManager) 
        : base(
            filesSettingsHelper,
            fileUploader,
            socketManager,
            fileDtoHelper,
            apiContext,
            fileStorageService,
            folderContentDtoHelper,
            httpContextAccessor,
            folderDtoHelper)
    {
        _documentServiceHelper = documentServiceHelper;
        _documentServiceTrackerHelper = documentServiceTrackerHelper;
        _encryptionKeyPairDtoHelper = encryptionKeyPairDtoHelper;
        _settingsManager = settingsManager;
        _entryManager = entryManager;
    }

    public async Task<FileDto<T>> SaveEditingAsync(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
    {
        return await _fileDtoHelper.GetAsync(await _fileStorageService.SaveEditingAsync(fileId, fileExtension, downloadUri, stream, doc, forcesave));
    }

    public Task<string> StartEditAsync(T fileId, bool editingAlone, string doc)
    {
        return _fileStorageService.StartEditAsync(fileId, editingAlone, doc);
    }

    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _fileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    public async Task<Configuration<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
    {
        var docParams = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
        var configuration = docParams.Configuration;

        configuration.EditorType = EditorType.External;
        if (configuration.EditorConfig.ModeWrite)
        {
            configuration.EditorConfig.CallbackUrl = _documentServiceTrackerHelper.GetCallbackUrl(configuration.Document.Info.GetFile().Id.ToString());
        }

        if (configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            var keyPair = _encryptionKeyPairDtoHelper.GetKeyPair();
            if (keyPair != null)
            {
                configuration.EditorConfig.EncryptionKeys = new EncryptionKeysConfig
                {
                    PrivateKeyEnc = keyPair.PrivateKeyEnc,
                    PublicKey = keyPair.PublicKey,
                };
            }
        }

        if (!configuration.Document.Info.GetFile().Encrypted && !configuration.Document.Info.GetFile().ProviderEntry)
        {
            _entryManager.MarkAsRecent(configuration.Document.Info.GetFile());
        }

        configuration.Token = _documentServiceHelper.GetSignature(configuration);

        return configuration;
    }

    public Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        return _fileStorageService.GetPresignedUriAsync(fileId);
    }

    public Task<bool> SetAceLinkAsync(T fileId, FileShare share)
    {
        return _fileStorageService.SetAceLinkAsync(fileId, share);
    }
}