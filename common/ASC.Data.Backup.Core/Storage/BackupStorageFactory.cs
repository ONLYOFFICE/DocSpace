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

namespace ASC.Data.Backup.Storage;

[Scope]
public class BackupStorageFactory
{
    private readonly ConfigurationExtension _configuration;
    private readonly DocumentsBackupStorage _documentsBackupStorage;
    private readonly ILogger<BackupStorageFactory> _logger;
    private readonly LocalBackupStorage _localBackupStorage;
    private readonly ConsumerBackupStorage _consumerBackupStorage;
    private readonly TenantManager _tenantManager;

    public BackupStorageFactory(
        ConsumerBackupStorage consumerBackupStorage,
        LocalBackupStorage localBackupStorage,
        ConfigurationExtension configuration,
        DocumentsBackupStorage documentsBackupStorage,
        TenantManager tenantManager,
        ILogger<BackupStorageFactory> logger)
    {
        _configuration = configuration;
        _documentsBackupStorage = documentsBackupStorage;
        _logger = logger;
        _localBackupStorage = localBackupStorage;
        _consumerBackupStorage = consumerBackupStorage;
        _tenantManager = tenantManager;
    }

    public async Task<IBackupStorage> GetBackupStorageAsync(BackupRecord record)
    {
        try
        {
            return await GetBackupStorageAsync(record.StorageType, record.TenantId, JsonConvert.DeserializeObject<Dictionary<string, string>>(record.StorageParams));
        }
        catch (Exception error)
        {
            _logger.ErrorCantGetBackupStorage(record.Id, error);

            return null;
        }
    }

    public async Task<IBackupStorage> GetBackupStorageAsync(BackupStorageType type, int tenantId, Dictionary<string, string> storageParams)
    {
        var settings = _configuration.GetSetting<BackupSettings>("backup");

        switch (type)
        {
            case BackupStorageType.Documents:
            case BackupStorageType.ThridpartyDocuments:
                {
                    await _documentsBackupStorage.InitAsync(tenantId);

                    return _documentsBackupStorage;
                }
            case BackupStorageType.DataStore:
                {
                    await _consumerBackupStorage.InitAsync(tenantId);

                    return _consumerBackupStorage;
                }
            case BackupStorageType.Local:
                return _localBackupStorage;
            case BackupStorageType.ThirdPartyConsumer:
                {
                    if (storageParams == null)
                    {
                        return null;
                    }

                    await _tenantManager.SetCurrentTenantAsync(tenantId);
                    await _consumerBackupStorage.InitAsync(storageParams);

                    return _consumerBackupStorage;
                }
            default:
                throw new InvalidOperationException("Unknown storage type.");
        }
    }
}
