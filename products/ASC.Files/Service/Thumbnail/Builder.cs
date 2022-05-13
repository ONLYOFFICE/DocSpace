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

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class BuilderQueue<T>
{
    private readonly ThumbnailSettings _config;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BuilderQueue(IServiceScopeFactory serviceScopeFactory, ILoggerProvider log, ThumbnailSettings settings)
    {
        _logger = log.CreateLogger("ASC.Files.ThumbnailBuilder");
        _serviceScopeFactory = serviceScopeFactory;
        _config = settings;
    }

    public async Task BuildThumbnails(IEnumerable<FileData<T>> filesWithoutThumbnails)
    {
        try
        {
            await Parallel.ForEachAsync(
            filesWithoutThumbnails,
            new ParallelOptions { MaxDegreeOfParallelism = _config.MaxDegreeOfParallelism },
                async (fileData, token) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var commonLinkUtilitySettings = scope.ServiceProvider.GetService<CommonLinkUtilitySettings>();
                commonLinkUtilitySettings.ServerUri = fileData.BaseUri;

                var builder = scope.ServiceProvider.GetService<Builder<T>>();
                await builder.BuildThumbnail(fileData);
            }
        );
        }
        catch (Exception exception)
        {
            _logger.ErrorBuildThumbnailsCount(filesWithoutThumbnails.Count(), exception);
        }
    }
}

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

    public Builder(
        ThumbnailSettings settings,
        TenantManager tenantManager,
        IDaoFactory daoFactory,
        DocumentServiceConnector documentServiceConnector,
        DocumentServiceHelper documentServiceHelper,
        Global global,
        PathProvider pathProvider,
        ILoggerProvider log,
        IHttpClientFactory clientFactory)
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
    }

    internal async Task BuildThumbnail(FileData<T> fileData)
    {
        try
        {
            _tenantManager.SetCurrentTenant(fileData.TenantId);

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
        finally
        {
            FileDataQueue.Queue.TryRemove(fileData.FileId, out _);
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

            if (file.ThumbnailStatus != Thumbnail.Waiting)
            {
                _logger.InformationGenerateThumbnail(fileData.FileId.ToString());

                return;
            }

            var ext = FileUtility.GetFileExtension(file.Title);

            if (!_config.FormatsArray.Contains(ext) || file.Encrypted || file.RootFolderType == FolderType.TRASH || file.ContentLength > _config.AvailableFileSize)
            {
                file.ThumbnailStatus = Thumbnail.NotRequired;
                await fileDao.SaveThumbnailAsync(file, null);

                return;
            }

            if (IsImage(file))
            {
                await CropImage(fileDao, file);
            }
            else
            {
                await MakeThumbnail(fileDao, file);
            }
        }
        catch (Exception exception)
        {
            _logger.ErrorGenerateThumbnail(fileData.FileId.ToString(), exception);
            if (file != null)
            {
                file.ThumbnailStatus = Thumbnail.Error;
                await fileDao.SaveThumbnailAsync(file, null);
            }
        }
    }

    private async Task MakeThumbnail(IFileDao<T> fileDao, File<T> file)
    {
        _logger.DebugMakeThumbnail1(file.Id.ToString());

        string thumbnailUrl = null;
        var attempt = 1;

        do
        {
            try
            {
                var (result, url) = await GetThumbnailUrl(file, _global.ThumbnailExtension);
                thumbnailUrl = url;

                if (result)
                {
                    break;
                }
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    var documentServiceException = exception.InnerException as DocumentService.DocumentServiceException;
                    if (documentServiceException != null)
                    {
                        if (documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.ConvertPassword)
                        {
                            throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Encrypted file.", file.Id));
                        }
                        if (documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.Convert)
                        {
                            throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Could not convert.", file.Id));
                        }
                    }
                }
            }

            if (attempt >= _config.AttemptsLimit)
            {
                throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Attempts limmit exceeded.", file.Id));
            }
            else
            {
                _logger.DebugMakeThumbnail2(file.Id.ToString(), _config.AttemptWaitInterval, attempt);
                attempt++;
            }

            Thread.Sleep(_config.AttemptWaitInterval);
        }
        while (string.IsNullOrEmpty(thumbnailUrl));

        await SaveThumbnail(fileDao, file, thumbnailUrl);
    }

    private async Task<(bool, string)> GetThumbnailUrl(File<T> file, string toExtension)
    {
        var fileUri = _pathProvider.GetFileStreamUrl(file);
        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var fileExtension = file.ConvertedExtension;
        var docKey = _documentServiceHelper.GetDocKey(file);
        var thumbnail = new DocumentService.ThumbnailData
        {
            Aspect = 2,
            First = true,
            //Height = config.ThumbnaillHeight,
            //Width = config.ThumbnaillWidth
        };
        var spreadsheetLayout = new DocumentService.SpreadsheetLayout
        {
            IgnorePrintArea = true,
            //Orientation = "landscape", // "297mm" x "210mm"
            FitToHeight = 0,
            FitToWidth = 1,
            Headings = false,
            GridLines = false,
            Margins = new DocumentService.SpreadsheetLayout.LayoutMargins
            {
                Top = "0mm",
                Right = "0mm",
                Bottom = "0mm",
                Left = "0mm"
            },
            PageSize = new DocumentService.SpreadsheetLayout.LayoutPageSize
            {
                Width = (_config.ThumbnaillWidth * 1.5) + "mm", // 192 * 1.5 = "288mm",
                Height = (_config.ThumbnaillHeight * 1.5) + "mm" // 128 * 1.5 = "192mm"
            }
        };

        var (operationResultProgress, url) = await _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, thumbnail, spreadsheetLayout, false);

        operationResultProgress = Math.Min(operationResultProgress, 100);
        return (operationResultProgress == 100, url);
    }

    private async Task SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl)
    {
        _logger.DebugMakeThumbnail3(file.Id.ToString(), thumbnailUrl);

        using var request = new HttpRequestMessage();
        request.RequestUri = new Uri(thumbnailUrl);

        var httpClient = _clientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using (var stream = new ResponseStream(response))
        {
            await Crop(fileDao, file, stream);
        }

        _logger.DebugMakeThumbnail4(file.Id.ToString());
    }

    private bool IsImage(File<T> file)
    {
        var extension = FileUtility.GetFileExtension(file.Title);

        return FileUtility.ExtsImage.Contains(extension);
    }

    private async Task CropImage(IFileDao<T> fileDao, File<T> file)
    {
        _logger.DebugCropImage(file.Id.ToString());

        using (var stream = await fileDao.GetFileStreamAsync(file))
        {
            await Crop(fileDao, file, stream);
        }

        _logger.DebugCropImageSuccessfullySaved(file.Id.ToString());
    }

    private async Task Crop(IFileDao<T> fileDao, File<T> file, Stream stream)
    {
        using (var sourceImg = Image.Load(stream))
        {
            using (var targetImg = GetImageThumbnail(sourceImg))
            {
                using (var targetStream = new MemoryStream())
                {
                    targetImg.Save(targetStream, PngFormat.Instance);
                    //targetImg.Save(targetStream, JpegFormat.Instance);
                    await fileDao.SaveThumbnailAsync(file, targetStream);
                }
            }
        }

        GC.Collect();
    }

    private Image GetImageThumbnail(Image sourceBitmap)
    {
        //bad for small or disproportionate images
        //return sourceBitmap.GetThumbnailImage(config.ThumbnaillWidth, config.ThumbnaillHeight, () => false, IntPtr.Zero);

        var targetSize = new Size(Math.Min(sourceBitmap.Width, _config.ThumbnaillWidth), Math.Min(sourceBitmap.Height, _config.ThumbnaillHeight));
        var point = new Point(0, 0);
        var size = targetSize;

        if (sourceBitmap.Width > _config.ThumbnaillWidth && sourceBitmap.Height > _config.ThumbnaillHeight)
        {
            if (sourceBitmap.Width > sourceBitmap.Height)
            {
                var width = (int)(_config.ThumbnaillWidth * (sourceBitmap.Height / (1.0 * _config.ThumbnaillHeight)));
                size = new Size(width, sourceBitmap.Height);
            }
            else
            {
                var height = (int)(_config.ThumbnaillHeight * (sourceBitmap.Width / (1.0 * _config.ThumbnaillWidth)));
                size = new Size(sourceBitmap.Width, height);
            }
        }

        if (sourceBitmap.Width > sourceBitmap.Height)
        {
            point.X = (sourceBitmap.Width - size.Width) / 2;
        }

        var targetThumbnailSettings = new UserPhotoThumbnailSettings(point, size);

        return UserPhotoThumbnailManager.GetImage(sourceBitmap, targetSize, targetThumbnailSettings);
    }
}
