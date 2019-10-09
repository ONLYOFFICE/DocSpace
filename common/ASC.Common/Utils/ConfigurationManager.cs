using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.Utils
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSettings { get; private set; }
        public static ConnectionStringCollection ConnectionStrings { get; private set; }
        public static LogManager LogManager { get; private set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            AppSettings = serviceProvider.GetService<IConfiguration>();
            LogManager = serviceProvider.GetService<LogManager>();
            ConnectionStrings = AppSettings.GetConnectionStrings();
        }
        public static void UseCm(this IApplicationBuilder applicationBuilder)
        {
            Init(applicationBuilder.ApplicationServices);
        }
        public static IEnumerable<T> GetSettings<T>(string section) where T : new()
        {
            return AppSettings.GetSettings<T>(section);
        }
        public static T GetSetting<T>(string section) where T : new()
        {
            return AppSettings.GetSetting<T>(section);
        }
    }

    public class ConnectionStringCollection : IEnumerable<ConnectionStringSettings>
    {
        private List<ConnectionStringSettings> Data { get; set; }

        public ConnectionStringCollection(IEnumerable<ConnectionStringSettings> data) => Data = data.ToList();

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ConnectionStringSettings this[string name]
        {
            get
            {
                return Data.FirstOrDefault(r => r.Name == name);
            }
        }
    }

    public static class ConfigurationExtension
    {
        public static IEnumerable<T> GetSettings<T>(this IConfiguration configuration, string section) where T : new()
        {
            var result = new List<T>();

            var sectionSettings = configuration.GetSection(section);

            foreach (var ch in sectionSettings.GetChildren())
            {
                var cs = new T();
                ch.Bind(cs);
                result.Add(cs);
            }

            return result;
        }
        public static T GetSetting<T>(this IConfiguration configuration, string section) where T : new()
        {
            var sectionSettings = configuration.GetSection(section);

            var cs = new T();
            sectionSettings.Bind(cs);

            return cs;
        }

        public static ConnectionStringCollection GetConnectionStrings(this IConfiguration configuration)
        {
            return new ConnectionStringCollection(configuration.GetSettings<ConnectionStringSettings>("ConnectionStrings"));
        }
        public static ConnectionStringSettings GetConnectionStrings(this IConfiguration configuration, string key)
        {
            return configuration.GetConnectionStrings()[key];
        }
    }
}
