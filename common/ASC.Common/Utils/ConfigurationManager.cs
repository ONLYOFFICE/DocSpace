using System;
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

    [Singletone]
    public class ConfigurationExtension
    {
        private IConfiguration Configuration { get; }
        private Lazy<ConnectionStringCollection> ConnectionStringSettings { get; }

        public ConfigurationExtension(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionStringSettings = new Lazy<ConnectionStringCollection>(new ConnectionStringCollection(GetSettings<ConnectionStringSettings>("ConnectionStrings")));
        }

        public IEnumerable<T> GetSettings<T>(string section) where T : new()
        {
            var result = new List<T>();

            var sectionSettings = Configuration.GetSection(section);

            foreach (var ch in sectionSettings.GetChildren())
            {
                var cs = new T();
                ch.Bind(cs);
                result.Add(cs);
            }

            return result;
        }

        public T GetSetting<T>(string section) where T : new()
        {
            return GetSetting(section, new T());
        }

        public T GetSetting<T>(string section, T instance)
        {
            var sectionSettings = Configuration.GetSection(section);

            sectionSettings.Bind(instance);

            return instance;
        }

        public ConnectionStringCollection GetConnectionStrings()
        {
            return ConnectionStringSettings.Value;
        }

        public ConnectionStringSettings GetConnectionStrings(string key)
        {
            return GetConnectionStrings()[key];
        }

        public string this[string key]
        {
            get => Configuration[key];
            set => Configuration[key] = value;
        }
    }
}
