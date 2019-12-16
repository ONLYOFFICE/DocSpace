using System;
using System.Linq;

using ASC.Common.Logging;
using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class ConfigureDbContext : IConfigureNamedOptions<BaseDbContext>
    {
        const string baseName = "default";
        public EFLoggerFactory LoggerFactory { get; }
        public IConfiguration Configuration { get; }

        public ConfigureDbContext(EFLoggerFactory loggerFactory, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public void Configure(string name, BaseDbContext context)
        {
            context.LoggerFactory = LoggerFactory;
            context.ConnectionStringSettings = Configuration.GetConnectionStrings(name) ?? Configuration.GetConnectionStrings(baseName);
        }

        public void Configure(BaseDbContext context)
        {
            Configure(baseName, context);
        }
    }

    public class ConfigureMultiRegionalDbContext<T> : IConfigureNamedOptions<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
    {
        const string baseName = "default";
        public IConfiguration Configuration { get; }
        public DbContextManager<T> DbContext { get; }

        public ConfigureMultiRegionalDbContext(IConfiguration configuration, DbContextManager<T> dbContext)
        {
            Configuration = configuration;
            DbContext = dbContext;
        }

        public void Configure(string name, MultiRegionalDbContext<T> context)
        {
            context.Context = new System.Collections.Generic.List<T>();

            const StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

            foreach (var c in Configuration.GetConnectionStrings().Where(r =>
            r.Name.Equals(name, cmp) || r.Name.StartsWith(name + ".", cmp) ||
            r.Name.Equals(baseName, cmp) || r.Name.StartsWith(baseName + ".", cmp)
            ))
            {
                context.Context.Add(DbContext.Get(c.Name));
            }
        }

        public void Configure(MultiRegionalDbContext<T> context)
        {
            Configure(baseName, context);
        }
    }
}
