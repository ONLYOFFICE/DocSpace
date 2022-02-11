namespace ASC.ClearEvents.Data;

public class EventsContext : MessagesContext
{
    public DbSet<DbTenant> Tenants { get; set; }
    public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
}