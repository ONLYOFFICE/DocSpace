namespace ASC.FederatedLogin;

public class MultiRegionAccountLinker
{
    private readonly Dictionary<string, AccountLinker> _accountLinkers = new Dictionary<string, AccountLinker>();
    private readonly string _baseDatabaseId = null;

    public MultiRegionAccountLinker(string databaseId, ConfigurationExtension configuration, IOptionsSnapshot<AccountLinker> snapshot)
    {
        foreach (var connection in configuration.GetConnectionStrings())
        {
            if (connection.Name.StartsWith(databaseId))
            {
                _accountLinkers.Add(connection.Name, snapshot.Get(connection.Name));
            }
        }
    }

    public IEnumerable<string> GetLinkedObjects(string id, string provider)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(id, provider));
    }

    public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(profile));
    }

    public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjectsByHashId(hashid));
    }

    public void AddLink(string hostedRegion, string obj, LoginProfile profile)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, profile);
    }

    public void AddLink(string hostedRegion, string obj, string id, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, id, provider);
    }

    public void RemoveLink(string hostedRegion, string obj, string id, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, id, provider);
    }

    public void RemoveLink(string hostedRegion, string obj, LoginProfile profile)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, profile);
    }

    public void Unlink(string region, string obj)
    {
        _accountLinkers[GetDatabaseId(region)].RemoveProvider(obj);
    }

    public void RemoveProvider(string hostedRegion, string obj, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveProvider(obj, provider);
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedProfiles(obj));
    }

    private string GetDatabaseId(string hostedRegion)
    {
        var databaseId = _baseDatabaseId;

        if (!string.IsNullOrEmpty(hostedRegion))
        {
            databaseId = string.Join(".", new[] { _baseDatabaseId, hostedRegion.Trim() });
        }

        if (!_accountLinkers.ContainsKey(databaseId))
        {
            throw new ArgumentException($"Region {databaseId} is not defined", nameof(hostedRegion));
        }

        return databaseId;
    }
}
