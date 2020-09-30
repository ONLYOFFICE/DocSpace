using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class DbContext : BaseDbContext
    {
        public DbSet<MobileAppInstall> MobileAppInstall { get; set; }
        public DbSet<DbipLocation> DbipLocation { get; set; }
        public DbSet<Regions> Regions { get; set; }

        public DbContext()
        {
        }

        public DbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.AddMobileAppInstall();
        }
    }

    public static class DbContextExtension
    {
        public static DIHelper AddDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<DbContext>();
        }
    }
}
