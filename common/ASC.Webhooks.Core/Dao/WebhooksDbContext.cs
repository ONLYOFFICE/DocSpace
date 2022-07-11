using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Webhooks.Core.Dao.Models;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ASC.Webhooks.Core.Dao
{
    public class MySqlWebhooksDbContext : WebhooksDbContext { }
    public class PostgreSqlWebhooksDbContext : WebhooksDbContext { }
    public partial class WebhooksDbContext : BaseDbContext
    {
        public virtual DbSet<WebhooksConfig> WebhooksConfigs { get; set; }
        public virtual DbSet<WebhooksLog> WebhooksLogs { get; set; }

        public WebhooksDbContext() { }
        public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options)
            : base(options)
        {

        }
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlWebhooksDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlWebhooksDbContext() } ,
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddWebhooksConfig()
            .AddWebhooksLog();
        }
    }

    public static class WebhooksDbExtension
    {
        public static DIHelper AddWebhooksDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<TenantDbContext>();
        }
    }
}
