using ASC.Common.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class ConfigureDbContext : IConfigureNamedOptions<BaseDbContext>
    {
        public void Configure(string name, BaseDbContext context)
        {
            var factory = new ConsoleLoggerFactory();
            factory.AddProvider(new ConsoleLoggerProvider());
            context.LoggerFactory = factory;
        }

        public void Configure(BaseDbContext context)
        {
            Configure("default", context);
        }
    }

    public class BaseDbContext : DbContext
    {
        internal ILoggerFactory LoggerFactory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory);
            optionsBuilder.UseMySql("Server=localhost;Database=onlyoffice;User ID=dev;Password=dev;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none");
        }
    }

    public class UserDbContext : BaseDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<UserPhoto> Photos { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbGroup> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionMethod> SubscriptionMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>()
                .HasKey(c => new { c.Tenant, c.Subject, c.Action, c.Object });

            modelBuilder.Entity<Subscription>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient, c.Object });

            modelBuilder.Entity<SubscriptionMethod>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient });

            modelBuilder.Entity<UserGroup>()
                .HasKey(c => new { c.Tenant, c.UserId, c.GroupId, c.RefType });
        }
    }
}
