namespace ASC.Data.Storage.Configuration;

public static class StorageConfigExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAddSingleton(r => r.GetService<ConfigurationExtension>().GetSetting<Storage>("Storage"));
    }
}


public class Storage
{
    public IEnumerable<Appender> Appender { get; set; }
    public IEnumerable<Handler> Handler { get; set; }
    public IEnumerable<Module> Module { get; set; }

    public Module GetModuleElement(string name)
    {
        return Module?.FirstOrDefault(r => r.Name == name);
    }
    public Handler GetHandler(string name)
    {
        return Handler?.FirstOrDefault(r => r.Name == name);
    }
}

public class Appender
{
    public string Name { get; set; }
    public string Append { get; set; }
    public string AppendSecure { get; set; }
    public string Extensions { get; set; }
}

public class Handler
{
    public string Name { get; set; }
    public string Type { get; set; }
    public IEnumerable<Properties> Property { get; set; }

    public IDictionary<string, string> GetProperties()
    {
        return Property == null || !Property.Any() ? new Dictionary<string, string>()
            : Property.ToDictionary(r => r.Name, r => r.Value);
    }
}
public class Properties
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class Module
{
    public string Name { get; set; }
    public string Data { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public ACL Acl { get; set; } = ACL.Read;
    public string VirtualPath { get; set; }
    public TimeSpan Expires { get; set; }
    public bool Visible { get; set; } = true;
    public bool AppendTenantId { get; set; } = true;
    public bool Public { get; set; }
    public bool DisableMigrate { get; set; }
    public bool Count { get; set; } = true;
    public bool DisabledEncryption { get; set; }
    public IEnumerable<Module> Domain { get; set; }
}

