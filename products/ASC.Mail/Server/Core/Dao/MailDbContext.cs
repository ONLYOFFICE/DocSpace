using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Mail.Core.Dao
{
    public class MailDbContext : BaseDbContext
    {
        public DbSet<Alert> Alert { get; set; }

        public DbSet<MailboxServer> MailboxServer { get; set; }
        public DbSet<MailboxProvider> MailboxProvider { get; set; }
        public DbSet<Mailbox> Mailbox { get; set; }

        public MailDbContext() { }
        public MailDbContext(DbContextOptions<MailDbContext> options)
            : base(options)
        {
        }
    }
    public static class MailDbExtension
    {
        public static IServiceCollection AddMailDbContextService(this IServiceCollection services)
        {
            return services.AddDbContextManagerService<MailDbContext>();
        }
    }
}
