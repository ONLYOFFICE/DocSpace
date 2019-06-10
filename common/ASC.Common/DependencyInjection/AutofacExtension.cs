using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.DependencyInjection
{
    internal class AutofacComponent
    {
        public string Type { get; set; }
        public IEnumerable<AutofacService> Services { get; set; }
    }
    internal class AutofacService
    {
        public string Type { get; set; }
    }

    public static class AutofacExtension
    {
        public static IContainer AddAutofac(this IServiceCollection services, IConfiguration configuration)
        {
            var sectionSettings = configuration.GetSection("components");

            var cs = new List<AutofacComponent>();
            sectionSettings.Bind(cs);

            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (var component in cs)
            {
                try
                {
                    LoadAssembly(component.Type);

                    foreach (var s in component.Services)
                    {
                        LoadAssembly(s.Type);
                    }
                }
                catch (System.Exception)
                {
                    //TODO
                }
            }


            var module = new ConfigurationModule(configuration);
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);

            var container = builder.Build();

            services.AddSingleton(container);

            return container;

            void LoadAssembly(string type)
            {
                var path = Path.Combine(currentDir, type.Substring(type.IndexOf(",") + 1).Trim());
                Assembly.LoadFrom($"{path}.dll");
            }
        }
    }
}
