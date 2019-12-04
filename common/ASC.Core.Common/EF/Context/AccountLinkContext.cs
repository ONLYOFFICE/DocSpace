using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
            services.TryAddScoped<DbContextManager<AccountLinkContext>>();
            services.TryAddScoped<IConfigureOptions<AccountLinkContext>, ConfigureDbContext>();
            services.TryAddScoped<AccountLinkContext>();

            return services;
        }
    }
}
