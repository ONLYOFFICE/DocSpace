﻿namespace ASC.Core.Common.EF;

public class MySqlCoreDbContext : CoreDbContext { }
public class PostgreSqlCoreDbContext : CoreDbContext { }
public class CoreDbContext : BaseDbContext
{
    public DbSet<DbTariff> Tariffs { get; set; }
    public DbSet<DbButton> Buttons { get; set; }
    public DbSet<DbQuota> Quotas { get; set; }
    public DbSet<DbQuotaRow> QuotaRows { get; set; }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext =>
        new Dictionary<Provider, Func<BaseDbContext>>()
        {
                    { Provider.MySql, () => new MySqlCoreDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlCoreDbContext() } ,
        };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
              .From(modelBuilder, Provider)
              .AddDbButton()
              .AddDbQuotaRow()
              .AddDbQuota()
              .AddDbTariff();
    }
}
