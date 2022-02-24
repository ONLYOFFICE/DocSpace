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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Files.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core;
using ASC.Web.Files.Services.DocumentService;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

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

        public void BuildThumbnails(IEnumerable<FileData<T>> filesWithoutThumbnails)
        {
            try
            {
                Parallel.ForEach(
                    filesWithoutThumbnails,
                    new ParallelOptions { MaxDegreeOfParallelism = config.MaxDegreeOfParallelism },
                    (fileData) =>
                    {
                        using var scope = ServiceProvider.CreateScope();
                        var commonLinkUtilitySettings = scope.ServiceProvider.GetService<CommonLinkUtilitySettings>();
                        commonLinkUtilitySettings.ServerUri = fileData.BaseUri;

                        var builder = scope.ServiceProvider.GetService<Builder<T>>();
                        builder.BuildThumbnail(fileData);
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
        private readonly ILog logger;

        private TenantManager TenantManager { get; }
        private IDaoFactory DaoFactory { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private Global Global { get; }
        private PathProvider PathProvider { get; }
        private IHttpClientFactory ClientFactory { get; }

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
            this.config = settings;
            TenantManager = tenantManager;
            DaoFactory = daoFactory;
            DocumentServiceConnector = documentServiceConnector;
            DocumentServiceHelper = documentServiceHelper;
            Global = global;
            PathProvider = pathProvider;
            logger = log.Get("ASC.Files.ThumbnailBuilder");
            ClientFactory = clientFactory;
        }

        internal void BuildThumbnail(FileData<T> fileData)
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

                GenerateThumbnail(fileDao, fileData);
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

        private void GenerateThumbnail(IFileDao<T> fileDao, FileData<T> fileData)
        {
            File<T> file = null;

            try
            {
                file = fileDao.GetFileAsync(fileData.FileId).Result;

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
                logger.Error(string.Format("GenerateThumbnail: FileId: {0}.", fileData.FileId), exception);
                if (file != null)
                {
                    file.ThumbnailStatus = Thumbnail.Error;
                    fileDao.SaveThumbnailAsync(file, null).Wait();
                }
            }
        }

        private void MakeThumbnail(IFileDao<T> fileDao, File<T> file)
        {
            logger.DebugFormat("MakeThumbnail: FileId: {0}.", file.ID);

            string thumbnailUrl = null;
            var attempt = 1;

            do
            {
                try
                {
                    if (GetThumbnailUrl(file, Global.ThumbnailExtension, out thumbnailUrl))
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

                Thread.Sleep(config.AttemptWaitInterval);
            }
            while (string.IsNullOrEmpty(thumbnailUrl));

            SaveThumbnail(fileDao, file, thumbnailUrl);
        }

        private bool GetThumbnailUrl(File<T> file, string toExtension, out string url)
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
                    Width = (config.ThumbnaillWidth * 1.5) + "mm", // 192 * 1.5 = "288mm",
                    Height = (config.ThumbnaillHeight * 1.5) + "mm" // 128 * 1.5 = "192mm"
                }
            };

            (var operationResultProgress, url) = DocumentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, thumbnail, spreadsheetLayout, false).Result;

            operationResultProgress = Math.Min(operationResultProgress, 100);
            return operationResultProgress == 100;
        }

        private void SaveThumbnail(IFileDao<T> fileDao, File<T> file, string thumbnailUrl)
        {
            logger.DebugFormat("SaveThumbnail: FileId: {0}. ThumbnailUrl {1}.", file.ID, thumbnailUrl);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(thumbnailUrl);

            var httpClient = ClientFactory.CreateClient();
            using var response = httpClient.Send(request);
            using (var stream = new ResponseStream(response))
            {
                Crop(fileDao, file, stream);
            }

            logger.DebugFormat("SaveThumbnail: FileId: {0}. Successfully saved.", file.ID);
        }

        private bool IsImage(File<T> file)
        {
            var extension = FileUtility.GetFileExtension(file.Title);
            return FileUtility.ExtsImage.Contains(extension);
        }

        private void CropImage(IFileDao<T> fileDao, File<T> file)
        {
            logger.DebugFormat("CropImage: FileId: {0}.", file.ID);

            using (var stream = fileDao.GetFileStreamAsync(file).Result)
            {
                Crop(fileDao, file, stream);
            }

            logger.DebugFormat("CropImage: FileId: {0}. Successfully saved.", file.ID);
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

            var targetSize = new Size(Math.Min(sourceBitmap.Width, config.ThumbnaillWidth), Math.Min(sourceBitmap.Height, config.ThumbnaillHeight));
            var point = new Point(0, 0);
            var size = targetSize;

            if (sourceBitmap.Width > config.ThumbnaillWidth && sourceBitmap.Height > config.ThumbnaillHeight)
            {
                if (sourceBitmap.Width > sourceBitmap.Height)
                {
                    var width = (int)(config.ThumbnaillWidth * (sourceBitmap.Height / (1.0 * config.ThumbnaillHeight)));
                    size = new Size(width, sourceBitmap.Height);
                }
                else
                {
                    var height = (int)(config.ThumbnaillHeight * (sourceBitmap.Width / (1.0 * config.ThumbnaillWidth)));
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
