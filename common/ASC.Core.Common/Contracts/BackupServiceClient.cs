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
using System.Collections.Generic;

using ASC.Common;
using ASC.Common.Caching;

using Google.Protobuf;

namespace ASC.Core.Common.Contracts
{
    public class BackupServiceClient : IBackupService
    {
        public ICacheNotify<DeleteBackupRequest> CacheDeleteBackupRequest { get; }
        public ICacheNotify<StartBackupRequest> СacheStartBackupRequest { get; set; }
        public ICacheNotify<StartRestoreRequest> СacheStartRestoreRequest { get; set; }
        public ICacheNotify<StartTransferRequest> СacheStartTransferRequest { get; set; }
        public ICacheNotify<BackupProgress> СacheBackupProgress { get; set; }
        public ICache Cache { get; }

        public BackupServiceClient(
            ICacheNotify<DeleteBackupRequest> cacheDeleteBackupRequest,
            ICacheNotify<StartBackupRequest> cacheStartBackupRequest,
            ICacheNotify<StartRestoreRequest> cacheStartRestoreRequest,
            ICacheNotify<StartTransferRequest> cacheStartTransferRequest,
            ICacheNotify<BackupProgress> сacheBackupProgress
            )
        {
            CacheDeleteBackupRequest = cacheDeleteBackupRequest;
            СacheStartBackupRequest = cacheStartBackupRequest;
            СacheStartRestoreRequest = cacheStartRestoreRequest;
            СacheStartTransferRequest = cacheStartTransferRequest;
            СacheBackupProgress = сacheBackupProgress;
            Cache = AscCache.Memory;

            СacheBackupProgress.Subscribe((a) =>
            {
                Cache.Insert(GetCacheKey(a.TenantId, a.BackupProgressEnum), a, DateTime.UtcNow.AddDays(1));
            },
            CacheNotifyAction.InsertOrUpdate);
        }

        public void StartBackup(StartBackupRequest request)
        {
            СacheStartBackupRequest.Publish(request, CacheNotifyAction.InsertOrUpdate);
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            return Cache.Get<BackupProgress>(GetCacheKey(tenantId, BackupProgressEnum.Backup));
        }

        public void DeleteBackup(Guid backupId)
        {
            CacheDeleteBackupRequest.Publish(new DeleteBackupRequest() { Id = ByteString.CopyFrom(backupId.ToByteArray()), TenantId = -1 }, CacheNotifyAction.InsertOrUpdate);

        }

        public void DeleteAllBackups(int tenantId)
        {
            CacheDeleteBackupRequest.Publish(new DeleteBackupRequest() { Id = ByteString.CopyFrom(Guid.Empty.ToByteArray()), TenantId = tenantId }, CacheNotifyAction.InsertOrUpdate);
        }

        public List<BackupHistoryRecord> GetBackupHistory(int tenantId)
        {
            // return Channel.GetBackupHistory(tenantId);
            return null;
        }

        public void StartTransfer(StartTransferRequest request)
        {
            СacheStartTransferRequest.Publish(request, CacheNotifyAction.InsertOrUpdate);
        }

        public BackupProgress GetTransferProgress(int tenantID)
        {
            return Cache.Get<BackupProgress>(GetCacheKey(tenantID, BackupProgressEnum.Transfer));
        }

        public List<TransferRegion> GetTransferRegions()
        {
            // return Channel.GetTransferRegions();
            return null;
        }

        public void StartRestore(StartRestoreRequest request)
        {
            СacheStartRestoreRequest.Publish(request, CacheNotifyAction.InsertOrUpdate);
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            return Cache.Get<BackupProgress>(GetCacheKey(tenantId, BackupProgressEnum.Restore));
        }

        public string GetTmpFolder()
        {
            // return Channel.GetTmpFolder();
            return null;
        }

        public void CreateSchedule(CreateScheduleRequest request)
        {
            //  Channel.CreateSchedule(request);
        }

        public void DeleteSchedule(int tenantId)
        {
            //  Channel.DeleteSchedule(tenantId);
        }

        public ScheduleResponse GetSchedule(int tenantId)
        {
            // return Channel.GetSchedule(tenantId);
            return null;
        }

        private string GetCacheKey(int tenantId, BackupProgressEnum backupProgressEnum) => $"{backupProgressEnum}backup{tenantId}";
    }
    public static class BackupServiceClientExtension
    {
        public static DIHelper AddBackupServiceClient(this DIHelper services)
        {
            services.TryAddSingleton<BackupServiceClient>();
            return services;
        }
    }
}