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
        _logger.LogDebug(name);
    }

    public void LogDebug(string message)
    {
        _logger.LogDebug(message);
    }

    public void LogDebug(Exception exception, string message)
    {
        _logger.LogDebug(exception, message);
    }

    public void LogDebug(string format, params object[] args)
    {
        _logger.LogDebug(format, args);
    }

    public void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props)
    {
        _logger.DebugWithProps(message, props);
    }

    public void LogError(string message)
    {
        _logger.LogError(message);
    }

    public void LogError(Exception exception, string message)
    {
        _logger.LogError(exception, message);
    }

    public void LogError(string format, params object[] args)
    {
        _logger.LogError(format, args);
    }

    public void LogCritical(string message)
    {
        _logger.LogCritical(message);
    }

    public void LogCritical(Exception exception, string message)
    {
        _logger.LogCritical(exception, message);
    }

    public void LogCritical(string format, params object[] args)
    {
        _logger.LogCritical(format, args);
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    public void LogInformation(Exception exception, string message)
    {
        _logger.LogInformation(exception, message);
    }

    public void LogInformation(string format, params object[] args)
    {
        _logger.LogInformation(format, args);
    }

    public void LogTrace(string message)
    {
        _logger.LogTrace(message);
    }

    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    public void LogWarning(Exception exception, string message)
    {
        _logger.LogWarning(exception, message);
    }

    public void LogWarning(string format, params object[] args)
    {
        _logger.LogWarning(format, args);
    }
}