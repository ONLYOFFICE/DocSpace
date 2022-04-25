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
    readonly Microsoft.Extensions.Logging.ILogger mlog;

    public void Configure(string name)
    {
        Name = name;
    }

    public void LogTrace(string message)
    {
        if (IsTraceEnabled)
        {
            Logger.Log(LogLevel.Trace, message);
        }
    }

    public void LogTrace(string message, params object[] args)
    {
        if (IsTraceEnabled)
        {
            Logger.Trace(message, args);
        }
    }

    public void LogDebug(string message)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(message);
        }
    }

    public void LogDebug(Exception exception, string message)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(exception, "{0}", message);
        }
    }

    public void LogDebug(string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, args);
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

    public void LogInformation(string message)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(message);
        }
    }

    public void LogInformation(Exception exception, string message)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(exception, message);
        }
    }

    public void LogInformation(string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, args);
        }
    }

    public void LogInformation(string format, object arg0)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0);
        }
    }

    public void LogInformation(string format, object arg0, object arg1)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0, arg1);
        }
    }

    public void LogInformation(string format, object arg0, object arg1, object arg2)
    {
        if (IsInfoEnabled)
        {
            Logger.Info(format, arg0, arg1, arg2);
        }
    }


    public void LogWarning(string message)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(message);
        }
    }

    public void LogWarning(Exception exception, string message)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(exception, "{0}", message);
        }
    }

    public void LogWarning(string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Logger.Warn(format, args);
        }
    }


    public void LogError(string message)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(message);
        }
    }

    public void LogError(Exception exception, string message)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(exception, "{0}", message);
        }
    }

    public void LogError(string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, args);
        }
    }

    public void LogCritical(string message)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(message);
        }
    }

    public void LogCritical(Exception exception, string message)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(exception, message);
        }
    }

    public void LogCritical(string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, args);
        }
    }
}
