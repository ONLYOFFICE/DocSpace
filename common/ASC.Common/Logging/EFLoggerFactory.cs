using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ASC.Common.Logging;

[Singletone]
public class EFLoggerFactory : ILoggerFactory
{
    private readonly Lazy<ILogger> _logger;
    private readonly ILoggerProvider _loggerProvider;

    public EFLoggerFactory(EFLoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
        _logger = new Lazy<ILogger>(() => _loggerProvider.CreateLogger(""));
    }

    public void AddProvider(ILoggerProvider provider)
    {
        //LoggerProvider = provider;
    }

    public ILogger CreateLogger(string categoryName) => _logger.Value;

    public void Dispose() { }
}

[Singletone]
public class EFLoggerProvider : ILoggerProvider
{
    private readonly IOptionsMonitor<ILog> _option;

    public EFLoggerProvider(IOptionsMonitor<ILog> option) => _option = option;

    public ILogger CreateLogger(string categoryName) => new EFLogger(_option.Get("ASC.SQL"));

    public void Dispose() { }
}

public class EFLogger : ILogger
{
    public ILog CustomLogger { get; }

    public EFLogger(ILog customLogger) => CustomLogger = customLogger;

    public IDisposable BeginScope<TState>(TState state) { return null; }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => CustomLogger.IsTraceEnabled,
            LogLevel.Information => CustomLogger.IsInfoEnabled,
            LogLevel.None => false,

            LogLevel.Debug => CustomLogger.IsDebugEnabled,
            LogLevel.Warning => CustomLogger.IsWarnEnabled,
            LogLevel.Error => CustomLogger.IsErrorEnabled,
            LogLevel.Critical => CustomLogger.IsErrorEnabled,

            _ => true,
        };
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        switch (eventId.Id)
        {
            //case 20000:
            //    CustomLogger.Debug(formatter(state, exception));
            //    break;
            case 20101:
                var keyValuePairs = state as IEnumerable<KeyValuePair<string, object>>;
                CustomLogger.DebugWithProps("", keyValuePairs);
                break;
        }
    }
}
