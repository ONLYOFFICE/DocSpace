using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlMessagesContext : MessagesContext { }
    public class PostgreSqlMessagesContext : MessagesContext { }
    public class MessagesContext : BaseDbContext
    {
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<LoginEvents> LoginEvents { get; set; }
        public DbSet<DbTenant> Tenants { get; set; }
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlMessagesContext() } ,
                    { Provider.Postgre, () => new PostgreSqlMessagesContext() } ,
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddDbTenant()
                .AddWebstudioSettings()
                .AddAuditEvent()
                .AddLoginEvents()
                .AddDbFunction();
        }
    }

    public static class MessagesContextExtension
    {
        public static DIHelper AddMessagesContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<MessagesContext>();
        }
    }
}
