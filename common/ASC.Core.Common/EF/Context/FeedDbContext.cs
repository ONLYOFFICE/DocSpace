using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class FeedDbContext : BaseDbContext
    {
        public DbSet<FeedLast> FeedLast { get; set; }
        public DbSet<FeedAggregate> FeedAggregates { get; set; }
        public DbSet<FeedUsers> FeedUsers { get; set; }
        public DbSet<FeedReaded> FeedReaded { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
                modelBuilder
                .MySqlAddFeedUsers()
                .MySqlAddFeedReaded()
                .MySqlAddFeedAggregate()
                .MySqlAddFeedLast();
            
           */
                modelBuilder
                  .PgSqlAddFeedUsers()
                  .PgSqlAddFeedReaded()
                  .PgSqlAddFeedAggregate()
                  .PgSqlAddFeedLast();
            
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class FeedDbExtension
    {
        public static DIHelper AddFeedDbService(this DIHelper services)
        {
            return services.AddDbContextManagerService<FeedDbContext>();
        }
    }
}
