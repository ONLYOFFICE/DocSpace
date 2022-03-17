﻿namespace ASC.Common.Utils;

public class ConnectionStringCollection : IEnumerable<ConnectionStringSettings>
{
    private readonly List<ConnectionStringSettings> _data;

    public ConnectionStringSettings this[string name] => _data.FirstOrDefault(r => r.Name == name);

    public ConnectionStringCollection(IEnumerable<ConnectionStringSettings> data)
    {
        _data = data.ToList();
    }

    public IEnumerator<ConnectionStringSettings> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Singletone]
public class ConfigurationExtension
{
    public string this[string key]
    {
        get => _configuration[key];
        set => _configuration[key] = value;
    }

    private readonly IConfiguration _configuration;
    private readonly Lazy<ConnectionStringCollection> _connectionStringSettings;

    public ConfigurationExtension(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionStringSettings = new Lazy<ConnectionStringCollection>(new ConnectionStringCollection(GetSettings<ConnectionStringSettings>("ConnectionStrings")));
    }

    public IEnumerable<T> GetSettings<T>(string section) where T : new()
    {
        var result = new List<T>();

        var sectionSettings = _configuration.GetSection(section);

        foreach (var ch in sectionSettings.GetChildren())
        {
            var cs = new T();
            ch.Bind(cs);
            result.Add(cs);
        }

        return result;
    }

    public T GetSetting<T>(string section) where T : new()
    {
        return GetSetting(section, new T());
    }

    public T GetSetting<T>(string section, T instance)
    {
        var sectionSettings = _configuration.GetSection(section);

        sectionSettings.Bind(instance);

        return instance;
    }

    public ConnectionStringCollection GetConnectionStrings()
    {
        return _connectionStringSettings.Value;
    }

    public ConnectionStringSettings GetConnectionStrings(string key)
    {
        return GetConnectionStrings()[key];
    }
}
