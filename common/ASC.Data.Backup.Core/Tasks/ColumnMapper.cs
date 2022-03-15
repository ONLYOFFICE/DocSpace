namespace ASC.Data.Backup.Tasks;

public class ColumnMapper
{
    private readonly Dictionary<string, object> _mappings = new Dictionary<string, object>();
    private readonly Dictionary<string, object> _newMappings = new Dictionary<string, object>();
    private readonly DateTime _now = DateTime.UtcNow;

    public int GetTenantMapping()
    {
        var mapping = GetMapping("tenants_tenants", "id");

        return mapping != null ? Convert.ToInt32(mapping) : -1;
    }

    public string GetUserMapping(string oldValue)
    {
        var mapping = GetMapping("core_user", "id", oldValue);

        return mapping != null ? Convert.ToString(mapping) : null;
    }

    public void SetDateMapping(string tableName, KeyValuePair<string, bool> column, object oldValue)
    {
        if (!column.Value)
        {
            SetMapping(tableName, column.Key, oldValue, _now);

            return;
        }

        var newValue = Convert.ToDateTime(oldValue);
        var tenantCreationDate = GetTenantCreationDate();
        if (tenantCreationDate != DateTime.MinValue && newValue > DateTime.MinValue.AddDays(1) && newValue < DateTime.MaxValue.AddDays(-1))
        {
            newValue = newValue.AddDays(_now.Subtract(tenantCreationDate).Days);
        }

        SetMapping(tableName, column.Key, oldValue, newValue);
    }

    public void SetMapping(string tableName, string columnName, object oldValue, object newValue)
    {
        if (tableName == "tenants_tenants")
        {
            var mapping = new MappingWithCondition { NewValue = newValue, OldValue = oldValue };
            AddMappingInternal(GetMappingKey(tableName, columnName), mapping);

        }

        AddMappingInternal(GetMappingKey(tableName, columnName, oldValue), newValue);
    }

    public object GetMapping(string tableName, string columnName)
    {
        var mappingKey = GetMappingKey(tableName, columnName);

        return HasMapping(mappingKey) ? ((MappingWithCondition)GetMappingInternal(mappingKey)).NewValue : null;
    }

    public object GetMapping(string tableName, string columnName, object oldValue)
    {
        var mappingKey = GetMappingKey(tableName, columnName, oldValue);
        if (HasMapping(mappingKey))
        {
            return GetMappingInternal(mappingKey);
        }

        mappingKey = GetMappingKey(tableName, columnName);
        if (HasMapping(mappingKey))
        {
            var mapping = (MappingWithCondition)GetMappingInternal(mappingKey);

            return mapping.NewValue;
        }

        return null;
    }

    public void Commit()
    {
        foreach (var mapping in _newMappings)
        {
            _mappings[mapping.Key] = mapping.Value;
        }

        _newMappings.Clear();
    }

    public void Rollback()
    {
        _newMappings.Clear();
    }

    private DateTime GetTenantCreationDate()
    {
        var mappingKey = GetMappingKey("tenants_tenants", "creationdatetime");
        if (HasMapping(mappingKey))
        {
            var mapping = (MappingWithCondition)GetMappingInternal(mappingKey);

            return mapping != null ? Convert.ToDateTime(mapping.OldValue) : DateTime.MinValue;
        }
        return DateTime.MinValue;
    }

    private void AddMappingInternal(string key, object value)
    {
        _newMappings[key] = value;
    }

    private object GetMappingInternal(string key)
    {
        return _newMappings.ContainsKey(key) ? _newMappings[key] : _mappings[key];
    }

    private bool HasMapping(string key)
    {
        return _newMappings.ContainsKey(key) || _mappings.ContainsKey(key);
    }

    private static string GetMappingKey(string tableName, string columnName)
    {
            return $"t:{tableName};c:{columnName}".ToLowerInvariant();
    }

    private static string GetMappingKey(string tableName, string columnName, object oldValue)
    {
        return string.Format("{0};v:{1}", GetMappingKey(tableName, columnName), oldValue).ToLowerInvariant();
    }

    private class MappingWithCondition
    {
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }
}
