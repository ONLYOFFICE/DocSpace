using ASC.Common;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public partial class UserDbContext : BaseDbContext
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
        {
            Database.EnsureCreated();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (baseName == "default")
            {
                modelBuilder
                .MySqlAddAcl()
                .MySqlAddSubscription()
                .MySqlAddSubscriptionMethod()
                .MySqlAddUser();
                modelBuilder.MySqlAddDbGroup();
                modelBuilder.MySqlAddUserSecurity();
                modelBuilder.MySqlAddUserGroup();
                modelBuilder.MySqlAddUserPhoto();
            }
            else
            {
                modelBuilder
                  .PgSqlAddAcl()
                  .PgSqlAddSubscription()
                  .PgSqlAddSubscriptionMethod()
                  .PgSqlAddUser();
                modelBuilder.PgSqlAddDbGroup();
                modelBuilder.PgSqlAddUserSecurity();
                modelBuilder.PgSqlAddUserGroup();
                modelBuilder.PgSqlAddUserPhoto();
            }
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class UserDbExtension
    {
        public static DIHelper AddUserDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<UserDbContext>();
        }
    }
}
