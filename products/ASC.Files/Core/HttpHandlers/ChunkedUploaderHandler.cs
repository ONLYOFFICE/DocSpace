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

namespace ASC.Web.Files.HttpHandlers
{
    public class ChunkedUploaderHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ChunkedUploaderHandler(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var chunkedUploaderHandlerService = scope.ServiceProvider.GetService<ChunkedUploaderHandlerService>();
            await chunkedUploaderHandlerService.Invoke(context).ConfigureAwait(false);
        }
    }

    [Scope]
    public class ChunkedUploaderHandlerService
    {
        private readonly TenantManager _tenantManager;
        private readonly FileUploader _fileUploader;
        private readonly FilesMessageService _filesMessageService;
        private readonly AuthManager _authManager;
        private readonly SecurityContext _securityContext;
        private readonly SetupInfo _setupInfo;
        private readonly InstanceCrypto _instanceCrypto;
        private readonly ChunkedUploadSessionHolder _chunkedUploadSessionHolder;
        private readonly ChunkedUploadSessionHelper _chunkedUploadSessionHelper;
        private readonly SocketManager _socketManager;
        private readonly ILog _logger;

        public ChunkedUploaderHandlerService(
            IOptionsMonitor<ILog> optionsMonitor,
            TenantManager tenantManager,
            FileUploader fileUploader,
            FilesMessageService filesMessageService,
            AuthManager authManager,
            SecurityContext securityContext,
            SetupInfo setupInfo,
            InstanceCrypto instanceCrypto,
            ChunkedUploadSessionHolder chunkedUploadSessionHolder,
            ChunkedUploadSessionHelper chunkedUploadSessionHelper,
            SocketManager socketManager)
        {
            _tenantManager = tenantManager;
            _fileUploader = fileUploader;
            _filesMessageService = filesMessageService;
            _authManager = authManager;
            _securityContext = securityContext;
            _setupInfo = setupInfo;
            _instanceCrypto = instanceCrypto;
            _chunkedUploadSessionHolder = chunkedUploadSessionHolder;
            _chunkedUploadSessionHelper = chunkedUploadSessionHelper;
            _socketManager = socketManager;
            _logger = optionsMonitor.CurrentValue;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var uploadSession = await _chunkedUploadSessionHolder.GetSessionAsync<int>(context.Request.Query["uid"]);
                if (uploadSession != null)
                {
                    await Invoke<int>(context);
                }
            }
            catch (Exception)
            {
                await Invoke<string>(context);
            }
        }

        public async Task Invoke<T>(HttpContext context)
        {
            try
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;

                    return;
                }

                var request = new ChunkedRequestHelper<T>(context.Request);

                if (!await TryAuthorizeAsync(request))
                {
                    await WriteError(context, "Can't authorize given initiate session request or session with specified upload id already expired");

                    return;
                }

                if (_tenantManager.GetCurrentTenant().Status != TenantStatus.Active)
                {
                    await WriteError(context, "Can't perform upload for deleted or transfering portals");

                    return;
                }

                switch (request.Type(_instanceCrypto))
                {
                    case ChunkedRequestType.Abort:
                        await _fileUploader.AbortUploadAsync<T>(request.UploadId);
                        await WriteSuccess(context, null);

                        return;

                    case ChunkedRequestType.Initiate:
                        var createdSession = await _fileUploader.InitiateUploadAsync(request.FolderId, request.FileId, request.FileName, request.FileSize, request.Encrypted);
                        await WriteSuccess(context, await _chunkedUploadSessionHelper.ToResponseObjectAsync(createdSession, true));

                        return;

                    case ChunkedRequestType.Upload:
                        var resumedSession = await _fileUploader.UploadChunkAsync<T>(request.UploadId, request.ChunkStream, request.ChunkSize);

                        if (resumedSession.BytesUploaded == resumedSession.BytesTotal)
                        {
                            await WriteSuccess(context, ToResponseObject(resumedSession.File), (int)HttpStatusCode.Created);
                            _filesMessageService.Send(resumedSession.File, MessageAction.FileUploaded, resumedSession.File.Title);

                            await _socketManager.CreateFileAsync(resumedSession.File);
                        }
                        else
                        {
                            await WriteSuccess(context, await _chunkedUploadSessionHelper.ToResponseObjectAsync(resumedSession));
                        }

                        return;

                    default:
                        await WriteError(context, "Unknown request type.");
                        return;
                }
            }
            catch (FileNotFoundException error)
            {
                _logger.Error(error);
                await WriteError(context, FilesCommonResource.ErrorMassage_FileNotFound);
            }
            catch (Exception error)
            {
                _logger.Error(error);
                await WriteError(context, error.Message);
            }
        }

        private async Task<bool> TryAuthorizeAsync<T>(ChunkedRequestHelper<T> request)
        {
            if (request.Type(_instanceCrypto) == ChunkedRequestType.Initiate)
            {
                _tenantManager.SetCurrentTenant(request.TenantId);
                _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(_tenantManager.GetCurrentTenant().TenantId, request.AuthKey(_instanceCrypto)));
                var cultureInfo = request.CultureInfo(_setupInfo);
                if (cultureInfo != null)
                {
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                }

                return true;
            }

            if (!string.IsNullOrEmpty(request.UploadId))
            {
                var uploadSession = await _chunkedUploadSessionHolder.GetSessionAsync<T>(request.UploadId);
                if (uploadSession != null)
                {
                    _tenantManager.SetCurrentTenant(uploadSession.TenantId);
                    _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(_tenantManager.GetCurrentTenant().TenantId, uploadSession.UserId));
                    var culture = _setupInfo.GetPersonalCulture(uploadSession.CultureName).Value;
                    if (culture != null)
                    {
                        Thread.CurrentThread.CurrentUICulture = culture;
                    }

                    return true;
                }
            }

            return false;
        }

        private static Task WriteError(HttpContext context, string message)
        {
            return WriteResponse(context, false, null, message, (int)HttpStatusCode.OK);
        }

        private static Task WriteSuccess(HttpContext context, object data, int statusCode = (int)HttpStatusCode.OK)
        {
            return WriteResponse(context, true, data, string.Empty, statusCode);
        }

        private static Task WriteResponse(HttpContext context, bool success, object data, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new { success, data, message }));
        }

        private static object ToResponseObject<T>(File<T> file)
        {
            return new
            {
                id = file.ID,
                folderId = file.FolderID,
                version = file.Version,
                title = file.Title,
                provider_key = file.ProviderKey,
                uploaded = true
            };
        }
    }

    public enum ChunkedRequestType
    {
        None,
        Initiate,
        Abort,
        Upload
    }

    [DebuggerDisplay("{Type} ({UploadId})")]
    public class ChunkedRequestHelper<T>
    {
        private readonly HttpRequest _request;
        private IFormFile _file;
        private int? _tenantId;
        private long? _fileContentLength;
        private Guid? _authKey;
        private CultureInfo _cultureInfo;

        public ChunkedRequestType Type(InstanceCrypto instanceCrypto)
        {
            if (_request.Query["initiate"] == "true" && IsAuthDataSet(instanceCrypto) && IsFileDataSet())
            {
                return ChunkedRequestType.Initiate;
            }

            if (_request.Query["abort"] == "true" && !string.IsNullOrEmpty(UploadId))
            {
                return ChunkedRequestType.Abort;
            }

            return !string.IsNullOrEmpty(UploadId)
                        ? ChunkedRequestType.Upload
                        : ChunkedRequestType.None;
        }

        public string UploadId => _request.Query["uid"];

        public int TenantId
        {
            get
            {
                if (!_tenantId.HasValue)
                {
                    if (int.TryParse(_request.Query["tid"], out var v))
                    {
                        _tenantId = v;
                    }
                    else
                    {
                        _tenantId = -1;
                    }
                }

                return _tenantId.Value;
            }
        }

        public Guid AuthKey(InstanceCrypto instanceCrypto)
        {
            if (!_authKey.HasValue)
            {
                _authKey = !string.IsNullOrEmpty(_request.Query["userid"])
                                ? new Guid(instanceCrypto.Decrypt(_request.Query["userid"]))
                                : Guid.Empty;
            }

            return _authKey.Value;
        }

        public T FolderId => (T)Convert.ChangeType(_request.Query[FilesLinkUtility.FolderId], typeof(T));

        public T FileId => (T)Convert.ChangeType(_request.Query[FilesLinkUtility.FileId], typeof(T));

        public string FileName => _request.Query[FilesLinkUtility.FileTitle];

        public long FileSize
        {
            get
            {
                if (!_fileContentLength.HasValue)
                {
                    long.TryParse(_request.Query["fileSize"], out var v);
                    _fileContentLength = v;
                }

                return _fileContentLength.Value;
            }
        }

        public long ChunkSize => File.Length;

        public Stream ChunkStream => File.OpenReadStream();

        public CultureInfo CultureInfo(SetupInfo setupInfo)
        {
            if (_cultureInfo != null)
            {
                return _cultureInfo;
            }

            var culture = _request.Query["culture"];
            if (string.IsNullOrEmpty(culture))
            {
                culture = "en-US";
            }

            return _cultureInfo = setupInfo.GetPersonalCulture(culture).Value;
        }

        public bool Encrypted => _request.Query["encrypted"] == "true";

        private IFormFile File
        {
            get
            {
                if (_file != null)
                {
                    return _file;
                }

                if (_request.Form.Files.Count > 0)
                {
                    return _file = _request.Form.Files[0];
                }

                throw new Exception("HttpRequest.Files is empty");
            }
        }

        public ChunkedRequestHelper(HttpRequest request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        private bool IsAuthDataSet(InstanceCrypto instanceCrypto)
        {
            return TenantId > -1 && AuthKey(instanceCrypto) != Guid.Empty;
        }

        private bool IsFileDataSet()
        {
            return !string.IsNullOrEmpty(FileName) && !EqualityComparer<T>.Default.Equals(FolderId, default(T));
        }
    }

    public static class ChunkedUploaderHandlerExtention
    {
        public static IApplicationBuilder UseChunkedUploaderHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ChunkedUploaderHandler>();
        }
    }
}