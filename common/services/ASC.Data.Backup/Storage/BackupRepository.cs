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
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF.Context;
using ASC.Core.Tenants;
using ASC.Data.Backup.EF.Context;
using ASC.Data.Backup.EF.Model;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Storage
{
    internal class BackupRepository : IBackupRepository
    {

        private readonly BackupRecordContext backupContext;
        private readonly ScheduleContext scheduleContext;
        private readonly TenantDbContext tenantsContext;
        private readonly IOptionsMonitor<ILog> options;
        private readonly TenantManager tenantManager;
        private readonly TenantUtil tenantUtil;
        private readonly BackupHelper backupHelper;

        public BackupRepository(BackupRecordContext backupContext, ScheduleContext scheduleContext, TenantDbContext tenantsContext, IOptionsMonitor<ILog> options,TenantManager tenantManager, TenantUtil tenantUtil, BackupHelper backupHelper)
        {
            this.tenantsContext = tenantsContext;
            this.backupContext = backupContext;
            this.scheduleContext = scheduleContext;
            this.options = options;
            this.tenantManager = tenantManager;
            this.tenantUtil = tenantUtil;
            this.backupHelper = backupHelper;
         //   this.connectionStringName = connectionStringName ?? "core";
        }

        public void SaveBackupRecord(BackupRecord backup)
        {
            backupContext.Backups.Add(backup);
            backupContext.SaveChanges();
          /*  var insert = new SqlInsert("backup_backup")
                .ReplaceExists(true)
                .InColumnValue("id", backupRecord.Id)
                .InColumnValue("tenant_id", backupRecord.TenantId)
                .InColumnValue("is_scheduled", backupRecord.IsScheduled)
                .InColumnValue("name", backupRecord.FileName)
                .InColumnValue("storage_type", (int)backupRecord.StorageType)
                .InColumnValue("storage_base_path", backupRecord.StorageBasePath)
                .InColumnValue("storage_path", backupRecord.StoragePath)
                .InColumnValue("created_on", backupRecord.CreatedOn)
                .InColumnValue("expires_on", backupRecord.ExpiresOn)
                .InColumnValue("storage_params", JsonConvert.SerializeObject(backupRecord.StorageParams));

            using (var db = GetDbManager())
            {
                db.ExecuteNonQuery(insert);
            }*/
        }

        public BackupRecord GetBackupRecord(Guid id)
        {
          return backupContext.Backups.FirstOrDefault(b => b.Id == id);
            
          /*  var select = new SqlQuery("backup_backup")
                .Select("id", "tenant_id", "is_scheduled", "name", "storage_type", "storage_base_path", "storage_path", "created_on", "expires_on", "storage_params")
                .Where("id", id);

            using (var db = GetDbManager())
            {
                return db.ExecuteList(select).Select(ToBackupRecord).SingleOrDefault();
            }*/
        }

        public List<BackupRecord> GetExpiredBackupRecords()
        {
            
            return new List<BackupRecord>(backupContext.Backups.Where(b=> b.ExpiresOn != DateTime.MinValue && b.ExpiresOn <= DateTime.UtcNow));
            /*  var select = new SqlQuery("backup_backup")
                  .Select("id", "tenant_id", "is_scheduled", "name", "storage_type", "storage_base_path", "storage_path", "created_on", "expires_on", "storage_params")
                  .Where(!Exp.Eq("expires_on", DateTime.MinValue) & Exp.Le("expires_on", DateTime.UtcNow));

              using (var db = GetDbManager())
              {
                  return db.ExecuteList(select).Select(ToBackupRecord).ToList();
              }*/
        }

        public List<BackupRecord> GetScheduledBackupRecords()
        {
            return new List<BackupRecord>(backupContext.Backups.Where(b=> b.IsScheduled == true));
            /*var select = new SqlQuery("backup_backup")
                .Select("id", "tenant_id", "is_scheduled", "name", "storage_type", "storage_base_path", "storage_path", "created_on", "expires_on", "storage_params")
                .Where(Exp.Eq("is_scheduled", true));

            using (var db = GetDbManager())
            {
                return db.ExecuteList(select).Select(ToBackupRecord).ToList();
            }*/
        }

        public List<BackupRecord> GetBackupRecordsByTenantId(int tenantId)
        {
            return new List<BackupRecord>(backupContext.Backups.Where(b=> b.TenantId == tenantId));
            /* var select = new SqlQuery("backup_backup")
                 .Select("id", "tenant_id", "is_scheduled", "name", "storage_type", "storage_base_path", "storage_path", "created_on", "expires_on", "storage_params")
                 .Where("tenant_id", tenantId);

             using (var db = GetDbManager())
             {
                 return db.ExecuteList(select).Select(ToBackupRecord).ToList();
             }*/
        }

        public void DeleteBackupRecord(Guid id)
        {

            var backup = backupContext.Backups.FirstOrDefault(b => b.Id == id);
            if (backup != null)
            {
                backupContext.Backups.Remove(backup);
                backupContext.SaveChanges();
            }
            /*  var delete = new SqlDelete("backup_backup")
                  .Where("id", id);

              using (var db = GetDbManager())
              {
                  db.ExecuteNonQuery(delete);
              }*/
        }

        public void SaveBackupSchedule(Schedule schedule)
        {
            scheduleContext.Schedules.Add(schedule);
            backupContext.SaveChanges();
            /*  var query = new SqlInsert("backup_schedule")
                  .ReplaceExists(true)
                  .InColumnValue("tenant_id", schedule.TenantId)
                  .InColumnValue("backup_mail", schedule.BackupMail)
                  .InColumnValue("cron", schedule.Cron)
                  .InColumnValue("backups_stored", schedule.NumberOfBackupsStored)
                  .InColumnValue("storage_type", (int)schedule.StorageType)
                  .InColumnValue("storage_base_path", schedule.StorageBasePath)
                  .InColumnValue("last_backup_time", schedule.LastBackupTime)
                  .InColumnValue("storage_params", JsonConvert.SerializeObject(schedule.StorageParams));

              using (var db = GetDbManager())
              {
                  db.ExecuteNonQuery(query);
              }*/
        }

        public void DeleteBackupSchedule(int tenantId)
        {
            var shedule = scheduleContext.Schedules.FirstOrDefault(s => s.TenantId == tenantId);
            if (shedule != null)
            {
                scheduleContext.Schedules.Remove(shedule);
                scheduleContext.SaveChanges();
            }
            /*
            var query = new SqlDelete("backup_schedule")
                .Where("tenant_id", tenantId);

            using (var db = GetDbManager())
            {
                db.ExecuteNonQuery(query);
            }*/
        }

        public List<Schedule> GetBackupSchedules()
        {
            var query = scheduleContext.Schedules.Join(tenantsContext.Tenants,
                s=> s.TenantId, 
                t=> t.Id,
                (s, t) => new{
                    TenantId = s.TenantId,
                    BackupMail = s.BackupMail,
                    Cron = s.Cron,
                    BackupsStored = s.BackupsStored,
                    StorageType = s.StorageType,
                    StorageBasePath = s.StorageBasePath,
                    LastBackupTime = s.LastBackupTime,
                    StorageParams = s.StorageParams,
                    Status = t.Status
                }).Where(q=> q.Status == (int)TenantStatus.Active);
            List<Schedule> shedules = new List<Schedule>();
            foreach (var q in query) {
                shedules.Add(new Schedule(options, q.TenantId, tenantManager, tenantUtil, backupHelper) { BackupMail = q.BackupMail, Cron = q.Cron, BackupsStored = q.BackupsStored, StorageType = q.StorageType, StorageBasePath = q.StorageBasePath, LastBackupTime = q.LastBackupTime, StorageParams = q.StorageParams });
            }
            return shedules;
            /* var query = new SqlQuery("backup_schedule s")
                 .Select("s.tenant_id", "s.backup_mail", "s.cron", "s.backups_stored", "s.storage_type", "s.storage_base_path", "s.last_backup_time", "s.storage_params")
                 .InnerJoin("tenants_tenants t", Exp.EqColumns("t.id", "s.tenant_id"))
                 .Where("t.status", (int)TenantStatus.Active);

             using (var db = GetDbManager())
             {
                 return db.ExecuteList(query).Select(ToSchedule).ToList();
             }*/
        }

        public Schedule GetBackupSchedule(int tenantId)
        {
          return scheduleContext.Schedules.FirstOrDefault(s => s.TenantId == tenantId);
          /* var query = new SqlQuery("backup_schedule")
                .Select("tenant_id", "backup_mail", "cron", "backups_stored", "storage_type", "storage_base_path", "last_backup_time", "storage_params")
                .Where("tenant_id", tenantId);

            using (var db = GetDbManager())
            {
                return db.ExecuteList(query).Select(ToSchedule).SingleOrDefault();
            }*/
        }

        
       /* private IDbManager GetDbManager()//тут
        {
            return new DbManager(connectionStringName);
        }*/
    }
}
