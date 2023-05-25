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
    private readonly IDbContextFactory<BackupsContext> _dbContextFactory;
    private readonly CreatorDbContext _creatorDbContext;

    public BackupRepository(IDbContextFactory<BackupsContext> dbContextFactory, CreatorDbContext creatorDbContext)
    {
        _dbContextFactory = dbContextFactory;
        _creatorDbContext = creatorDbContext;
    }

    public async Task SaveBackupRecordAsync(BackupRecord backup)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        await backupContext.AddOrUpdateAsync(b => b.Backups, backup);
        await backupContext.SaveChangesAsync();
    }

    public async Task<BackupRecord> GetBackupRecordAsync(Guid id)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.FindBackupAsync(backupContext, id);
    }

    public async Task<BackupRecord> GetBackupRecordAsync(string hash, int tenant)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetBackupAsync(backupContext, tenant, hash);
    }

    public async Task<List<BackupRecord>> GetExpiredBackupRecordsAsync()
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetExpiredBackupsAsync(backupContext).ToListAsync();
    }

    public async Task<List<BackupRecord>> GetScheduledBackupRecordsAsync()
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetScheduledBackupsAsync(backupContext).ToListAsync();
    }

    public async Task<List<BackupRecord>> GetBackupRecordsByTenantIdAsync(int tenantId)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetBackupsAsync(backupContext, tenantId).ToListAsync();
    }

    public async Task MigrationBackupRecordsAsync(int tenantId, int newTenantId, string region)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();

        var backups = await Queries.GetBackupsForMigrationAsync(backupContext, tenantId).ToListAsync();

        backups.ForEach(backup =>
        {
            backup.TenantId = newTenantId;
            backup.Id = Guid.NewGuid();
        });

        var backupContextByNewTenant = _creatorDbContext.CreateDbContext<BackupsContext>(region);
        await backupContextByNewTenant.Backups.AddRangeAsync(backups);
        await backupContextByNewTenant.SaveChangesAsync();
    }

    public async Task DeleteBackupRecordAsync(Guid id)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();

        var backup = await Queries.FindBackupAsync(backupContext ,id);

        if (backup != null)
        {
            backup.Removed = true;
            await backupContext.SaveChangesAsync();
        }
    }

    public async Task SaveBackupScheduleAsync(BackupSchedule schedule)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        await backupContext.AddOrUpdateAsync(q => q.Schedules, schedule);
        await backupContext.SaveChangesAsync();
    }

    public async Task DeleteBackupScheduleAsync(int tenantId)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        await Queries.DeleteSchedulesAsync(backupContext, tenantId);
    }

    public async Task<List<BackupSchedule>> GetBackupSchedulesAsync()
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetBackupSchedulesAsync(backupContext).ToListAsync();
    }

    public async Task<BackupSchedule> GetBackupScheduleAsync(int tenantId)
    {
        using var backupContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetBackupScheduleAsync(backupContext, tenantId);
    }
}

file static class Queries
{
    public static readonly Func<BackupsContext, Guid, Task<BackupRecord>> FindBackupAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, Guid id) =>
        ctx.Backups.Find(id));

    public static readonly Func<BackupsContext, int, string, Task<BackupRecord>> GetBackupAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, int tenantId, string hash) =>
        ctx.Backups
            .AsNoTracking()
            .SingleOrDefault(b => b.Hash == hash && b.TenantId == tenantId));
    
    public static readonly Func<BackupsContext, IAsyncEnumerable<BackupRecord>> GetExpiredBackupsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx) =>
        ctx.Backups
            .AsNoTracking()
            .Where(b => b.ExpiresOn != DateTime.MinValue 
                && b.ExpiresOn <= DateTime.UtcNow 
                && b.Removed == false));
    
    public static readonly Func<BackupsContext, IAsyncEnumerable<BackupRecord>> GetScheduledBackupsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx) =>
        ctx.Backups
            .AsNoTracking()
            .Where(b => b.IsScheduled == true && b.Removed == false));
    
    public static readonly Func<BackupsContext, int, IAsyncEnumerable<BackupRecord>> GetBackupsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, int tenantId) =>
        ctx.Backups
            .AsNoTracking()
            .Where(b => b.TenantId == tenantId && b.Removed == false));
    
    public static readonly Func<BackupsContext, int, IAsyncEnumerable<BackupRecord>> GetBackupsForMigrationAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, int tenantId) =>
        ctx.Backups
            .AsNoTracking()
            .Where(b => b.TenantId == tenantId)); 
    
    public static readonly Func<BackupsContext, int, Task<int>> DeleteSchedulesAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, int tenantId) =>
        ctx.Schedules
            .Where(s => s.TenantId == tenantId)
            .ExecuteDelete());
    
    public static readonly Func<BackupsContext, IAsyncEnumerable<BackupSchedule>> GetBackupSchedulesAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx) =>
        ctx.Schedules.Join(ctx.Tenants,
                s => s.TenantId,
                t => t.Id,
                (s, t) => new { schedule = s, tenant = t })
            .Where(q => q.tenant.Status == TenantStatus.Active)
            .Select(q => q.schedule));
    
    public static readonly Func<BackupsContext, int, Task<BackupSchedule>> GetBackupScheduleAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (BackupsContext ctx, int tenantId) =>
        ctx.Schedules
            .AsNoTracking()
            .SingleOrDefault(s => s.TenantId == tenantId));
}