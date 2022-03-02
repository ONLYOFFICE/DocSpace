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
public class RestoreProgressItem : BaseBackupProgressItem
{
    private readonly TenantManager _tenantManager;
    private readonly BackupStorageFactory _backupStorageFactory;
    private readonly NotifyHelper _notifyHelper;
    private readonly BackupRepository _backupRepository;
    private readonly RestorePortalTask _restorePortalTask;
    private readonly CoreBaseSettings _coreBaseSettings;

    private string _currentRegion;
    private string _upgradesPath;
    private Dictionary<string, string> _configPaths;

    public RestoreProgressItem(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        BackupStorageFactory backupStorageFactory,
        NotifyHelper notifyHelper,
        BackupRepository backupRepository,
        CoreBaseSettings coreBaseSettings)
        : base(options)
    {
        _tenantManager = tenantManager;
        _backupStorageFactory = backupStorageFactory;
        _notifyHelper = notifyHelper;
        _coreBaseSettings = coreBaseSettings;
        _backupRepository = backupRepository;
    }

    public override BackupProgressItemEnum BackupProgressItemEnum { get => BackupProgressItemEnum.Restore; }
    public BackupStorageType StorageType { get; set; }
    public string StoragePath { get; set; }
    public bool Notify { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
    public string TempFolder { get; set; }

    public void Init(StartRestoreRequest request, string tempFolder, string upgradesPath, string currentRegion, Dictionary<string, string> configPaths)
    {
        TenantId = request.TenantId;
        Notify = request.NotifyAfterCompletion;
        StoragePath = request.FilePathOrId;
        StorageType = request.StorageType;
        TempFolder = tempFolder;
        _upgradesPath = upgradesPath;
        _currentRegion = currentRegion;
        _configPaths = configPaths;
    }

    protected override void DoJob()
    {
        Tenant tenant = null;

        var tempFile = PathHelper.GetTempFileName(TempFolder);

        try
        {
            tenant = _tenantManager.GetTenant(TenantId);
            
            _tenantManager.SetCurrentTenant(tenant);
            _notifyHelper.SendAboutRestoreStarted(tenant, Notify);
            
            var storage = _backupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
            
            storage.Download(StoragePath, tempFile);

            if (!_coreBaseSettings.Standalone)
            {
                var backupHash = BackupWorker.GetBackupHash(tempFile);
                var record = _backupRepository.GetBackupRecord(backupHash, TenantId);
                
                if (record == null)
                {
                    throw new Exception(BackupResource.BackupNotFound);
                }
            }

            Percentage = 10;

            tenant.SetStatus(TenantStatus.Restoring);
            _tenantManager.SaveTenant(tenant);

            var columnMapper = new ColumnMapper();
            columnMapper.SetMapping("tenants_tenants", "alias", tenant.Alias, Guid.Parse(Id).ToString("N"));
            columnMapper.Commit();

            var restoreTask = _restorePortalTask;
            restoreTask.Init(_configPaths[_currentRegion], tempFile, TenantId, columnMapper, _upgradesPath);
            restoreTask.ProgressChanged += (sender, args) =>
            {
                Percentage = Percentage = 10d + 0.65 * args.Progress;
                PublishChanges();
            };
            restoreTask.RunJob();

            Tenant restoredTenant = null;

            if (restoreTask.Dump)
            {
                AscCacheNotify.OnClearCache();

                if (Notify)
                {
                    var tenants = _tenantManager.GetTenants();
                    foreach (var t in tenants)
                    {
                        _notifyHelper.SendAboutRestoreCompleted(t, Notify);
                    }
                }
            }
            else
            {
                _tenantManager.RemoveTenant(tenant.Id);

                restoredTenant = _tenantManager.GetTenant(columnMapper.GetTenantMapping());
                restoredTenant.SetStatus(TenantStatus.Active);
                restoredTenant.Alias = tenant.Alias;
                restoredTenant.PaymentId = string.Empty;

                if (string.IsNullOrEmpty(restoredTenant.MappedDomain) && !string.IsNullOrEmpty(tenant.MappedDomain))
                {
                    restoredTenant.MappedDomain = tenant.MappedDomain;
                }
                
                _tenantManager.SaveTenant(restoredTenant);
                _tenantManager.SetCurrentTenant(restoredTenant);
                // sleep until tenants cache expires
                Thread.Sleep(TimeSpan.FromMinutes(2));

                _notifyHelper.SendAboutRestoreCompleted(restoredTenant, Notify);
            }

            Percentage = 75;

            PublishChanges();

            File.Delete(tempFile);

            Percentage = 100;
            PublishChanges();
        }
        catch (Exception error)
        {
            Logger.Error(error);
            Exception = error;

            if (tenant != null)
            {
                tenant.SetStatus(TenantStatus.Active);
                _tenantManager.SaveTenant(tenant);
            }
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                Logger.Error("publish", error);
            }

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}
