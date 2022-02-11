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

using Microsoft.EntityFrameworkCore;

namespace ASC.Data.Backup.Storage
{
    [Scope]
    public class BackupRepository : IBackupRepository
    {
        private readonly Lazy<BackupsContext> _backupContext;
        public BackupRepository(DbContextManager<BackupsContext> dbContactManager)
        {
            _backupContext = new Lazy<BackupsContext>(() => dbContactManager.Value);           
        }

        public void SaveBackupRecord(BackupRecord backup)
        {
            _backupContext.Value.AddOrUpdate(r => r.Backups, backup);
            _backupContext.Value.SaveChanges();
        }

        public BackupRecord GetBackupRecord(Guid id)
        {
            return _backupContext.Value.Backups.Find(id);
        }

        public BackupRecord GetBackupRecord(string hash, int tenant)
        {
            return _backupContext.Value.Backups.AsNoTracking().SingleOrDefault(b => b.Hash == hash && b.TenantId == tenant);
        }

        public List<BackupRecord> GetExpiredBackupRecords()
        {
            return _backupContext.Value.Backups.AsNoTracking().AsQueryable().Where(b => b.ExpiresOn != DateTime.MinValue && b.ExpiresOn <= DateTime.UtcNow).ToList();
        }

        public List<BackupRecord> GetScheduledBackupRecords()
        {
            return _backupContext.Value.Backups.AsQueryable().AsNoTracking().Where(b => b.IsScheduled == true).ToList();
        }

        public List<BackupRecord> GetBackupRecordsByTenantId(int tenantId)
        {
            return _backupContext.Value.Backups.AsQueryable().AsNoTracking().Where(b => b.TenantId == tenantId).ToList();
        }

        public void DeleteBackupRecord(Guid id)
        {
            var backup = _backupContext.Value.Backups.Find(id);
         
            if (backup != null)
            {
                _backupContext.Value.Backups.Remove(backup);
                _backupContext.Value.SaveChanges();
            }
        }

        public void SaveBackupSchedule(BackupSchedule schedule)
        {
            _backupContext.Value.AddOrUpdate(r => r.Schedules, schedule);
            _backupContext.Value.SaveChanges();
        }

        public void DeleteBackupSchedule(int tenantId)
        {
            var shedule = _backupContext.Value.Schedules.AsQueryable().Where(s => s.TenantId == tenantId).ToList();

            _backupContext.Value.Schedules.RemoveRange(shedule);
            _backupContext.Value.SaveChanges();
        }

        public List<BackupSchedule> GetBackupSchedules()
        {
            var query = _backupContext.Value.Schedules.AsQueryable().Join(_backupContext.Value.Tenants,
                s => s.TenantId,
                t => t.Id,
                (s, t) => new { schedule = s, tenant = t })
                .Where(q => q.tenant.Status == TenantStatus.Active)
                .Select(q => q.schedule);

            return query.ToList();
        }

        public BackupSchedule GetBackupSchedule(int tenantId)
        {
            return _backupContext.Value.Schedules.AsNoTracking().SingleOrDefault(s => s.TenantId == tenantId);
        }
    }
}
