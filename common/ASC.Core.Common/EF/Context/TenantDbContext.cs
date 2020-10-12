using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlTenantDbContext : TenantDbContext { }
    public class PostgreSqlTenantDbContext : TenantDbContext { }
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
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlTenantDbContext() } ,
                    { Provider.Postgre, () => new PostgreSqlTenantDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddUser()
                .AddDbTenant()
                .AddCoreSettings()
                .AddUserSecurity()
                .AddDbTenantForbiden()
                .AddTenantIpRestrictions()
                .AddDbTenantPartner()
                .AddDbTenantVersion();
        }
    }

    public static class TenantDbExtension
    {
        public static DIHelper AddTenantDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<TenantDbContext>();
        }
    }
}
