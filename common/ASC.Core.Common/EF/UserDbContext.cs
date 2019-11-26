using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Logging;
using ASC.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class UserDbContextManager : OptionsManager<UserDbContext>, IDisposable
    {
        private Dictionary<string, UserDbContext> Pairs { get; set; }
        private List<UserDbContext> AsyncList { get; set; }

        public IOptionsFactory<UserDbContext> Factory { get; }

        public UserDbContextManager(IOptionsFactory<UserDbContext> factory) : base(factory)
        {
            Pairs = new Dictionary<string, UserDbContext>();
            AsyncList = new List<UserDbContext>();
            Factory = factory;
        }

        public override UserDbContext Get(string name)
        {
            var result = base.Get(name);

            if (!Pairs.ContainsKey(name))
            {
                Pairs.Add(name, result);
            }

            return result;
        }

        public UserDbContext GetNew(string name = "default")
        {
            var result = Factory.Create(name);

            AsyncList.Add(result);

            return result;
        }

        public void Dispose()
        {
            foreach (var v in Pairs)
            {
                v.Value.Dispose();
            }
        }
    }

    public class ConfigureDbContext : IConfigureNamedOptions<BaseDbContext>
    {
        const string baseName = "default";
        public EFLoggerFactory LoggerFactory { get; }
        public IConfiguration Configuration { get; }

        public ConfigureDbContext(EFLoggerFactory loggerFactory, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public void Configure(string name, BaseDbContext context)
        {
            context.LoggerFactory = LoggerFactory;
            context.ConnectionStringSettings = Configuration.GetConnectionStrings(name) ?? Configuration.GetConnectionStrings(baseName);
        }

        public void Configure(BaseDbContext context)
        {
            Configure(baseName, context);
        }
    }

    public class BaseDbContext : DbContext
    {
        internal ILoggerFactory LoggerFactory { get; set; }
        internal ConnectionStringSettings ConnectionStringSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseMySql(ConnectionStringSettings.ConnectionString);
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
