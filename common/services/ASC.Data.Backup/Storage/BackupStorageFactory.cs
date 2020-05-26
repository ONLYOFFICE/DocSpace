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


using System;
using System.Collections.Generic;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.Data.Backup.EF.Context;
using ASC.Data.Backup.EF.Model;
using ASC.Data.Backup.Service;
using ASC.Data.Backup.Utils;
using ASC.Data.Storage;
using ASC.Files.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Storage
{
    public class BackupStorageFactory
    {

        private TenantManager tenantManager;
        private SecurityContext securityContext;
        private IDaoFactory daoFactory; 
        private StorageFactory storageFactory;
        private BackupRecordContext backupContext;
        private ScheduleContext scheduleContext;
        private TenantDbContext tenantsContext;
        private IOptionsMonitor<ILog> options;
        private TenantUtil tenantUtil;
        private BackupHelper backupHelper;

        public BackupStorageFactory(TenantManager tenantManager, SecurityContext securityContext, IDaoFactory daoFactory, StorageFactory storageFactory, BackupRecordContext backupContext, ScheduleContext scheduleContext, TenantDbContext tenantsContext, IOptionsMonitor<ILog> options, TenantUtil tenantUtil, BackupHelper backupHelper)
        {
            this.tenantManager = tenantManager;
            this.securityContext = securityContext;
            this.daoFactory = daoFactory;
            this.storageFactory = storageFactory;
            this.backupContext = backupContext;
            this.scheduleContext = scheduleContext;
            this.tenantsContext = tenantsContext;
            this.options = options;
            this.tenantUtil = tenantUtil;
            this.backupHelper = backupHelper;
        }
        public IBackupStorage GetBackupStorage(BackupRecord record)
        {
            return GetBackupStorage(record.StorageType, record.TenantId, record.StorageParams);
        }
        
        public IServiceProvider ServiceProvider { get; }
        private DocumentsBackupStorage documentsBackupStorage;
        public BackupStorageFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public IBackupStorage GetBackupStorage(BackupStorageType type, int tenantId, Dictionary<string, string> storageParams)
        {
            var config = BackupConfigurationSection.GetSection();
            var webConfigPath = PathHelper.ToRootedConfigPath(config.WebConfigs.CurrentPath);
            using var scope = ServiceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            switch (type)
            {
                case BackupStorageType.Documents:
                case BackupStorageType.ThridpartyDocuments:
                    return new DocumentsBackupStorage(tenantId, webConfigPath, tenantManager, securityContext, daoFactory, storageFactory);
                case BackupStorageType.DataStore:
                    return new DataStoreBackupStorage(tenantId, webConfigPath, storageFactory);
                case BackupStorageType.Local:
                    return new LocalBackupStorage();
                case BackupStorageType.ThirdPartyConsumer:
                    if (storageParams == null) return null;
                    tenantManager.SetCurrentTenant(tenantId);
                    return new ConsumerBackupStorage(storageParams);
                default:
                    throw new InvalidOperationException("Unknown storage type.");
            }
        }

        public IBackupRepository GetBackupRepository()
        {
            return new BackupRepository(backupContext, scheduleContext, tenantsContext, options, tenantManager, tenantUtil, backupHelper);
        }
    }
}
