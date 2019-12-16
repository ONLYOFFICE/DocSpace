using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.Core.Common.EF
{
    public class BaseDbContext : DbContext
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

    public class MultiRegionalDbContext<T> : IDisposable, IAsyncDisposable where T : BaseDbContext, new()
    {
        public MultiRegionalDbContext() { }

        internal List<T> Context { get; set; }

        public void Dispose()
        {
            if (Context == null) return;

            foreach (var c in Context)
            {
                if (c != null)
                {
                    c.Dispose();
                }
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (Context == null) return;

            foreach (var c in Context)
            {
                if (c != null)
                {
                    await c.DisposeAsync();
                }
            }
        }
    }
}
