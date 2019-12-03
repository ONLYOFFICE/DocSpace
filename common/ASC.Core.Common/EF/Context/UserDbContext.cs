using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAcl();

            modelBuilder.Entity<Subscription>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient, c.Object });

            modelBuilder.Entity<DbSubscriptionMethod>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient });

            modelBuilder.AddUser();
        }
    }

    public static class UserDbExtension
    {
        public static IServiceCollection AddUserDbContextService(this IServiceCollection services)
        {
            services.TryAddScoped<DbContextManager<UserDbContext>>();
            services.TryAddScoped<IConfigureOptions<UserDbContext>, ConfigureDbContext>();
            services.TryAddScoped<UserDbContext>();

            return services;
        }
    }
}
