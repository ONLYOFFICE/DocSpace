
using ASC.Common;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public partial class CoreDbContext : BaseDbContext
    {
        public DbSet<DbTariff> Tariffs { get; set; }
        public DbSet<DbButton> Buttons { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbQuota> Quotas { get; set; }
        public DbSet<DbQuotaRow> QuotaRows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .MySqlAddAcl()
                .MySqlAddDbButton()
                .MySqlAddDbQuotaRow()
                .MySqlAddDbQuota()
                .MySqlAddDbTariff();

            modelBuilder
                .PgSqlAddAcl()
                .PgSqlAddDbButton()
                .PgSqlAddDbQuotaRow()
                .PgSqlAddDbQuota()
                .PgSqlAddDbTariff();

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


    public static class CoreDbExtension
    {
        public static DIHelper AddCoreDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<CoreDbContext>();
        }
    }
}
