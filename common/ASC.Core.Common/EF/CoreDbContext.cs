using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class CoreDbContext : BaseDbContext
    {
        public DbSet<EFTariff> Tariffs { get; set; }
        public DbSet<EFButton> Buttons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFButton>()
                .HasKey(c => new { c.TariffId, c.PartnerId });
        }
    }
}
