﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using ASC.Common.Utils;

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
        public static void Register(this ContainerBuilder builder, IConfiguration configuration, bool loadproducts = true, bool loadconsumers = true, params string[] intern)
        {
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
            }

            return;
        }

        public static List<string> FindAndLoad(IConfiguration configuration, string currentDir, string section = "autofac.products.json")
        {
            var config = new ConfigurationBuilder();
            config.SetBasePath(configuration["pathToConf"]);
            config.AddJsonFile(section);
            var root = config.Build();

            var sectionSettings = root.GetSection("components");

            if (sectionSettings == null)
            {
                return new List<string>();
            }

            var folder = configuration["core:products:folder"];
            var subfolder = configuration["core:products:subfolder"];
            string productsDir;

            if (!Path.IsPathRooted(folder))
            {
                if (currentDir.EndsWith(CrossPlatform.PathCombine(Path.GetFileName(folder), Assembly.GetEntryAssembly().GetName().Name, subfolder)))
                {
                    productsDir = Path.GetFullPath(CrossPlatform.PathCombine("..", ".."));
                }
                else
                {
                    productsDir = Path.GetFullPath(CrossPlatform.PathCombine(currentDir, folder));
                }
            }
            else
            {
                productsDir = folder;
            }

            var cs = new List<AutofacComponent>();
            sectionSettings.Bind(cs);

            var types = new List<string>();

            foreach (var component in cs)
            {
                try
                {
                    LoadAssembly(component.Type);
                    types.Add(component.Type);
                }
                catch (System.Exception)
                {
                    //TODO
                }
            }

            return types;

            void LoadAssembly(string type)
            {
                var dll = type.Substring(type.IndexOf(',') + 1).Trim();
                var path = GetFullPath(dll);

                if (!string.IsNullOrEmpty(path))
                {
                    AssemblyLoadContext.Default.Resolving += new Resolver(path).Resolving;
                }
            }

            string GetFullPath(string n)
            {
                var productPath = CrossPlatform.PathCombine(productsDir, n, subfolder);
                return GetPath(CrossPlatform.PathCombine(productPath, "bin"), n, SearchOption.AllDirectories) ?? GetPath(productPath, n, SearchOption.TopDirectoryOnly);
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
            var path = CrossPlatform.PathCombine(Path.GetDirectoryName(ResolvePath), $"{assemblyName.Name}.dll");

            if (!File.Exists(path)) return null;

            return context.LoadFromAssemblyPath(path);
        }
    }
}
