using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace Frontend.Translations.Tests
{
    public class Tests
    {
        public static string BasePath
        {
            get
            {
                return "..\\..\\..\\..\\..\\..\\";
            }
        }

        public List<string> Workspaces { get; set; }

        public List<TranslationFile> TranslationFiles { get; set; }
        public List<JavaScriptFile> JavaScriptFiles { get; set; }
        public List<ModuleFolder> ModuleFolders { get; set; }

        public List<LanguageItem> CommonTranslations { get; set; }

        [SetUp]
        public void Setup()
        {
            var packageJsonPath = Path.Combine(BasePath, @"package.json");

            var jsonPackage = JObject.Parse(File.ReadAllText(packageJsonPath));

            var moduleWorkspaces = ((JArray)jsonPackage["workspaces"]).Select(p => ((string)p).Replace("/", "\\")).ToList();

            Workspaces = new List<string>();

            Workspaces.AddRange(moduleWorkspaces);

            Workspaces.Add("public\\locales");

            var translationFiles = from wsPath in Workspaces
                                   let clientDir = Path.Combine(BasePath, wsPath)
                                   from file in Directory.EnumerateFiles(clientDir, "*.json", SearchOption.AllDirectories)
                                   where file.Contains("public\\locales\\")
                                   select file;

            TranslationFiles = new List<TranslationFile>();

            foreach (var path in translationFiles)
            {
                var jsonTranslation = JObject.Parse(File.ReadAllText(path));

                var translationFile = new TranslationFile(path);

                translationFile.Translations = jsonTranslation.Properties()
                    .Select(p => new TranslationItem(p.Name, (string)p.Value))
                    .ToList();

                TranslationFiles.Add(translationFile);

                /*   Re-write by order */

                //var orderedList = jsonTranslation.Properties().OrderBy(t => t.Name);

                //var result = new JObject(orderedList);

                //var sortedJsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

                //File.WriteAllText(path, sortedJsonString);
            }

            var javascriptFiles = (from wsPath in Workspaces
                                   let clientDir = Path.Combine(BasePath, wsPath)
                                   from file in Directory.EnumerateFiles(clientDir, "*.js", SearchOption.AllDirectories)
                                   where !file.Contains("dist\\")
                                   select file)
                                  .ToList();

            javascriptFiles.AddRange(from wsPath in Workspaces
                                     let clientDir = Path.Combine(BasePath, wsPath)
                                     from file in Directory.EnumerateFiles(clientDir, "*.jsx", SearchOption.AllDirectories)
                                     where !file.Contains("dist\\")
                                     select file);

            JavaScriptFiles = new List<JavaScriptFile>();

            var pattern1 = "[.{\\s\\(]t\\(\\s*[\"\'`]([a-zA-Z0-9_.:_\\s{}/_-]+)[\"\'`]\\s*[\\),]";
            var pattern2 = "i18nKey=\"([a-zA-Z0-9_.-]+)\"";

            var regexp = new Regex($"({pattern1})|({pattern2})", RegexOptions.Multiline | RegexOptions.ECMAScript);

            foreach (var path in javascriptFiles)
            {
                var jsFileText = File.ReadAllText(path);

                var matches = regexp.Matches(jsFileText);

                var translationKeys = matches
                    .Select(m => m.Groups[2].Value == ""
                        ? m.Groups[4].Value
                        : m.Groups[2].Value)
                    .ToList();

                if (!translationKeys.Any())
                    continue;

                var jsFile = new JavaScriptFile(path);

                jsFile.TranslationKeys = translationKeys;

                JavaScriptFiles.Add(jsFile);
            }

            ModuleFolders = new List<ModuleFolder>();

            var list = TranslationFiles
                //.Where(file => file.Language == "en")
                .Select(t => new
                {
                    ModulePath = moduleWorkspaces.FirstOrDefault(m => t.Path.Contains(m)),
                    Language = new LanguageItem
                    {
                        Path = t.Path,
                        Language = t.Language,
                        Translations = t.Translations
                    },
                    lng = t.Language
                }).ToList();

            var moduleTranslations = list
                .GroupBy(t => t.ModulePath)
                .Select(g => new
                {
                    ModulePath = g.Key,
                    Languages = g.ToList().Select(t => t.Language).ToList()
                    .ToList()
                })
                .ToList();

            var moduleJsTranslatedFiles = JavaScriptFiles
                .Select(t => new
                {
                    ModulePath = moduleWorkspaces.FirstOrDefault(m => t.Path.Contains(m)),
                    t.Path,
                    t.TranslationKeys
                })
                .GroupBy(t => t.ModulePath)
                .Select(g => new
                {
                    ModulePath = g.Key,
                    TranslationKeys = g.ToList().SelectMany(t => t.TranslationKeys).ToList()
                })
                .ToList();

            foreach (var ws in moduleWorkspaces)
            {
                var t = moduleTranslations.FirstOrDefault(t => t.ModulePath == ws);
                var j = moduleJsTranslatedFiles.FirstOrDefault(t => t.ModulePath == ws);

                if (j == null && t == null)
                    continue;

                ModuleFolders.Add(new ModuleFolder
                {
                    Path = ws,
                    AvailableLanguages = t?.Languages,
                    AppliedJsTranslationKeys = j?.TranslationKeys
                });
            }

            CommonTranslations = TranslationFiles
                .Where(file => file.Path.StartsWith($"{BasePath}public\\locales"))
                .Select(t => new LanguageItem
                {
                    Path = t.Path,
                    Language = t.Language,
                    Translations = t.Translations
                }).ToList();
        }

        [Test]
        public void FullDublicatesTest()
        {
            var fullEnDuplicates = TranslationFiles
                .Where(file => file.Language == "en")
                .SelectMany(item => item.Translations)
                .GroupBy(t => new { t.Key, t.Value })
                .Where(grp => grp.Count() > 1)
                .Select(grp => new { Key = grp.Key, Count = grp.Count(), Keys = grp.ToList() })
                .OrderByDescending(itm => itm.Count)
                .Select(grp => new { Key = grp.Key.Key, Value = grp.Key.Value, grp.Count })
                .ToList();

            Assert.AreEqual(0, fullEnDuplicates.Count, string.Join("\r\n", fullEnDuplicates.Select(d => JObject.FromObject(d).ToString())));
        }

        [Test]
        public void DublicatesByContentTest()
        {
            var allRuTranslations = TranslationFiles
                .Where(file => file.Language == "ru")
                .SelectMany(item => item.Translations)
                .ToList();

            var allEnDuplicates = TranslationFiles
                .Where(file => file.Language == "en")
                .SelectMany(item => item.Translations)
                .GroupBy(t => t.Value)
                .Where(grp => grp.Count() > 1)
                .Select(grp => new { ContentKey = grp.Key, Count = grp.Count(), List = grp.ToList() })
                .OrderByDescending(itm => itm.Count)
                .ToList();

            var duplicatesKeys = new List<TranslationItem>();

            foreach (var item in allEnDuplicates)
            {
                var ruEquivalents = allRuTranslations
                    .Where(t => item.List.Select(k => k.Key).Contains(t.Key))
                    .GroupBy(t => t.Value)
                    .Select(grp => new
                    {
                        ContentKey = grp.Key,
                        Count = grp.Count(),
                        Keys = grp.Select(k => k.Key).ToList()
                    })
                    .Where(t => t.Count > 1)
                    .ToList();

                if (!ruEquivalents.Any())
                    continue;

                duplicatesKeys.AddRange(
                    item.List.Where(item => ruEquivalents
                        .SelectMany(k => k.Keys)
                        .Any(k => k == item.Key)
                        )
                    );

            }

            var duplicates = duplicatesKeys
                .GroupBy(k => k.Value)
                .Select(g => new { ContentKey = g.Key, Count = g.Count(), Keys = g.ToList() })
                .ToList();

            /*== Save dublicates.json ==*/
            /* var listForSave = duplicates
               .SelectMany(g => g.Keys)
               .GroupBy(item => item.Key)
               .Select(grp => new
               {
                   Key = grp.Key,
                   Value = grp.FirstOrDefault().Value
               })
               .OrderByDescending(t => t.Value);

           string json = JsonSerializer.Serialize(listForSave);
           json = json.Replace("\"Key\":", "").Replace(",\"Value\"", "").Replace("},{", ",");
           json = json.Substring(1, json.Length - 2);
           File.WriteAllText(@"D:\dublicates.json", json);*/

            Assert.AreEqual(0, duplicates.Count, string.Join(", ", duplicates.Select(d => JObject.FromObject(d).ToString())));
        }

        private static void SaveNotFoundLanguage(string existJsonPath, string notExistJsonPath)
        {
            if (!File.Exists(existJsonPath) || File.Exists(notExistJsonPath))
                return;

            var jsonTranslation = JObject.Parse(File.ReadAllText(existJsonPath));

            var properties = jsonTranslation.Properties().Select(t => t).ToList();

            properties.ForEach(p => p.Value = "");

            var result = new JObject(properties);

            var sortedJsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

            string currentDirectory = Path.GetDirectoryName(notExistJsonPath);

            string fullPathOnly = Path.GetFullPath(currentDirectory);

            if (!Directory.Exists(fullPathOnly))
                Directory.CreateDirectory(fullPathOnly);

            File.WriteAllText(notExistJsonPath, sortedJsonString);
        }

        [Test]
        public void NotAllLanguageTranslatedTest()
        {
            var groupedByLng = TranslationFiles
                .GroupBy(t => t.Language)
                .Select(grp => new { Lng = grp.Key, Count = grp.Count(), Files = grp.ToList() })
                .ToList();

            var enGroup = groupedByLng.Find(f => f.Lng == "en");
            var expectedCount = enGroup.Count;

            var otherLngs = groupedByLng.Where(g => g.Lng != "en");

            var incompleteList = otherLngs
                    .Where(lng => lng.Count != expectedCount)
                    .Select(lng => new { Issue = $"Language '{lng.Lng}' (Count={lng.Count}). Not found files:\r\n", lng.Lng, lng.Files })
                    .ToList();

            var message = $"Next languages are not equal 'en' (Count= {expectedCount}) by translated files count:\r\n\r\n";

            if (incompleteList.Count > 0)
            {
                var enFilePaths = enGroup.Files.Select(f => f.Path);

                for (int i = 0; i < incompleteList.Count; i++)
                {
                    var lng = incompleteList[i];

                    message += $"{i}. {lng.Issue}\r\n";

                    var lngFilePaths = lng.Files.Select(f => f.Path).ToList();

                    var notFoundFilePaths = enFilePaths
                        .Select(p => p.Replace("\\en\\", $"\\{lng.Lng}\\"))
                        .Where(p => !lngFilePaths.Contains(p));

                    message += string.Join("\r\n", notFoundFilePaths);

                    /* Save empty 'EN' keys to not found files */

                    //foreach (var path in notFoundFilePaths)
                    //{
                    //    SaveNotFoundLanguage(path.Replace($"\\{lng.Lng}\\", "\\en\\"), path);
                    //}
                }
            }

            Assert.AreEqual(0, incompleteList.Count, message);
        }

        [Test]
        public void NotTranslatedKeysTest()
        {
            var groupedByLng = TranslationFiles
                .GroupBy(t => t.Language)
                .Select(grp => new
                {
                    Lng = grp.Key,
                    Keys = grp
                        .SelectMany(k => k.Translations)
                        .OrderByDescending(itm => itm.Key)
                        .ToList()
                })
                .ToList();

            var enGroup = groupedByLng.Find(f => f.Lng == "en");

            var expectedCount = enGroup.Keys.Count;

            var otherLngs = groupedByLng.Where(g => g.Lng != "en");

            var incompleteList = otherLngs
                    .Where(lng => lng.Keys.Count != expectedCount)
                    .Select(lng => new { Issue = $"Language '{lng.Lng}' (Count={lng.Keys.Count}). Not found keys:\r\n", lng.Lng, lng.Keys })
                    .ToList();

            var message = $"Next languages are not equal 'en' (Count= {expectedCount}) by translated keys count:\r\n\r\n";

            if (incompleteList.Count > 0)
            {
                var enKeys = enGroup.Keys.Select(f => f.Key);

                for (int i = 0; i < incompleteList.Count; i++)
                {
                    var lng = incompleteList[i];

                    message += $"{i}. {lng.Issue}\r\n";

                    var lngKeys = lng.Keys.Select(f => f.Key).ToList();

                    var notFoundKeys = enKeys.Except(lngKeys);

                    message += string.Join("\r\n", notFoundKeys) + "\r\n\r\n";
                }
            }

            Assert.AreEqual(0, incompleteList.Count, message);
        }

        [Test]
        public void NotFoundKeysTest()
        {
            var allEnKeys = TranslationFiles
                 .Where(file => file.Language == "en")
                 .SelectMany(item => item.Translations)
                 .Select(item => item.Key);

            var allJsTranslationKeys = JavaScriptFiles
                .SelectMany(j => j.TranslationKeys)
                .Select(k => k.Replace("Common:", "").Replace("Translations:", ""))
                .Distinct();

            var notFoundJsKeys = allJsTranslationKeys.Except(allEnKeys);

            Assert.AreEqual(0, notFoundJsKeys.Count(),
                "Some i18n-keys are not exist in translations in 'en' language: Keys: '{0}'",
                string.Join("\r\n", notFoundJsKeys));
        }

        [Test]
        public void UselessTranslationKeysTest()
        {
            var allEnKeys = TranslationFiles
                 .Where(file => file.Language == "en")
                 .SelectMany(item => item.Translations)
                 .Select(item => item.Key)
                 .Where(k => !k.StartsWith("Culture_"));

            var allJsTranslationKeys = JavaScriptFiles
                .SelectMany(j => j.TranslationKeys)
                .Select(k => k.Replace("Common:", "").Replace("Translations:", ""))
                .Where(k => !k.StartsWith("Culture_"))
                .Distinct();

            var notFoundi18nKeys = allEnKeys.Except(allJsTranslationKeys);

            Assert.AreEqual(0, notFoundi18nKeys.Count(),
                "Some i18n-keys are not found in js: \r\nKeys: '\r\n{0}'",
                string.Join("\r\n", notFoundi18nKeys));
        }

        [Test]
        public void UselessModuleTranslationKeysTest()
        {
            var notFoundi18nKeys = new List<KeyValuePair<string, List<string>>>();

            var message = $"Some i18n-keys are not found in js: \r\nKeys: \r\n\r\n";

            for (int i = 0; i < ModuleFolders.Count; i++)
            {
                var module = ModuleFolders[i];

                if (module.AppliedJsTranslationKeys == null && module.AvailableLanguages != null)
                {
                    message += $"{i}. 'ANY LANGUAGES' '{module.Path}' NOT USES";

                    var list = module.AvailableLanguages
                        .SelectMany(l => l.Translations.Select(t => t.Key).ToList())
                        .ToList();

                    notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>("ANY LANGUAGES", list));

                    continue;
                }

                var notCommonKeys = module.AppliedJsTranslationKeys
                    .Where(k => !k.StartsWith("Common:"))
                    .ToList();

                var onlyCommonKeys = module.AppliedJsTranslationKeys
                    .Except(notCommonKeys)
                    .Select(k => k.Replace("Common:", ""))
                    .ToList();

                if (onlyCommonKeys.Any())
                {
                    foreach (var lng in CommonTranslations)
                    {
                        var list = onlyCommonKeys
                            .Except(lng.Translations.Select(t => t.Key))
                            .ToList();

                        if (!list.Any())
                            continue;

                        message += $"{i}. '{lng.Language}' '{module.Path}' \r\n {string.Join("\r\n", list)} \r\n";

                        notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>(lng.Language, list));
                    }
                }

                if (module.AvailableLanguages == null)
                {
                    if (notCommonKeys.Any())
                    {
                        message += $"{i}. 'ANY LANGUAGES' '{module.Path}' \r\n {string.Join("\r\n", notCommonKeys)} \r\n";

                        notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>("ANY LANGUAGES", notCommonKeys));
                    }

                    continue;
                }

                foreach (var lng in module.AvailableLanguages)
                {
                    var list = lng.Translations
                         .Select(t => t.Key)
                         .Except(notCommonKeys.Select(k => k.Replace("Translations:", "")))
                         .ToList();

                    if (!list.Any())
                        continue;

                    message += $"{i}. '{lng.Language}' '{module.Path}' \r\n {string.Join("\r\n", list)} \r\n";

                    notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>(lng.Language, list));
                }
            }

            Assert.AreEqual(0, notFoundi18nKeys.Count(), message);
        }

        [Test]
        public void NotTranslatedCommonKeysTest()
        {
            var notFoundi18nKeys = new List<KeyValuePair<string, List<string>>>();
            var message = $"Some i18n-keys are not found in COMMON translations: \r\nKeys: \r\n\r\n";

            var enLanguageKeys = CommonTranslations
                .Where(l => l.Language == "en")
                .FirstOrDefault()
                .Translations
                .Select(k => k.Key)
                .ToList();

            var otherCommonLanguages = CommonTranslations.Where(l => l.Language != "en");

            var i = 0;
            foreach (var lng in otherCommonLanguages)
            {
                var list = enLanguageKeys
                    .Except(lng.Translations.Select(t => t.Key))
                    .ToList();

                if (!list.Any())
                    continue;

                message += $"{++i}. '{lng.Language}' Keys: \r\n {string.Join("\r\n", list)} \r\n";

                notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>(lng.Language, list));
            }

            Assert.AreEqual(0, notFoundi18nKeys.Count(), message);
        }
    }
}