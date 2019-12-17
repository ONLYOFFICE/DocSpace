using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
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

    public static class BaseDbContextExtension
    {
        public static void AddOrUpdate<T, TContext>(this TContext b, Expression<Func<TContext, DbSet<T>>> expressionDbSet, T entity) where T : BaseEntity where TContext : BaseDbContext
        {
            var dbSet = expressionDbSet.Compile().Invoke(b);
            var existingBlog = dbSet.Find(entity.GetKeys());
            if (existingBlog == null)
            {
                dbSet.Add(entity);
            }
            else
            {
                b.Entry(existingBlog).CurrentValues.SetValues(entity);
            }
        }
    }

    public abstract class BaseEntity
    {
        internal abstract object[] GetKeys();
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
