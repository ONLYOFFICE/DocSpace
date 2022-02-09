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

namespace ASC.Data.Backup.Storage
{
    [Scope]
    public class BackupStorageFactory
    {
        private readonly ConfigurationExtension _configuration;
        private readonly DocumentsBackupStorage _documentsBackupStorage;
        private readonly DataStoreBackupStorage _dataStoreBackupStorage;
        private readonly ILog _logger;
        private readonly LocalBackupStorage _localBackupStorage;
        private readonly ConsumerBackupStorage _consumerBackupStorage;
        private readonly TenantManager _tenantManager;

        public BackupStorageFactory(
            ConsumerBackupStorage consumerBackupStorage,
            LocalBackupStorage localBackupStorage,
            ConfigurationExtension configuration,
            DocumentsBackupStorage documentsBackupStorage,
            TenantManager tenantManager,
            DataStoreBackupStorage dataStoreBackupStorage,
            IOptionsMonitor<ILog> options)
        {
            _configuration = configuration;
            _documentsBackupStorage = documentsBackupStorage;
            _dataStoreBackupStorage = dataStoreBackupStorage;
            _logger = options.CurrentValue;
            _localBackupStorage = localBackupStorage;
            _consumerBackupStorage = consumerBackupStorage;
            _tenantManager = tenantManager;
        }

        public IBackupStorage GetBackupStorage(BackupRecord record)
        {
            try
            {
                return GetBackupStorage(record.StorageType, record.TenantId, JsonConvert.DeserializeObject<Dictionary<string, string>>(record.StorageParams));
            }
            catch (Exception error)
            {
                _logger.Error("can't get backup storage for record " + record.Id, error);

                return null;
            }
        }

        public IBackupStorage GetBackupStorage(BackupStorageType type, int tenantId, Dictionary<string, string> storageParams)
        {
            var settings = _configuration.GetSetting<BackupSettings>("backup");
            var webConfigPath = PathHelper.ToRootedConfigPath(settings.WebConfigs.CurrentPath);


            switch (type)
            {
                case BackupStorageType.Documents:
                case BackupStorageType.ThridpartyDocuments:
                {
                    _documentsBackupStorage.Init(tenantId, webConfigPath);

                    return _documentsBackupStorage;
                }
                case BackupStorageType.DataStore:
                {
                    _dataStoreBackupStorage.Init(tenantId, webConfigPath);

                    return _dataStoreBackupStorage;
                }
                case BackupStorageType.Local:
                    return _localBackupStorage;
                case BackupStorageType.ThirdPartyConsumer:
                {
                    if (storageParams == null)
                    {
                        return null;
                    }

                    _tenantManager.SetCurrentTenant(tenantId);
                    _consumerBackupStorage.Init(storageParams);

                    return _consumerBackupStorage;
                }
                default:
                    throw new InvalidOperationException("Unknown storage type.");
            }
        }
    }
}
