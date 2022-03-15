namespace ASC.Core.Tenants;

public enum TenantStatus
{
    Active = 0,
    Suspended = 1,
    RemovePending = 2,
    Transfering = 3,
    Restoring = 4,
    Migrating = 5,
    Encryption = 6
}
