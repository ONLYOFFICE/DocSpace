/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using LogLevel = NLog.LogLevel;

namespace ASC.Common.Logging;

public class LogNLog : ILog
{
    public bool IsDebugEnabled { get; private set; }
    public bool IsInfoEnabled { get; private set; }
    public bool IsWarnEnabled { get; private set; }
    public bool IsErrorEnabled { get; private set; }
    public bool IsFatalEnabled { get; private set; }
    public bool IsTraceEnabled { get; private set; }
    public string LogDirectory => LogManager.Configuration.Variables["dir"].Text;
    public string Name
    {
        get => _name;

        set
        {
            _name = value;
            Logger = LogManager.GetLogger(_name);
        }
    }

    private NLog.ILogger Logger
    {
        get => _logger;

        set
        {
            _logger = value;
            IsDebugEnabled = _logger.IsDebugEnabled;
            IsInfoEnabled = _logger.IsInfoEnabled;
            IsWarnEnabled = _logger.IsWarnEnabled;
            IsErrorEnabled = _logger.IsErrorEnabled;
            IsFatalEnabled = _logger.IsFatalEnabled;
            IsTraceEnabled = _logger.IsEnabled(LogLevel.Trace);
        }
    }

    private NLog.ILogger _logger;
    private string _name;

    public void Configure(string name)
    {
        Name = name;
    }

    public void Trace(object message)
    {
        if (IsTraceEnabled)
        {
            Logger.Log(LogLevel.Trace, message);
        }
    }

    public void TraceFormat(string message, object arg0)
    {
        if (IsTraceEnabled)
        {
            Logger.Log(LogLevel.Trace, string.Format(message, arg0));
        }
    }

    public void Debug(object message)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(message);
        }
    }

    public void Debug(object message, Exception exception)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(exception, "{0}", message);
        }
    }

    public void DebugFormat(string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, args);
        }
    }

    public void DebugFormat(string format, object arg0)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, arg0);
        }
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, arg0, arg1);
        }
    }

    public void DebugFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, arg0, arg1, arg2);
        }
    }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(provider, format, args);
        }
    }

    public void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props)
    {
        if (!IsDebugEnabled)
        {
            return;
        }

        var theEvent = new LogEventInfo { Message = message, LoggerName = Name, Level = LogLevel.Debug };

        foreach (var p in props)
        {
            theEvent.Properties[p.Key] = p.Value;
        }

        Logger.Log(theEvent);
    }

    public void Info(object message)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(message);
        }
    }

    public void Info(string message, Exception exception)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(exception, message);
        }
    }

    public void InfoFormat(string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, args);
        }
    }

    public void InfoFormat(string format, object arg0)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0);
        }
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0, arg1);
        }
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0, arg1, arg2);
        }
    }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(provider, format, args);
        }
    }


    public void Warn(object message)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(message);
        }
    }

    public void Warn(object message, Exception exception)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(exception, "{0}", message);
        }
    }

    public void WarnFormat(string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(format, args);
        }
    }

    public void WarnFormat(string format, object arg0)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(format, arg0);
        }
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(format, arg0, arg1);
        }
    }

    public void WarnFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(format, arg0, arg1, arg2);
        }
    }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(provider, format, args);
        }
    }


    public void Error(object message)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(message);
        }
    }

    public void Error(object message, Exception exception)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(exception, "{0}", message);
        }
    }

    public void ErrorFormat(string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, args);
        }
    }

    public void ErrorFormat(string format, object arg0)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, arg0);
        }
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, arg0, arg1);
        }
    }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, arg0, arg1, arg2);
        }
    }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(provider, format, args);
        }
    }


    public void Fatal(object message)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(message);
        }
    }

    public void Fatal(string message, Exception exception)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(exception, message);
        }
    }

    public void FatalFormat(string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, args);
        }
    }

    public void FatalFormat(string format, object arg0)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, arg0);
        }
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, arg0, arg1);
        }
    }

    public void FatalFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, arg0, arg1, arg2);
        }
    }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(provider, format, args);
        }
    }
}
