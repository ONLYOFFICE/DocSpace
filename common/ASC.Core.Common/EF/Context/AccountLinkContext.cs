using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class AccountLinkContext : BaseDbContext
    {
        public DbSet<AccountLinks> AccountLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAccountLinks();
        }
    }

    public static class AccountLinkContextExtension
    {
        public static IServiceCollection AddAccountLinkContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<AccountLinkContext>();
        }
    }
}
