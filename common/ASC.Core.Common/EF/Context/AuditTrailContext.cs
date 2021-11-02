
using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlAuditTrailContext : AuditTrailContext { }
    public class PostgreSqlAuditTrailContext : AuditTrailContext { }
    public class AuditTrailContext : BaseDbContext
    {
        public DbSet<AuditEvent> AuditEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddAuditEvent()
            .AddDbFunction();
        }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlAuditTrailContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlAuditTrailContext() } ,
                };
            }
        }
    }

    public static class AuditTrailContextExtension
    {
        public static DIHelper AddAuditTrailContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<AuditTrailContext>();
        }
    }
}
