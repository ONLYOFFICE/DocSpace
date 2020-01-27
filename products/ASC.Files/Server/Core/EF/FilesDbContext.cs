using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.EF
{
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<DbFile> Files { get; set; }
        public DbSet<DbFolder> Folders { get; set; }
        public DbSet<DbFolderTree> FolderTree { get; set; }
        public DbSet<DbFilesBunchObjects> FilesBunchObjects { get; set; }
        public DbSet<DbFilesSecurity> FilesSecurity { get; set; }
        public DbSet<DbFilesThirdpartyIdMapping> FilesThirdpartyIdMapping { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddDbFiles()
                .AddDbFolderTree()
                .AddDbFilesBunchObjects()
                .AddDbFilesSecurity()
                .AddDbFilesThirdpartyIdMapping();
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
