using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
            services.TryAddScoped<DbContextManager<WebstudioDbContext>>();
            services.TryAddScoped<IConfigureOptions<WebstudioDbContext>, ConfigureDbContext>();
            services.TryAddScoped<WebstudioDbContext>();

            return services;
        }
    }
}
