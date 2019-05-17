using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ASC.Common.Utils
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSettings { get => CommonServiceProvider.GetService<IConfiguration>(); }
        public static ConnectionStringCollection ConnectionStrings { get => new ConnectionStringCollection(GetSettings<ConnectionStringSettings>("ConnectionStrings")); }

        public static IEnumerable<T> GetSettings<T> (string section) where T : new ()
        {
            var result = new List<T>();

            var sectionSettings = AppSettings.GetSection(section);

            foreach (var ch in sectionSettings.GetChildren())
            {
                var cs = new T();
                ch.Bind(cs);
                result.Add(cs);
            }

            return result;
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
                return Data.FirstOrDefault(r=> r.Name == name);
            }
        }
    }
}
