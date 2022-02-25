/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class BuilderQueue<T>
{
    private readonly ThumbnailSettings _config;
    private readonly ILog _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BuilderQueue(IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<ILog> log, ThumbnailSettings settings)
    {
        _logger = log.Get("ASC.Files.ThumbnailBuilder");
        _serviceScopeFactory = serviceScopeFactory;
        _config = settings;
    }

    public void BuildThumbnails(IEnumerable<FileData<T>> filesWithoutThumbnails)
    {
        try
        {
            Parallel.ForEach(
                filesWithoutThumbnails,
                new ParallelOptions { MaxDegreeOfParallelism = _config.MaxDegreeOfParallelism },
                (fileData) =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var commonLinkUtilitySettings = scope.ServiceProvider.GetService<CommonLinkUtilitySettings>();
                    commonLinkUtilitySettings.ServerUri = fileData.BaseUri;

                    var builder = scope.ServiceProvider.GetService<Builder<T>>();
                    builder.BuildThumbnail(fileData);
                }
            );
        }
        catch (Exception exception)
        {
            _logger.Error(string.Format("BuildThumbnails: filesWithoutThumbnails.Count: {0}.", filesWithoutThumbnails.Count()), exception);
        }
    }
}

[Scope]
public class Builder<T>
{
    private readonly ThumbnailSettings _config;
    private readonly ILog _logger;
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
        IOptionsMonitor<ILog> log,
        IHttpClientFactory clientFactory)
    {
        _config = settings;
        _tenantManager = tenantManager;
        _daoFactory = daoFactory;
        _documentServiceConnector = documentServiceConnector;
        _documentServiceHelper = documentServiceHelper;
        _global = global;
        _pathProvider = pathProvider;
        _logger = log.Get("ASC.Files.ThumbnailBuilder");
        _clientFactory = clientFactory;
    }

    internal void BuildThumbnail(FileData<T> fileData)
    {
        try
        {
            _tenantManager.SetCurrentTenant(fileData.TenantId);

            var fileDao = _daoFactory.GetFileDao<T>();
            if (fileDao == null)
            {
                _logger.ErrorFormat("BuildThumbnail: TenantId: {0}. FileDao could not be null.", fileData.TenantId);

                return;
            }

            GenerateThumbnail(fileDao, fileData);
        }
        catch (Exception exception)
        {
            _logger.Error(string.Format("BuildThumbnail: TenantId: {0}.", fileData.TenantId), exception);
        }
        finally
        {
            FileDataQueue.Queue.TryRemove(fileData.FileId, out _);
        }
    }

    private void GenerateThumbnail(IFileDao<T> fileDao, FileData<T> fileData)
    {
        File<T> file = null;

        try
        {
            file = fileDao.GetFileAsync(fileData.FileId).Result;

            if (file == null)
            {
                _logger.ErrorFormat("GenerateThumbnail: FileId: {0}. File not found.", fileData.FileId);

                return;
            }

            if (file.ThumbnailStatus != Thumbnail.Waiting)
            {
                _logger.InfoFormat("GenerateThumbnail: FileId: {0}. Thumbnail already processed.", fileData.FileId);

                return;
            }

            var ext = FileUtility.GetFileExtension(file.Title);

            if (!_config.FormatsArray.Contains(ext) || file.Encrypted || file.RootFolderType == FolderType.TRASH || file.ContentLength > _config.AvailableFileSize)
            {
                file.ThumbnailStatus = Thumbnail.NotRequired;
                fileDao.SaveThumbnailAsync(file, null).Wait();

                return;
            }

            if (IsImage(file))
            {
                CropImage(fileDao, file);
            }
            else
            {
                MakeThumbnail(fileDao, file);
            }
        }
        catch (Exception exception)
        {
            _logger.Error(string.Format("GenerateThumbnail: FileId: {0}.", fileData.FileId), exception);
            if (file != null)
            {
                file.ThumbnailStatus = Thumbnail.Error;
                fileDao.SaveThumbnailAsync(file, null).Wait();
            }
        }
    }

    private void MakeThumbnail(IFileDao<T> fileDao, File<T> file)
    {
        _logger.DebugFormat("MakeThumbnail: FileId: {0}.", file.ID);

        string thumbnailUrl = null;
        var attempt = 1;

        do
        {
            try
            {
                if (GetThumbnailUrl(file, _global.ThumbnailExtension, out thumbnailUrl))
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
                            throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Encrypted file.", file.ID));
                        }
                        if (documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.Convert)
                        {
                            throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Could not convert.", file.ID));
                        }
                    }
                }
            }

            if (attempt >= _config.AttemptsLimit)
            {
                throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Attempts limmit exceeded.", file.ID));
            }
            else
            {
                _logger.DebugFormat("MakeThumbnail: FileId: {0}. Sleep {1} after attempt #{2}. ", file.ID, _config.AttemptWaitInterval, attempt);
                attempt++;
            }

            Thread.Sleep(_config.AttemptWaitInterval);
        }
        while (string.IsNullOrEmpty(thumbnailUrl));

        SaveThumbnail(fileDao, file, thumbnailUrl);
    }

    private bool GetThumbnailUrl(File<T> file, string toExtension, out string url)
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

        (var operationResultProgress, url) = _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, thumbnail, spreadsheetLayout, false).Result;

        operationResultProgress = Math.Min(operationResultProgress, 100);

        return operationResultProgress == 100;
    }

    private void SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl)
    {
        _logger.DebugFormat("SaveThumbnail: FileId: {0}. ThumbnailUrl {1}.", file.ID, thumbnailUrl);

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(thumbnailUrl);

        var httpClient = _clientFactory.CreateClient();
        using var response = httpClient.Send(request);
        using (var stream = new ResponseStream(response))
        {
            Crop(fileDao, file, stream);
        }

        _logger.DebugFormat("SaveThumbnail: FileId: {0}. Successfully saved.", file.ID);
    }

    private bool IsImage(File<T> file)
    {
        var extension = FileUtility.GetFileExtension(file.Title);

        return FileUtility.ExtsImage.Contains(extension);
    }

    private void CropImage(IFileDao<T> fileDao, File<T> file)
    {
        _logger.DebugFormat("CropImage: FileId: {0}.", file.ID);

        using (var stream = fileDao.GetFileStreamAsync(file).Result)
        {
            Crop(fileDao, file, stream);
        }

        _logger.DebugFormat("CropImage: FileId: {0}. Successfully saved.", file.ID);
    }

    private void Crop(IFileDao<T> fileDao, File<T> file, Stream stream)
    {
        using (var sourceImg = Image.Load(stream))
        {
            using (var targetImg = GetImageThumbnail(sourceImg))
            {
                using (var targetStream = new MemoryStream())
                {
                    targetImg.Save(targetStream, PngFormat.Instance);
                    fileDao.SaveThumbnailAsync(file, targetStream).Wait();
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
