namespace ASC.Core;

[Scope(typeof(DbAzService), typeof(CachedAzService))]
public interface IAzService
{
    AzRecord SaveAce(int tenant, AzRecord r);
    IEnumerable<AzRecord> GetAces(int tenant, DateTime from);
    void RemoveAce(int tenant, AzRecord r);
}
