using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.EF
{
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<DbFile> Files { get; set; }
        public DbSet<DbFolder> Folders { get; set; }
        public DbSet<DbFolderTree> Tree { get; set; }
        public DbSet<DbFilesBunchObjects> BunchObjects { get; set; }
        public DbSet<DbFilesSecurity> Security { get; set; }
        public DbSet<DbFilesThirdpartyIdMapping> ThirdpartyIdMapping { get; set; }
        public DbSet<DbFilesThirdpartyAccount> ThirdpartyAccount { get; set; }
        public DbSet<DbFilesTagLink> TagLink { get; set; }
        public DbSet<DbFilesTag> Tag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddDbFiles()
                .AddDbFolderTree()
                .AddDbFilesBunchObjects()
                .AddDbFilesSecurity()
                .AddDbFilesThirdpartyIdMapping()
                .AddDbFilesTagLink();
        }
    }

    public static class FilesDbExtension
    {
        public static IServiceCollection AddFilesDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<FilesDbContext>();
        }
    }
}
