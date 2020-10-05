using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<FilesConverts> FilesConverts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddFilesConverts();
        }
    }

    public static class FilesDbExtension
    {
        public static DIHelper AddFilesDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<FilesDbContext>();
        }
    }
}
