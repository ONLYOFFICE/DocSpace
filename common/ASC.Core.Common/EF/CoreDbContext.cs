using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class CoreDbContext : BaseDbContext
    {
        public DbSet<EFTariff> Tariffs { get; set; }
        public DbSet<EFButton> Buttons { get; set; }
        public DbSet<Acl> Acl { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAcl();

            modelBuilder.Entity<EFButton>()
                .HasKey(c => new { c.TariffId, c.PartnerId });
        }
    }

    public static class CoreDbExtension
    {
        public static IServiceCollection AddCoreDbContextService(this IServiceCollection services)
        {
            services.TryAddScoped<DbContextManager<CoreDbContext>>();
            services.TryAddScoped<IConfigureOptions<CoreDbContext>, ConfigureDbContext>();
            services.TryAddScoped<CoreDbContext>();

            return services;
        }
    }
}
