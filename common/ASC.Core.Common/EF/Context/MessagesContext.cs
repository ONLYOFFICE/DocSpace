namespace ASC.Core.Common.EF.Context
{
    public class MySqlMessagesContext : MessagesContext { }
    public class PostgreSqlMessagesContext : MessagesContext { }
    public class MessagesContext : BaseDbContext
    {
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<LoginEvent> LoginEvents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlMessagesContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlMessagesContext() } ,
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddAuditEvent()
                .AddLoginEvents()
                .AddUser()
                .AddDbFunction();
        }
    }

    public static class MessagesContextExtension
    {
        public static DIHelper AddMessagesContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<MessagesContext>();
        }
    }
}
