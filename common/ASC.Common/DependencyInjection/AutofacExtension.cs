using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static IContainer AddAutofac(this IServiceCollection services, IConfiguration configuration, string currentDir)
        {
            var folder = configuration["core:products:folder"];
            var subfolder = configuration["core:products:subfolder"];
            string productsDir;

            if (currentDir.EndsWith(Path.Combine(Path.GetFileName(folder), Assembly.GetCallingAssembly().GetName().Name, subfolder)))
            {
                productsDir = Path.GetFullPath(Path.Combine("..", ".."));
            }
            else
            {
                productsDir = Path.GetFullPath(Path.Combine(currentDir, folder));
            }

            var module = new ConfigurationModule(configuration);
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);

            var sectionSettings = configuration.GetSection("products");

            if (sectionSettings != null)
            {
                var cs = new List<AutofacComponent>();
                sectionSettings.Bind(cs);

                foreach (var component in cs)
                {
                    try
                    {
                        var types = new List<string>();
                        LoadAssembly(component.Type);

                        foreach (var s in component.Services)
                        {
                            //LoadAssembly(s.Type);
                            types.Add(s.Type);
                        }

                        builder.RegisterType(Type.GetType(component.Type)).As(types.Select(r => Type.GetType(r)).ToArray());
                    }
                    catch (System.Exception)
                    {
                        //TODO
                    }
                }
            }

            var container = builder.Build();

            services.TryAddSingleton(container);

            return container;

            void LoadAssembly(string type)
            {
                var dll = type.Substring(type.IndexOf(",") + 1).Trim();
                var productPath = Path.Combine(productsDir, dll, subfolder);
                var path = GetPath(Path.Combine(productPath, "bin"), dll, SearchOption.AllDirectories) ?? GetPath(productPath, dll, SearchOption.TopDirectoryOnly);

                if (!string.IsNullOrEmpty(path))
                {
                    AssemblyLoadContext.Default.Resolving += Default_Resolving(path);

                    Func<AssemblyLoadContext, AssemblyName, Assembly> Default_Resolving(string path)
                    {
                        return (c, n) =>
                        {
                            return c.LoadFromAssemblyPath(Path.Combine(Path.GetDirectoryName(path), $"{n.Name}.dll"));
                        };
                    }
                }
            }

            string GetPath(string dirPath, string dll, SearchOption searchOption)
            {
                if (!Directory.Exists(dirPath)) return null;

                return Directory.GetFiles(dirPath, $"{dll}.dll", searchOption).FirstOrDefault();
            }
        }
    }
}
