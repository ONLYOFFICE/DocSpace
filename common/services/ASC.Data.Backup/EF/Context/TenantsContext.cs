

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Data.Backup.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Data.Backup.EF.Context
{
    public class TenantsContext : BaseDbContext
    {
        public DbSet<Tenants> tenants { get; set; }
    }
    public static class TenantsContextExtension
    {
        public static DIHelper AddSheduleContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<TenantsContext>();
        }
    }
}
