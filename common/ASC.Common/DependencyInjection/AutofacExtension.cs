// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Common.DependencyInjection;

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
    public static void Register(this ContainerBuilder builder, IConfiguration configuration,
        bool loadproducts = true, bool loadconsumers = true, params string[] intern)
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
            if (currentDir.TrimEnd('\\').EndsWith(CrossPlatform.PathCombine(Path.GetFileName(folder), Assembly.GetEntryAssembly().GetName().Name, subfolder)))
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
            catch (Exception)
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

            return GetPath(CrossPlatform.PathCombine(productPath, "bin"), n, SearchOption.AllDirectories)
                ?? GetPath(productPath, n, SearchOption.TopDirectoryOnly);
        }

        static string GetPath(string dirPath, string dll, SearchOption searchOption)
        {
            if (!Directory.Exists(dirPath))
            {
                return null;
            }

            return Directory.GetFiles(dirPath, $"{dll}.dll", searchOption).FirstOrDefault();
        }
    }
}

class Resolver
{
    private readonly string _resolvePath;

    public Resolver(string assemblyPath)
    {
        _resolvePath = assemblyPath;
    }

    public Assembly Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var path = CrossPlatform.PathCombine(Path.GetDirectoryName(_resolvePath), $"{assemblyName.Name}.dll");

        if (!File.Exists(path))
        {
            return null;
        }

        return context.LoadFromAssemblyPath(path);
    }
}