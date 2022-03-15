namespace ASC.Core.Caching;

class UserGroupRefStore : IDictionary<string, UserGroupRef>
{
    private readonly IDictionary<string, UserGroupRef> _refs;
    private ILookup<Guid, UserGroupRef> _index;
    private bool _changed;


    public UserGroupRefStore(IDictionary<string, UserGroupRef> refs)
    {
        _refs = refs;
        _changed = true;
    }


    public void Add(string key, UserGroupRef value)
    {
        _refs.Add(key, value);
        RebuildIndex();
    }

    public bool ContainsKey(string key)
    {
        return _refs.ContainsKey(key);
    }

    public ICollection<string> Keys => _refs.Keys;

    public bool Remove(string key)
    {
        var result = _refs.Remove(key);
        RebuildIndex();

        return result;
    }

    public bool TryGetValue(string key, out UserGroupRef value)
    {
        return _refs.TryGetValue(key, out value);
    }

    public ICollection<UserGroupRef> Values => _refs.Values;

    public UserGroupRef this[string key]
    {
        get => _refs[key];
        set
        {
            _refs[key] = value;
            RebuildIndex();
        }
    }

    public void Add(KeyValuePair<string, UserGroupRef> item)
    {
        _refs.Add(item);
        RebuildIndex();
    }

    public void Clear()
    {
        _refs.Clear();
        RebuildIndex();
    }

    public bool Contains(KeyValuePair<string, UserGroupRef> item)
    {
        return _refs.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, UserGroupRef>[] array, int arrayIndex)
    {
        _refs.CopyTo(array, arrayIndex);
    }

    public int Count => _refs.Count;

    public bool IsReadOnly => _refs.IsReadOnly;

    public bool Remove(KeyValuePair<string, UserGroupRef> item)
    {
        var result = _refs.Remove(item);
        RebuildIndex();

        return result;
    }

    public IEnumerator<KeyValuePair<string, UserGroupRef>> GetEnumerator()
    {
        return _refs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _refs.GetEnumerator();
    }

    public IEnumerable<UserGroupRef> GetRefsByUser(Guid userId)
    {
        if (_changed)
        {
            _index = _refs.Values.ToLookup(r => r.UserId);
            _changed = false;
        }

        return _index[userId];
    }

    private void RebuildIndex()
    {
        _changed = true;
    }
}
