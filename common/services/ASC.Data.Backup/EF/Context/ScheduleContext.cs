using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Data.Backup.EF.Model;
using Microsoft.EntityFrameworkCore;


namespace ASC.Data.Backup.EF.Context
{
    public class ScheduleContext : BaseDbContext
    {
        public DbSet<Schedule> Schedules { get; set; }
    }

    public static class ScheduleContextExtension
    {
        public static DIHelper AddSheduleContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<ScheduleContext>();
        }
    }
}
