namespace ASC.Core.Caching;

class AzRecordStore : IEnumerable<AzRecord>
{
    private readonly Dictionary<string, List<AzRecord>> byObjectId = new Dictionary<string, List<AzRecord>>();


    public AzRecordStore(IEnumerable<AzRecord> aces)
    {
        foreach (var a in aces)
        {
            Add(a);
        }
    }


    public IEnumerable<AzRecord> Get(string objectId)
    {
        byObjectId.TryGetValue(objectId ?? string.Empty, out var aces);

        return aces ?? new List<AzRecord>();
    }

    public void Add(AzRecord r)
    {
        if (r == null)
        {
            return;
        }

        var id = r.Object ?? string.Empty;
        if (!byObjectId.ContainsKey(id))
        {
            byObjectId[id] = new List<AzRecord>();
        }
        byObjectId[id].RemoveAll(a => a.Subject == r.Subject && a.Action == r.Action); // remove escape, see DbAzService
        byObjectId[id].Add(r);
    }

    public void Remove(AzRecord r)
    {
        if (r == null)
        {
            return;
        }

        var id = r.Object ?? string.Empty;
        if (byObjectId.TryGetValue(id, out var list))
        {
            list.RemoveAll(a => a.Subject == r.Subject && a.Action == r.Action && a.AceType == r.AceType);
        }
    }

    public IEnumerator<AzRecord> GetEnumerator()
    {
        return byObjectId.Values.SelectMany(v => v).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
