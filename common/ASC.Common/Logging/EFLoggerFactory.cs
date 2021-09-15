using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Common.Logging
{
    [Singletone]
    public class EFLoggerFactory : ILoggerFactory
    {
        Lazy<ILogger> Logger { get; set; }
        ILoggerProvider LoggerProvider { get; set; }

        public EFLoggerFactory(EFLoggerProvider loggerProvider)
        {
            LoggerProvider = loggerProvider;
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

    [Singletone]
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
                    CustomLogger.DebugWithProps("", keyValuePairs);
                    break;
            }
        }
    }
}
