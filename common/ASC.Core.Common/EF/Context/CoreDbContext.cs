
using ASC.Common;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class CoreDbContext : BaseDbContext
    {
        public DbSet<DbTariff> Tariffs { get; set; }
        public DbSet<DbButton> Buttons { get; set; }
        public DbSet<Acl> Acl { get; set; }
        public DbSet<DbQuota> Quotas { get; set; }
        public DbSet<DbQuotaRow> QuotaRows { get; set; }

        public CoreDbContext() { }
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder
                .AddAcl()
                .AddDbButton()
                .AddDbQuotaRow();
        }
    }


    public static class CoreDbExtension
    {
        public static DIHelper AddCoreDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<CoreDbContext>();
        }
    }
}
