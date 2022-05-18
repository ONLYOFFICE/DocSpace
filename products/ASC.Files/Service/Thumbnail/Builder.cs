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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Files.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;

namespace ASC.Files.ThumbnailBuilder
{
    [Singletone]
    internal class BuilderQueue<T>
    {
        private readonly ThumbnailSettings config;
        private readonly ILog logger;
        private IServiceProvider ServiceProvider { get; }

        public BuilderQueue(IServiceProvider serviceProvider, IOptionsMonitor<ILog> log, ThumbnailSettings settings)
        {
            logger = log.Get("ASC.Files.ThumbnailBuilder");
            ServiceProvider = serviceProvider;
            config = settings;
        }

        public async Task BuildThumbnails(IEnumerable<FileData<T>> filesWithoutThumbnails)
        {
            try
            {
                await Parallel.ForEachAsync(
                    filesWithoutThumbnails,
                    new ParallelOptions { MaxDegreeOfParallelism = config.MaxDegreeOfParallelism },
                    async (fileData, token) =>
                    {
                        using var scope = ServiceProvider.CreateScope();
                        var commonLinkUtilitySettings = scope.ServiceProvider.GetService<CommonLinkUtilitySettings>();
                        commonLinkUtilitySettings.ServerUri = fileData.BaseUri;

                        var builder = scope.ServiceProvider.GetService<Builder<T>>();
                        await builder.BuildThumbnail(fileData);
                    }
                );
            }
            catch (Exception exception)
            {
                logger.Error(string.Format("BuildThumbnails: filesWithoutThumbnails.Count: {0}.", filesWithoutThumbnails.Count()), exception);
            }
        }
    }

    [Scope]
    internal class Builder<T>
    {
        private readonly ThumbnailSettings config;
        private readonly ThumbnailSettings _thumbnailSettings;
        private readonly ILog logger;

        private TenantManager TenantManager { get; }
        private IDaoFactory DaoFactory { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private Global Global { get; }
        private PathProvider PathProvider { get; }
        private IHttpClientFactory ClientFactory { get; }

        public SocketManager SocketManager { get; }

        public Builder(
            ThumbnailSettings settings,
            TenantManager tenantManager,
            IDaoFactory daoFactory,
            DocumentServiceConnector documentServiceConnector,
            DocumentServiceHelper documentServiceHelper,
            Global global,
            PathProvider pathProvider,
            IOptionsMonitor<ILog> log,
            IHttpClientFactory clientFactory,
            SocketManager socketManager,
            ThumbnailSettings thumbnailSettings)
        {
            this.config = settings;
            TenantManager = tenantManager;
            DaoFactory = daoFactory;
            DocumentServiceConnector = documentServiceConnector;
            DocumentServiceHelper = documentServiceHelper;
            Global = global;
            PathProvider = pathProvider;
            logger = log.Get("ASC.Files.ThumbnailBuilder");
            ClientFactory = clientFactory;
            SocketManager = socketManager;
            _thumbnailSettings = thumbnailSettings;
        }

        internal async Task BuildThumbnail(FileData<T> fileData)
        {
            try
            {
                TenantManager.SetCurrentTenant(fileData.TenantId);

                var fileDao = DaoFactory.GetFileDao<T>();
                if (fileDao == null)
                {
                    logger.ErrorFormat("BuildThumbnail: TenantId: {0}. FileDao could not be null.", fileData.TenantId);
                    return;
                }

                await GenerateThumbnail(fileDao, fileData);
            }
            catch (Exception exception)
            {
                logger.Error(string.Format("BuildThumbnail: TenantId: {0}.", fileData.TenantId), exception);
            }
            finally
            {
                Launcher.Queue.TryRemove(fileData.FileId, out _);
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
                    logger.ErrorFormat("GenerateThumbnail: FileId: {0}. File not found.", fileData.FileId);
                    return;
                }

                if (file.ThumbnailStatus != Thumbnail.Waiting)
                {
                    logger.InfoFormat("GenerateThumbnail: FileId: {0}. Thumbnail already processed.", fileData.FileId);
                    return;
                }

                var ext = FileUtility.GetFileExtension(file.Title);

                if (!config.FormatsArray.Contains(ext) || file.Encrypted || file.RootFolderType == FolderType.TRASH || file.ContentLength > config.AvailableFileSize)
                {
                    file.ThumbnailStatus = Thumbnail.NotRequired;
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

                var newFile = await fileDao.GetFileStableAsync(file.ID);

                await SocketManager.UpdateFileAsync(newFile);
            }
            catch (Exception exception)
            {
                logger.Error(string.Format("GenerateThumbnail: FileId: {0}.", fileData.FileId), exception);
                if (file != null)
                {
                    file.ThumbnailStatus = Thumbnail.Error;
                    foreach (var size in _thumbnailSettings.Sizes)
                    {
                        await fileDao.SaveThumbnailAsync(file, null, size.Width, size.Height);
                    }
                }
            }
        }

        private async Task MakeThumbnail(IFileDao<T> fileDao, File<T> file)
        {
            foreach (var w in config.Sizes)
            {
                logger.DebugFormat("MakeThumbnail: FileId: {0}.", file.ID);

                string thumbnailUrl = null;
                var attempt = 1;

                do
                {
                    try
                    {
                        var (result, url) = await GetThumbnailUrl(file, Global.ThumbnailExtension, w.Width, w.Height);
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
                                    throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Encrypted file.", file.ID));
                                }
                                if (documentServiceException.Code == DocumentService.DocumentServiceException.ErrorCode.Convert)
                                {
                                    throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Could not convert.", file.ID));
                                }
                            }
                        }
                    }

                    if (attempt >= config.AttemptsLimit)
                    {
                        throw new Exception(string.Format("MakeThumbnail: FileId: {0}. Attempts limmit exceeded.", file.ID));
                    }
                    else
                    {
                        logger.DebugFormat("MakeThumbnail: FileId: {0}. Sleep {1} after attempt #{2}. ", file.ID, config.AttemptWaitInterval, attempt);
                        attempt++;
                    }

                    await Task.Delay(config.AttemptWaitInterval);
                }
                while (string.IsNullOrEmpty(thumbnailUrl));

                await SaveThumbnail(fileDao, file, thumbnailUrl, w.Width, w.Height);
            }
        }

        private async Task<(bool, string)> GetThumbnailUrl(File<T> file, string toExtension, int width, int height)
        {
            var fileUri = PathProvider.GetFileStreamUrl(file);
            fileUri = DocumentServiceConnector.ReplaceCommunityAdress(fileUri);

            var fileExtension = file.ConvertedExtension;
            var docKey = DocumentServiceHelper.GetDocKey(file);
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
                    Width = (width * 1.5) + "mm", // 192 * 1.5 = "288mm",
                    Height = (height * 1.5) + "mm" // 128 * 1.5 = "192mm"
                }
            };

            var (operationResultProgress, url) = await DocumentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, thumbnail, spreadsheetLayout, false);

            operationResultProgress = Math.Min(operationResultProgress, 100);
            return (operationResultProgress == 100, url);
        }

        private async Task SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl, int width, int height)
        {
            logger.DebugFormat("SaveThumbnail: FileId: {0}. ThumbnailUrl {1}.", file.ID, thumbnailUrl);

            using var request = new HttpRequestMessage();
            request.RequestUri = new Uri(thumbnailUrl);

            var httpClient = ClientFactory.CreateClient();
            using var response = httpClient.Send(request);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var sourceImg = await Image.LoadAsync(stream))
                {
                    await CropAsync(sourceImg, fileDao, file, width, height);
                }
            }

            logger.DebugFormat("SaveThumbnail: FileId: {0}. Successfully saved.", file.ID);
        }

        private bool IsImage(File<T> file)
        {
            var extension = FileUtility.GetFileExtension(file.Title);
            return FileUtility.ExtsImage.Contains(extension);
        }

        private async Task CropImage(IFileDao<T> fileDao, File<T> file)
        {
            logger.DebugFormat("CropImage: FileId: {0}.", file.ID);

            using (var stream = await fileDao.GetFileStreamAsync(file))
            {
                await Crop(fileDao, file, stream);
            }

            logger.DebugFormat("CropImage: FileId: {0}. Successfully saved.", file.ID);
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

                foreach (var w in config.Sizes)
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
            await targetImg.SaveAsJpegAsync(targetStream);
            await fileDao.SaveThumbnailAsync(file, targetStream, width, height);
        }

        private Image GetImageThumbnail(Image sourceBitmap, Size targetSize, int thumbnaillWidth, int thumbnaillHeight)
        {
            //bad for small or disproportionate images
            //return sourceBitmap.GetThumbnailImage(config.ThumbnaillWidth, config.ThumbnaillHeight, () => false, IntPtr.Zero);
            var point = new Point(0, 0);
            var size = targetSize;

            if (sourceBitmap.Width > thumbnaillWidth && sourceBitmap.Height > thumbnaillHeight)
            {
                if (sourceBitmap.Width > sourceBitmap.Height)
                {
                    var width = (int)(thumbnaillWidth * (sourceBitmap.Height / (1.0 * thumbnaillHeight)));
                    size = new Size(width, sourceBitmap.Height);
                }
                else
                {
                    var height = (int)(thumbnaillWidth * (sourceBitmap.Width / (1.0 * thumbnaillWidth)));
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
}
