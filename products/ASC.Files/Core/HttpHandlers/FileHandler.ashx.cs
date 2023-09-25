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

using SixLabors.ImageSharp.Processing;

using Image = SixLabors.ImageSharp.Image;
using JsonException = System.Text.Json.JsonException;
using MimeMapping = ASC.Common.Web.MimeMapping;
using Status = ASC.Files.Core.Security.Status;

namespace ASC.Web.Files;

public class FileHandler
{
    public FileHandler(RequestDelegate _)
    {
    }

    public async Task Invoke(HttpContext context, FileHandlerService fileHandlerService)
    {
        await fileHandlerService.InvokeAsync(context);
    }
}

[Scope]
public class FileHandlerService
{
    private readonly ThumbnailSettings _thumbnailSettings;

    public string FileHandlerPath
    {
        get { return _filesLinkUtility.FileHandlerPath; }
    }

    private readonly CompressToArchive _compressToArchive;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthContext _authContext;
    private readonly SecurityContext _securityContext;
    private readonly GlobalStore _globalStore;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly FileMarker _fileMarker;
    private readonly SetupInfo _setupInfo;
    private readonly FileUtility _fileUtility;
    private readonly Global _global;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly PathProvider _pathProvider;
    private readonly DocumentServiceTrackerHelper _documentServiceTrackerHelper;
    private readonly FilesMessageService _filesMessageService;
    private readonly FileShareLink _fileShareLink;
    private readonly FileConverter _fileConverter;
    private readonly FFmpegService _fFmpegService;
    private readonly IServiceProvider _serviceProvider;
    private readonly TempStream _tempStream;
    private readonly UserManager _userManager;
    private readonly SocketManager _socketManager;
    private readonly ILogger<FileHandlerService> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ExternalLinkHelper _externalLinkHelper;
    private readonly ExternalShare _externalShare;

    public FileHandlerService(
        FilesLinkUtility filesLinkUtility,
        TenantExtra tenantExtra,
        AuthContext authContext,
        SecurityContext securityContext,
        GlobalStore globalStore,
        ILogger<FileHandlerService> logger,
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
        TempStream tempStream,
        SocketManager socketManager,
        CompressToArchive compressToArchive,
        InstanceCrypto instanceCrypto,
        IHttpClientFactory clientFactory,
        ThumbnailSettings thumbnailSettings,
        ExternalLinkHelper externalLinkHelper,
        ExternalShare externalShare)
    {
        _filesLinkUtility = filesLinkUtility;
        _tenantExtra = tenantExtra;
        _authContext = authContext;
        _securityContext = securityContext;
        _globalStore = globalStore;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _fileMarker = fileMarker;
        _setupInfo = setupInfo;
        _fileUtility = fileUtility;
        _global = global;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _coreBaseSettings = coreBaseSettings;
        _globalFolderHelper = globalFolderHelper;
        _pathProvider = pathProvider;
        _documentServiceTrackerHelper = documentServiceTrackerHelper;
        _filesMessageService = filesMessageService;
        _fileShareLink = fileShareLink;
        _fileConverter = fileConverter;
        _fFmpegService = fFmpegService;
        _serviceProvider = serviceProvider;
        _socketManager = socketManager;
        _compressToArchive = compressToArchive;
        _instanceCrypto = instanceCrypto;
        _tempStream = tempStream;
        _userManager = userManager;
        _logger = logger;
        _clientFactory = clientFactory;
        _thumbnailSettings = thumbnailSettings;
        _externalLinkHelper = externalLinkHelper;
        _externalShare = externalShare;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (await _tenantExtra.IsNotPaidAsync())
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
                    await DownloadFile(context, true);
                    break;
                case "download":
                    await DownloadFile(context);
                    break;
                case "bulk":
                    await BulkDownloadFile(context);
                    break;
                case "stream":
                    await StreamFile(context);
                    break;
                case "empty":
                    await EmptyFile(context);
                    break;
                case "tmp":
                    await TempFile(context);
                    break;
                case "create":
                    await CreateFile(context);
                    break;
                case "redirect":
                    await RedirectAsync(context);
                    break;
                case "diff":
                    await DifferenceFile(context);
                    break;
                case "thumb":
                    await ThumbnailFile(context, false);
                    break;
                case "preview":
                    await ThumbnailFile(context, true);
                    break;
                case "track":
                    await TrackFile(context);
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

    private async ValueTask BulkDownloadFile(HttpContext context)
    {
        var filename = context.Request.Query["filename"].FirstOrDefault();
        var sessionKey = context.Request.Query["session"].FirstOrDefault();

        if (!_securityContext.IsAuthenticated && string.IsNullOrEmpty(sessionKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        if (String.IsNullOrEmpty(filename))
        {
            var ext = _compressToArchive.GetExt(context.Request.Query["ext"]);
            filename = FileConstant.DownloadTitle + ext;
        }
        else
        {
            filename = _instanceCrypto.Decrypt(Uri.UnescapeDataString(filename));
        }

        string path;

        if (!string.IsNullOrEmpty(sessionKey))
        {
            var session = await _externalShare.ParseDownloadSessionKeyAsync(sessionKey);
            var sessionId = await _externalShare.GetSessionIdAsync();

            if (session != null && sessionId != default && session.Id == sessionId &&
                (await _externalShare.ValidateAsync(session.LinkId)) == Status.Ok)
            {
                path = string.Format(@"{0}\{1}\{2}", session.LinkId, session.Id, filename);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }
        else
        {
            path = string.Format(@"{0}\{1}", _securityContext.CurrentAccount.ID, filename);
        }

        var store = await _globalStore.GetStoreAsync();

        if (!await store.IsFileAsync(FileConstant.StorageDomainTmp, path))
        {
            _logger.ErrorBulkDownloadFile(_authContext.CurrentAccount.ID);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        if (store.IsSupportedPreSignedUri)
        {
            var headers = _securityContext.IsAuthenticated ? null : new[] { SecureHelper.GenerateSecureKeyHeader(path, _emailValidationKeyProvider) };

            var tmp = await store.GetPreSignedUriAsync(FileConstant.StorageDomainTmp, path, TimeSpan.FromHours(1), headers);
            var url = tmp.ToString();
            context.Response.Redirect(HttpUtility.UrlPathEncode(url));
            return;
        }

        context.Response.Clear();

        try
        {
            var flushed = false;
            await using (var readStream = await store.GetReadStreamAsync(FileConstant.StorageDomainTmp, path))
            {
                long offset = 0;
                var length = readStream.Length;
                if (readStream.CanSeek)
                {
                    length = ProcessRangeHeader(context, readStream.Length, ref offset);
                    readStream.Seek(offset, SeekOrigin.Begin);
                }

                await SendStreamByChunksAsync(context, length, filename, readStream, flushed);
            }

            await context.Response.Body.FlushAsync();
            await context.Response.CompleteAsync();
            //context.Response.SuppressContent = true;
            //context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception e)
        {
            _logger.ErrorBulkDownloadFileFailed(_securityContext.CurrentAccount.ID, e);
            throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
        }
    }

    private async Task DownloadFile(HttpContext context, bool forView = false)
    {
        var q = context.Request.Query[FilesLinkUtility.FileId];

        if (int.TryParse(q, out var id))
        {
            await DownloadFile(context, id, forView);
        }
        else
        {
            await DownloadFile(context, q.FirstOrDefault() ?? "", forView);
        }
    }

    private async Task DownloadFile<T>(HttpContext context, T id, bool forView)
    {
        var flushed = false;
        try
        {
            var doc = context.Request.Query[FilesLinkUtility.DocShareKey].FirstOrDefault() ?? "";

            var fileDao = _daoFactory.GetFileDao<T>();
            var version = 0;
            var (readLink, file, linkShare) = await _fileShareLink.CheckAsync(doc, true, fileDao);
            if (!readLink && file == null)
            {
                await fileDao.InvalidateCacheAsync(id);

                file = int.TryParse(context.Request.Query[FilesLinkUtility.Version], out version) && version > 0
                           ? await fileDao.GetFileAsync(id, version)
                           : await fileDao.GetFileAsync(id);
            }

            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            if (!readLink && !await _fileSecurity.CanDownloadAsync(file))
            {
                var linkId = await _externalShare.GetLinkIdAsync();

                if (!(_fileUtility.CanImageView(file.Title) || _fileUtility.CanMediaView(file.Title) || FileUtility.GetFileExtension(file.Title) == ".pdf") ||
                    linkId == default || !await _fileSecurity.CanReadAsync(file, linkId))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            if (readLink && (linkShare == FileShare.Comment || linkShare == FileShare.Read) && file.DenyDownload)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            if (!string.IsNullOrEmpty(file.Error))
            {
                throw new Exception(file.Error);
            }

            if (!await fileDao.IsExistOnStorageAsync(file))
            {
                _logger.ErrorDownloadFile2(file.Id.ToString());
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            await _fileMarker.RemoveMarkAsNewAsync(file);

            context.Response.Clear();
            context.Response.Headers.Clear();
            //TODO
            //context.Response.Headers.Charset = "utf-8";

            var range = (context.Request.Headers["Range"].FirstOrDefault() ?? "").Split(new[] { '=', '-' });
            var isNeedSendAction = range.Count() < 2 || Convert.ToInt64(range[1]) == 0;

            if (isNeedSendAction)
            {
                if (forView)
                {
                    _ = _filesMessageService.SendAsync(MessageAction.FileReaded, file, file.Title);
                }
                else
                {
                    if (version == 0)
                    {
                        _ = _filesMessageService.SendAsync(MessageAction.FileDownloaded, file, file.Title);
                    }
                    else
                    {
                        await _filesMessageService.SendAsync(MessageAction.FileRevisionDownloaded, file, file.Title, file.Version.ToString());
                    }
                }
            }

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

                    if (file.ContentLength <= _setupInfo.AvailableFileSize)
                    {
                        var ext = FileUtility.GetFileExtension(file.Title);

                        var outType = (context.Request.Query[FilesLinkUtility.OutType].FirstOrDefault() ?? "").Trim();
                        if (!string.IsNullOrEmpty(outType)
                            && (await _fileUtility.GetExtsConvertibleAsync()).ContainsKey(ext) && (await _fileUtility.GetExtsConvertibleAsync())[ext].Contains(outType))
                        {
                            ext = outType;
                        }

                        long offset = 0;
                        long length;
                        if (!file.ProviderEntry
                            && string.Equals(context.Request.Query["convpreview"], "true", StringComparison.InvariantCultureIgnoreCase)
                            && _fFmpegService.IsConvertable(ext))
                        {
                            const string mp4Name = "content.mp4";
                            var mp4Path = fileDao.GetUniqFilePath(file, mp4Name);
                            var store = await _globalStore.GetStoreAsync();
                            if (!await store.IsFileAsync(mp4Path))
                            {
                                fileStream = await fileDao.GetFileStreamAsync(file);

                                _logger.InformationConvertingToMp4(file.Title, file.Id.ToString());
                                var stream = await _fFmpegService.ConvertAsync(fileStream, ext);
                                await store.SaveAsync(string.Empty, mp4Path, stream, mp4Name);
                            }

                            var fullLength = await store.GetFileSizeAsync(string.Empty, mp4Path);

                            length = ProcessRangeHeader(context, fullLength, ref offset);
                            fileStream = await store.GetReadStreamAsync(string.Empty, mp4Path, offset);

                            title = FileUtility.ReplaceFileExtension(title, ".mp4");
                        }
                        else
                        {
                            if (!await _fileConverter.EnableConvertAsync(file, ext))
                            {
                                if (!readLink && await fileDao.IsSupportedPreSignedUriAsync(file))
                                {
                                    context.Response.Redirect((await fileDao.GetPreSignedUriAsync(file, TimeSpan.FromHours(1))).ToString(), false);

                                    return;
                                }

                                fileStream = await fileDao.GetFileStreamAsync(file); // getStream to fix file.ContentLength

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
                                fileStream = await _fileConverter.ExecAsync(file, ext);

                                length = fileStream.Length;
                            }
                        }

                        flushed = await SendStreamByChunksAsync(context, length, title, fileStream, flushed);
                    }
                    else
                    {
                        if (!readLink && await fileDao.IsSupportedPreSignedUriAsync(file))
                        {
                            context.Response.Redirect((await fileDao.GetPreSignedUriAsync(file, TimeSpan.FromHours(1))).ToString(), true);

                            return;
                        }

                        fileStream = await fileDao.GetFileStreamAsync(file); // getStream to fix file.ContentLength

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
                    _logger.ErrorDownloadFile(tae);
                }
                catch (HttpException e)
                {
                    _logger.ErrorDownloadFile(e);
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
                    _logger.ErrorDownloadFile(ex);
                }
            }
        }
        catch (ThreadAbortException tae)
        {
            _logger.ErrorDownloadFile(tae);
        }
        catch (Exception ex)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();

            _logger.ErrorUrl(context.Request.Url(), !context.RequestAborted.IsCancellationRequested, line, frame, ex);
            if (!flushed && !context.RequestAborted.IsCancellationRequested)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(HttpUtility.HtmlEncode(ex.Message));
            }
        }
    }

    private long ProcessRangeHeader(HttpContext context, long fullLength, ref long offset)
    {
        if (context == null)
        {
            throw new ArgumentNullException();
        }

        if (context.Request.Headers["Range"].FirstOrDefault() == null)
        {
            return fullLength;
        }

        long endOffset = -1;

        var range = context.Request.Headers["Range"].FirstOrDefault().Split(new[] { '=', '-' });
        offset = Convert.ToInt64(range[1]);
        if (range.Length > 2 && !string.IsNullOrEmpty(range[2]))
        {
            endOffset = Convert.ToInt64(range[2]);
        }
        if (endOffset < 0 || endOffset >= fullLength)
        {
            endOffset = fullLength - 1;
        }

        var length = endOffset - offset + 1;

        if (length <= 0)
        {
            throw new HttpException(HttpStatusCode.RequestedRangeNotSatisfiable);
        }

        _logger.InformationStartingFileDownLoad(offset, endOffset);
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
            await StreamFileAsync(context, id);
        }
        else
        {
            await StreamFileAsync(context, q.FirstOrDefault() ?? "");
        }
    }

    private async Task StreamFileAsync<T>(HttpContext context, T id)
    {
        try
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            if (!int.TryParse(context.Request.Query[FilesLinkUtility.Version].FirstOrDefault() ?? "", out var version))
            {
                version = 0;
            }
            var doc = context.Request.Query[FilesLinkUtility.DocShareKey];
            var share = context.Request.Query[FilesLinkUtility.FolderShareKey];

            await fileDao.InvalidateCacheAsync(id);

            var linkRight = FileShare.Restrict;
            File<T> file = null;

            if (!string.IsNullOrEmpty(share))
            {
                var result = await _externalLinkHelper.ValidateAsync(share);

                if (result.Access != FileShare.Restrict)
                {
                    file = version > 0
                        ? await fileDao.GetFileAsync(id, version)
                        : await fileDao.GetFileAsync(id);

                    if (file != null && await _fileSecurity.CanDownloadAsync(file))
                    {
                        linkRight = result.Access;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(doc))
            {
                (linkRight, file) = await _fileShareLink.CheckAsync(doc, fileDao);
            }

            if (linkRight == FileShare.Restrict && !_securityContext.IsAuthenticated)
            {
                var auth = context.Request.Query[FilesLinkUtility.AuthKey];
                var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(id.ToString() + version, auth.FirstOrDefault() ?? "", _global.StreamUrlExpire);
                if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                    _logger.Error(FilesLinkUtility.AuthKey, validateResult, context.Request.Url(), exc);

                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                    return;
                }

                if (!string.IsNullOrEmpty(_fileUtility.SignatureSecret))
                {
                    try
                    {
                        var header = context.Request.Headers[_fileUtility.SignatureHeader].FirstOrDefault();
                        if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                        {
                            var requestHeaderTrace = String.Empty;

                            foreach (var requestHeader in context.Request.Headers)
                            {
                                requestHeaderTrace += $"{requestHeader.Key}={requestHeader.Value}" + Environment.NewLine;
                            }

                            var exceptionMessage = $"Invalid signature header {_fileUtility.SignatureHeader} with value {header}." +
                                                   $"Trace headers: {requestHeaderTrace}  ";


                            throw new Exception(exceptionMessage);
                        }

                        header = header.Substring("Bearer ".Length);

#pragma warning disable CS0618 // Type or member is obsolete
                        var stringPayload = JwtBuilder.Create()
                                                .WithAlgorithm(new HMACSHA256Algorithm())
                                                .WithJsonSerializer(new JwtSerializer())
                                                .WithSecret(_fileUtility.SignatureSecret)
                                                .MustVerifySignature()
                                                .Decode(header);
#pragma warning restore CS0618 // Type or member is obsolete

                        _logger.DebugDocServiceStreamFilePayload(stringPayload);
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
                        _logger.ErrorDownloadStreamHeader(context.Request.Url(), ex);
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
                           ? await fileDao.GetFileAsync(id, version)
                           : await fileDao.GetFileAsync(id);
            }

            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (linkRight == FileShare.Restrict && _securityContext.IsAuthenticated && !await _fileSecurity.CanDownloadAsync(file))
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

            await using var stream = await fileDao.GetFileStreamAsync(file);
            context.Response.Headers.Add("Content-Length",
                stream.CanSeek
                ? stream.Length.ToString(CultureInfo.InvariantCulture)
                : file.ContentLength.ToString(CultureInfo.InvariantCulture));
            await stream.CopyToAsync(context.Response.Body);
        }
        catch (Exception ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
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
            _logger.ErrorStreamFile(he);
        }
    }

    private async Task EmptyFile(HttpContext context)
    {
        try
        {
            var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
            if (!string.IsNullOrEmpty(_fileUtility.SignatureSecret))
            {
                try
                {
                    var header = context.Request.Headers[_fileUtility.SignatureHeader].FirstOrDefault();
                    if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                    {
                        throw new Exception("Invalid header " + header);
                    }

                    header = header.Substring("Bearer ".Length);

#pragma warning disable CS0618 // Type or member is obsolete
                    var stringPayload = JwtBuilder.Create()
                                                    .WithAlgorithm(new HMACSHA256Algorithm())
                                                    .WithJsonSerializer(new JwtSerializer())
                                                    .WithSecret(_fileUtility.SignatureSecret)
                                                    .MustVerifySignature()
                                                    .Decode(header);
#pragma warning restore CS0618 // Type or member is obsolete

                    _logger.DebugDocServiceStreamFilePayload(stringPayload);
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
                    _logger.ErrorDownloadStreamHeader(context.Request.Url(), ex);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                    return;
                }
            }

            var toExtension = FileUtility.GetFileExtension(fileName);
            var fileExtension = _fileUtility.GetInternalExtension(toExtension);
            fileName = "new" + fileExtension;
            var path = FileConstant.NewDocPath
                       + (_coreBaseSettings.CustomMode ? "ru-RU/" : "en-US/")
                       + fileName;

            var storeTemplate = await _globalStore.GetStoreTemplateAsync();
            if (!await storeTemplate.IsFileAsync("", path))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_FileNotFound);
                return;
            }

            context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(fileName));
            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);

            await using var stream = await storeTemplate.GetReadStreamAsync("", path);
            context.Response.Headers.Add("Content-Length",
                stream.CanSeek
                ? stream.Length.ToString(CultureInfo.InvariantCulture)
                : (await storeTemplate.GetFileSizeAsync("", path)).ToString(CultureInfo.InvariantCulture));
            await stream.CopyToAsync(context.Response.Body);
        }
        catch (Exception ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
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
            _logger.ErrorEmptyFile(he);
        }
    }

    private async Task TempFile(HttpContext context)
    {
        var fileName = context.Request.Query[FilesLinkUtility.FileTitle];
        var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();

        var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(fileName, auth ?? "", _global.StreamUrlExpire);
        if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
        {
            var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

            _logger.Error(FilesLinkUtility.AuthKey, validateResult, context.Request.Url(), exc);

            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
            return;
        }

        context.Response.Clear();
        context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);
        context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(fileName));

        var store = await _globalStore.GetStoreAsync();

        var path = CrossPlatform.PathCombine("temp_stream", fileName);

        if (!await store.IsFileAsync(FileConstant.StorageDomainTmp, path))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_FileNotFound);
            return;
        }

        await using (var readStream = await store.GetReadStreamAsync(FileConstant.StorageDomainTmp, path))
        {
            context.Response.Headers.Add("Content-Length", readStream.Length.ToString(CultureInfo.InvariantCulture));
            await readStream.CopyToAsync(context.Response.Body);
        }

        await store.DeleteAsync(FileConstant.StorageDomainTmp, path);

        try
        {
            await context.Response.Body.FlushAsync();
            //context.Response.SuppressContent = true;
            //context.ApplicationInstance.CompleteRequest();
        }
        catch (HttpException he)
        {
            _logger.ErrorTempFile(he);
        }
    }

    private async Task DifferenceFile(HttpContext context)
    {
        var q = context.Request.Query[FilesLinkUtility.FileId];

        if (int.TryParse(q, out var id))
        {
            await DifferenceFileAsync(context, id);
        }
        else
        {
            await DifferenceFileAsync(context, q.FirstOrDefault() ?? "");
        }
    }

    private async Task DifferenceFileAsync<T>(HttpContext context, T id)
    {
        try
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            int.TryParse(context.Request.Query[FilesLinkUtility.Version].FirstOrDefault() ?? "", out var version);
            var doc = context.Request.Query[FilesLinkUtility.DocShareKey];

            var (linkRight, file) = await _fileShareLink.CheckAsync(doc, fileDao);
            if (linkRight == FileShare.Restrict && !_securityContext.IsAuthenticated)
            {
                var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();
                var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(id.ToString() + version, auth ?? "", _global.StreamUrlExpire);
                if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);

                    _logger.Error(FilesLinkUtility.AuthKey, validateResult, context.Request.Url(), exc);

                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(FilesCommonResource.ErrorMassage_SecurityException);
                    return;
                }
            }

            await fileDao.InvalidateCacheAsync(id);

            if (file == null
                || version > 0 && file.Version != version)
            {
                file = version > 0
                           ? await fileDao.GetFileAsync(id, version)
                           : await fileDao.GetFileAsync(id);
            }

            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (linkRight == FileShare.Restrict && _securityContext.IsAuthenticated && !await _fileSecurity.CanReadAsync(file))
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

            await using var stream = await fileDao.GetDifferenceStreamAsync(file);
            context.Response.Headers.Add("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
            await stream.CopyToAsync(context.Response.Body);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(ex.Message);
            _logger.ErrorForUrl(context.Request.Url(), ex);
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
            _logger.ErrorDifferenceFile(he);
        }
    }

    private async Task ThumbnailFile(HttpContext context, bool force)
    {
        var q = context.Request.Query[FilesLinkUtility.FileId];

        if (int.TryParse(q, out var id))
        {
            await ThumbnailFile(context, id, force);
        }
        else
        {
            await ThumbnailFileFromThirdparty(context, q.FirstOrDefault() ?? "", force);
        }
    }

    private async Task ThumbnailFile(HttpContext context, int id, bool force)
    {
        try
        {
            var defaultSize = _thumbnailSettings.Sizes.FirstOrDefault();

            if (defaultSize == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var width = defaultSize.Width;
            var height = defaultSize.Height;

            var size = context.Request.Query["size"].ToString() ?? "";
            var sizes = size.Split('x');
            if (sizes.Length == 2)
            {
                _ = int.TryParse(sizes[0], out width);
                _ = int.TryParse(sizes[1], out height);
            }

            var fileDao = _daoFactory.GetFileDao<int>();
            var file = int.TryParse(context.Request.Query[FilesLinkUtility.Version], out var version) && version > 0
               ? await fileDao.GetFileAsync(id, version)
               : await fileDao.GetFileAsync(id);

            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!await _fileSecurity.CanReadAsync(file))
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

            if (file.ThumbnailStatus != Thumbnail.Created && !force)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            if (force)
            {
                context.Response.ContentType = MimeMapping.GetMimeMapping(".jpeg");
                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue(".jpeg", true));

                await using (var stream = await fileDao.GetFileStreamAsync(file))
                {
                    var processedImage = await Image.LoadAsync(stream);

                    processedImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = ResizeMode.Crop
                    }));

                    // save as jpeg more fast, then webp
                    await processedImage.SaveAsJpegAsync(context.Response.Body);
                }
            }
            else
            {
                context.Response.ContentType = MimeMapping.GetMimeMapping("." + _global.ThumbnailExtension);
                context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue("." + _global.ThumbnailExtension));

                var thumbnailFilePath = fileDao.GetUniqThumbnailPath(file, width, height);

                await using (var stream = await (await _globalStore.GetStoreAsync()).GetReadStreamAsync(thumbnailFilePath))
                {
                    context.Response.Headers.Add("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                    await stream.CopyToAsync(context.Response.Body);
                }
            }

        }
        catch (FileNotFoundException ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync(ex.Message);
            return;
        }
        catch (Exception ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
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
            _logger.ErrorThumbnail(he);
        }
    }

    private async Task ThumbnailFileFromThirdparty(HttpContext context, string id, bool _)
    {
        try
        {
            var defaultSize = _thumbnailSettings.Sizes.FirstOrDefault();

            if (defaultSize == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var width = defaultSize.Width;
            var height = defaultSize.Height;

            var size = context.Request.Query["size"].ToString() ?? "";
            var sizes = size.Split('x');
            if (sizes.Length == 2)
            {
                _ = int.TryParse(sizes[0], out width);
                _ = int.TryParse(sizes[1], out height);
            }

            context.Response.Headers.Add("Content-Disposition", ContentDispositionUtil.GetHeaderValue("." + _global.ThumbnailExtension));
            context.Response.ContentType = MimeMapping.GetMimeMapping("." + _global.ThumbnailExtension);

            var fileDao = _daoFactory.GetFileDao<string>();

            await using (var stream = await fileDao.GetThumbnailAsync(id, width, height))
            {
                await stream.CopyToAsync(context.Response.Body);
            }
        }
        catch (FileNotFoundException ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync(ex.Message);
            return;
        }
        catch (Exception ex)
        {
            _logger.ErrorForUrl(context.Request.Url(), ex);
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
            _logger.ErrorThumbnail(he);
        }
    }

    private static string GetEtag<T>(File<T> file)
    {
        return file.Id + ":" + file.Version + ":" + file.Title.GetHashCode() + ":" + file.ContentLength;
    }

    private async ValueTask CreateFile(HttpContext context)
    {
        if (!_securityContext.IsAuthenticated)
        {
            //var refererURL = context.Request.GetUrlRewriter().AbsoluteUri;

            //context.Session["refererURL"] = refererURL;
            const string authUrl = "~/Auth.aspx";
            context.Response.Redirect(authUrl, true);
            return;
        }

        var folderId = context.Request.Query[FilesLinkUtility.FolderId].FirstOrDefault();
        if (string.IsNullOrEmpty(folderId))
        {
            await CreateFile(context, await _globalFolderHelper.FolderMyAsync);
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

        var folderDao = _daoFactory.GetFolderDao<T>();
        folder = await folderDao.GetFolderAsync(folderId);

        if (folder == null)
        {
            throw new HttpException((int)HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        var canCreate = await _fileSecurity.CanCreateAsync(folder);
        if (!canCreate)
        {
            throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException_Create);
        }

        File<T> file;
        var fileUri = context.Request.Query[FilesLinkUtility.FileUri];
        var fileTitle = context.Request.Query[FilesLinkUtility.FileTitle];
        try
        {
            if (!string.IsNullOrEmpty(fileUri))
            {
                file = await CreateFileFromUriAsync(folder, fileUri, fileTitle);
            }
            else
            {
                var docType = context.Request.Query["doctype"];
                file = await CreateFileFromTemplateAsync(folder, fileTitle, docType);
            }

            await _socketManager.CreateFileAsync(file);
        }
        catch (Exception ex)
        {
            await WriteError(context, ex, responseMessage);
            return;
        }

        await _fileMarker.MarkAsNewAsync(file);

        if (responseMessage)
        {
            await WriteOk(context, folder, file);
            return;
        }

        context.Response.Redirect(
            (context.Request.Query["openfolder"].FirstOrDefault() ?? "").Equals("true")
                    ? await _pathProvider.GetFolderUrlByIdAsync(file.ParentId)
                    : (_filesLinkUtility.GetFileWebEditorUrl(file.Id) + "#message/" + HttpUtility.UrlEncode(string.Format(FilesCommonResource.MessageFileCreated, folder.Title))));
    }

    private async Task WriteError(HttpContext context, Exception ex, bool responseMessage)
    {
        _logger.ErrorFileHandler(ex);

        if (responseMessage)
        {
            await context.Response.WriteAsync("error: " + ex.Message);
            return;
        }
        context.Response.Redirect(PathProvider.StartURL + "#error/" + HttpUtility.UrlEncode(ex.Message), true);
        return;
    }

    private async Task WriteOk<T>(HttpContext context, Folder<T> folder, File<T> file)
    {
        var message = string.Format(FilesCommonResource.MessageFileCreated, folder.Title);
        if (_fileUtility.CanWebRestrictedEditing(file.Title))
        {
            message = string.Format(FilesCommonResource.MessageFileCreatedForm, folder.Title);
        }

        await context.Response.WriteAsync("ok: " + message);
    }

    private async Task<File<T>> CreateFileFromTemplateAsync<T>(Folder<T> folder, string fileTitle, string docType)
    {
        var storeTemplate = await _globalStore.GetStoreTemplateAsync();

        var lang = (await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID)).GetCulture();

        var fileExt = _fileUtility.InternalExtension[FileType.Document];
        if (!string.IsNullOrEmpty(docType))
        {
            var tmpFileType = Configuration<T>.DocType.FirstOrDefault(r => r.Value.Equals(docType, StringComparison.OrdinalIgnoreCase));
            _fileUtility.InternalExtension.TryGetValue(tmpFileType.Key, out var tmpFileExt);
            if (!string.IsNullOrEmpty(tmpFileExt))
            {
                fileExt = tmpFileExt;
            }
        }

        var templateName = "new" + fileExt;

        var templatePath = FileConstant.NewDocPath + lang + "/";
        if (!await storeTemplate.IsDirectoryAsync(templatePath))
        {
            templatePath = FileConstant.NewDocPath + "en-US/";
        }

        templatePath += templateName;

        if (string.IsNullOrEmpty(fileTitle))
        {
            fileTitle = templateName;
        }
        else
        {
            fileTitle += fileExt;
        }

        var file = _serviceProvider.GetService<File<T>>();
        file.Title = fileTitle;
        file.ParentId = folder.Id;
        file.Comment = FilesCommonResource.CommentCreate;

        var fileDao = _daoFactory.GetFileDao<T>();
        var stream = await storeTemplate.GetReadStreamAsync("", templatePath, 0);
        file.ContentLength = stream.CanSeek ? stream.Length : await storeTemplate.GetFileSizeAsync(templatePath);
        return await fileDao.SaveFileAsync(file, stream);
    }

    private async Task<File<T>> CreateFileFromUriAsync<T>(Folder<T> folder, string fileUri, string fileTitle)
    {
        if (string.IsNullOrEmpty(fileTitle))
        {
            fileTitle = Path.GetFileName(HttpUtility.UrlDecode(fileUri));
        }

        var file = _serviceProvider.GetService<File<T>>();
        file.Title = fileTitle;
        file.ParentId = folder.Id;
        file.Comment = FilesCommonResource.CommentCreate;

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(fileUri)
        };

        var fileDao = _daoFactory.GetFileDao<T>();
        var httpClient = _clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
        await using var fileStream = await response.Content.ReadAsStreamAsync();

        if (fileStream.CanSeek)
        {
            file.ContentLength = fileStream.Length;
            return await fileDao.SaveFileAsync(file, fileStream);
        }
        else
        {
            await using var buffered = _tempStream.GetBuffered(fileStream);
            file.ContentLength = buffered.Length;
            return await fileDao.SaveFileAsync(file, buffered);
        }


    }

    private async Task RedirectAsync(HttpContext context)
    {
        var q = context.Request.Query[FilesLinkUtility.FileId];
        var q1 = context.Request.Query[FilesLinkUtility.FolderId];

        if (int.TryParse(q, out var fileId) && int.TryParse(q1, out var folderId))
        {
            await RedirectAsync(context, fileId, folderId);
        }
        else
        {
            await RedirectAsync(context, q.FirstOrDefault() ?? "", q1.FirstOrDefault() ?? "");
        }
    }

    private async Task RedirectAsync<T>(HttpContext context, T folderId, T fileId)
    {
        if (!_securityContext.IsAuthenticated)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }
        var urlRedirect = string.Empty;
        if (folderId != null)
        {
            try
            {
                urlRedirect = await _pathProvider.GetFolderUrlByIdAsync(folderId);
            }
            catch (ArgumentNullException e)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
            }
        }

        if (fileId != null)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            var file = await fileDao.GetFileAsync(fileId);
            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            urlRedirect = _filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.Id);
        }

        if (string.IsNullOrEmpty(urlRedirect))
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);
        }

        context.Response.Redirect(urlRedirect);
    }

    private async Task TrackFile(HttpContext context)
    {
        var q = context.Request.Query[FilesLinkUtility.FileId];

        if (int.TryParse(q, out var id))
        {
            await TrackFileAsync(context, id);
        }
        else
        {
            await TrackFileAsync(context, q.FirstOrDefault() ?? "");
        }
    }

    private async ValueTask TrackFileAsync<T>(HttpContext context, T fileId)
    {
        var auth = context.Request.Query[FilesLinkUtility.AuthKey].FirstOrDefault();
        _logger.DebugDocServiceTrackFileid(fileId.ToString());

        var callbackSpan = TimeSpan.FromDays(128);
        var validateResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(fileId.ToString(), auth ?? "", callbackSpan);
        if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
        {
            _logger.ErrorDocServiceTrackAuth(validateResult, FilesLinkUtility.AuthKey, auth);
            throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
        }

        TrackerData fileData;
        try
        {
            string body;
            var receiveStream = context.Request.Body;
            using var readStream = new StreamReader(receiveStream);
            body = await readStream.ReadToEndAsync();

            _logger.DebugDocServiceTrackBody(body);
            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentException("DocService request body is incorrect");
            }

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            fileData = JsonSerializer.Deserialize<TrackerData>(body, options);
        }
        catch (JsonException e)
        {
            _logger.ErrorDocServiceTrackReadBody(e);
            throw new HttpException((int)HttpStatusCode.BadRequest, "DocService request is incorrect");
        }
        catch (Exception e)
        {
            _logger.ErrorDocServiceTrackReadBody(e);
            throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
        }

        if (!string.IsNullOrEmpty(_fileUtility.SignatureSecret))
        {
            if (!string.IsNullOrEmpty(fileData.Token))
            {
                try
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var dataString = JwtBuilder.Create()
                            .WithAlgorithm(new HMACSHA256Algorithm())
                            .WithJsonSerializer(new JwtSerializer())
                            .WithSecret(_fileUtility.SignatureSecret)
                            .MustVerifySignature()
                            .Decode(fileData.Token);
#pragma warning restore CS0618 // Type or member is obsolete

                    var data = JObject.Parse(dataString);
                    if (data == null)
                    {
                        throw new ArgumentException("DocService request token is incorrect");
                    }
                    fileData = data.ToObject<TrackerData>();
                }
                catch (SignatureVerificationException ex)
                {
                    _logger.ErrorDocServiceTrackHeader(ex);
                    throw new HttpException((int)HttpStatusCode.Forbidden, ex.Message);
                }
            }
            else
            {
                //todo: remove old scheme
                var header = context.Request.Headers[_fileUtility.SignatureHeader].FirstOrDefault();
                if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer "))
                {
                    _logger.ErrorDocServiceTrackHeaderIsNull();
                    throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
                }
                header = header.Substring("Bearer ".Length);

                try
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var stringPayload = JwtBuilder.Create()
                            .WithAlgorithm(new HMACSHA256Algorithm())
                            .WithJsonSerializer(new JwtSerializer())
                            .WithSecret(_fileUtility.SignatureSecret)
                            .MustVerifySignature()
                            .Decode(header);
#pragma warning restore CS0618 // Type or member is obsolete

                    _logger.DebugDocServiceTrackPayload(stringPayload);
                    var jsonPayload = JObject.Parse(stringPayload);
                    var data = jsonPayload["payload"];
                    if (data == null)
                    {
                        throw new ArgumentException("DocService request header is incorrect");
                    }
                    fileData = data.ToObject<TrackerData>();
                }
                catch (SignatureVerificationException ex)
                {
                    _logger.ErrorDocServiceTrackHeader(ex);
                    throw new HttpException((int)HttpStatusCode.Forbidden, ex.Message);
                }
            }
        }

        TrackResponse result;
        try
        {
            result = await _documentServiceTrackerHelper.ProcessDataAsync(fileId, fileData);
        }
        catch (Exception e)
        {
            _logger.ErrorDocServiceTrack(e);
            throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
        }
        result ??= new TrackResponse();

        await context.Response.WriteAsync(TrackResponse.Serialize(result));
    }
}

public static class FileHandlerExtensions
{
    public static IApplicationBuilder UseFileHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FileHandler>();
    }
}