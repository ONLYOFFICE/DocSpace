using System;
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;

using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    [Scope]
    public class ConfigureDbContext<T> : IConfigureNamedOptions<T> where T : BaseDbContext, new()
    {
        public const string baseName = "default";
        private EFLoggerFactory LoggerFactory { get; }
        private ConfigurationExtension Configuration { get; }
        private string MigrateAssembly { get; }

        public ConfigureDbContext(EFLoggerFactory loggerFactory, ConfigurationExtension configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
            MigrateAssembly = Configuration["testAssembly"];
        }

        public void Configure(string name, T context)
        {
            context.LoggerFactory = LoggerFactory;
            context.ConnectionStringSettings = Configuration.GetConnectionStrings(name) ?? Configuration.GetConnectionStrings(baseName);
            context.MigrateAssembly = MigrateAssembly;
        }

        public void Configure(T context)
        {
            Configure(baseName, context);
        }
    }

    public class ConfigureMultiRegionalDbContext<T> : IConfigureNamedOptions<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
    {
        private readonly string _baseName = "default";
        private ConfigurationExtension Configuration { get; }
        private DbContextManager<T> DbContext { get; }

        public ConfigureMultiRegionalDbContext(ConfigurationExtension configuration, DbContextManager<T> dbContext)
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
            r.Name.Equals(_baseName, cmp) || r.Name.StartsWith(_baseName + ".", cmp)
            ))
            {
                context.Context.Add(DbContext.Get(c.Name));
            }
        }

        public void Configure(MultiRegionalDbContext<T> context)
        {
            Configure(_baseName, context);
        }
    }
}
