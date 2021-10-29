using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlTelegramDbContext : TelegramDbContext { }
    public class PostgreSqlTelegramDbContext : TelegramDbContext { }
    public class TelegramDbContext : BaseDbContext
    {
        public DbSet<TelegramUser> Users { get; set; }

        public TelegramDbContext() { }
        public TelegramDbContext(DbContextOptions<TelegramDbContext> options)
            : base(options)
        {
        }
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlTelegramDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlTelegramDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddTelegramUsers();
        }
    }

    public static class TelegramDbContextExtension
    {
        public static DIHelper AddTelegramDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<TelegramDbContext>();
        }
    }
}
