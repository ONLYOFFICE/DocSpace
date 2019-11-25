using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ASC.Common.Logging
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        ILoggerProvider LoggerProvider { get; set; }
        public void AddProvider(ILoggerProvider provider)
        {
            LoggerProvider = provider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return LoggerProvider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }

    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger();
        }

        public void Dispose() { }
    }

    public class ConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) { return null; }
        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Information:
                case LogLevel.None:
                    return false;

                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                default:
                    return true;
            };
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id == 20101)
            {
                var keyValuePairs = state as IEnumerable<KeyValuePair<string, object>>;

                foreach (var a in keyValuePairs)
                {
                    var key = a.Key;

                    if (key == "commandText")
                    {
                        var value = a.Value;
                    }
                }
            }
        }
    }
}
