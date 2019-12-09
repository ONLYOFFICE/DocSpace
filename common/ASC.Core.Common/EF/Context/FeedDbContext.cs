using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF.Context
{
    public class FeedDbContext : BaseDbContext
    {
        public DbSet<FeedLast> FeedLast { get; set; }
        public DbSet<FeedAggregate> FeedAggregates { get; set; }
        public DbSet<FeedUsers> FeedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddFeedUsers();
        }
    }

    public static class FeedDbExtension
    {
        public static IServiceCollection AddFeedDbService(this IServiceCollection services)
        {
            services.TryAddScoped<DbContextManager<FeedDbContext>>();
            services.TryAddScoped<IConfigureOptions<FeedDbContext>, ConfigureDbContext>();
            services.TryAddScoped<FeedDbContext>();

            return services;
        }
    }
}
