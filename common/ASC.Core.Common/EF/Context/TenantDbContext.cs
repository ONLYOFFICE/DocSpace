using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class TenantDbContext : BaseDbContext
    {
        public DbSet<DbTenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<DbTenantVersion> TenantVersion { get; set; }
        public DbSet<DbTenantPartner> TenantPartner { get; set; }
        public DbSet<DbTenantForbiden> TenantForbiden { get; set; }
        public DbSet<TenantIpRestrictions> TenantIpRestrictions { get; set; }
        public DbSet<DbCoreSettings> CoreSettings { get; set; }

        public TenantDbContext() { }
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddUser();

            modelBuilder.AddCoreSettings();

            modelBuilder.Entity<DbTenant>()
                .HasOne(r => r.Partner)
                .WithOne(r => r.Tenant)
                .HasForeignKey<DbTenantPartner>(r => new { r.TenantId })
                .HasPrincipalKey<DbTenant>(r => new { r.Id });
        }
    }

    public static class TenantDbExtension
    {
        public static IServiceCollection AddTenantDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<TenantDbContext>();
        }
    }
}
