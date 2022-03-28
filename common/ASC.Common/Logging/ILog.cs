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

namespace ASC.Common.Logging;

public interface ILog
{
    bool IsDebugEnabled { get; }
    bool IsInfoEnabled { get; }
    bool IsWarnEnabled { get; }
    bool IsErrorEnabled { get; }
    bool IsFatalEnabled { get; }
    bool IsTraceEnabled { get; }

    void Trace(object message);
    void TraceFormat(string message, object arg0);

    void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props);
    void Debug(object message);
    void Debug(object message, Exception exception);
    void DebugFormat(string format, params object[] args);
    void DebugFormat(string format, object arg0);
    void DebugFormat(string format, object arg0, object arg1);
    void DebugFormat(string format, object arg0, object arg1, object arg2);
    void DebugFormat(IFormatProvider provider, string format, params object[] args);


    void Info(object message);
    void Info(string message, Exception exception);
    void InfoFormat(string format, params object[] args);
    void InfoFormat(string format, object arg0);
    void InfoFormat(string format, object arg0, object arg1);
    void InfoFormat(string format, object arg0, object arg1, object arg2);
    void InfoFormat(IFormatProvider provider, string format, params object[] args);

    void Warn(object message);
    void Warn(object message, Exception exception);
    void WarnFormat(string format, params object[] args);
    void WarnFormat(string format, object arg0);
    void WarnFormat(string format, object arg0, object arg1);
    void WarnFormat(string format, object arg0, object arg1, object arg2);
    void WarnFormat(IFormatProvider provider, string format, params object[] args);

    void Error(object message);
    void Error(object message, Exception exception);
    void ErrorFormat(string format, params object[] args);
    void ErrorFormat(string format, object arg0);
    void ErrorFormat(string format, object arg0, object arg1);
    void ErrorFormat(string format, object arg0, object arg1, object arg2);
    void ErrorFormat(IFormatProvider provider, string format, params object[] args);

    void Fatal(object message);
    void Fatal(string message, Exception exception);
    void FatalFormat(string format, params object[] args);
    void FatalFormat(string format, object arg0);
    void FatalFormat(string format, object arg0, object arg1);
    void FatalFormat(string format, object arg0, object arg1, object arg2);
    void FatalFormat(IFormatProvider provider, string format, params object[] args);

    string LogDirectory { get; }
    string Name { get; set; }

    void Configure(string name);
}

