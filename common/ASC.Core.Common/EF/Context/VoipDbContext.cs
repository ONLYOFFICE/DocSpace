using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Core.Common.EF.Context
{
    public class VoipDbContext : BaseDbContext
    {
        public DbSet<VoipNumber> VoipNumbers { get; set; }
        public DbSet<DbVoipCall> VoipCalls { get; set; }
        public DbSet<CrmContact> CrmContact { get; set; }
    }

    public static class VoipDbExtension
    {
        public static IServiceCollection AddVoipDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<VoipDbContext>();
        }
    }
}
