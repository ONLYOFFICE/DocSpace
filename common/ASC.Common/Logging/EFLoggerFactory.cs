// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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

    public ILogger CreateLogger(string categoryName)
    {
        return _logger.Value;
    }

    public void Dispose() { }
}

[Singletone]
public class EFLoggerProvider : ILoggerProvider
{
    private readonly IOptionsMonitor<ILog> _option;

    public EFLoggerProvider(IOptionsMonitor<ILog> option)
    {
        _option = option;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new EFLogger(_option.Get("ASC.SQL"));
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

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

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
