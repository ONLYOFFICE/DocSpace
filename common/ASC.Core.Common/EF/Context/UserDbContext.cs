using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class UserDbContext : BaseDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<UserPhoto> Photos { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbGroup> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<DbSubscriptionMethod> SubscriptionMethods { get; set; }

        public UserDbContext() { }
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddSubscriptionMethod()
            .AddUser()
            .AddAcl()
            .AddUserSecurity()
            .AddUserPhoto()
            .AddDbGroup()
            .AddUserGroup()
            .AddSubscription()
            .Finish();
        }
    }

    public static class UserDbExtension
    {
        public static DIHelper AddUserDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<UserDbContext>();
        }
    }
}
