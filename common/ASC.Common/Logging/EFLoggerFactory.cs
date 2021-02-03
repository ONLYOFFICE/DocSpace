using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Common.Logging
{
    [Scope]
    public class EFLoggerFactory : ILoggerFactory
    {
        Dictionary<string, ILogger> Loggers { get; set; }
        Lazy<ILogger> Logger { get; set; }
        ILoggerProvider LoggerProvider { get; set; }

        public EFLoggerFactory(EFLoggerProvider loggerProvider)
        {
            LoggerProvider = loggerProvider;
            Loggers = new Dictionary<string, ILogger>();
            Logger = new Lazy<ILogger>(() => LoggerProvider.CreateLogger(""));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            //LoggerProvider = provider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return Logger.Value;
        }

        public void Dispose()
        {
        }
    }

    [Scope]
    public class EFLoggerProvider : ILoggerProvider
    {
        private IOptionsMonitor<ILog> Option { get; }

        public EFLoggerProvider(IOptionsMonitor<ILog> option)
        {
            Option = option;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new EFLogger(Option.Get("ASC.SQL"));
        }

        public void Dispose() { }
    }

    public class EFLogger : ILogger
    {
        public ILog CustomLogger { get; }

        public EFLogger(ILog customLogger)
        {
            CustomLogger = customLogger;
        }

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
                    string commandText = null;
                    string parameters = null;
                    string elapsed = null;

                    foreach (var kvp in keyValuePairs)
                    {
                        commandText = GetParam(kvp, "commandText", commandText);
                        parameters = GetParam(kvp, "parameters", parameters);
                        elapsed = GetParam(kvp, "elapsed", elapsed);
                    }

                    if (!string.IsNullOrEmpty(commandText))
                    {
                        CustomLogger.DebugWithProps("",
                            new KeyValuePair<string, object>("duration", elapsed ?? ""),
                            new KeyValuePair<string, object>("sql", RemoveWhiteSpaces(commandText)),
                            new KeyValuePair<string, object>("sqlParams", parameters ?? "")
                        );
                    }

                    static string GetParam(KeyValuePair<string, object> keyValuePair, string key, string currentVal)
                    {
                        return keyValuePair.Key == key ? keyValuePair.Value.ToString() : currentVal;
                    }
                    break;
            }
        }

        private string RemoveWhiteSpaces(string str)
        {
            return !string.IsNullOrEmpty(str) ?
                str.Replace(Environment.NewLine, " ").Replace("\n", "").Replace("\r", "").Replace("\t", " ") :
                string.Empty;
        }
    }
}
