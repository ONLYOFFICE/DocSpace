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


namespace ASC.Data.Backup.Services;

[Transient]
public class BackupProgressItem : BaseBackupProgressItem
{
    public Dictionary<string, string> StorageParams { get; set; }
    public string TempFolder { get; set; }

    private bool _isScheduled;
    private Guid _userId;
    private BackupStorageType _storageType;
    private string _storageBasePath;
    private int _limit;

    private TenantManager _tenantManager;
    private BackupStorageFactory _backupStorageFactory;
    private BackupRepository _backupRepository;
    private BackupPortalTask _backupPortalTask;
    private TempStream _tempStream;
    private readonly ILogger<BackupProgressItem> _logger;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly NotifyHelper _notifyHelper;

    public BackupProgressItem(
        ILogger<BackupProgressItem> logger,
        IServiceScopeFactory serviceProvider,
        CoreBaseSettings coreBaseSettings,
        NotifyHelper notifyHelper)
        : base(logger, serviceProvider)
    {
        _logger = logger;
        _coreBaseSettings = coreBaseSettings;
        _notifyHelper = notifyHelper;
    }

    public void Init(BackupSchedule schedule, bool isScheduled, string tempFolder, int limit)
    {
        _userId = Guid.Empty;
        TenantId = schedule.TenantId;
        _storageType = schedule.StorageType;
        _storageBasePath = schedule.StorageBasePath;
        StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams);
        _isScheduled = isScheduled;
        TempFolder = tempFolder;
        _limit = limit;
    }

    public void Init(StartBackupRequest request, bool isScheduled, string tempFolder, int limit)
    {
        _userId = request.UserId;
        TenantId = request.TenantId;
        _storageType = request.StorageType;
        _storageBasePath = request.StorageBasePath;
        StorageParams = request.StorageParams.ToDictionary(r => r.Key, r => r.Value);
        _isScheduled = isScheduled;
        TempFolder = tempFolder;
        _limit = limit;
    }

    protected override async Task DoJob()
    {
        await using var scope = _serviceScopeProvider.CreateAsyncScope();

        _tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        _backupStorageFactory = scope.ServiceProvider.GetService<BackupStorageFactory>();
        _backupRepository = scope.ServiceProvider.GetService<BackupRepository>();
        _backupPortalTask = scope.ServiceProvider.GetService<BackupPortalTask>();
        _tempStream = scope.ServiceProvider.GetService<TempStream>();

        var dateTime = _coreBaseSettings.Standalone ? DateTime.Now : DateTime.UtcNow;
        string hash;
        var tempFile = "";
        var storagePath = "";

        try
        {
            var backupStorage = await _backupStorageFactory.GetBackupStorageAsync(_storageType, TenantId, StorageParams);

            var getter = backupStorage as IGetterWriteOperator;
            var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", (await _tenantManager.GetTenantAsync(TenantId)).Alias, dateTime, await getter.GetBackupExtensionAsync(_storageBasePath));

            tempFile = CrossPlatform.PathCombine(TempFolder, backupName);
            storagePath = tempFile;

            var writer = await DataOperatorFactory.GetWriteOperatorAsync(_tempStream, _storageBasePath, backupName, TempFolder, _userId, getter);

            _backupPortalTask.Init(TenantId, tempFile, _limit, writer);

            _backupPortalTask.ProgressChanged += (sender, args) =>
            {
                Percentage = 0.9 * args.Progress;
                PublishChanges();
            };

            await _backupPortalTask.RunJob();

            if (writer.NeedUpload)
            {
                storagePath = await backupStorage.UploadAsync(_storageBasePath, tempFile, _userId);
                hash = BackupWorker.GetBackupHashSHA(tempFile);
            }
            else
            {
                storagePath = writer.StoragePath;
                hash = writer.Hash;
            }
            Link = await backupStorage.GetPublicLinkAsync(storagePath);

            var repo = _backupRepository;

            await repo.SaveBackupRecordAsync(
                new BackupRecord
                {
                    Id = Guid.Parse(Id),
                    TenantId = TenantId,
                    IsScheduled = _isScheduled,
                    Name = Path.GetFileName(tempFile),
                    StorageType = _storageType,
                    StorageBasePath = _storageBasePath,
                    StoragePath = storagePath,
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = _storageType == BackupStorageType.DataStore ? DateTime.UtcNow.AddDays(1) : DateTime.MinValue,
                    StorageParams = JsonConvert.SerializeObject(StorageParams),
                    Hash = hash,
                    Removed = false
                });

            Percentage = 100;

            if (_userId != Guid.Empty && !_isScheduled)
            {
                await _notifyHelper.SendAboutBackupCompletedAsync(TenantId, _userId);
            }


            IsCompleted = true;
            PublishChanges();
        }
        catch (Exception error)
        {
            _logger.ErrorRunJob(Id, TenantId, tempFile, _storageBasePath, error);
            Exception = error;
            IsCompleted = true;
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                _logger.ErrorPublish(error);
            }

            try
            {
                if (!(storagePath == tempFile && _storageType == BackupStorageType.Local))
                {
                    File.Delete(tempFile);
                }
            }
            catch (Exception error)
            {
                _logger.ErrorCantDeleteFile(error);
            }
        }
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}
