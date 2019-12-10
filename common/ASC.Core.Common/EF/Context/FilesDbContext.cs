using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<FilesConverts> FilesConverts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddFilesConverts();
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
