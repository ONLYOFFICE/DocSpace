/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Common.Web;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Compress;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.FFmpegService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using JWT;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using FileShare = ASC.Files.Core.Security.FileShare;
using MimeMapping = ASC.Common.Web.MimeMapping;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files
{
    public class FileHandler
    {
        private IServiceProvider ServiceProvider { get; }

        public FileHandler(RequestDelegate next, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            using var scope = ServiceProvider.CreateScope();
            var fileHandlerService = scope.ServiceProvider.GetService<FileHandlerService>();
            await fileHandlerService.Invoke(context).ConfigureAwait(false);
        }
    }

    [Scope]
    public class FileHandlerService
    {
        public string FileHandlerPath
        {
            get { return FilesLinkUtility.FileHandlerPath; }
        }
        private FilesLinkUtility FilesLinkUtility { get; }
        private TenantExtra TenantExtra { get; }
        private AuthContext AuthContext { get; }
        private SecurityContext SecurityContext { get; }
        private GlobalStore GlobalStore { get; }
        private IDaoFactory DaoFactory { get; }
        private FileSecurity FileSecurity { get; }
        private FileMarker FileMarker { get; }
        private SetupInfo SetupInfo { get; }
        private FileUtility FileUtility { get; }
        private Global Global { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private PathProvider PathProvider { get; }
        private DocumentServiceTrackerHelper DocumentServiceTrackerHelper { get; }
        private FilesMessageService FilesMessageService { get; }
        private FileShareLink FileShareLink { get; }
        private FileConverter FileConverter { get; }
        private FFmpegService FFmpegService { get; }
        private IServiceProvider ServiceProvider { get; }
        public TempStream TempStream { get; }
        private UserManager UserManager { get; }
        private ILog Logger { get; }

        public FileHandlerService(
            FilesLinkUtility filesLinkUtility,
            TenantExtra tenantExtra,
            CookiesManager cookiesManager,
            AuthContext authContext,
            SecurityContext securityContext,
            GlobalStore globalStore,
            IOptionsMonitor<ILog> optionsMonitor,
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            FileMarker fileMarker,
            SetupInfo setupInfo,
            FileUtility fileUtility,
            Global global,
            EmailValidationKeyProvider emailValidationKeyProvider,
            CoreBaseSettings coreBaseSettings,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider,
            UserManager userManager,
            DocumentServiceTrackerHelper documentServiceTrackerHelper,
            FilesMessageService filesMessageService,
            FileShareLink fileShareLink,
            FileConverter fileConverter,
            FFmpegService fFmpegService,
            IServiceProvider serviceProvider,
            TempStream tempStream)
        {
            FilesLinkUtility = filesLinkUtility;
            TenantExtra = tenantExtra;
            AuthContext = authContext;
            SecurityContext = securityContext;
            GlobalStore = globalStore;
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            FileMarker = fileMarker;
            SetupInfo = setupInfo;
            FileUtility = fileUtility;
            Global = global;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            CoreBaseSettings = coreBaseSettings;
            GlobalFolderHelper = globalFolderHelper;
            PathProvider = pathProvider;
            DocumentServiceTrackerHelper = documentServiceTrackerHelper;
            FilesMessageService = filesMessageService;
            FileShareLink = fileShareLink;
            FileConverter = fileConverter;
            FFmpegService = fFmpegService;
            ServiceProvider = serviceProvider;
            TempStream = tempStream;
            UserManager = userManager;
            Logger = optionsMonitor.CurrentValue;
        }

        public async Task Invoke(HttpContext context)
        {
            if (TenantExtra.IsNotPaid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.PaymentRequired;
                //context.Response.StatusDescription = "Payment Required.";
                return;
            }

            try
            {
                switch ((context.Request.Query[FilesLinkUtility.Action].FirstOrDefault() ?? "").ToLower())
                {
                    case "view":
                    case "download":
                        await DownloadFile(context).ConfigureAwait(false);
                        break;
                    case "bulk":
                        await BulkDownloadFile(context).ConfigureAwait(false);
                        break;
                    case "stream":
                        await StreamFile(context).ConfigureAwait(false);
                        break;
                    case "empty":
                        await EmptyFile(context).ConfigureAwait(false);
                        break;
                    case "tmp":
                        await TempFile(context).ConfigureAwait(false);
                        break;
                    case "create":
                        await CreateFile(context).ConfigureAwait(false);
                        break;
                    case "redirect":
                        Redirect(context);
                        break;
                    case "diff":
                        await DifferenceFile(context).ConfigureAwait(false);
                        break;
                    case "thumb":
                        await ThumbnailFile(context).ConfigureAwait(false);
                        break;
                    case "track":
                        await TrackFile(context).ConfigureAwait(false);
                        break;
                    default:
                        throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);
                }

            }
            catch (InvalidOperationException e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, FilesCommonResource.ErrorMassage_BadRequest, e);
            }
        }

        private async Task BulkDownloadFile(HttpContext context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            var ext = CompressToArchive.GetExt(ServiceProvider, context.Request.Query["ext"]);
            var store = GlobalStore.GetStore();
            var path = string.Format(@"{0}\{1}{2}", SecurityContext.CurrentAccount.ID, FileConstant.DownloadTitle, ext);

            if (!store.IsFile(FileConstant.StorageDomainTmp, path))
            {
                Logger.ErrorFormat("BulkDownload file error. File is not exist on storage. UserId: {0}.", AuthContext.CurrentAccount.ID);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (store.IsSupportedPreSignedUri)
            {
                var url = store.GetPreSignedUri(FileConstant.StorageDomainTmp, path, TimeSpan.FromHours(1), null).ToString();
                context.Response.Redirect(url);
                return;
            }

            context.Response.Clear();

            try
            {
                var flushed = false;
                using (var readStream = store.GetReadStream(FileConstant.StorageDomainTmp, path))
                {
                    long offset = 0;
                    var length = readStream.Length;
                    if (readStream.CanSeek)
                    {
                        length = ProcessRangeHeader(context, readStream.Length, ref offset);
                        readStream.Seek(offset, SeekOrigin.Begin);
                    }

                    flushed = await SendStreamByChunksAsync(context, length, FileConstant.DownloadTitle + ext, readStream, flushed);
                }

                await context.Response.Body.FlushAsync();
                await context.Response.CompleteAsync();
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("BulkDownloadFile failed for user {0} with error: ", SecurityContext.CurrentAccount.ID, e.Message);
                throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
            }
        }

        private async Task DownloadFile(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];

            if (int.TryParse(q, out var id))
            {
                await DownloadFile(context, id);
            }
            else
            {
                await DownloadFile(context, q.FirstOrDefault() ?? "");
            }
        }

        private async Task DownloadFile<T>(HttpContext context, T id)
        {
            var flushed = false;
            try
            {
                var doc = context.Request.Query[FilesLinkUtility.DocShareKey].FirstOrDefault() ?? "";

                var fileDao = DaoFactory.GetFileDao<T>();
                var readLink = FileShareLink.Check(doc, true, fileDao, out var file);
                if (!readLink && file == null)
                {
                    fileDao.InvalidateCache(id);

                    file = int.TryParse(context.Request.Query[FilesLinkUtility.Version], out var version) && version > 0
                               ? fileDao.GetFile(id, version)
                               : fileDao.GetFile(id);
                }

                if (file == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    return;
                }

                if (!readLink && !FileSecurity.CanRead(file))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (!string.IsNullOrEmpty(file.Error)) throw new Exception(file.Error);

                if (!fileDao.IsExistOnStorage(file))
                {
                    Logger.ErrorFormat("Download file error. File is not exist on storage. File id: {0}.", file.ID);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    return;
                }

                FileMarker.RemoveMarkAsNew(file);

                context.Response.Clear();
                context.Response.Headers.Clear();
                //TODO
                //context.Response.Headers.Charset = "utf-8";

                FilesMessageService.Send(file, MessageAction.FileDownloaded, file.Title);

                if (string.Equals(context.Request.Headers["If-None-Match"], GetEtag(file)))
                {
                    //Its cached. Reply 304
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                    //context.Response.Cache.SetETag(GetEtag(file));
                }
                else
                {
                    //context.Response.CacheControl = "public";
                    //context.Response.Cache.SetETag(GetEtag(file));
                    //context.Response.Cache.SetCacheability(HttpCacheability.Public);

                    Stream fileStream = null;
                    try
                    {
                        var title = file.Title;

                        if (file.ContentLength <= SetupInfo.AvailableFileSize)
                        {
                            var ext = FileUtility.GetFileExtension(file.Title);

                            var outType = (context.Request.Query[FilesLinkUtility.OutType].FirstOrDefault() ?? "").Trim();
                            if (!string.IsNullOrEmpty(outType)
                                && FileUtility.ExtsConvertible.Keys.Contains(ext)
                                && FileUtility.ExtsConvertible[ext].Contains(outType))
                            {
                                ext = outType;
                            }

                            long offset = 0;
                            long length;
                            if (!file.ProviderEntry
                                && string.Equals(context.Request.Query["convpreview"], "true", StringComparison.InvariantCultureIgnoreCase)
                                && FFmpegService.IsConvertable(ext))
                            {
                                const string mp4Name = "content.mp4";
                                var mp4Path = fileDao.GetUniqFilePath(file, mp4Name);
                                var store = GlobalStore.GetStore();
                                if (!store.IsFile(mp4Path))
                                {
                                    fileStream = fileDao.GetFileStream(file);

                                    Logger.InfoFormat("Converting {0} (fileId: {1}) to mp4", file.Title, file.ID);
                                    var stream = await FFmpegService.Convert(fileStream, ext);
                                    store.Save(string.Empty, mp4Path, stream, mp4Name);
                                }

                                var fullLength = store.GetFileSize(string.Empty, mp4Path);

                                length = ProcessRangeHeader(context, fullLength, ref offset);
                                fileStream = store.GetReadStream(string.Empty, mp4Path, (int)offset);

                                title = FileUtility.ReplaceFileExtension(title, ".mp4");
                            }
                            else
                            {
                                if (!FileConverter.EnableConvert(file, ext))
                                {
                                    if (!readLink && fileDao.IsSupportedPreSignedUri(file))
                                    {
                                        context.Response.Redirect(fileDao.GetPreSignedUri(file, TimeSpan.FromHours(1)).ToString(), true);

                                        return;
                                    }

                                    fileStream = fileDao.GetFileStream(file); // getStream to fix file.ContentLength

                                    if (fileStream.CanSeek)
                                    {
                                        var fullLength = file.ContentLength;
                                        length = ProcessRangeHeader(context, fullLength, ref offset);
                                        fileStream.Seek(offset, SeekOrigin.Begin);
                                    }
                                    else
                                    {
                                        length = file.ContentLength;
                                    }
                                }
                                else
                                {
                                    title = FileUtility.ReplaceFileExtension(title, ext);
                                    fileStream = FileConverter.Exec(file, ext);

                                    length = fileStream.Length;
                                }
                            }

                            flushed = await SendStreamByChunksAsync(context, length, title, fileStream, flushed);
                        }
                        else
                        {
                            if (!readLink && fileDao.IsSupportedPreSignedUri(file))
                            {
                                context.Response.Redirect(fileDao.GetPreSignedUri(file, TimeSpan.FromHours(1)).ToString(), true);

                                return;
                            }

                            fileStream = fileDao.GetFileStream(file); // getStream to fix file.ContentLength

                            long offset = 0;
                            var length = file.ContentLength;
                            if (fileStream.CanSeek)
                            {
                                length = ProcessRangeHeader(context, file.ContentLength, ref offset);
                                fileStream.Seek(offset, SeekOrigin.Begin);
                            }

                            flushed = await SendStreamByChunksAsync(context, length, title, fileStream, flushed);
                        }
                    }
                    catch (ThreadAbortException tae)
                    {
                        Logger.Error("DownloadFile", tae);
                    }
                    catch (HttpException e)
                    {
                        Logger.Error("DownloadFile", e);
                        throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
                    }
                    finally
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                            await fileStream.DisposeAsync();
                        }
                    }

                    try
                    {
                        await context.Response.Body.FlushAsync();
                        //context.Response.SuppressContent = true;
                        await context.Response.CompleteAsync();
                        flushed = true;
                    }
                    catch (HttpException ex)
                    {
                        Logger.Error("DownloadFile", ex);
                    }
                }
            }
            catch (ThreadAbortException tae)
            {
                Logger.Error("DownloadFile", tae);
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Logger.ErrorFormat("Url: {0} {1} IsClientConnected:{2}, line number:{3} frame:{4}", context.Request.Url(), ex, !context.RequestAborted.IsCancellationRequested, line, frame);
                if (!flushed && !context.RequestAborted.IsCancellationRequested)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(HttpUtility.HtmlEncode(ex.Message));
                }
            }
        }

        private long ProcessRangeHeader(HttpContext context, long fullLength, ref long offset)
        {
            if (context == null) throw new ArgumentNullException();
            if (context.Request.Headers["Range"].FirstOrDefault() == null) return fullLength;

            long endOffset = -1;

            var range = context.Request.Headers["Range"].FirstOrDefault().Split(new[] { '=', '-' });
            offset = Convert.ToInt64(range[1]);
            if (range.Count() > 2 && !string.IsNullOrEmpty(range[2]))
            {
                endOffset = Convert.ToInt64(range[2]);
            }
            if (endOffset < 0 || endOffset >= fullLength)
            {
                endOffset = fullLength - 1;
            }

            var length = endOffset - offset + 1;

            if (length <= 0) throw new HttpException(HttpStatusCode.RequestedRangeNotSatisfiable);

            Logger.InfoFormat("Starting file download (chunk {0}-{1})", offset, endOffset);
            if (length < fullLength)
            {
                context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
            }
            context.Response.Headers.Add("Accept-Ranges", "bytes");
            context.Response.Headers.Add("Content-Range", string.Format(" bytes {0}-{1}/{2}", offset, endOffset, fullLength));

            return length;
        }

        private async Task<bool> SendStreamByChunksAsync(HttpContext context, long toRead, string title, Stream fileStream, bool flushed)
        {
            //context.Response.Buffer = false;
            context.Response.Headers.Add("Connection", "Keep-Alive");
            context.Response.ContentLength = toRead;
            context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(title));
            context.Response.ContentType = MimeMapping.GetMimeMapping(title);

            var bufferSize = Convert.ToInt32(Math.Min(32 * 1024, toRead)); // 32KB
            var buffer = new byte[bufferSize];
            while (toRead > 0)
            {
                var length = await fileStream.ReadAsync(buffer, 0, bufferSize);
                await context.Response.Body.WriteAsync(buffer, 0, length, context.RequestAborted);
                await context.Response.Body.FlushAsync();
                flushed = true;
                toRead -= length;
            }


            return flushed;
        }

        private async Task StreamFile(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];

            if (int.TryParse(q, out var id))
            {
                await StreamFile(context, id);
            }
            else
            {
                await StreamFile(context, q.FirstOrDefault() ?? "");
            }
        }

        private async Task StreamFile<T>(HttpContext context, T id)
        {
            try
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                if (!int.TryParse(context.Request.Query[FilesLinkUtility.Version].FirstOrDefault() ?? "", out var version))
                {
                    version = 0;
                }
                var doc = context.Request.Query[FilesLinkUtility.DocShareKey];

                fileDao.InvalidateCache(id);

                var linkRight = FileShareLink.Check(doc, fileDao, out var file);
                if (linkRight == FileShare.Restrict && !SecurityContext.IsAuthenticated)
                {
                    var auth = context.Request.Query[FilesLinkUtility.AuthKey];
                    var validateResult = EmailValidationKeyProvider.ValidateEmailKey(id.ToString() + version, auth.FirstOrDefault() ?? "", Global.StreamUrlExpire);
                    if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                    {
                        var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                        Logger.Error($"{FilesLinkUtility.AuthKey} {validateResult}: {context.Request.Url()}", exc);

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                        return;
                    }

                    if (!string.IsNullOrEmpty(FileUtility.SignatureSecret))
                    {
                        try
                        {
                            var header = context.Request.Headers[FileUtility.SignatureHeader].FirstOrDefault();
                            if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                            {
                                throw new Exception("Invalid header " + header);
                            }

                            header = header.Substring("Bearer ".Length);

                            var stringPayload = JsonWebToken.Decode(header, FileUtility.SignatureSecret);

                            Logger.Debug("DocService StreamFile payload: " + stringPayload);
                            //var data = JObject.Parse(stringPayload);
                            //if (data == null)
                            //{
                            //    throw new ArgumentException("DocService StreamFile header is incorrect");
                            //}

                            //var signedStringUrl = data["url"] ?? (data["payload"] != null ? data["payload"]["url"] : null);
                            //if (signedStringUrl == null)
                            //{
                            //    throw new ArgumentException("DocService StreamFile header url is incorrect");
                            //}
                            //var signedUrl = new Uri(signedStringUrl.ToString());

                            //var signedQuery = signedUrl.Query;
                            //if (!context.Request.Url.Query.Equals(signedQuery))
                            //{
                            //    throw new SecurityException(string.Format("DocService StreamFile header id not equals: {0} and {1}", context.Request.Url.Query, signedQuery));
                            //}
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Download stream header " + context.Request.Url(), ex);
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                            return;
                        }
                    }
                }

                if (file == null
                    || version > 0 && file.Version != version)
                {
                    file = version > 0
                               ? fileDao.GetFile(id, version)
                               : fileDao.GetFile(id);
                }

                if (file == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (linkRight == FileShare.Restrict && SecurityContext.IsAuthenticated && !FileSecurity.CanRead(file))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (!string.IsNullOrEmpty(file.Error))
                {
                    await context.Response.WriteAsync(file.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(file.Title));
                context.Response.ContentType = MimeMapping.GetMimeMapping(file.Title);

                using var stream = fileDao.GetFileStream(file);
                context.Response.Headers.Add("Content-Length",
                    stream.CanSeek
                    ? stream.Length.ToString(CultureInfo.InvariantCulture)
                    : file.ContentLength.ToString(CultureInfo.InvariantCulture));
                await stream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                Logger.Error("Error for: " + context.Request.Url(), ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            try
            {
                await context.Response.Body.FlushAsync();
                await context.Response.CompleteAsync();
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (HttpException he)
            {
                Logger.ErrorFormat("StreamFile", he);
            }
        }

        private async Task EmptyFile(HttpContext context)
        {
            try
            {
                var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
                if (!string.IsNullOrEmpty(FileUtility.SignatureSecret))
                {
                    try
                    {
                        var header = context.Request.Headers[FileUtility.SignatureHeader].FirstOrDefault();
                        if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                        {
                            throw new Exception("Invalid header " + header);
                        }

                        header = header.Substring("Bearer ".Length);

                        var stringPayload = JsonWebToken.Decode(header, FileUtility.SignatureSecret);

                        Logger.Debug("DocService EmptyFile payload: " + stringPayload);
                        //var data = JObject.Parse(stringPayload);
                        //if (data == null)
                        //{
                        //    throw new ArgumentException("DocService EmptyFile header is incorrect");
                        //}

                        //var signedStringUrl = data["url"] ?? (data["payload"] != null ? data["payload"]["url"] : null);
                        //if (signedStringUrl == null)
                        //{
                        //    throw new ArgumentException("DocService EmptyFile header url is incorrect");
                        //}
                        //var signedUrl = new Uri(signedStringUrl.ToString());

                        //var signedQuery = signedUrl.Query;
                        //if (!context.Request.Url.Query.Equals(signedQuery))
                        //{
                        //    throw new SecurityException(string.Format("DocService EmptyFile header id not equals: {0} and {1}", context.Request.Url.Query, signedQuery));
                        //}
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Download stream header " + context.Request.Url(), ex);
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                        return;
                    }
                }

                var toExtension = FileUtility.GetFileExtension(fileName);
                var fileExtension = FileUtility.GetInternalExtension(toExtension);
                fileName = "new" + fileExtension;
                var path = FileConstant.NewDocPath
                           + (CoreBaseSettings.CustomMode ? "ru-RU/" : "en-US/")
                           + fileName;

                var storeTemplate = GlobalStore.GetStoreTemplate();
                if (!storeTemplate.IsFile("", path))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_FileNotFound);
                    return;
                }

                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(fileName));
                context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);

                using var stream = storeTemplate.GetReadStream("", path);
                context.Response.Headers.Add("Content-Length",
                    stream.CanSeek
                    ? stream.Length.ToString(CultureInfo.InvariantCulture)
                    : storeTemplate.GetFileSize("", path).ToString(CultureInfo.InvariantCulture));
                await stream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                Logger.Error("Error for: " + context.Request.Url(), ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            try
            {
                await context.Response.Body.FlushAsync();
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (HttpException he)
            {
                Logger.ErrorFormat("EmptyFile", he);
            }
        }

        private async Task TempFile(HttpContext context)
        {
            var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
            var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();

            var validateResult = EmailValidationKeyProvider.ValidateEmailKey(fileName, auth ?? "", Global.StreamUrlExpire);
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                Logger.Error($"{FilesLinkUtility.AuthKey} {validateResult}: {context.Request.Url()}", exc);

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);
            context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(fileName));

            var store = GlobalStore.GetStore();

            var path = CrossPlatform.PathCombine("temp_stream", fileName);

            if (!store.IsFile(FileConstant.StorageDomainTmp, path))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_FileNotFound);
                return;
            }

            using (var readStream = store.GetReadStream(FileConstant.StorageDomainTmp, path))
            {
                context.Response.Headers.Add("Content-Length", readStream.Length.ToString(CultureInfo.InvariantCulture));
                await readStream.CopyToAsync(context.Response.Body);
            }

            store.Delete(FileConstant.StorageDomainTmp, path);

            try
            {
                await context.Response.Body.FlushAsync();
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (HttpException he)
            {
                Logger.ErrorFormat("TempFile", he);
            }
        }

        private async Task DifferenceFile(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];

            if (int.TryParse(q, out var id))
            {
                await DifferenceFile(context, id);
            }
            else
            {
                await DifferenceFile(context, q.FirstOrDefault() ?? "");
            }
        }

        private async Task DifferenceFile<T>(HttpContext context, T id)
        {
            try
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                int.TryParse(context.Request.Query[FilesLinkUtility.Version].FirstOrDefault() ?? "", out var version);
                var doc = context.Request.Query[FilesLinkUtility.DocShareKey];

                var linkRight = FileShareLink.Check(doc, fileDao, out var file);
                if (linkRight == FileShare.Restrict && !SecurityContext.IsAuthenticated)
                {
                    var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();
                    var validateResult = EmailValidationKeyProvider.ValidateEmailKey(id.ToString() + version, auth ?? "", Global.StreamUrlExpire);
                    if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                    {
                        var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                        Logger.Error($"{FilesLinkUtility.AuthKey} {validateResult}: {context.Request.Url()}", exc);

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                        return;
                    }
                }

                fileDao.InvalidateCache(id);

                if (file == null
                    || version > 0 && file.Version != version)
                {
                    file = version > 0
                               ? fileDao.GetFile(id, version)
                               : fileDao.GetFile(id);
                }

                if (file == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (linkRight == FileShare.Restrict && SecurityContext.IsAuthenticated && !FileSecurity.CanRead(file))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (!string.IsNullOrEmpty(file.Error))
                {
                    await context.Response.WriteAsync(file.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(".zip"));
                context.Response.ContentType = MimeMapping.GetMimeMapping(".zip");

                using var stream = fileDao.GetDifferenceStream(file);
                context.Response.Headers.Add("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                await stream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                Logger.Error("Error for: " + context.Request.Url(), ex);
                return;
            }

            try
            {
                await context.Response.Body.FlushAsync();
                //context.Response.SuppressContent = true;
                //context.ApplicationInstance.CompleteRequest();
            }
            catch (HttpException he)
            {
                Logger.ErrorFormat("DifferenceFile", he);
            }
        }

        private async Task ThumbnailFile(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];

            if (int.TryParse(q, out var id))
            {
                await ThumbnailFile(context, id);
            }
            else
            {
                await ThumbnailFile(context, q.FirstOrDefault() ?? "");
            }
        }

        private async Task ThumbnailFile<T>(HttpContext context, T id)
        {
            try
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                var file = int.TryParse(context.Request.Query[FilesLinkUtility.Version], out var version) && version > 0
                   ? fileDao.GetFile(id, version)
                   : fileDao.GetFile(id);

                if (file == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (!FileSecurity.CanRead(file))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (!string.IsNullOrEmpty(file.Error))
                {
                    await context.Response.WriteAsync(file.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                if (file.ThumbnailStatus != Thumbnail.Created)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue("." + Global.ThumbnailExtension));
                context.Response.ContentType = MimeMapping.GetMimeMapping("." + Global.ThumbnailExtension);

                using (var stream = fileDao.GetThumbnail(file))
                {
                    context.Response.Headers.Add("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                    await stream.CopyToAsync(context.Response.Body);
                }
            }
            catch (FileNotFoundException ex)
            {
                Logger.Error("Error for: " + context.Request.Url(), ex);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error("Error for: " + context.Request.Url(), ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            try
            {
                await context.Response.Body.FlushAsync();
                await context.Response.CompleteAsync();
            }
            catch (HttpException he)
            {
                Logger.ErrorFormat("Thumbnail", he);
            }
        }

        private static string GetEtag<T>(File<T> file)
        {
            return file.ID + ":" + file.Version + ":" + file.Title.GetHashCode() + ":" + file.ContentLength;
        }

        private async Task CreateFile(HttpContext context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                //var refererURL = context.Request.GetUrlRewriter().AbsoluteUri;

                //context.Session["refererURL"] = refererURL;
                var authUrl = "~/Auth.aspx";
                context.Response.Redirect(authUrl, true);
                return;
            }

            var folderId = context.Request.Query[FilesLinkUtility.FolderId].FirstOrDefault();
            if (string.IsNullOrEmpty(folderId))
            {
                await CreateFile(context, GlobalFolderHelper.FolderMy);
            }
            else
            {
                if (int.TryParse(folderId, out var id))
                {
                    await CreateFile(context, id);
                }
                else
                {
                    await CreateFile(context, folderId);
                }
            }
        }

        private async Task CreateFile<T>(HttpContext context, T folderId)
        {
            var responseMessage = context.Request.Query["response"] == "message";
            Folder<T> folder;

            var folderDao = DaoFactory.GetFolderDao<T>();
            folder = folderDao.GetFolder(folderId);

            if (folder == null) throw new HttpException((int)HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FolderNotFound);
            if (!FileSecurity.CanCreate(folder)) throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException_Create);

            File<T> file;
            var fileUri = context.Request.Query[FilesLinkUtility.FileUri];
            var fileTitle = context.Request.Query[FilesLinkUtility.FileTitle];
            try
            {
                if (!string.IsNullOrEmpty(fileUri))
                {
                    file = CreateFileFromUri(folder, fileUri, fileTitle);
                }
                else
                {
                    var docType = context.Request.Query["doctype"];
                    file = CreateFileFromTemplate(folder, fileTitle, docType);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (responseMessage)
                {
                    await context.Response.WriteAsync("error: " + ex.Message);
                    return;
                }
                context.Response.Redirect(PathProvider.StartURL + "#error/" + HttpUtility.UrlEncode(ex.Message), true);
                return;
            }

            FileMarker.MarkAsNew(file);

            if (responseMessage)
            {
                await context.Response.WriteAsync("ok: " + string.Format(FilesCommonResource.MessageFileCreated, folder.Title));
                return;
            }

            context.Response.Redirect(
                (context.Request.Query["openfolder"].FirstOrDefault() ?? "").Equals("true")
                    ? PathProvider.GetFolderUrlById(file.FolderID)
                    : (FilesLinkUtility.GetFileWebEditorUrl(file.ID) + "#message/" + HttpUtility.UrlEncode(string.Format(FilesCommonResource.MessageFileCreated, folder.Title))));
        }

        private File<T> CreateFileFromTemplate<T>(Folder<T> folder, string fileTitle, string docType)
        {
            var storeTemplate = GlobalStore.GetStoreTemplate();

            var lang = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();

            var fileExt = FileUtility.InternalExtension[FileType.Document];
            if (!string.IsNullOrEmpty(docType))
            {
                var tmpFileType = Configuration<T>.DocType.FirstOrDefault(r => r.Value.Equals(docType, StringComparison.OrdinalIgnoreCase));
                FileUtility.InternalExtension.TryGetValue(tmpFileType.Key, out var tmpFileExt);
                if (!string.IsNullOrEmpty(tmpFileExt))
                    fileExt = tmpFileExt;
            }

            var templateName = "new" + fileExt;

            var templatePath = FileConstant.NewDocPath + lang + "/";
            if (!storeTemplate.IsDirectory(templatePath))
                templatePath = FileConstant.NewDocPath + "en-US/";
            templatePath += templateName;

            if (string.IsNullOrEmpty(fileTitle))
            {
                fileTitle = templateName;
            }
            else
            {
                fileTitle += fileExt;
            }

            var file = ServiceProvider.GetService<File<T>>();
            file.Title = fileTitle;
            file.FolderID = folder.ID;
            file.Comment = FilesCommonResource.CommentCreate;

            var fileDao = DaoFactory.GetFileDao<T>();
            var stream = storeTemplate.GetReadStream("", templatePath);
            file.ContentLength = stream.CanSeek ? stream.Length : storeTemplate.GetFileSize(templatePath);
            return fileDao.SaveFile(file, stream);
        }

        private File<T> CreateFileFromUri<T>(Folder<T> folder, string fileUri, string fileTitle)
        {
            if (string.IsNullOrEmpty(fileTitle))
                fileTitle = Path.GetFileName(HttpUtility.UrlDecode(fileUri));

            var file = ServiceProvider.GetService<File<T>>();
            file.Title = fileTitle;
            file.FolderID = folder.ID;
            file.Comment = FilesCommonResource.CommentCreate;

            var req = WebRequest.Create(fileUri);

            // hack. http://ubuntuforums.org/showthread.php?t=1841740
            if (WorkContext.IsMono)
            {
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
            }

            var fileDao = DaoFactory.GetFileDao<T>();
            using var fileStream = req.GetResponse().GetResponseStream();

            if (fileStream.CanSeek)
            {
                file.ContentLength = fileStream.Length;
                return fileDao.SaveFile(file, fileStream);
            }
            else
            {
                using var buffered = TempStream.GetBuffered(fileStream);
                file.ContentLength = buffered.Length;
                return fileDao.SaveFile(file, buffered);
            }


        }

        private void Redirect(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];
            var q1 = context.Request.Query[FilesLinkUtility.FolderId];

            if (int.TryParse(q, out var fileId) && int.TryParse(q1, out var folderId))
            {
                Redirect(context, fileId, folderId);
            }
            else
            {
                Redirect(context, q.FirstOrDefault() ?? "", q1.FirstOrDefault() ?? "");
            }
        }

        private void Redirect<T>(HttpContext context, T folderId, T fileId)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
            var urlRedirect = string.Empty;
            if (folderId != null)
            {
                try
                {
                    urlRedirect = PathProvider.GetFolderUrlById(folderId);
                }
                catch (ArgumentNullException e)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
                }
            }

            if (fileId != null)
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                var file = fileDao.GetFile(fileId);
                if (file == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                urlRedirect = FilesLinkUtility.GetFileWebPreviewUrl(FileUtility, file.Title, file.ID);
            }

            if (string.IsNullOrEmpty(urlRedirect))
                throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);
            context.Response.Redirect(urlRedirect);
        }

        private async Task TrackFile(HttpContext context)
        {
            var q = context.Request.Query[FilesLinkUtility.FileId];

            if (int.TryParse(q, out var id))
            {
                await TrackFile(context, id);
            }
            else
            {
                await TrackFile(context, q.FirstOrDefault() ?? "");
            }
        }

        private async Task TrackFile<T>(HttpContext context, T fileId)
        {
            var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();
            Logger.Debug("DocService track fileid: " + fileId);

            var callbackSpan = TimeSpan.FromDays(128);
            var validateResult = EmailValidationKeyProvider.ValidateEmailKey(fileId.ToString(), auth ?? "", callbackSpan);
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                Logger.ErrorFormat("DocService track auth error: {0}, {1}: {2}", validateResult.ToString(), FilesLinkUtility.AuthKey, auth);
                throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
            }

            DocumentServiceTracker.TrackerData fileData;
            try
            {
                string body;
                var receiveStream = context.Request.Body;
                using var readStream = new StreamReader(receiveStream);
                body = await readStream.ReadToEndAsync();

                Logger.Debug("DocService track body: " + body);
                if (string.IsNullOrEmpty(body))
                {
                    throw new ArgumentException("DocService request body is incorrect");
                }

                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                fileData = JsonSerializer.Deserialize<DocumentServiceTracker.TrackerData>(body, options);
            }
            catch (JsonException e)
            {
                Logger.Error("DocService track error read body", e);
                throw new HttpException((int)HttpStatusCode.BadRequest, "DocService request is incorrect");
            }
            catch (Exception e)
            {
                Logger.Error("DocService track error read body", e);
                throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
            }

            if (!string.IsNullOrEmpty(FileUtility.SignatureSecret))
            {
                if (!string.IsNullOrEmpty(fileData.Token))
                {
                    try
                    {
                        var dataString = JsonWebToken.Decode(fileData.Token, FileUtility.SignatureSecret);
                        var data = JObject.Parse(dataString);
                        if (data == null)
                        {
                            throw new ArgumentException("DocService request token is incorrect");
                        }
                        fileData = data.ToObject<DocumentServiceTracker.TrackerData>();
                    }
                    catch (SignatureVerificationException ex)
                    {
                        Logger.Error("DocService track header", ex);
                        throw new HttpException((int)HttpStatusCode.Forbidden, ex.Message);
                    }
                }
                else
                {
                    //todo: remove old scheme
                    var header = context.Request.Headers[FileUtility.SignatureHeader].FirstOrDefault();
                    if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                    {
                        Logger.Error("DocService track header is null");
                        throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
                    }
                    header = header.Substring("Bearer ".Length);

                    try
                    {
                        var stringPayload = JsonWebToken.Decode(header, FileUtility.SignatureSecret);

                        Logger.Debug("DocService track payload: " + stringPayload);
                        var jsonPayload = JObject.Parse(stringPayload);
                        var data = jsonPayload["payload"];
                        if (data == null)
                        {
                            throw new ArgumentException("DocService request header is incorrect");
                        }
                        fileData = data.ToObject<DocumentServiceTracker.TrackerData>();
                    }
                    catch (SignatureVerificationException ex)
                    {
                        Logger.Error("DocService track header", ex);
                        throw new HttpException((int)HttpStatusCode.Forbidden, ex.Message);
                    }
                }
            }

            DocumentServiceTracker.TrackResponse result;
            try
            {
                result = DocumentServiceTrackerHelper.ProcessData(fileId, fileData);
            }
            catch (Exception e)
            {
                Logger.Error("DocService track:", e);
                throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
            }
            result ??= new DocumentServiceTracker.TrackResponse();

            await context.Response.WriteAsync(DocumentServiceTracker.TrackResponse.Serialize(result));
        }
    }

    public static class FileHandlerExtensions
    {
        public static IApplicationBuilder UseFileHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FileHandler>();
        }
    }
}