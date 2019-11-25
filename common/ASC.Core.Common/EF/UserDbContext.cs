using ASC.Common.Logging;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<UserPhoto> Photos { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbGroup> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionMethod> SubscriptionMethods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var factory = new ConsoleLoggerFactory();
            factory.AddProvider(new ConsoleLoggerProvider());
            optionsBuilder.UseLoggerFactory(factory);

            optionsBuilder.UseMySql("Server=localhost;Database=onlyoffice;User ID=dev;Password=dev;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(c => new { c.Tenant, c.UserId, c.GroupId, c.RefType });
        }
    }
}
