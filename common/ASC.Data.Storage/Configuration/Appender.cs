using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using ASC.Common;
using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Storage.Configuration
{
    public static class StorageConfigExtension
    {
        public static DIHelper AddStorage(this DIHelper services)
        {
            services.TryAddSingleton(r => r.GetService<IConfiguration>().GetSetting<Storage>("Storage"));
            return services;
        }
    }

    public class Storage
    {
        public IEnumerable<Appender> Appender { get; set; }
        public IEnumerable<Handler> Handler { get; set; }
        public IEnumerable<Module> Module { get; set; }

        public Module GetModuleElement(string name)
        {
            return Module?.FirstOrDefault(r => r.Name == name);
        }
        public Handler GetHandler(string name)
        {
            return Handler?.FirstOrDefault(r => r.Name == name);
        }
    }

    public class Appender
    {
        public string Name { get; set; }
        public string Append { get; set; }
        public string AppendSecure { get; set; }
        public string Extensions { get; set; }
    }

    public class Handler
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IEnumerable<Properties> Property { get; set; }

        public IDictionary<string, string> GetProperties()
        {
            if (Property == null || !Property.Any()) return new Dictionary<string, string>();

            return Property.ToDictionary(r => r.Name, r => r.Value);
        }
    }
    public class Properties
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Module : ConfigurationElement
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public ACL Acl { get; set; }
        public string VirtualPath { get; set; }
        public TimeSpan Expires { get; set; }
        public IEnumerable<Module> Domain { get; set; }

        [ConfigurationProperty("visible", DefaultValue = true)]
        public bool Visible
        {
            get { return (bool)this["visible"]; }
            set { this["visible"] = value; }
        }

        [ConfigurationProperty("appendTenantId", DefaultValue = true)]
        public bool AppendTenantId
        {
            get { return (bool)this["appendTenantId"]; }
            set { this["appendTenantId"] = value; }
        }

        [ConfigurationProperty("public", DefaultValue = true)]
        public bool Public
        {
            get { return (bool)this["public"]; }
            set { this["public"] = value; }
        }

        [ConfigurationProperty("disableMigrate", DefaultValue = true)]
        public bool DisableMigrate
        {
            get { return (bool)this["disableMigrate"]; }
            set { this["disableMigrate"] = value; }
        }

        [ConfigurationProperty("count", DefaultValue = true)]
        public bool Count
        {
            get { return (bool)this["count"]; }
            set { this["count"] = value; }
        }
    }
}

