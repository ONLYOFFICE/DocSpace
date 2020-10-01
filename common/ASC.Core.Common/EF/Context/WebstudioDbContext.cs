using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class WebstudioDbContext : BaseDbContext
    {
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
        public DbSet<DbWebstudioUserVisit> WebstudioUserVisit { get; set; }
        public DbSet<DbWebstudioIndex> WebstudioIndex { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddWebstudioSettings()
                .AddWebstudioUserVisit()
                .AddDbWebstudioIndex()
                .Finish();
            modelBuilder.WebstudioUserVisitData();
            modelBuilder.WebstudioSettingsData();
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class WebstudioDbExtension
    {
        public static DIHelper AddWebstudioDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<WebstudioDbContext>();
        }
    }
}
