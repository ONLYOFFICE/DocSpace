using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

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
        public static DIHelper AddVoipDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<VoipDbContext>();
        }
    }
}
