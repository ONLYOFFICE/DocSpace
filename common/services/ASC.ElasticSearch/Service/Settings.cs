namespace ASC.ElasticSearch.Service;

[Singletone]
public class Settings
{
    public string Host
    {
        get => _host ?? "localhost";
        set => _host = value;
    }
    public int? Port
    {
        get => _port ?? 9200;
        set => _port = value;
    }
    public string Scheme
    {
        get => _scheme ?? "http";
        set => _scheme = value;
    }
    public int? Period
    {
        get => _period ?? 1;
        set => _period = value;
    }
    public long? MaxContentLength
    {
        get => _maxContentLength ?? 100 * 1024 * 1024L;
        set => _maxContentLength = value;
    }
    public long? MaxFileSize
    {
        get => _maxFileSize ?? 10 * 1024 * 1024L;
        set => _maxFileSize = value;
    }
    public int? Threads
    {
        get => _threads ?? 1;
        set => _threads = value;
    }
    public bool? HttpCompression
    {
        get => _httpCompression ?? true;
        set => _httpCompression = value;
    }

    private string _host;
    private int? _port;
    private string _scheme;
    private int? _period;
    private long? _maxContentLength;
    private long? _maxFileSize;
    private int? _threads;
    private bool? _httpCompression;

    public Settings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("elastic", this);
    }
}
