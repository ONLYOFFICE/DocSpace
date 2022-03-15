namespace ASC.Notify.Config;

[Singletone]
public class ConfigureNotifyServiceCfg : IConfigureOptions<NotifyServiceCfg>
{
    public ConfigureNotifyServiceCfg(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly IServiceProvider _serviceProvider;

    public void Configure(NotifyServiceCfg options)
    {
        options.Init(_serviceProvider);
    }
}

[Singletone(typeof(ConfigureNotifyServiceCfg))]
public class NotifyServiceCfg
{
    public string ConnectionStringName { get; set; }
    public int StoreMessagesDays { get; set; }
    public string ServerRoot { get; set; }
    public NotifyServiceCfgProcess Process { get; set; }
    public List<NotifyServiceCfgSender> Senders { get; set; }
    public List<NotifyServiceCfgScheduler> Schedulers { get; set; }

    public void Init(IServiceProvider serviceProvider)
    {
        ServerRoot = string.IsNullOrEmpty(ServerRoot) ? "http://*/" : ServerRoot;

        Process.Init();

        foreach (var s in Senders)
        {
            try
            {
                s.Init(serviceProvider);
            }
            catch (Exception)
            {

            }
        }
        foreach (var s in Schedulers)
        {
            try
            {
                s.Init();
            }
            catch (Exception)
            {

            }
        }
    }
}

public class NotifyServiceCfgProcess
{
    public int MaxThreads { get; set; }
    public int BufferSize { get; set; }
    public int MaxAttempts { get; set; }
    public string AttemptsInterval { get; set; }

    public void Init()
    {
        if (MaxThreads == 0)
        {
            MaxThreads = Environment.ProcessorCount;
        }
    }
}

public class NotifyServiceCfgSender
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public INotifySender NotifySender { get; set; }

    public void Init(IServiceProvider serviceProvider)
    {
        var sender = (INotifySender)serviceProvider.GetService(System.Type.GetType(Type, true));
        sender.Init(Properties);
        NotifySender = sender;
    }
}

public class NotifyServiceCfgScheduler
{
    public string Name { get; set; }
    public string Register { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public void Init()
    {
        var typeName = Register.Substring(0, Register.IndexOf(','));
        var assemblyName = Register.Substring(Register.IndexOf(','));
        var type = Type.GetType(string.Concat(typeName.AsSpan(0, typeName.LastIndexOf('.')), assemblyName), true);
        MethodInfo = type.GetMethod(typeName.Substring(typeName.LastIndexOf('.') + 1), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }
}
