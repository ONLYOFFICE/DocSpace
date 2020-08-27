using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class FilesDbContext : BaseDbContext
    {
        public DbSet<FilesConverts> FilesConverts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (baseName == "default")
            {
                modelBuilder.MySqlAddFilesConverts();
            }
            else
            {
                modelBuilder.PgSqlAddFilesConverts();
            }

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class FilesDbExtension
    {
        public static DIHelper AddFilesDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<FilesDbContext>();
        }
    }
}
