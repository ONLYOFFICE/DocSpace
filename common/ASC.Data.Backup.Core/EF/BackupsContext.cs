﻿namespace ASC.Data.Backup.EF.Context;

public class BackupsContext : BaseDbContext
{
    public DbSet<BackupRecord> Backups { get; set; }
    public DbSet<BackupSchedule> Schedules { get; set; }
    public DbSet<DbTenant> Tenants { get; set; }

    public BackupsContext() { }

    public BackupsContext(DbContextOptions<BackupsContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddDbTenant();
    }
}

public static class BackupsContextExtension
{
    public static DIHelper AddBackupsContext(this DIHelper services)
    {
        return services.AddDbContextManagerService<BackupsContext>();
    }
}
