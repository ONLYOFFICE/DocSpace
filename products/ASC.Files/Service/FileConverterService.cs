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

[Singletone(Additional = typeof(FileConverterQueueExtension))]
internal class FileConverterService<T> : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _timerDelay = 1000;
    private readonly ILogger<FileConverterService<T>> _logger;

    public FileConverterService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<FileConverterService<T>> logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.DebugFileConverterServiceRuning();

        stoppingToken.Register(() => _logger.DebugFileConverterServiceStopping());

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var serviceScope = _serviceScopeFactory.CreateAsyncScope();

            var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<FileConverterService<T>>>();

            if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<FileConverterService<T>>.InstanceId))
            {
                await Task.Delay(1000, stoppingToken);

                continue;
            }

            await ExecuteCheckFileConverterStatusAsync(serviceScope);

            await Task.Delay(_timerDelay, stoppingToken);
        }
    }

    private async Task ExecuteCheckFileConverterStatusAsync(IServiceScope scope)
    {
        TenantManager tenantManager;
        UserManager userManager;
        SecurityContext securityContext;
        IDaoFactory daoFactory;
        FileSecurity fileSecurity;
        PathProvider pathProvider;
        SetupInfo setupInfo;
        FileUtility fileUtility;
        DocumentServiceHelper documentServiceHelper;
        DocumentServiceConnector documentServiceConnector;
        EntryStatusManager entryManager;
        FileConverter fileConverter;
        FileConverterQueue fileConverterQueue;

        var logger = scope.ServiceProvider.GetService<ILogger<FileConverterQueue>>();

        try
        {
            fileConverterQueue = scope.ServiceProvider.GetService<FileConverterQueue>();

            var _conversionQueue = fileConverterQueue.GetAllTask<T>().ToList();

            if (_conversionQueue.Count > 0)
            {
                logger.DebugRunCheckConvertFilesStatus(_conversionQueue.Count);
            }

            var filesIsConverting = _conversionQueue
                                    .Where(x => string.IsNullOrEmpty(x.Processed))
                                    .ToList();

            foreach (var converter in filesIsConverting)
            {
                converter.Processed = "1";

                var fileId = JsonDocument.Parse(converter.Source).RootElement.GetProperty("id").Deserialize<T>();
                var fileVersion = JsonDocument.Parse(converter.Source).RootElement.GetProperty("version").Deserialize<int>();

                int operationResultProgress;
                var password = converter.Password;

                var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();
                commonLinkUtility.ServerUri = converter.ServerRootPath;

                var scopeClass = scope.ServiceProvider.GetService<FileConverterQueueScope>();
                (_, tenantManager, userManager, securityContext, daoFactory, fileSecurity, pathProvider, setupInfo, fileUtility, documentServiceHelper, documentServiceConnector, entryManager, fileConverter) = scopeClass;

                await tenantManager.SetCurrentTenantAsync(converter.TenantId);

                await securityContext.AuthenticateMeWithoutCookieAsync(converter.Account);

                var file = await daoFactory.GetFileDao<T>().GetFileAsync(fileId, fileVersion);
                var fileUri = file.Id.ToString();

                string convertedFileUrl;
                string convertedFileType;

                try
                {
                    var externalShare = scope.ServiceProvider.GetRequiredService<ExternalShare>();

                    if (!string.IsNullOrEmpty(converter.ExternalShareData))
                    {
                        externalShare.SetCurrentShareData(JsonSerializer.Deserialize<ExternalShareData>(converter.ExternalShareData));
                    }

                    var user = await userManager.GetUsersAsync(converter.Account);

                    var culture = string.IsNullOrEmpty(user.CultureName) ? (await tenantManager.GetCurrentTenantAsync()).GetCulture() : CultureInfo.GetCultureInfo(user.CultureName);

                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;

                    if (!await fileSecurity.CanReadAsync(file) && file.RootFolderType != FolderType.BUNCH)
                    {
                        //No rights in CRM after upload before attach
                        throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
                    }

                    if (file.ContentLength > setupInfo.AvailableFileSize)
                    {
                        throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(setupInfo.AvailableFileSize)));
                    }

                    fileUri = await pathProvider.GetFileStreamUrlAsync(file);

                    var toExtension = fileUtility.GetInternalConvertExtension(file.Title);
                    var fileExtension = file.ConvertedExtension;
                    var docKey = await documentServiceHelper.GetDocKeyAsync(file);

                    fileUri = await documentServiceConnector.ReplaceCommunityAdressAsync(fileUri);
                    (operationResultProgress, convertedFileUrl, convertedFileType) = await documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, password, CultureInfo.CurrentUICulture.Name, null, null, true);
                }
                catch (Exception exception)
                {
                    var password1 = exception.InnerException is DocumentServiceException documentServiceException
                                          && documentServiceException.Code == DocumentServiceException.ErrorCode.ConvertPassword;

                    logger.ErrorConvertFileWithUrl(file.Id.ToString(), fileUri, exception);

                    var operationResult = converter;

                    if (operationResult.Delete)
                    {
                        _conversionQueue.Remove(operationResult);
                    }
                    else
                    {
                        operationResult.Progress = 100;
                        operationResult.StopDateTime = DateTime.UtcNow;
                        operationResult.Error = exception.Message;

                        if (password1)
                        {
                            operationResult.Result = "password";
                        }
                    }

                    continue;
                }

                operationResultProgress = Math.Min(operationResultProgress, 100);

                if (operationResultProgress < 100)
                {
                    var operationResult = converter;

                    if (DateTime.UtcNow - operationResult.StartDateTime > TimeSpan.FromMinutes(10))
                    {
                        operationResult.StopDateTime = DateTime.UtcNow;
                        operationResult.Error = FilesCommonResource.ErrorMassage_ConvertTimeout;

                        logger.ErrorCheckConvertFilesStatus(file.Id.ToString(), file.ContentLength);
                    }
                    else
                    {
                        operationResult.Processed = "";
                    }

                    operationResult.Progress = operationResultProgress;

                    logger.DebugCheckConvertFilesStatusIterationContinue();

                    continue;
                }

                File<T> newFile = null;

                var operationResultError = string.Empty;

                try
                {
                    newFile = await fileConverter.SaveConvertedFileAsync(file, convertedFileUrl, convertedFileType);
                }
                catch (Exception e)
                {
                    operationResultError = e.Message;

                    logger.ErrorOperation(operationResultError, convertedFileUrl, fileUri, convertedFileType, e);

                    continue;
                }
                finally
                {
                    var operationResult = converter;

                    if (operationResult.Delete)
                    {
                        _conversionQueue.Remove(operationResult);
                    }
                    else
                    {
                        if (newFile != null)
                        {
                            var folderDao = daoFactory.GetFolderDao<T>();
                            var folder = await folderDao.GetFolderAsync(newFile.ParentId);
                            var folderTitle = await fileSecurity.CanReadAsync(folder) ? folder.Title : null;

                            operationResult.Result = fileConverterQueue.FileJsonSerializerAsync(entryManager, newFile, folderTitle).Result;
                        }

                        operationResult.Progress = 100;
                        operationResult.StopDateTime = DateTime.UtcNow;
                        operationResult.Processed = "1";

                        if (!string.IsNullOrEmpty(operationResultError))
                        {
                            operationResult.Error = operationResultError;
                        }
                    }
                }

                logger.DebugCheckConvertFilesStatusIterationEnd();
            }

            fileConverterQueue.SetAllTask<T>(_conversionQueue);

        }
        catch (Exception exception)
        {
            logger.ErrorWithException(exception);
        }
    }
}

public static class FileConverterQueueExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<FileConverterQueueScope>();
    }
}

[Scope]
public class FileConverterQueueScope
{
    private readonly ILogger _options;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly PathProvider _pathProvider;
    private readonly SetupInfo _setupInfo;
    private readonly FileUtility _fileUtility;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly EntryStatusManager _entryManager;
    private readonly FileConverter _fileConverter;

    public FileConverterQueueScope(
        ILogger<FileConverterQueueScope> options,
        TenantManager tenantManager,
        UserManager userManager,
        SecurityContext securityContext,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        PathProvider pathProvider,
        SetupInfo setupInfo,
        FileUtility fileUtility,
        DocumentServiceHelper documentServiceHelper,
        DocumentServiceConnector documentServiceConnector,
        EntryStatusManager entryManager,
        FileConverter fileConverter)
    {
        _options = options;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _pathProvider = pathProvider;
        _setupInfo = setupInfo;
        _fileUtility = fileUtility;
        _documentServiceHelper = documentServiceHelper;
        _documentServiceConnector = documentServiceConnector;
        _entryManager = entryManager;
        _fileConverter = fileConverter;
    }


    public void Deconstruct(out ILogger optionsMonitor,
        out TenantManager tenantManager,
        out UserManager userManager,
        out SecurityContext securityContext,
        out IDaoFactory daoFactory,
        out FileSecurity fileSecurity,
        out PathProvider pathProvider,
        out SetupInfo setupInfo,
        out FileUtility fileUtility,
        out DocumentServiceHelper documentServiceHelper,
        out DocumentServiceConnector documentServiceConnector,
        out EntryStatusManager entryManager,
        out FileConverter fileConverter)
    {
        optionsMonitor = _options;
        tenantManager = _tenantManager;
        userManager = _userManager;
        securityContext = _securityContext;
        daoFactory = _daoFactory;
        fileSecurity = _fileSecurity;
        pathProvider = _pathProvider;
        setupInfo = _setupInfo;
        fileUtility = _fileUtility;
        documentServiceHelper = _documentServiceHelper;
        documentServiceConnector = _documentServiceConnector;
        entryManager = _entryManager;
        fileConverter = _fileConverter;
    }

}
