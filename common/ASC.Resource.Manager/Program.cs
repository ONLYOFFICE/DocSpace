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


namespace ASC.Resource.Manager;

class Program
{
    private const string CsProjScheme = "http://schemas.microsoft.com/developer/msbuild/2003";
    private static readonly XName ItemGroupXnameOld = XName.Get("ItemGroup", CsProjScheme);
    private static readonly XName EmbededXnameOld = XName.Get("EmbeddedResource", CsProjScheme);
    private static readonly XName DependentUponOld = XName.Get("DependentUpon", CsProjScheme);
    private static readonly XName ItemGroupXname = XName.Get("ItemGroup");
    private static readonly XName EmbededXname = XName.Get("EmbeddedResource");
    private static readonly XName DependentUpon = XName.Get("DependentUpon");
    private const string IncludeAttribute = "Include";
    private const string UpdateAttribute = "Update";
    private const string ConditionAttribute = "Condition";
    public static string[] Args;
    public static void Main(string[] args)
    {
        Args = args;

        var copy = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--pathToConf" || args[i] == "--ConnectionStrings:default:connectionString")
            {
                i++;
                continue;
            }
            copy.Add(args[i]);
        }

        Parser.Default.ParseArguments<Options>(copy).WithParsed(Export);
    }

    public static void Export(Options options)
    {
        //var csPath = @"C:\Git\portals_core\web\ASC.Web.Core\";
        //AddResourceForCsproj($"{csPath}ASC.Web.Core.csproj",
        //    Directory.EnumerateFiles(csPath, "*.resx", SearchOption.AllDirectories).Select(r => new Tuple<string, string>("", r.Substring(csPath.Length))));

        //return;

        var services = new ServiceCollection();
        var startup = new Startup(Args);
        startup.ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<ProgramScope>();

        var cultures = new List<string>();
        var projects = new List<ResFile>();
        var enabledSettings = new EnabledSettings();
        Func<IServiceProvider, string, string, string, string, string, string, bool> export = null;

        try
        {
            var (project, module, filePath, exportPath, culture, format, key) = options;

            //project = "CRM";
            //module = "Common";
            //filePath = "FilesCommonResource.resx";
            //culture = "ru";
            //exportPath = @"C:\Git\portals\";
            //key = "*,HtmlMaster*";
            //key = "*";

            if (format == "json")
            {
                export = JsonManager.Export;
            }
            else
            {
                export = ResxManager.Export;
            }

            if (string.IsNullOrEmpty(exportPath))
            {
                exportPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            if (!Path.IsPathRooted(exportPath))
            {
                exportPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), exportPath));
            }

            if (!Directory.Exists(exportPath))
            {
                Console.WriteLine("Error!!! Export path doesn't exist! Please enter a valid directory path.");
                return;
            }

            enabledSettings = scopeClass.Configuration.GetSetting<EnabledSettings>("enabled");
            cultures = scopeClass.ResourceData.GetCultures().Where(r => r.Available).Select(r => r.Title).ToList();//.Intersect(enabledSettings.Langs).ToList();
            projects = scopeClass.ResourceData.GetAllFiles();

            ExportWithProject(project, module, filePath, culture, exportPath, key);

            Console.WriteLine("The data has been successfully exported!");
        }
        catch (Exception err)
        {
            Console.WriteLine(err);
        }

        void ExportWithProject(string projectName, string moduleName, string fileName, string culture, string exportPath, string key = null)
        {
            if (!string.IsNullOrEmpty(projectName))
            {
                ExportWithModule(projectName, moduleName, fileName, culture, exportPath, key);
            }
            else
            {
                var projectToExport = projects
                    .Where(r => string.IsNullOrEmpty(r.ModuleName) || r.ModuleName == moduleName)
                    .Where(r => string.IsNullOrEmpty(r.FileName) || r.FileName == fileName)
                    .Select(r => r.ProjectName)
                    .Intersect(enabledSettings.Projects);

                foreach (var p in projectToExport)
                {
                    ExportWithModule(p, moduleName, fileName, culture, exportPath, key);
                }
            }
        }

        void ExportWithModule(string projectName, string moduleName, string fileName, string culture, string exportPath, string key = null)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                ExportWithFile(projectName, moduleName, fileName, culture, exportPath, key);
            }
            else
            {
                var moduleToExport = projects
                    .Where(r => r.ProjectName == projectName)
                    .Where(r => string.IsNullOrEmpty(fileName) || r.FileName == fileName)
                    .Select(r => r.ModuleName)
                    .Distinct();

                foreach (var m in moduleToExport)
                {
                    ExportWithFile(projectName, m, fileName, culture, exportPath, key);
                }
            }
        }
        void ExportWithFile(string projectName, string moduleName, string fileName, string culture, string exportPath, string key = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                ExportWithCulture(projectName, moduleName, fileName, culture, exportPath, key);
            }
            else
            {
                foreach (var f in projects.Where(r => r.ProjectName == projectName && r.ModuleName == moduleName).Select(r => r.FileName))
                {
                    ExportWithCulture(projectName, moduleName, f, culture, exportPath, key);
                }
            }
        }
        void ExportWithCulture(string projectName, string moduleName, string fileName, string culture, string exportPath, string key)
        {
            var filePath = Directory.GetFiles(exportPath, $"{fileName}", SearchOption.AllDirectories).FirstOrDefault();

            if (!string.IsNullOrEmpty(culture))
            {
                exportPath = Path.GetDirectoryName(filePath);
                export(serviceProvider, projectName, moduleName, fileName, culture, exportPath, key);

                Console.WriteLine(filePath);
            }
            else
            {
                var resultFiles = new ConcurrentBag<Tuple<string, string>>();

                var asmbl = "";
                var assmlPath = "";
                var nsp = "";

                var keys = key.Split(",");
                if (keys.Contains("*"))
                {
                    if (string.IsNullOrEmpty(filePath)) return;

                    assmlPath = Path.GetDirectoryName(filePath);

                    var name = Path.GetFileNameWithoutExtension(fileName);
                    var designerPath = Path.Combine(Path.GetDirectoryName(filePath), $"{name}.Designer.cs");
                    var data = File.ReadAllText(designerPath);
                    var regex = new Regex(@"namespace\s(\S*)\s", RegexOptions.IgnoreCase);
                    var matches = regex.Matches(data);
                    if (!matches.Any() || matches[0].Groups.Count < 2)
                    {
                        return;
                    }

                    //File.Delete(designerPath);

                    nsp = matches[0].Groups[1].Value;

                    do
                    {
                        asmbl = Directory.GetFiles(assmlPath, "*.csproj").FirstOrDefault();
                        if (string.IsNullOrEmpty(asmbl))
                        {
                            assmlPath = Path.GetFullPath(Path.Combine(assmlPath, ".."));
                        }
                    }
                    while (string.IsNullOrEmpty(asmbl));

                    regex = new Regex(@"\<AssemblyName\>(\S*)\<\/AssemblyName\>", RegexOptions.IgnoreCase);
                    matches = regex.Matches(File.ReadAllText(asmbl));
                    string assName = "";
                    if (!matches.Any() || matches[0].Groups.Count < 2)
                    {
                        assName = Path.GetFileNameWithoutExtension(asmbl);
                    }
                    else
                    {
                        assName = matches[0].Groups[1].Value;
                    }

                    key = CheckExist(fileName, $"{nsp}.{name},{assName}", exportPath);
                    var additional = string.Join(",", keys.Where(r => r.Length > 1 && r.Contains("*")).ToArray());

                    if (!string.IsNullOrEmpty(additional))
                    {
                        key += "," + additional;
                    }

                    exportPath = Path.GetDirectoryName(filePath);
                }
                else
                {
                    if (export != JsonManager.Export)
                    {
                        exportPath = Path.GetDirectoryName(filePath);
                    }
                }

                if (string.IsNullOrEmpty(exportPath))
                {
                    return;
                }

                var exportPath1 = exportPath;

                ParallelEnumerable.ForAll(cultures.AsParallel(), c =>
                {
                    if (export == JsonManager.Export)
                    {
                        var files = Directory.GetFiles(exportPath1, $"{fileName}", SearchOption.AllDirectories);

                        exportPath = files.FirstOrDefault(r => Path.GetDirectoryName(r) == c);
                        if (exportPath == null)
                        {
                            exportPath = Path.GetDirectoryName(Path.GetDirectoryName(files.FirstOrDefault()));
                        }
                    }

                    var any = export(serviceProvider, projectName, moduleName, fileName, c, exportPath, key);
                    if (any)
                    {
                        resultFiles.Add(new Tuple<string, string>(c, $"{filePath.Replace(".resx", (c == "Neutral" ? $".resx" : $".{c}.resx"))}".Substring(assmlPath.Length + 1)));
                    }
                });

                Console.WriteLine(filePath);

                if (string.IsNullOrEmpty(asmbl)) return;
                AddResourceForCommunityCsproj(asmbl, filePath.Substring(assmlPath.Length + 1), resultFiles.OrderBy(r => r.Item2));
                //AddResourceForCsproj(asmbl, resultFiles.OrderBy(r => r.Item2));

                var assmblName = Path.GetFileNameWithoutExtension(asmbl);
                var f = Path.GetDirectoryName(filePath.Substring(assmlPath.Length + 1)).Replace('\\', '.');
                nsp = assmblName;

                if (!string.IsNullOrEmpty(f))
                {
                    nsp += "." + Path.GetDirectoryName(filePath.Substring(assmlPath.Length + 1)).Replace('\\', '.');
                }

                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = scopeClass.Configuration["resGen"],
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"{Path.GetFileName(filePath)} /str:cs,{nsp},{Path.GetFileNameWithoutExtension(filePath)},{Path.GetFileNameWithoutExtension(filePath)}.Designer.cs /publicClass",
                    WorkingDirectory = Path.GetDirectoryName(filePath)
                };

                using (var p = Process.Start(startInfo))
                {
                    if (p.WaitForExit(10000))
                    {
                        Console.WriteLine($"{Path.GetFileNameWithoutExtension(filePath)}.Designer.cs");
                    }
                    p.Close();
                }

                var resourcesFile = filePath.Replace(".resx", ".resources");
                if (File.Exists(resourcesFile))
                {
                    File.Delete(resourcesFile);
                }
            }
        }
    }

    public static string CheckExist(string fileName, string fullClassName, string path)
    {
        var resName = Path.GetFileNameWithoutExtension(fileName);
        var bag = new ConcurrentBag<string>();

        var csFiles = Directory.GetFiles(Path.GetFullPath(path), "*.cs", SearchOption.AllDirectories).Except(Directory.GetFiles(Path.GetFullPath(path), "*Resource.Designer.cs", SearchOption.AllDirectories));
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.cshtml", SearchOption.AllDirectories)).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.aspx", SearchOption.AllDirectories)).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.Master", SearchOption.AllDirectories)).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.ascx", SearchOption.AllDirectories)).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.html", SearchOption.AllDirectories)).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.js", SearchOption.AllDirectories).Where(r => !r.Contains("node_modules"))).ToArray();
        csFiles = csFiles.Concat(Directory.GetFiles(Path.GetFullPath(path), "*.xsl", SearchOption.AllDirectories)).ToArray();
        var xmlFiles = Directory.GetFiles(Path.GetFullPath(path), "*.xml", SearchOption.AllDirectories);

        string localInit() => "";

        Func<string, ParallelLoopState, long, string, string> func(string regexp) => (f, state, index, a) =>
        {
            var data = File.ReadAllText(f);
            var regex = new Regex(regexp, RegexOptions.IgnoreCase);
            var matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                var result = string.Join(",", matches.Select(r => r.Groups[1].Value));
                if (!string.IsNullOrEmpty(a))
                    return a + "," + result;

                return result;
            }
            return a;
        };

        void localFinally(string r)
        {
            if (!bag.Contains(r) && !string.IsNullOrEmpty(r))
            {
                bag.Add(r.Trim(','));
            }
        }

        _ = Parallel.ForEach(csFiles, localInit, func(@$"\W+{resName}\.(\w*)"), localFinally);
        _ = Parallel.ForEach(csFiles, localInit, func(@$"CustomNamingPeople\.Substitute\<{resName}\>\(""(\w*)""\)"), localFinally);
        _ = Parallel.ForEach(csFiles, localInit, func(@$"{resName}\.ResourceManager\.GetString\(""(\w*)""[\),\,]"), localFinally);
        _ = Parallel.ForEach(csFiles, localInit, func(@$"{resName}\.ResourceManager\.GetString\(""(\w*)""\s*\+"), (r) =>
        {

            if (!bag.Contains(r) && !string.IsNullOrEmpty(r))
            {
                bag.Add(r.Replace(",", "*,").Trim(',') + "*");
            }
        });
        _ = Parallel.ForEach(xmlFiles, localInit, func(@$"\|(\w*)\|{fullClassName.Replace(".", "\\.")}"), localFinally);

        if (fileName == "TipsResource.resx")
        {
            _ = Parallel.ForEach(xmlFiles, localInit, func(@$"<tip id=""(\w*)"""), (r) =>
            {

                if (!string.IsNullOrEmpty(r))
                {
                    var ids = r.Split(',');
                    foreach (var id in ids)
                    {
                        bag.Add(id);
                        bag.Add($"{id}MessageBody");
                        bag.Add($"{id}MessageHeader");
                    }
                }
            });
        }

        if (fileName == "AuditReportResource.resx")
        {
            _ = Parallel.ForEach(csFiles.Where(r => r.EndsWith("ActionMapper.cs")), localInit, func(@$"ResourceName\s*=\s*""(\w*)"""), localFinally);
            _ = Parallel.ForEach(csFiles, localInit, func(@$"\[Event\(""(\w*)"""), localFinally);
        }

        if (fileName == "NamingPeopleResource.resx")
        {
            _ = Parallel.ForEach(xmlFiles.Where(r => r.EndsWith("PeopleNames.xml")), localInit, func(@$"\>(\w*)\<"), localFinally);
        }

        return string.Join(',', bag.ToArray().Distinct());
    }

    private static void AddResourceForCommunityCsproj(string csproj, string fileName, IEnumerable<Tuple<string, string>> files)
    {
        if (!files.Any()) return;

        var doc = XDocument.Parse(File.ReadAllText(csproj));
        if (doc.Root == null) return;

        foreach (var file in files)
        {
            var node = doc.Root.Elements().FirstOrDefault(r =>
            {
                var elements = r.Elements(EmbededXnameOld);
                return
                r.Name == ItemGroupXnameOld &&
                r.Elements(EmbededXnameOld).Any(x =>
                {
                    var attr = x.Attribute(IncludeAttribute);
                    return attr != null && attr.Value == fileName;
                });
            }) ??
            doc.Root.Elements().FirstOrDefault(r =>
            r.Name == ItemGroupXnameOld &&
            r.Elements(EmbededXnameOld).Any());

            XElement reference;
            bool referenceNotExist;

            if (node == null)
            {
                node = new XElement(ItemGroupXnameOld);
                doc.Root.Add(node);
                reference = new XElement(EmbededXnameOld);
                referenceNotExist = true;
            }
            else
            {
                var embeded = node.Elements(EmbededXnameOld).ToList();

                reference = embeded.FirstOrDefault(r =>
                {
                    var attr = r.Attribute(IncludeAttribute);
                    return attr != null && attr.Value == file.Item2;
                });

                referenceNotExist = reference == null;
                if (referenceNotExist)
                {
                    reference = new XElement(EmbededXnameOld);
                    if (file.Item2 != fileName)
                    {
                        reference.Add(new XElement(DependentUponOld, Path.GetFileName(fileName)));
                    }
                }
            }

            if (referenceNotExist)
            {
                reference.SetAttributeValue(IncludeAttribute, file.Item2);
                reference.SetAttributeValue(ConditionAttribute, string.Format("$(Cultures.Contains('{0}'))", file.Item1));
                node.Add(reference);
            }
        }

        doc.Save(csproj);
    }

    private static void AddResourceForCsproj(string csproj, IEnumerable<Tuple<string, string>> files)
    {
        if (!files.Any()) return;

        var doc = XDocument.Parse(File.ReadAllText(csproj));
        if (doc.Root == null) return;

        foreach (var file in files)
        {
            var fileName = $"{file.Item2.Split('.')[0]}.resx";
            var node = doc.Root.Elements().FirstOrDefault(r =>
            r.Name == ItemGroupXname &&
            r.Elements(EmbededXname).Any(x =>
            {
                var attr = x.Attribute(UpdateAttribute);
                return attr != null && attr.Value == fileName;
            })) ??
            doc.Root.Elements().FirstOrDefault(r =>
            r.Name == ItemGroupXname &&
            r.Elements(EmbededXname).Any());

            XElement reference;
            bool referenceNotExist;

            if (node == null)
            {
                node = new XElement(ItemGroupXname);
                doc.Root.Add(node);
                reference = new XElement(EmbededXname);
                referenceNotExist = true;
            }
            else
            {
                var embeded = node.Elements(EmbededXname).ToList();

                reference = embeded.FirstOrDefault(r =>
                {
                    var attr = r.Attribute(UpdateAttribute);
                    return attr != null && attr.Value == file.Item2;
                });

                referenceNotExist = reference == null;
                if (referenceNotExist)
                {
                    reference = new XElement(EmbededXname);
                    if (file.Item2 != fileName)
                    {
                        reference.Add(new XElement(DependentUpon, Path.GetFileName(fileName)));
                    }
                }
            }

            if (referenceNotExist)
            {
                reference.SetAttributeValue(UpdateAttribute, file.Item2);
                //reference.SetAttributeValue(ConditionAttribute, string.Format("$(Cultures.Contains('{0}'))", file.Item1));
                node.Add(reference);
            }
        }

        doc.Save(csproj);
    }

    private static void Sort(string path)
    {
        foreach (var f in Directory.GetFiles(path))
        {
            if (File.ReadAllText(f) == "{}")
            {
                File.Delete(f);
                continue;
            }

            var name = Path.GetFileName(f);
            var baseDirName = Path.GetDirectoryName(f);
            var ext = name.Split('.');
            string dirName;
            if (ext.Length <= 2)
            {
                dirName = "en";
            }
            else
            {
                dirName = ext[^2];
                name = name.Replace(ext[^2] + ".", "");
            }

            dirName = Path.Combine(baseDirName, dirName);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            File.Move(f, Path.Combine(dirName, name));
        }
    }

    private static void SortFromFolder(string path)
    {
        foreach (var f in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
            if (File.ReadAllText(f) == "{}") continue;

            var name = Path.GetFileName(f);
            var baseDir = Path.GetDirectoryName(f);
            var baseDirName = Path.GetFileName(baseDir);

            if (baseDirName != "en")
            {
                name = name.Replace(".json", $".{baseDirName}.json");
            }


            File.Move(f, Path.GetFullPath(Path.Combine(baseDir, "..", name)));
        }
    }

    private static void SortJson(string path)
    {
        foreach (var f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            var text = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(f, Encoding.UTF8));
            text = text.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);
            File.WriteAllText(f, JsonSerializer.Serialize(text, new JsonSerializerOptions() { WriteIndented = true }), Encoding.UTF8);
        }
    }
}

[Scope]
public class ProgramScope
{
    internal ResourceData ResourceData { get; }
    internal ConfigurationExtension Configuration { get; }

    public ProgramScope(ResourceData resourceData, ConfigurationExtension configuration)
    {
        ResourceData = resourceData;
        Configuration = configuration;
    }
}
