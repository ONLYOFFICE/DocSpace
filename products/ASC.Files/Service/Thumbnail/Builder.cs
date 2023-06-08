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


using ASC.Data.Storage;
using ASC.Data.Storage.DiscStorage;

namespace ASC.Files.ThumbnailBuilder;

[Scope]
public class Builder<T>
{
    private readonly ThumbnailSettings _config;
    private readonly ILogger _logger;
    private readonly TenantManager _tenantManager;
    private readonly IDaoFactory _daoFactory;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly Global _global;
    private readonly PathProvider _pathProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly SocketManager _socketManager;
    private readonly FFmpegService _fFmpegService;
    private readonly TempPath _tempPath;
    private readonly TempStream _tempStream;
    private readonly StorageFactory _storageFactory;
    private IDataStore _dataStore;

    private readonly List<string> _imageFormatsCanBeCrop = new List<string>
            {
                ".bmp", ".gif", ".jpeg", ".jpg", ".pbm", ".png", ".tiff", ".tga", ".webp",
            };

    public Builder(
        ThumbnailSettings settings,
        TenantManager tenantManager,
        IDaoFactory daoFactory,
        DocumentServiceConnector documentServiceConnector,
        DocumentServiceHelper documentServiceHelper,
        Global global,
        PathProvider pathProvider,
        ILoggerProvider log,
        IHttpClientFactory clientFactory,
        FFmpegService fFmpegService,
        TempPath tempPath,
        SocketManager socketManager,
        TempStream tempStream,
        StorageFactory storageFactory)
    {
        _config = settings;
        _tenantManager = tenantManager;
        _daoFactory = daoFactory;
        _documentServiceConnector = documentServiceConnector;
        _documentServiceHelper = documentServiceHelper;
        _global = global;
        _pathProvider = pathProvider;
        _logger = log.CreateLogger("ASC.Files.ThumbnailBuilder");
        _clientFactory = clientFactory;
        _fFmpegService = fFmpegService;
        _tempPath = tempPath;
        _tempStream = tempStream;
        _storageFactory = storageFactory;
        _socketManager = socketManager;
    }

    internal async Task BuildThumbnail(FileData<T> fileData)
    {
        try
        {
            await _tenantManager.SetCurrentTenantAsync(fileData.TenantId);

            _dataStore = await _storageFactory.GetStorageAsync(fileData.TenantId, FileConstant.StorageModule, (IQuotaController)null);

            var fileDao = _daoFactory.GetFileDao<T>();
            if (fileDao == null)
            {
                _logger.ErrorBuildThumbnailFileDaoIsNull(fileData.TenantId);

                return;
            }

            await GenerateThumbnail(fileDao, fileData);
        }
        catch (Exception exception)
        {
            _logger.ErrorBuildThumbnailsTenantId(fileData.TenantId, exception);
        }
    }

    private async Task GenerateThumbnail(IFileDao<T> fileDao, FileData<T> fileData)
    {
        File<T> file = null;

        try
        {
            file = await fileDao.GetFileAsync(fileData.FileId);

            if (file == null)
            {
                _logger.ErrorGenerateThumbnailFileNotFound(fileData.FileId.ToString());

                return;
            }

            if (file.ThumbnailStatus != Core.Thumbnail.Waiting)
            {
                _logger.InformationGenerateThumbnail(fileData.FileId.ToString());

                return;
            }

            if (!CanCreateThumbnail(file))
            {
                file.ThumbnailStatus = Core.Thumbnail.NotRequired;

                await fileDao.SetThumbnailStatusAsync(file, Core.Thumbnail.NotRequired);

                return;
            }

            await fileDao.SetThumbnailStatusAsync(file, Core.Thumbnail.Creating);

            var ext = FileUtility.GetFileExtension(file.Title);

            if (IsVideo(ext))
            {
                await MakeThumbnailFromVideo(fileDao, file);
            }
            else if (IsImage(ext))
            {
                await MakeThumbnailFromImage(fileDao, file);
            }
            else
            {
                await MakeThumbnailFromDocs(fileDao, file);
            }

            await fileDao.SetThumbnailStatusAsync(file, Core.Thumbnail.Created);

            var newFile = await fileDao.GetFileStableAsync(file.Id);

            await _socketManager.UpdateFileAsync(newFile);
        }
        catch (Exception exception)
        {
            _logger.ErrorGenerateThumbnail(fileData.FileId.ToString(), exception);
            if (file != null)
            {
                file.ThumbnailStatus = Core.Thumbnail.Error;

                await fileDao.SetThumbnailStatusAsync(file, Core.Thumbnail.Error);
            }
        }
    }

    private async Task MakeThumbnailFromVideo(IFileDao<T> fileDao, File<T> file)
    {
        var streamFile = await fileDao.GetFileStreamAsync(file);

        var thumbPath = _tempPath.GetTempFileName("jpg");
        var tempFilePath = _tempPath.GetTempFileName(Path.GetExtension(file.Title));

        try
        {
            using (var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, System.IO.FileShare.Read))
            {
                await streamFile.CopyToAsync(fileStream);
            }

            await _fFmpegService.CreateThumbnail(tempFilePath, thumbPath);

            using (var streamThumb = new FileStream(thumbPath, FileMode.Open, FileAccess.ReadWrite, System.IO.FileShare.Read))
            {
                await CropAsync(fileDao, file, streamThumb);
            }
        }
        finally
        {
            if (Path.Exists(thumbPath))
            {
                File.Delete(thumbPath);
            }

            if (Path.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    private async Task MakeThumbnailFromDocs(IFileDao<T> fileDao, File<T> file)
    {
        foreach (var w in _config.Sizes)
        {
            _logger.DebugMakeThumbnail1(file.Id.ToString());

            string thumbnailUrl = null;
            var resultPercent = 0;
            var attempt = 1;

            do
            {
                try
                {
                    (resultPercent, thumbnailUrl) = await GetThumbnailUrl(file, _global.DocThumbnailExtension.ToString(), w.Width, w.Height);

                    if (resultPercent == 100)
                    {
                        break;
                    }
                }
                catch (Exception exception)
                {
                    if (exception.InnerException != null)
                    {
                        var documentServiceException = exception.InnerException as DocumentServiceException;
                        if (documentServiceException != null)
                        {
                            if (documentServiceException.Code == DocumentServiceException.ErrorCode.ConvertPassword)
                            {
                                throw new Exception(String.Format("MakeThumbnail: FileId: {0}. Encrypted file.", file.Id), exception);
                            }
                            if (documentServiceException.Code == DocumentServiceException.ErrorCode.Convert)
                            {
                                throw new Exception(String.Format("MakeThumbnail: FileId: {0}. Could not convert.", file.Id), exception);
                            }
                        }
                        else
                        {
                            _logger.WarningMakeThumbnail(file.Id.ToString(), thumbnailUrl, resultPercent, attempt, exception);
                        }
                    }
                    else
                    {
                        _logger.WarningMakeThumbnail(file.Id.ToString(), thumbnailUrl, resultPercent, attempt, exception);
                    }
                }

                if (attempt >= _config.AttemptsLimit)
                {
                    throw new Exception(string.Format("MakeThumbnail: FileId: {0}, ThumbnailUrl: {1}, ResultPercent: {2}. Attempts limmit exceeded.", file.Id, thumbnailUrl, resultPercent));
                }
                else
                {
                    _logger.DebugMakeThumbnailAfter(file.Id.ToString(), _config.AttemptWaitInterval, attempt);
                    attempt++;
                }

                await Task.Delay(_config.AttemptWaitInterval);
            }
            while (string.IsNullOrEmpty(thumbnailUrl));

            await SaveThumbnail(fileDao, file, thumbnailUrl, w.Width, w.Height, w.ResizeMode, AnchorPositionMode.Top);
        }
    }

    private async Task<(int, string)> GetThumbnailUrl(File<T> file, string toExtension, int width, int height)
    {
        var fileUri = await _pathProvider.GetFileStreamUrlAsync(file);
        fileUri = await _documentServiceConnector.ReplaceCommunityAdressAsync(fileUri);

        var fileExtension = file.ConvertedExtension;
        var docKey = await _documentServiceHelper.GetDocKeyAsync(file);
        var thumbnail = new ThumbnailData
        {
            Aspect = 2,
            First = true,
            //Height = height,
            //Width = width
        };
        var spreadsheetLayout = new SpreadsheetLayout
        {
            IgnorePrintArea = true,
            //Orientation = "landscape", // "297mm" x "210mm"
            FitToHeight = height,
            FitToWidth = width,
            Headings = false,
            GridLines = false,
            Margins = new SpreadsheetLayout.LayoutMargins
            {
                Top = "0mm",
                Right = "0mm",
                Bottom = "0mm",
                Left = "0mm"
            },
            PageSize = new SpreadsheetLayout.LayoutPageSize
            {
            }
        };

        var (operationResultProgress, url, _) = await _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, CultureInfo.CurrentCulture.Name, thumbnail, spreadsheetLayout, false);

        operationResultProgress = Math.Min(operationResultProgress, 100);
        return (operationResultProgress, url);
    }

    private async Task SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl, int width, int height, ResizeMode resizeMode, AnchorPositionMode anchorPositionMode = AnchorPositionMode.Center)
    {
        _logger.DebugMakeThumbnail3(file.Id.ToString(), thumbnailUrl);

        using var request = new HttpRequestMessage();
        request.RequestUri = new Uri(thumbnailUrl);

        var httpClient = _clientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using (var sourceImg = await Image.LoadAsync(stream))
            {
                await CropAsync(sourceImg, fileDao, file, width, height, resizeMode, anchorPositionMode);
            }
        }

        _logger.DebugMakeThumbnail4(file.Id.ToString());
    }

    public bool CanCreateThumbnail(File<T> file)
    {
        var ext = FileUtility.GetFileExtension(file.Title);

        if (!CanCreateThumbnail(ext) || file.Encrypted || file.RootFolderType == FolderType.TRASH) return false;
        if (IsVideo(ext) && file.ContentLength > _config.MaxVideoFileSize) return false;
        if (IsImage(ext) && file.ContentLength > _config.MaxImageFileSize) return false;

        return true;
    }

    private bool CanCreateThumbnail(string extention)
    {
        return _config.FormatsArray.Contains(extention) || IsVideo(extention) || IsImage(extention);
    }

    private bool IsImage(string extention)
    {
        return _imageFormatsCanBeCrop.Contains(extention);
    }

    private bool IsVideo(string extention)
    {
        return _fFmpegService.ExistFormat(extention);
    }

    private async Task MakeThumbnailFromImage(IFileDao<T> fileDao, File<T> file)
    {
        _logger.DebugCropImage(file.Id.ToString());

        using (var stream = await fileDao.GetFileStreamAsync(file))
        {
            await CropAsync(fileDao, file, stream);
        }

        _logger.DebugCropImageSuccessfullySaved(file.Id.ToString());
    }

    private async Task CropAsync(IFileDao<T> fileDao, File<T> file, Stream stream)
    {
        using var sourceImg = await Image.LoadAsync(stream);

        if (_dataStore is DiscDataStore)
        {
            foreach (var w in _config.Sizes)
            {
                await CropAsync(sourceImg, fileDao, file, w.Width, w.Height, w.ResizeMode);
            }
        }
        else
        {
            await Parallel.ForEachAsync(_config.Sizes, new ParallelOptions { MaxDegreeOfParallelism = 3 }, async (w, t) =>
            {
                await CropAsync(sourceImg, fileDao, file, w.Width, w.Height, w.ResizeMode);
            });
        }

        GC.Collect();
    }

    private async ValueTask CropAsync(Image sourceImg,
                                      IFileDao<T> fileDao,
                                      File<T> file,
                                      int width,
                                      int height,
                                      ResizeMode resizeMode,
                                      AnchorPositionMode anchorPositionMode = AnchorPositionMode.Center)
    {
        using var targetImg = GetImageThumbnail(sourceImg, width, height, resizeMode, anchorPositionMode);
        using var targetStream = _tempStream.Create();

        switch (_global.ThumbnailExtension)
        {
            case ThumbnailExtension.bmp:
                await targetImg.SaveAsBmpAsync(targetStream);
                break;
            case ThumbnailExtension.gif:
                await targetImg.SaveAsGifAsync(targetStream);
                break;
            case ThumbnailExtension.jpg:
                await targetImg.SaveAsJpegAsync(targetStream);
                break;
            case ThumbnailExtension.png:
                await targetImg.SaveAsPngAsync(targetStream);
                break;
            case ThumbnailExtension.pbm:
                await targetImg.SaveAsPbmAsync(targetStream);
                break;
            case ThumbnailExtension.tiff:
                await targetImg.SaveAsTiffAsync(targetStream);
                break;
            case ThumbnailExtension.tga:
                await targetImg.SaveAsTgaAsync(targetStream);
                break;
            case ThumbnailExtension.webp:
                await targetImg.SaveAsWebpAsync(targetStream);
                break;
        }

        await _dataStore.SaveAsync(fileDao.GetUniqThumbnailPath(file, width, height), targetStream);
    }

    private Image GetImageThumbnail(Image sourceBitmap, int thumbnaillWidth, int thumbnaillHeight, ResizeMode resizeMode, AnchorPositionMode anchorPositionMode)
    {
        return sourceBitmap.Clone(x =>
        {
            x.Resize(new ResizeOptions
            {
                Size = new Size(thumbnaillWidth, thumbnaillHeight),
                Mode = resizeMode,
                Position = anchorPositionMode,
                Sampler = KnownResamplers.Lanczos8
            });
        });
    }
}
