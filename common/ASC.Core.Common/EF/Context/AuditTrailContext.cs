
using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class AuditTrailContext : BaseDbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<LoginEvents> LoginEvents { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddUser()
            .AddLoginEvents()
            .AddAuditEvent()
            .AddDbFunction();
        }
    }

    public static class AuditTrailContextExtension
    {
        public static DIHelper AddAuditTrailContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<AuditTrailContext>();
        }
    }
}
