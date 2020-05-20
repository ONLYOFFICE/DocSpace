using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Data.Backup.EF.Model;
using Microsoft.EntityFrameworkCore;


namespace ASC.Data.Backup.EF.Context
{
    public class BackupRecordContext : BaseDbContext
    {
        public DbSet<BackupRecord> Backups { get; set; }
    }

    public static class BackupRecordContextExtension
    {
        public static DIHelper AddBackupContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<BackupRecordContext>();
        }    
    }
}
