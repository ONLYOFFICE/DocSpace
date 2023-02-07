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

/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

namespace ASC.Data.Backup.Services;

[Transient]
public class BackupProgressItem : BaseBackupProgressItem
{
    public Dictionary<string, string> StorageParams { get; set; }
    public string TempFolder { get; set; }

    private const string ArchiveFormat = "tar.gz";

    private bool _isScheduled;
    private Guid _userId;
    private BackupStorageType _storageType;
    private string _storageBasePath;
    private int _limit;

    private TenantManager _tenantManager;
    private BackupStorageFactory _backupStorageFactory;
    private BackupRepository _backupRepository;
    private BackupPortalTask _backupPortalTask;
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
        if (ThreadPriority.BelowNormal < Thread.CurrentThread.Priority)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
        }

        await using var scope = _serviceScopeProvider.CreateAsyncScope();

        _tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        _backupStorageFactory = scope.ServiceProvider.GetService<BackupStorageFactory>();
        _backupRepository = scope.ServiceProvider.GetService<BackupRepository>();
        _backupPortalTask = scope.ServiceProvider.GetService<BackupPortalTask>();

        var dateTime = _coreBaseSettings.Standalone ? DateTime.Now : DateTime.UtcNow;
        var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", _tenantManager.GetTenant(TenantId).Alias, dateTime, ArchiveFormat);

        var tempFile = CrossPlatform.PathCombine(TempFolder, backupName);
        var storagePath = tempFile;

        try
        {
            var backupTask = _backupPortalTask;

            backupTask.Init(TenantId, tempFile, _limit);

            backupTask.ProgressChanged += (sender, args) =>
            {
                Percentage = 0.9 * args.Progress;
                PublishChanges();
            };

            await backupTask.RunJob();

            var backupStorage = _backupStorageFactory.GetBackupStorage(_storageType, TenantId, StorageParams);
            if (backupStorage != null)
            {
                storagePath = await backupStorage.Upload(_storageBasePath, tempFile, _userId);
                Link = await backupStorage.GetPublicLink(storagePath);
            }

            var repo = _backupRepository;

            repo.SaveBackupRecord(
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
                    Hash = BackupWorker.GetBackupHash(tempFile),
                    Removed = false
                });

            Percentage = 100;

            if (_userId != Guid.Empty && !_isScheduled)
            {
                _notifyHelper.SendAboutBackupCompleted(TenantId, _userId);
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
