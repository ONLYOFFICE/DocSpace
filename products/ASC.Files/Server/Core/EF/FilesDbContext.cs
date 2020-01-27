using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.EF
{
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<DbFile> Files { get; set; }
        public DbSet<DbFolderTree> FolderTree { get; set; }
        public DbSet<DbFilesBunchObjects> FilesBunchObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddDbFiles()
                .AddDbFolderTree()
                .AddDbFilesBunchObjects();
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
