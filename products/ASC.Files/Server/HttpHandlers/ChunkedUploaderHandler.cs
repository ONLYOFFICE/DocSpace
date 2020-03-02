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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Resources;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Core.Files;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using File = ASC.Files.Core.File;

namespace ASC.Web.Files.HttpHandlers
{
    public class ChunkedUploaderHandler //: AbstractHttpAsyncHandler
    {
        public RequestDelegate Next { get; }
        public TenantManager TenantManager { get; }
        public FileUploader FileUploader { get; }
        public FilesMessageService FilesMessageService { get; }
        public AuthManager AuthManager { get; }
        public SecurityContext SecurityContext { get; }
        public SetupInfo SetupInfo { get; }
        public EntryManager EntryManager { get; }
        public InstanceCrypto InstanceCrypto { get; }
        public ChunkedUploadSessionHolder ChunkedUploadSessionHolder { get; }
        public ChunkedUploadSessionHelper ChunkedUploadSessionHelper { get; }
        public ILog Logger { get; }

        public ChunkedUploaderHandler(
            RequestDelegate next,
            IOptionsMonitor<ILog> optionsMonitor,
            TenantManager tenantManager,
            FileUploader fileUploader,
            FilesMessageService filesMessageService,
            AuthManager authManager,
            SecurityContext securityContext,
            SetupInfo setupInfo,
            EntryManager entryManager,
            InstanceCrypto instanceCrypto,
            ChunkedUploadSessionHolder chunkedUploadSessionHolder,
            ChunkedUploadSessionHelper chunkedUploadSessionHelper)
        {
            Next = next;
            TenantManager = tenantManager;
            FileUploader = fileUploader;
            FilesMessageService = filesMessageService;
            AuthManager = authManager;
            SecurityContext = securityContext;
            SetupInfo = setupInfo;
            EntryManager = entryManager;
            InstanceCrypto = instanceCrypto;
            ChunkedUploadSessionHolder = chunkedUploadSessionHolder;
            ChunkedUploadSessionHelper = chunkedUploadSessionHelper;
            Logger = optionsMonitor.CurrentValue;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var request = new ChunkedRequestHelper(context.Request);

                if (!TryAuthorize(request))
                {
                    WriteError(context, "Can't authorize given initiate session request or session with specified upload id already expired");
                    return;
                }

                if (TenantManager.GetCurrentTenant().Status != TenantStatus.Active)
                {
                    WriteError(context, "Can't perform upload for deleted or transfering portals");
                    return;
                }

                switch (request.Type(InstanceCrypto))
                {
                    case ChunkedRequestType.Abort:
                        FileUploader.AbortUpload<string>(request.UploadId);
                        WriteSuccess(context, null);
                        return;

                    case ChunkedRequestType.Initiate:
                        var createdSession = FileUploader.InitiateUpload(request.FolderId, request.FileId, request.FileName, request.FileSize, request.Encrypted);
                        WriteSuccess(context, ChunkedUploadSessionHelper.ToResponseObject(createdSession, true));
                        return;

                    case ChunkedRequestType.Upload:
                        var resumedSession = FileUploader.UploadChunk<string>(request.UploadId, request.ChunkStream, request.ChunkSize);

                        if (resumedSession.BytesUploaded == resumedSession.BytesTotal)
                        {
                            WriteSuccess(context, ToResponseObject(resumedSession.File), (int)HttpStatusCode.Created);
                            FilesMessageService.Send(resumedSession.File, MessageAction.FileUploaded, resumedSession.File.Title);
                        }
                        else
                        {
                            WriteSuccess(context, ChunkedUploadSessionHelper.ToResponseObject(resumedSession));
                        }
                        return;

                    default:
                        WriteError(context, "Unknown request type.");
                        return;
                }
            }
            catch (FileNotFoundException error)
            {
                Logger.Error(error);
                WriteError(context, FilesCommonResource.ErrorMassage_FileNotFound);
            }
            catch (Exception error)
            {
                Logger.Error(error);
                WriteError(context, error.Message);
            }

            await Next.Invoke(context);
        }

        private bool TryAuthorize(ChunkedRequestHelper request)
        {
            if (request.Type(InstanceCrypto) == ChunkedRequestType.Initiate)
            {
                TenantManager.SetCurrentTenant(request.TenantId);
                SecurityContext.AuthenticateMe(AuthManager.GetAccountByID(TenantManager.GetCurrentTenant().TenantId, request.AuthKey(InstanceCrypto)));
                var cultureInfo = request.CultureInfo(SetupInfo);
                if (cultureInfo != null)
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                return true;
            }

            if (!string.IsNullOrEmpty(request.UploadId))
            {
                var uploadSession = ChunkedUploadSessionHolder.GetSession<string>(request.UploadId);
                if (uploadSession != null)
                {
                    TenantManager.SetCurrentTenant(uploadSession.TenantId);
                    SecurityContext.AuthenticateMe(AuthManager.GetAccountByID(TenantManager.GetCurrentTenant().TenantId, uploadSession.UserId));
                    var culture = SetupInfo.EnabledCulturesPersonal.Find(c => string.Equals(c.Name, uploadSession.CultureName, StringComparison.InvariantCultureIgnoreCase));
                    if (culture != null)
                        Thread.CurrentThread.CurrentUICulture = culture;
                    return true;
                }
            }

            return false;
        }

        private static void WriteError(HttpContext context, string message)
        {
            WriteResponse(context, false, null, message, (int)HttpStatusCode.OK);
        }

        private static void WriteSuccess(HttpContext context, object data, int statusCode = (int)HttpStatusCode.OK)
        {
            WriteResponse(context, true, data, string.Empty, statusCode);
        }

        private static void WriteResponse(HttpContext context, bool success, object data, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.WriteAsync(JsonConvert.SerializeObject(new { success, data, message })).Wait();
            context.Response.ContentType = "application/json";
        }

        private static object ToResponseObject(File file)
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

        private enum ChunkedRequestType
        {
            None,
            Initiate,
            Abort,
            Upload
        }

        [DebuggerDisplay("{Type} ({UploadId})")]
        private class ChunkedRequestHelper
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
                    return ChunkedRequestType.Initiate;

                if (_request.Query["abort"] == "true" && !string.IsNullOrEmpty(UploadId))
                    return ChunkedRequestType.Abort;

                return !string.IsNullOrEmpty(UploadId)
                            ? ChunkedRequestType.Upload
                            : ChunkedRequestType.None;
            }

            public string UploadId
            {
                get { return _request.Query["uid"]; }
            }

            public int TenantId
            {
                get
                {
                    if (!_tenantId.HasValue)
                    {
                        if (int.TryParse(_request.Query["tid"], out var v))
                            _tenantId = v;
                        else
                            _tenantId = -1;
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

            public string FolderId
            {
                get { return _request.Query[FilesLinkUtility.FolderId]; }
            }

            public string FileId
            {
                get { return _request.Query[FilesLinkUtility.FileId]; }
            }

            public string FileName
            {
                get { return _request.Query[FilesLinkUtility.FileTitle]; }
            }

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

            public long ChunkSize
            {
                get { return File.Length; }
            }

            public Stream ChunkStream
            {
                get { return File.OpenReadStream(); }
            }

            public CultureInfo CultureInfo(SetupInfo setupInfo)
            {
                if (_cultureInfo != null)
                    return _cultureInfo;

                var culture = _request.Query["culture"];
                if (string.IsNullOrEmpty(culture)) culture = "en-US";

                return _cultureInfo = setupInfo.EnabledCulturesPersonal.Find(c => string.Equals(c.Name, culture, StringComparison.InvariantCultureIgnoreCase));
            }

            public bool Encrypted
            {
                get { return _request.Query["encrypted"] == "true"; }
            }

            private IFormFile File
            {
                get
                {
                    if (_file != null)
                        return _file;

                    if (_request.Form.Files.Count > 0)
                        return _file = _request.Form.Files[0];

                    throw new Exception("HttpRequest.Files is empty");
                }
            }

            public ChunkedRequestHelper(HttpRequest request)
            {
                _request = request ?? throw new ArgumentNullException("request");
            }

            private bool IsAuthDataSet(InstanceCrypto instanceCrypto)
            {
                return TenantId > -1 && AuthKey(instanceCrypto) != Guid.Empty;
            }

            private bool IsFileDataSet()
            {
                return !string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(FolderId);
            }
        }
    }

    public static class ChunkedUploaderHandlerExtention
    {
        public static DIHelper AddChunkedUploaderHandlerService(this DIHelper services)
        {
            services.TryAddScoped<ChunkedUploaderHandler>();
            return services
                .AddTenantManagerService()
                .AddFileUploaderService()
                .AddFilesMessageService()
                .AddAuthManager()
                .AddSecurityContextService()
                .AddSetupInfo()
                .AddEntryManagerService()
                .AddInstanceCryptoService()
                .AddChunkedUploadSessionHolderService()
                .AddChunkedUploadSessionHelperService();
        }
    }
}