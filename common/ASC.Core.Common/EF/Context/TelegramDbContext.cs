
using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class TelegramDbContext : BaseDbContext
    {
        public DbSet<TelegramUser> Users { get; set; }

        public TelegramDbContext() { }
        public TelegramDbContext(DbContextOptions<TelegramDbContext> options)
            : base(options)
        {
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
