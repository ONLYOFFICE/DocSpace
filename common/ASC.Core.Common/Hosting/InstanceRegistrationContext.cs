using System;
using System.Collections.Generic;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.Core.Common.Hosting.Extensions;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.Hosting;

public class MySqlIntegrationEventLogContext : InstanceRegistrationContext { }
public class PostgreSqlIntegrationEventLogContext : InstanceRegistrationContext { }

public class InstanceRegistrationContext : BaseDbContext
{
    public DbSet<InstanceRegistration> InstanceRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper.From(modelBuilder, Provider)
                           .AddInstanceRegistration();

    }

    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlIntegrationEventLogContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlIntegrationEventLogContext() } ,
                };
        }
    }
}
