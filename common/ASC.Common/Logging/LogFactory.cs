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

namespace ASC.Common.Logging;
public interface ILog<TCategoryName> : ILog
{

}

public class LogFactory<T> : ILog<T>
{
    private readonly ILog _logger;

    public LogFactory(IOptionsMonitor<ILog> options)
    {
        _logger = options.Get(typeof(T).FullName);
    }

    public bool IsDebugEnabled => _logger.IsDebugEnabled;

    public bool IsInfoEnabled => _logger.IsInfoEnabled;

    public bool IsWarnEnabled => _logger.IsWarnEnabled;

    public bool IsErrorEnabled => _logger.IsErrorEnabled;

    public bool IsFatalEnabled => _logger.IsFatalEnabled;

    public bool IsTraceEnabled => _logger.IsTraceEnabled;

    public string LogDirectory => _logger.LogDirectory;

    public string Name { get => _logger.Name; set => _logger.Name = value; }

    public void Configure(string name)
    {
        _logger.Debug(name);
    }

    public void Debug(object message)
    {
        _logger.Debug(message);
    }

    public void Debug(object message, Exception exception)
    {
        _logger.Debug(message, exception);
    }

    public void DebugFormat(string format, params object[] args)
    {
        _logger.DebugFormat(format, args);
    }

    public void DebugFormat(string format, object arg0)
    {
        _logger.DebugFormat(format, arg0);
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
        _logger.DebugFormat(format, arg0, arg1);
    }

    public void DebugFormat(string format, object arg0, object arg1, object arg2)
    {
        _logger.DebugFormat(format, arg0, arg1, arg2);
    }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args)
    {
        _logger.DebugFormat(provider, format, args);
    }

    public void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props)
    {
        _logger.DebugWithProps(message, props);
    }

    public void Error(object message)
    {
        _logger.Error(message);
    }

    public void Error(object message, Exception exception)
    {
        _logger.Error(message, exception);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        _logger.ErrorFormat(format, args);
    }

    public void ErrorFormat(string format, object arg0)
    {
        _logger.ErrorFormat(format, arg0);
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
        _logger.ErrorFormat(format, arg0, arg1);
    }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2)
    {
        _logger.ErrorFormat(format, arg0, arg1, arg2);
    }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
    {
        _logger.ErrorFormat(provider, format, args);
    }

    public void Fatal(object message)
    {
        _logger.Fatal(message);
    }

    public void Fatal(string message, Exception exception)
    {
        _logger.Fatal(message, exception);
    }

    public void FatalFormat(string format, params object[] args)
    {
        _logger.FatalFormat(format, args);
    }

    public void FatalFormat(string format, object arg0)
    {
        _logger.FatalFormat(format, arg0);
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
        _logger.FatalFormat(format, arg0, arg1);
    }

    public void FatalFormat(string format, object arg0, object arg1, object arg2)
    {
        _logger.FatalFormat(format, arg0, arg1, arg2);
    }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args)
    {
        _logger.FatalFormat(provider, format, args);
    }

    public void Info(object message)
    {
        _logger.Info(message);
    }

    public void Info(string message, Exception exception)
    {
        _logger.Info(message, exception);
    }

    public void InfoFormat(string format, params object[] args)
    {
        _logger.InfoFormat(format, args);
    }

    public void InfoFormat(string format, object arg0)
    {
        _logger.InfoFormat(format, arg0);
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
        _logger.InfoFormat(format, arg0, arg1);
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
        _logger.InfoFormat(format, arg0, arg1, arg2);
    }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args)
    {
        _logger.InfoFormat(provider, format, args);
    }

    public void Trace(object message)
    {
        _logger.Trace(message);
    }

    public void TraceFormat(string message, params object[] args)
    {
        _logger.TraceFormat(message, args);
    }

    public void Warn(object message)
    {
        _logger.Warn(message);
    }

    public void Warn(object message, Exception exception)
    {
        _logger.Warn(message, exception);
    }

    public void WarnFormat(string format, params object[] args)
    {
        _logger.WarnFormat(format, args);
    }

    public void WarnFormat(string format, object arg0)
    {
        _logger.WarnFormat(format, arg0);
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
        _logger.WarnFormat(format, arg0, arg1);
    }

    public void WarnFormat(string format, object arg0, object arg1, object arg2)
    {
        _logger.WarnFormat(format, arg0, arg1, arg2);
    }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args)
    {
        _logger.WarnFormat(provider, format, args);
    }
}