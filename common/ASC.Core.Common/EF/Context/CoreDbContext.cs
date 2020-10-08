
using ASC.Common;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ASC.Core.Common.EF
{
    public class MySqlCoreDbContext : CoreDbContext { }
    public class PostgreSqlCoreDbContext : CoreDbContext { }
    public class CoreDbContext : BaseDbContext
    {
        public DbSet<DbTariff> Tariffs { get; set; }
        public DbSet<DbButton> Buttons { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbQuota> Quotas { get; set; }
        public DbSet<DbQuotaRow> QuotaRows { get; set; }
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlCoreDbContext() } ,
                    { Provider.Postgre, () => new PostgreSqlCoreDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                  .From(modelBuilder, Provider)
                  .AddAcl()
                  .AddDbButton()
                  .AddDbQuotaRow()
                  .AddDbQuota()
                  .AddDbTariff();
        }
    }


    public static class CoreDbExtension
    {
        public static DIHelper AddCoreDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<CoreDbContext>();
        }
    }
}
