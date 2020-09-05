using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class AccountLinkContext : BaseDbContext
    {
        public DbSet<AccountLinks> AccountLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
               // modelBuilder.MySqlAddAccountLinks();
            modelBuilder.PgSqlAddAccountLinks();

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class AccountLinkContextExtension
    {
        public static DIHelper AddAccountLinkContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<AccountLinkContext>();
        }
    }
}
