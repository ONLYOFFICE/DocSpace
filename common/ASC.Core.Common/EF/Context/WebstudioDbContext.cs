using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class WebstudioDbContext : BaseDbContext
    {
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
        public DbSet<DbWebstudioUserVisit> WebstudioUserVisit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddWebstudioSettings()
                .AddWebstudioUserVisit();
        }
    }

    public static class WebstudioDbExtension
    {
        public static IServiceCollection AddWebstudioDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<WebstudioDbContext>();
        }
    }
}
