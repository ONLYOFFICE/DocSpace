using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class VoipDbContext : BaseDbContext
    {
        public DbSet<VoipNumber> VoipNumbers { get; set; }
        public DbSet<DbVoipCall> VoipCalls { get; set; }
        public DbSet<CrmContact> CrmContact { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MySqlAddVoipNumber()
                .MySqlAddDbVoipCall()
                .MySqlAddCrmContact();

            modelBuilder.PgSqlAddVoipNumber()
                .PgSqlAddDbVoipCall()
                .PgSqlAddCrmContact();

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class VoipDbExtension
    {
        public static DIHelper AddVoipDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<VoipDbContext>();
        }
    }
}
