
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class MessagesContext : BaseDbContext
    {
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<LoginEvents> LoginEvents { get; set; }
        public DbSet<DbTenant> Tenants { get; set; }
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddDbFunction();
            modelBuilder.AddWebstudioSettings();
        }
    }

    public static class MessagesContextExtension
    {
        public static IServiceCollection AddMessagesContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<MessagesContext>();
        }
    }
}
