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


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ASC.Common.Utils;
using log4net.Config;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NLog;

namespace ASC.Common.Logging
{
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

        void DebugWithProps(string message, params KeyValuePair<string, object>[] props);
        void DebugWithProps(string message, KeyValuePair<string, object> prop1, KeyValuePair<string, object> prop2, KeyValuePair<string, object> prop3);
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
    }

    public class Log : ILog
    {
        static Log()
        {
            XmlConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetCallingAssembly()));
        }

        private readonly log4net.ILog loger;

        public bool IsDebugEnabled { get; private set; }

        public bool IsInfoEnabled { get; private set; }

        public bool IsWarnEnabled { get; private set; }

        public bool IsErrorEnabled { get; private set; }

        public bool IsFatalEnabled { get; private set; }

        public bool IsTraceEnabled { get; private set; }

        public Log(string name)
        {
            loger = log4net.LogManager.GetLogger(Assembly.GetCallingAssembly(), name);

            IsDebugEnabled = loger.IsDebugEnabled;
            IsInfoEnabled = loger.IsInfoEnabled;
            IsWarnEnabled = loger.IsWarnEnabled;
            IsErrorEnabled = loger.IsErrorEnabled;
            IsFatalEnabled = loger.IsFatalEnabled;
            IsTraceEnabled = loger.Logger.IsEnabledFor(Level.Trace);
        }

        public void Trace(object message)
        {
            if (IsTraceEnabled) loger.Logger.Log(GetType(), Level.Trace, message, null);
        }

        public void TraceFormat(string message, object arg0)
        {
            if (IsTraceEnabled) loger.Logger.Log(GetType(), Level.Trace, string.Format(message, arg0), null);
        }

        public void Debug(object message)
        {
            if (IsDebugEnabled) loger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            if (IsDebugEnabled) loger.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled) loger.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            if (IsDebugEnabled) loger.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            if (IsDebugEnabled) loger.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsDebugEnabled) loger.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsDebugEnabled) loger.DebugFormat(provider, format, args);
        }

        public void DebugWithProps(string message, params KeyValuePair<string, object>[] props)
        {
            if (!IsDebugEnabled) return;

            foreach (var p in props)
            {
                log4net.ThreadContext.Properties[p.Key] = p.Value;
            }

            loger.Debug(message);
        }


        public void Info(object message)
        {
            if (IsInfoEnabled) loger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled) loger.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled) loger.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            if (IsInfoEnabled) loger.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            if (IsInfoEnabled) loger.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsInfoEnabled) loger.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsInfoEnabled) loger.InfoFormat(provider, format, args);
        }


        public void Warn(object message)
        {
            if (IsWarnEnabled) loger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled) loger.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled) loger.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            if (IsWarnEnabled) loger.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            if (IsWarnEnabled) loger.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsWarnEnabled) loger.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsWarnEnabled) loger.WarnFormat(provider, format, args);
        }


        public void Error(object message)
        {
            if (IsErrorEnabled) loger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled) loger.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled) loger.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            if (IsErrorEnabled) loger.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            if (IsErrorEnabled) loger.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsErrorEnabled) loger.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsErrorEnabled) loger.ErrorFormat(provider, format, args);
        }


        public void Fatal(object message)
        {
            if (IsFatalEnabled) loger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled) loger.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled) loger.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            if (IsFatalEnabled) loger.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            if (IsFatalEnabled) loger.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsFatalEnabled) loger.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsFatalEnabled) loger.FatalFormat(provider, format, args);
        }

        public void DebugWithProps(string message, KeyValuePair<string, object> prop1, KeyValuePair<string, object> prop2, KeyValuePair<string, object> prop3)
        {
            if (!IsDebugEnabled) return;

            AddProp(prop1);
            AddProp(prop2);
            AddProp(prop3);

            loger.Debug(message);

            static void AddProp(KeyValuePair<string, object> p)
            {
                log4net.ThreadContext.Properties[p.Key] = p.Value;
            }
        }

        public string LogDirectory
        {
            get
            {
                return log4net.GlobalContext.Properties["LogDirectory"].ToString();
            }
        }

        public string Name
        {
            get;

            set;
        }
    }

    public class NLogSettings
    {
        public string Name { get; set; }
        public string Dir { get; set; }
    }

    public class ConfigureLogNLog : IConfigureOptions<LogNLog>
    {
        public ConfigureLogNLog(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(LogNLog options)
        {
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(Path.Combine(Configuration["pathToConf"], "nlog.config"), true);

            var settings = Configuration.GetSetting<NLogSettings>("log");
            if (!string.IsNullOrEmpty(settings.Name))
            {
                LogManager.Configuration.Variables["name"] = settings.Name;
            }

            if (!string.IsNullOrEmpty(settings.Dir))
            {
                LogManager.Configuration.Variables["dir"] = settings.Dir.TrimEnd('/').TrimEnd('\\') + Path.DirectorySeparatorChar;
            }

            NLog.Targets.Target.Register<SelfCleaningTarget>("SelfCleaning");
        }
    }

    public class LogNLog : ILog
    {
        private NLog.ILogger loger;
        private NLog.ILogger Loger
        {
            get
            {
                return loger;
            }
            set
            {
                loger = value;
                IsDebugEnabled = loger.IsDebugEnabled;
                IsInfoEnabled = loger.IsInfoEnabled;
                IsWarnEnabled = loger.IsWarnEnabled;
                IsErrorEnabled = loger.IsErrorEnabled;
                IsFatalEnabled = loger.IsFatalEnabled;
                IsTraceEnabled = loger.IsEnabled(LogLevel.Trace);
            }
        }

        public bool IsDebugEnabled { get; private set; }

        public bool IsInfoEnabled { get; private set; }

        public bool IsWarnEnabled { get; private set; }

        public bool IsErrorEnabled { get; private set; }

        public bool IsFatalEnabled { get; private set; }

        public bool IsTraceEnabled { get; private set; }

        public void Trace(object message)
        {
            if (IsTraceEnabled) Loger.Log(LogLevel.Trace, message);
        }

        public void TraceFormat(string message, object arg0)
        {
            if (IsTraceEnabled) Loger.Log(LogLevel.Trace, string.Format(message, arg0));
        }

        public void Debug(object message)
        {
            if (IsDebugEnabled) Loger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            if (IsDebugEnabled) Loger.Debug(exception, "{0}", message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled) Loger.Debug(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            if (IsDebugEnabled) Loger.Debug(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            if (IsDebugEnabled) Loger.Debug(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsDebugEnabled) Loger.Debug(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsDebugEnabled) Loger.Debug(provider, format, args);
        }

        public void DebugWithProps(string message, params KeyValuePair<string, object>[] props)
        {
            if (!IsDebugEnabled) return;

            var theEvent = new LogEventInfo { Message = message, LoggerName = Name, Level = LogLevel.Debug };

            foreach (var p in props)
            {
                theEvent.Properties[p.Key] = p.Value;
            }

            Loger.Log(theEvent);
        }
        public void DebugWithProps(string message, KeyValuePair<string, object> prop1, KeyValuePair<string, object> prop2, KeyValuePair<string, object> prop3)
        {
            if (!IsDebugEnabled) return;

            var theEvent = new LogEventInfo
            {
                Message = message,
                LoggerName = Name,
                Level = LogLevel.Debug
            };

            AddProp(prop1);
            AddProp(prop2);
            AddProp(prop3);


            Loger.Log(theEvent);

            void AddProp(KeyValuePair<string, object> p)
            {
                theEvent.Properties[p.Key] = p.Value;
            }
        }

        public void Info(object message)
        {
            if (IsInfoEnabled) Loger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled) Loger.Info(exception, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled) Loger.Info(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            if (IsInfoEnabled) Loger.Info(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            if (IsInfoEnabled) Loger.Info(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsInfoEnabled) Loger.Info(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsInfoEnabled) Loger.Info(provider, format, args);
        }


        public void Warn(object message)
        {
            if (IsWarnEnabled) Loger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled) Loger.Warn(exception, "{0}", message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled) Loger.Warn(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            if (IsWarnEnabled) Loger.Warn(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            if (IsWarnEnabled) Loger.Warn(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsWarnEnabled) Loger.Warn(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsWarnEnabled) Loger.Warn(provider, format, args);
        }


        public void Error(object message)
        {
            if (IsErrorEnabled) Loger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled) Loger.Error(exception, "{0}", message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled) Loger.Error(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            if (IsErrorEnabled) Loger.Error(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            if (IsErrorEnabled) Loger.Error(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsErrorEnabled) Loger.Error(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsErrorEnabled) Loger.Error(provider, format, args);
        }


        public void Fatal(object message)
        {
            if (IsFatalEnabled) Loger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled) Loger.Fatal(exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled) Loger.Fatal(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            if (IsFatalEnabled) Loger.Fatal(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            if (IsFatalEnabled) Loger.Fatal(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            if (IsFatalEnabled) Loger.Fatal(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (IsFatalEnabled) Loger.Fatal(provider, format, args);
        }

        public string LogDirectory { get { return NLog.LogManager.Configuration.Variables["logDirectory"].Text; } }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                Loger = NLog.LogManager.GetLogger(name);
            }
        }
    }

    public class NullLog : ILog
    {
        public bool IsDebugEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsWarnEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsFatalEnabled { get; set; }
        public bool IsTraceEnabled { get; set; }

        public void Trace(object message)
        {
        }

        public void TraceFormat(string message, object arg0)
        {
        }

        public void DebugWithProps(string message, params KeyValuePair<string, object>[] props)
        {
        }

        public void Debug(object message)
        {
        }

        public void Debug(object message, Exception exception)
        {
        }

        public void DebugFormat(string format, params object[] args)
        {
        }

        public void DebugFormat(string format, object arg0)
        {
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Info(object message)
        {
        }

        public void Info(string message, Exception exception)
        {
        }

        public void InfoFormat(string format, params object[] args)
        {
        }

        public void InfoFormat(string format, object arg0)
        {
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Warn(object message)
        {
        }

        public void Warn(object message, Exception exception)
        {
        }

        public void WarnFormat(string format, params object[] args)
        {
        }

        public void WarnFormat(string format, object arg0)
        {
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Error(object message)
        {
        }

        public void Error(object message, Exception exception)
        {
        }

        public void ErrorFormat(string format, params object[] args)
        {
        }

        public void ErrorFormat(string format, object arg0)
        {
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Fatal(object message)
        {
        }

        public void Fatal(string message, Exception exception)
        {
        }

        public void FatalFormat(string format, params object[] args)
        {
        }

        public void FatalFormat(string format, object arg0)
        {
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void DebugWithProps(string message, KeyValuePair<string, object> prop1, KeyValuePair<string, object> prop2, KeyValuePair<string, object> prop3)
        {
        }

        public string LogDirectory { get { return ""; } }

        public string Name { get; set; }
    }


    public class LogManager<T> : OptionsMonitor<T> where T : class, ILog, new()
    {
        public LogManager(IOptionsFactory<T> factory, IEnumerable<IOptionsChangeTokenSource<T>> sources, IOptionsMonitorCache<T> cache) : base(factory, sources, cache)
        {
        }

        public override T Get(string name)
        {
            var log = base.Get(name);

            if (string.IsNullOrEmpty(log?.Name))
            {
                log = CurrentValue;
            }

            return log;
        }
    }

    public static class StudioNotifyHelperExtension
    {
        public static IServiceCollection AddLogManager<T>(this IServiceCollection services, params string[] additionalLoggers) where T : class, ILog, new()
        {
            const string baseName = "ASC";
            var baseSqlName = $"{baseName}.SQL";
            services.Configure<T>(r => r.Name = baseName);
            services.Configure<T>(baseName, r => r.Name = baseName);
            services.Configure<T>(baseSqlName, r => r.Name = baseSqlName);

            foreach (var l in additionalLoggers)
            {
                services.Configure<T>(l, r => r.Name = l);
            }

            services.TryAddSingleton(typeof(IOptionsMonitor<ILog>), typeof(LogManager<T>));
            return services;
        }

        public static IServiceCollection AddNLogManager(this IServiceCollection services, params string[] additionalLoggers)
        {
            services.TryAddSingleton<IConfigureOptions<LogNLog>, ConfigureLogNLog>();
            return services.AddLogManager<LogNLog>(additionalLoggers);
        }
    }
}
