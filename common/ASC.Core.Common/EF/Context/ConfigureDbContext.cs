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
}
