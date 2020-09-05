using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class TenantDbContext : BaseDbContext
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
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
                modelBuilder.MySqlAddUser();
                modelBuilder.MySqlAddCoreSettings();
                modelBuilder.MySqlAddDbTenant();
                modelBuilder.MySqlAddUserSecurity();
                modelBuilder.MySqlAddDbTenantForbiden();
                modelBuilder.MySqlAddTenantIpRestrictions();
                modelBuilder.MySqlAddDbTenantPartner();
                modelBuilder.MySqlAddDbTenantVersion();
            */
                modelBuilder.PgSqlAddUser();
                modelBuilder.PgSqlAddCoreSettings();
                modelBuilder.PgSqlAddDbTenant();
                modelBuilder.PgSqlAddUserSecurity();
                modelBuilder.PgSqlAddDbTenantForbiden();
                modelBuilder.PgSqlAddTenantIpRestrictions();
                modelBuilder.PgSqlAddDbTenantPartner();
                modelBuilder.PgSqlAddDbTenantVersion();
            
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class TenantDbExtension
    {
        public static DIHelper AddTenantDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<TenantDbContext>();
        }
    }
}
