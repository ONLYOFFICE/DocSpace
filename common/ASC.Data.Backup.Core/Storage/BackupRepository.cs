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
public class BackupRepository : IBackupRepository
{
    private BackupsContext BackupContext => _lazyBackupContext.Value;
    private readonly Lazy<BackupsContext> _lazyBackupContext;

    public BackupRepository(DbContextManager<BackupsContext> dbContactManager)
    {
        _lazyBackupContext = new Lazy<BackupsContext>(() => dbContactManager.Value);
    }

    public void SaveBackupRecord(BackupRecord backup)
    {
        BackupContext.AddOrUpdate(r => r.Backups, backup);
        BackupContext.SaveChanges();
    }

    public BackupRecord GetBackupRecord(Guid id)
    {
        return BackupContext.Backups.Find(id);
    }

    public BackupRecord GetBackupRecord(string hash, int tenant)
    {
        return BackupContext.Backups.AsNoTracking().AsQueryable().SingleOrDefault(b => b.Hash == hash && b.TenantId == tenant);
    }

    public List<BackupRecord> GetExpiredBackupRecords()
    {
        return BackupContext.Backups.AsNoTracking().Where(b => b.ExpiresOn != DateTime.MinValue && b.ExpiresOn <= DateTime.UtcNow).ToList();
    }

    public List<BackupRecord> GetScheduledBackupRecords()
    {
        return BackupContext.Backups.AsNoTracking().AsQueryable().Where(b => b.IsScheduled == true).ToList();
    }

    public List<BackupRecord> GetBackupRecordsByTenantId(int tenantId)
    {
        return BackupContext.Backups.AsNoTracking().AsQueryable().Where(b => b.TenantId == tenantId).ToList();
    }

    public void DeleteBackupRecord(Guid id)
    {
        var backup = BackupContext.Backups.Find(id);

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
        var shedule = BackupContext.Schedules.AsQueryable().Where(s => s.TenantId == tenantId).ToList();

        BackupContext.Schedules.RemoveRange(shedule);
        BackupContext.SaveChanges();
    }

    public List<BackupSchedule> GetBackupSchedules()
    {
        var query = BackupContext.Schedules.AsQueryable().Join(BackupContext.Tenants,
            s => s.TenantId,
            t => t.Id,
            (s, t) => new { schedule = s, tenant = t })
            .Where(q => q.tenant.Status == TenantStatus.Active)
            .Select(q => q.schedule);

        return query.ToList();
    }

    public BackupSchedule GetBackupSchedule(int tenantId)
    {
        return BackupContext.Schedules.AsNoTracking().SingleOrDefault(s => s.TenantId == tenantId);
    }
}
