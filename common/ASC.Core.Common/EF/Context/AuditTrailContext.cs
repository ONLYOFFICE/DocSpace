namespace ASC.Core.Common.EF.Context
{
    public class MySqlAuditTrailContext : AuditTrailContext { }
    public class PostgreSqlAuditTrailContext : AuditTrailContext { }
    public class AuditTrailContext : BaseDbContext
    {
        public DbSet<DbAuditEvent> AuditEvents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddAuditEvent()
            .AddUser()
            .AddDbFunction();
        }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlAuditTrailContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlAuditTrailContext() } ,
                };
            }
        }
    }

    public static class AuditTrailContextExtension
    {
        public static DIHelper AddAuditTrailContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<AuditTrailContext>();
        }
    }
}
