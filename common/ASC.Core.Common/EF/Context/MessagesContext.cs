
using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class MessagesContext : BaseDbContext
    {
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<LoginEvents> LoginEvents { get; set; }
        public DbSet<DbTenant> Tenants { get; set; }
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddDbFunction();
            modelBuilder.MySqlAddWebstudioSettings()
                .MySqlAddAuditEvent()
                .MySqlAddLoginEvents()
                .MySqlAddDbTenant();

            modelBuilder.PgSqlAddWebstudioSettings()
                .PgSqlAddAuditEvent()
                .PgSqlAddLoginEvents()
                .PgSqlAddDbTenant();

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class MessagesContextExtension
    {
        public static DIHelper AddMessagesContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<MessagesContext>();
        }
    }
}
