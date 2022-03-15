namespace ASC.Data.Storage;

public class DataList : Dictionary<string, string>
{
    public DataList(Module config)
    {
        Add(string.Empty, config.Data);
        if (config.Domain != null)
        {
            foreach (var domain in config.Domain)
            {
                Add(domain.Name, domain.Data);
            }
        }
        else
        {
            config.Domain = new List<Module>();
        }
    }

    public string GetData(string name)
    {
        if (ContainsKey(name))
        {
            return this[name] ?? string.Empty;
        }

        return string.Empty;
    }
}
