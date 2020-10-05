using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class AccountLinkContext : BaseDbContext
    {
        public DbSet<AccountLinks> AccountLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
               .From(modelBuilder, Provider)
               .AddAccountLinks()
               .Finish();
        }
    }

    public static class AccountLinkContextExtension
    {
        public static DIHelper AddAccountLinkContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<AccountLinkContext>();
        }
    }
}
