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
using System.Linq;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Data.Backup.EF.Context;
using ASC.Data.Backup.EF.Model;
namespace ASC.Data.Backup.Storage
{
    [Scope]
    public class BackupRepository : IBackupRepository
    {
        private Lazy<BackupsContext> LazyBackupsContext { get; }
        private BackupsContext BackupContext { get => LazyBackupsContext.Value; }

        public BackupRepository(DbContextManager<BackupsContext> backupContext)
        {
            LazyBackupsContext = new Lazy<BackupsContext>(() => backupContext.Value);
        }

        public void SaveBackupRecord(BackupRecord backup)
        {
            BackupContext.AddOrUpdate(r => r.Backups, backup);
            BackupContext.SaveChanges();
        }

        public BackupRecord GetBackupRecord(Guid id)
        {
            return BackupContext.Backups.SingleOrDefault(b => b.Id == id);
        }

        public BackupRecord GetBackupRecord(string hash, int tenant)
        {
            return BackupContext.Backups.SingleOrDefault(b => b.Hash == hash && b.TenantId == tenant);
        }

        public List<BackupRecord> GetExpiredBackupRecords()
        {
            return BackupContext.Backups.Where(b => b.ExpiresOn != DateTime.MinValue && b.ExpiresOn <= DateTime.UtcNow).ToList();
        }

        public List<BackupRecord> GetScheduledBackupRecords()
        {
            return BackupContext.Backups.Where(b => b.IsScheduled == true).ToList();
        }

        public List<BackupRecord> GetBackupRecordsByTenantId(int tenantId)
        {
            return BackupContext.Backups.Where(b => b.TenantId == tenantId).ToList();
        }

        public void DeleteBackupRecord(Guid id)
        {

            var backup = BackupContext.Backups.FirstOrDefault(b => b.Id == id);
            if (backup != null)
            {
                BackupContext.Backups.Remove(backup);
                BackupContext.SaveChanges();
            }
        }

        public void SaveBackupSchedule(BackupSchedule schedule)
        {
            BackupContext.AddOrUpdate(r => r.Schedules, schedule);
            BackupContext.SaveChanges();
        }

        public void DeleteBackupSchedule(int tenantId)
        {
            var shedule = BackupContext.Schedules.Where(s => s.TenantId == tenantId).ToList();
            BackupContext.Schedules.RemoveRange(shedule);
            BackupContext.SaveChanges();
        }

        public List<BackupSchedule> GetBackupSchedules()
        {
            var query = BackupContext.Schedules.Join(BackupContext.Tenants,
                s => s.TenantId,
                t => t.Id,
                (s, t) => new { schedule = s, tenant = t })
                .Where(q => q.tenant.Status == TenantStatus.Active)
                .Select(q => q.schedule);

            return query.ToList();
        }

        public BackupSchedule GetBackupSchedule(int tenantId)
        {
            return BackupContext.Schedules.SingleOrDefault(s => s.TenantId == tenantId);
        }
    }
}
