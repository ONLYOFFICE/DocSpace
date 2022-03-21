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