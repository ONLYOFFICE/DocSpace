using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace ASC.Common.Utils
{
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
