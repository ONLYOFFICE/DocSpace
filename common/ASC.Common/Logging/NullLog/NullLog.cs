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

public class NullLog : ILog
{
    public bool IsDebugEnabled { get; set; }
    public bool IsInfoEnabled { get; set; }
    public bool IsWarnEnabled { get; set; }
    public bool IsErrorEnabled { get; set; }
    public bool IsFatalEnabled { get; set; }
    public bool IsTraceEnabled { get; set; }
    public string Name { get; set; }
    public string LogDirectory => string.Empty;

    public void Trace(object message) { }

    public void TraceFormat(string message, object arg0) { }

    public void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props) { }

    public void Debug(object message) { }

    public void Debug(object message, Exception exception) { }

    public void DebugFormat(string format, params object[] args) { }

    public void DebugFormat(string format, object arg0) { }

    public void DebugFormat(string format, object arg0, object arg1) { }

    public void DebugFormat(string format, object arg0, object arg1, object arg2) { }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args) { }

    public void Info(object message) { }

    public void Info(string message, Exception exception) { }

    public void InfoFormat(string format, params object[] args) { }

    public void InfoFormat(string format, object arg0) { }

    public void InfoFormat(string format, object arg0, object arg1) { }

    public void InfoFormat(string format, object arg0, object arg1, object arg2) { }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args) { }

    public void Warn(object message) { }

    public void Warn(object message, Exception exception) { }

    public void WarnFormat(string format, params object[] args) { }

    public void WarnFormat(string format, object arg0) { }

    public void WarnFormat(string format, object arg0, object arg1) { }

    public void WarnFormat(string format, object arg0, object arg1, object arg2) { }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args) { }

    public void Error(object message) { }

    public void Error(object message, Exception exception) { }

    public void ErrorFormat(string format, params object[] args) { }

    public void ErrorFormat(string format, object arg0) { }

    public void ErrorFormat(string format, object arg0, object arg1) { }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2) { }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args) { }

    public void Fatal(object message) { }

    public void Fatal(string message, Exception exception) { }

    public void FatalFormat(string format, params object[] args) { }

    public void FatalFormat(string format, object arg0) { }

    public void FatalFormat(string format, object arg0, object arg1) { }

    public void FatalFormat(string format, object arg0, object arg1, object arg2) { }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args) { }

    public void Configure(string name) { }
}
