﻿// (c) Copyright Ascensio System SIA 2010-2022
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

using SixLabors.ImageSharp.Processing;

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
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly SocketManager _socketManager;

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
        SocketManager socketManager,
        ThumbnailSettings thumbnailSettings)
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
        _socketManager = socketManager;
        _thumbnailSettings = thumbnailSettings;
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

            if (file.ThumbnailStatus != ASC.Files.Core.Thumbnail.Waiting)
            {
                _logger.InformationGenerateThumbnail(fileData.FileId.ToString());

                return;
            }

            var ext = FileUtility.GetFileExtension(file.Title);

            if (!_config.FormatsArray.Contains(ext) || file.Encrypted || file.RootFolderType == FolderType.TRASH || file.ContentLength > _config.AvailableFileSize)
            {
                file.ThumbnailStatus = ASC.Files.Core.Thumbnail.NotRequired;
                foreach (var size in _thumbnailSettings.Sizes)
                {
                    await fileDao.SaveThumbnailAsync(file, null, size.Width, size.Height);
                }

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

            var newFile = await fileDao.GetFileStableAsync(file.Id);

            await _socketManager.UpdateFileAsync(newFile);
        }
        catch (Exception exception)
        {
            _logger.ErrorGenerateThumbnail(fileData.FileId.ToString(), exception);
            if (file != null)
            {
                file.ThumbnailStatus = ASC.Files.Core.Thumbnail.Error;
                foreach (var size in _thumbnailSettings.Sizes)
                {
                    await fileDao.SaveThumbnailAsync(file, null, size.Width, size.Height);
                }
            }
        }
    }

    private async Task MakeThumbnail(IFileDao<T> fileDao, File<T> file)
    {
        foreach (var w in _config.Sizes)
        {
            _logger.DebugMakeThumbnail1(file.Id.ToString());

            string thumbnailUrl = null;
            var attempt = 1;

            do
            {
                try
                {
                    var (result, url) = await GetThumbnailUrl(file, _global.DocThumbnailExtension.ToString(), w.Width, w.Height);
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
                    _logger.DebugMakeThumbnailAfter(file.Id.ToString(), _config.AttemptWaitInterval, attempt);
                    attempt++;
                }

                await Task.Delay(_config.AttemptWaitInterval);
            }
            while (string.IsNullOrEmpty(thumbnailUrl));

            await SaveThumbnail(fileDao, file, thumbnailUrl, w.Width, w.Height);
        }
    }

    private async Task<(bool, string)> GetThumbnailUrl(File<T> file, string toExtension, int width, int height)
    {
        var fileUri = _pathProvider.GetFileStreamUrl(file);
        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var fileExtension = file.ConvertedExtension;
        var docKey = _documentServiceHelper.GetDocKey(file);
        var thumbnail = new DocumentService.ThumbnailData
        {
            Aspect = 2,
            First = true,
            //Height = height,
            //Width = width
        };
        var spreadsheetLayout = new DocumentService.SpreadsheetLayout
        {
            IgnorePrintArea = true,
            //Orientation = "landscape", // "297mm" x "210mm"
            FitToHeight = height,
            FitToWidth = width,
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
            }
        };

        var (operationResultProgress, url) = await _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, CultureInfo.CurrentCulture.Name, thumbnail, spreadsheetLayout, false);

        operationResultProgress = Math.Min(operationResultProgress, 100);
        return (operationResultProgress == 100, url);
    }

    private async Task SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl, int width, int height)
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
                await CropAsync(sourceImg, fileDao, file, width, height);
            }
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
        using (var sourceImg = await Image.LoadAsync(stream))
        {
            //var tasks = new List<Task>();

            //foreach (var w in config.Sizes)
            //{
            //    tasks.Add(CropAsync(sourceImg, fileDao, file, w.Width, w.Height));
            //}

            //await Task.WhenAll(tasks.ToArray());

            //await Parallel.ForEachAsync(config.Sizes, (w, b) => CropAsync(sourceImg, fileDao, file, w.Width, w.Height));

            foreach (var w in _config.Sizes)
            {
                await CropAsync(sourceImg, fileDao, file, w.Width, w.Height);
            }
        }

        GC.Collect();
    }

    private async ValueTask CropAsync(Image sourceImg, IFileDao<T> fileDao, File<T> file, int width, int height)
    {
        var targetSize = new Size(Math.Min(sourceImg.Width, width), Math.Min(sourceImg.Height, height));
        using var targetImg = GetImageThumbnail(sourceImg, targetSize, width, height);
        using var targetStream = new MemoryStream();
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
        await fileDao.SaveThumbnailAsync(file, targetStream, width, height);
    }

    private Image GetImageThumbnail(Image sourceBitmap, Size targetSize, int thumbnaillWidth, int thumbnaillHeight)
    {
        return sourceBitmap.Clone(x => x.BackgroundColor(Color.White).Resize(thumbnaillWidth, 0));
    }
}
