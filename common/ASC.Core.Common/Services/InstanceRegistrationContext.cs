using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Common.Services.Extensions;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.Services;

public class MySqlIntegrationEventLogContext : InstanceRegistrationContext { }
public class PostgreSqlIntegrationEventLogContext : InstanceRegistrationContext { }

public class InstanceRegistrationContext : BaseDbContext
{
    public DbSet<InstanceRegistrationEntry> InstanceRegistrations { get; set; }

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
