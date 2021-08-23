﻿using ASC.Core.Common.EF;
using ASC.Webhooks.Dao.Models;

using Microsoft.EntityFrameworkCore;
using ASC.Core.Common.EF.Model;
using System.Collections.Generic;
using System;
using ASC.Core.Common.EF.Context;
using ASC.Common;

#nullable disable

namespace ASC.Webhooks.Dao
{
    public class MySqlWebhooksDbContext : WebhooksDbContext { }
    public class PostgreSqlWebhooksDbContext : WebhooksDbContext { }
    public partial class WebhooksDbContext : BaseDbContext
    {
        public virtual DbSet<WebhooksConfig> WebhooksConfigs { get; set; }
        public virtual DbSet<WebhooksPayload> WebhooksPayloads { get; set; }

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
                     { Provider.Postgre, () => new PostgreSqlWebhooksDbContext() } ,
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddWebhooksConfig()
            .AddWebhooksPayload();
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
