using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class DbContext : BaseDbContext
    {
        public DbSet<MobileAppInstall> MobileAppInstall { get; set; }
        public DbSet<DbipLocation> DbipLocation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddMobileAppInstall();
        }
    }

    public static class DbContextExtension
    {
        public static IServiceCollection AddDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<DbContext>();
        }
    }
}
