using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class DbContext : BaseDbContext
    {
        public DbSet<MobileAppInstall> MobileAppInstall { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddMobileAppInstall();
        }
    }
}
