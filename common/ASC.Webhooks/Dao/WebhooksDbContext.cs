using ASC.Core.Common.EF;
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

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //                optionsBuilder.UseMySQL("server=localhost;port=3306;database=onlyoffice;uid=root;password=onlyoffice");
        //            }
        //        }

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
