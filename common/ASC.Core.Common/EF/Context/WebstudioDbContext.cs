using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class WebstudioDbContext : BaseDbContext
    {
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
        public DbSet<DbWebstudioUserVisit> WebstudioUserVisit { get; set; }
        public DbSet<DbWebstudioIndex> WebstudioIndex { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder
                .AddWebstudioSettings()
                .AddWebstudioUserVisit();
        }
    }

    public static class WebstudioDbExtension
    {
        public static DIHelper AddWebstudioDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<WebstudioDbContext>();
        }
    }
}
