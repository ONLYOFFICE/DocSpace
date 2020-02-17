using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class NotifyDbContext : BaseDbContext
    {
        public DbSet<NotifyInfo> NotifyInfo { get; set; }
        public DbSet<NotifyQueue> NotifyQueue { get; set; }
    }

    public static class NotifyDbExtension
    {
        public static DIHelper AddNotifyDbContext(this DIHelper services)
        {
            return services.AddDbContextManagerService<NotifyDbContext>();
        }
    }
}
