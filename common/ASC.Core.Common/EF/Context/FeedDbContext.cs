using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class FeedDbContext : BaseDbContext
    {
        public DbSet<FeedLast> FeedLast { get; set; }
        public DbSet<FeedAggregate> FeedAggregates { get; set; }
        public DbSet<FeedUsers> FeedUsers { get; set; }
        public DbSet<FeedReaded> FeedReaded { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddFeedUsers()
                .AddFeedReaded();
        }
    }

    public static class FeedDbExtension
    {
        public static IServiceCollection AddFeedDbService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<FeedDbContext>();
        }
    }
}
