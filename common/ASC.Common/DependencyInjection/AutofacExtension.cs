using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
        public static IContainer AddAutofac(this IServiceCollection services, IConfiguration configuration, ILogger logger, string currentDir, bool loadproducts = true, bool loadconsumers = true, params string[] intern)
        {
            var folder = configuration["core:products:folder"];
            var subfolder = configuration["core:products:subfolder"];
            string productsDir;

            if (logger != null)
            {
                logger.LogInformation($"{folder}");
                logger.LogInformation($"{subfolder}");
            }

            if (!Path.IsPathRooted(folder))
            {
                if (currentDir.EndsWith(Path.Combine(Path.GetFileName(folder), Assembly.GetCallingAssembly().GetName().Name, subfolder)))
                {
                    productsDir = Path.GetFullPath(Path.Combine("..", ".."));
                }
                else
                {
                    productsDir = Path.GetFullPath(Path.Combine(currentDir, folder));
                }
            }
            else
            {
                productsDir = folder;
            }

            if (logger != null)
            {
                logger.LogInformation($"{productsDir}");
            }

            var builder = new ContainerBuilder();
            var modules = new List<(bool, string)>
            {
                (true, "autofac.json")
            };

            if (loadproducts)
            {
                modules.Add((true, "autofac.products.json"));
            }

            if (loadconsumers)
            {
                modules.Add((true, "autofac.consumers.json"));
            }

            if (intern != null)
            {
                modules.AddRange(intern.Select(r => (false, r)));
            }

            foreach (var p in modules)
            {
                var config = new ConfigurationBuilder();
                if (p.Item1)
                {
                    config.SetBasePath(configuration["pathToConf"]);
                }
                config.AddJsonFile(p.Item2);

                var root = config.Build();
                var module = new ConfigurationModule(root);
                builder.RegisterModule(module);

                if (p.Item2 == "autofac.products.json")
                {
                    FindAndLoad(root.GetSection("components"));
                }
            }

            builder.Populate(services);

            var container = builder.Build();

            services.TryAddSingleton(container);

            return container;

            void FindAndLoad(IConfigurationSection sectionSettings)
            {
                if (sectionSettings == null)
                {
                    return;
                }
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
                    }
                    catch (System.Exception)
                    {
                        //TODO
                    }
                }
            }

            void LoadAssembly(string type)
            {
                var dll = type.Substring(type.IndexOf(",") + 1).Trim();
                var path = GetFullPath(dll);

                if (logger != null)
                {
                    logger.LogInformation($"LoadAssembly {path}");
                }

                if (!string.IsNullOrEmpty(path))
                {
                    AssemblyLoadContext.Default.Resolving += (c, n) =>
                    {
                        var path = GetFullPath(n.Name);

                        if (logger != null)
                        {
                            logger.LogInformation($"Resolve {path}");
                        }

                        return path != null ?
                                c.LoadFromAssemblyPath(Path.Combine(Path.GetDirectoryName(path), $"{n.Name}.dll")) :
                                null;
                    };
                }
            }

            string GetFullPath(string n)
            {
                var productPath = Path.Combine(productsDir, n, subfolder);
                return GetPath(Path.Combine(productPath, "bin"), n, SearchOption.AllDirectories) ?? GetPath(productPath, n, SearchOption.TopDirectoryOnly);
            }

            string GetPath(string dirPath, string dll, SearchOption searchOption)
            {
                if (!Directory.Exists(dirPath)) return null;

                return Directory.GetFiles(dirPath, $"{dll}.dll", searchOption).FirstOrDefault();
            }
        }
    }
}
