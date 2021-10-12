using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlFeedDbContext : FeedDbContext { }
    public class PostgreSqlFeedDbContext : FeedDbContext { }
    public class FeedDbContext : BaseDbContext
    {
        public DbSet<FeedLast> FeedLast { get; set; }
        public DbSet<FeedAggregate> FeedAggregates { get; set; }
        public DbSet<FeedUsers> FeedUsers { get; set; }
        public DbSet<FeedReaded> FeedReaded { get; set; }
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlFeedDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlFeedDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddFeedUsers()
                .AddFeedReaded()
                .AddFeedAggregate()
                .AddFeedLast();
        }
    }

    public static class FeedDbExtension
    {
        public static DIHelper AddFeedDbService(this DIHelper services)
        {
            return services.AddDbContextManagerService<FeedDbContext>();
        }
    }
}
