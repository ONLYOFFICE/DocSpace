using System.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.Core.Common.EF
{
    public class BaseDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BaseDbContext() { }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        internal ILoggerFactory LoggerFactory { get; set; }
        internal ConnectionStringSettings ConnectionStringSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseMySql(ConnectionStringSettings.ConnectionString);
        }
    }
}
