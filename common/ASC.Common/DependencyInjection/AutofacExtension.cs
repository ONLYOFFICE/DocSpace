using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Autofac;
using Autofac.Configuration;

using Microsoft.Extensions.Configuration;

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
        public static void Register(this ContainerBuilder builder, IConfiguration configuration, string currentDir, bool loadproducts = true, bool loadconsumers = true, params string[] intern)
        {
            var folder = configuration["core:products:folder"];
            var subfolder = configuration["core:products:subfolder"];
            string productsDir;

            if (!Path.IsPathRooted(folder))
            {
                if (currentDir.EndsWith(Path.Combine(Path.GetFileName(folder), Assembly.GetEntryAssembly().GetName().Name, subfolder)))
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

            return;

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

                if (!string.IsNullOrEmpty(path))
                {
                    AssemblyLoadContext.Default.Resolving += new Resolver(path).Resolving;
                }
            }

            string GetFullPath(string n)
            {
                var productPath = Path.Combine(productsDir, n, subfolder);
                return GetPath(Path.Combine(productPath, "bin"), n, SearchOption.AllDirectories) ?? GetPath(productPath, n, SearchOption.TopDirectoryOnly);
            }

            static string GetPath(string dirPath, string dll, SearchOption searchOption)
            {
                if (!Directory.Exists(dirPath)) return null;

                return Directory.GetFiles(dirPath, $"{dll}.dll", searchOption).FirstOrDefault();
            }
        }
    }

    class Resolver
    {
        private string ResolvePath { get; set; }

        public Resolver(string assemblyPath)
        {
            ResolvePath = assemblyPath;
        }

        public Assembly Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var path = Path.Combine(Path.GetDirectoryName(ResolvePath), $"{assemblyName.Name}.dll");

            if (!File.Exists(path)) return null;

            return context.LoadFromAssemblyPath(path);
        }
    }
}
