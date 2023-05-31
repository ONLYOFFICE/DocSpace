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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Frontend.Tests;
using Frontend.Tests.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

using WeCantSpell.Hunspell;

namespace Frontend.Tests;

public class LocalesTest
{
    public static string BasePath
    {
        get
        {
            return Environment.GetEnvironmentVariable("BASE_DIR") ?? Path.GetFullPath(Utils.ConvertPathToOS("../../../../../../"));
        }
    }

    public static bool Save
    {
        get
        {
            bool save;
            if (bool.TryParse(Environment.GetEnvironmentVariable("SAVE"), out save))
            {
                return save;
            }

            return false;
        }
    }

    public List<string> Workspaces { get; set; }
    public List<TranslationFile> TranslationFiles { get; set; }
    public List<JavaScriptFile> JavaScriptFiles { get; set; }
    public List<ModuleFolder> ModuleFolders { get; set; }
    public List<KeyValuePair<string, string>> NotTranslatedToasts { get; set; }
    public List<KeyValuePair<string, string>> NotTranslatedProps { get; set; }
    public List<LanguageItem> CommonTranslations { get; set; }
    public List<ParseJsonError> ParseJsonErrors { get; set; }
    public static string ConvertPathToOS { get; private set; }

    //public List<JsonEncodingError> WrongEncodingJsonErrors { get; set; }

    private static readonly string _md5ExcludesPath = Path.GetFullPath(Utils.ConvertPathToOS("../../../md5-excludes.json"));
    private static readonly string _spellCheckCommonExcludesPath = Path.GetFullPath(Utils.ConvertPathToOS("../../../spellcheck-excludes-common.json"));
    private static readonly string _spellCheckExcludesPath = Path.GetFullPath(Utils.ConvertPathToOS("../../../spellcheck-excludes.json"));

    //private static string _encodingExcludesPath = "../../../encoding-excludes.json";

    private static readonly List<string> Md5Excludes = File.Exists(_md5ExcludesPath)
        ? JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_md5ExcludesPath))
        : new List<string>();

    private static readonly List<string> SpellCheckCommonExcludes = File.Exists(_spellCheckCommonExcludesPath)
        ? JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_spellCheckCommonExcludesPath))
        : new List<string>();

    //private static List<string> encodingExcludes = File.Exists(_encodingExcludesPath)
    //    ? JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_encodingExcludesPath))
    //    : new List<string>();

    [OneTimeSetUp]
    public void Setup()
    {
        ParseJsonErrors = new List<ParseJsonError>();
        //WrongEncodingJsonErrors = new List<JsonEncodingError>();

        var moduleWorkspaces = new List<string>
        {
            Utils.ConvertPathToOS("packages/client"),
            Utils.ConvertPathToOS("packages/common"),
            Utils.ConvertPathToOS("packages/components"),
            Utils.ConvertPathToOS("packages/editor"),
            Utils.ConvertPathToOS("packages/login")
        };

        Workspaces = new List<string>();

        Workspaces.AddRange(moduleWorkspaces);

        Workspaces.Add(Utils.ConvertPathToOS("public/locales"));

        var translationFiles = from wsPath in Workspaces
                               let clientDir = Path.Combine(BasePath, wsPath)
                               from filePath in Directory.EnumerateFiles(clientDir, "*.json", SearchOption.AllDirectories)
                               where filePath.Contains(Utils.ConvertPathToOS("public/locales/"))
                               select Path.GetFullPath(filePath);

        TestContext.Progress.WriteLine($"Base path = {BasePath}");
        TestContext.Progress.WriteLine($"Found translationFiles by *.json filter = {translationFiles.Count()}. First path is '{translationFiles.FirstOrDefault()}'");

        TranslationFiles = new List<TranslationFile>();

        foreach (var path in translationFiles)
        {
            try
            {
                //var result = CharsetDetector.DetectFromFile(path);

                //if (!encodingExcludes.Contains(result.Detected.EncodingName))
                //{
                //    WrongEncodingJsonErrors.Add(
                //        new JsonEncodingError(path, result.Detected));
                //}

#if SORT

                JObject jsonSorted;
#endif
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        var md5hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                        stream.Position = 0;

                        using var sr = new StreamReader(stream, Encoding.UTF8);
                        {
                            var jsonTranslation = JObject.Parse(sr.ReadToEnd());

                            var translationFile = new TranslationFile(path, jsonTranslation.Properties()
                                .Select(p => new TranslationItem(p.Name, (string)p.Value))
                                .ToList(), md5hash);

                            TranslationFiles.Add(translationFile);

#if SORT
                            var orderedList = jsonTranslation.Properties().OrderBy(t => t.Name);
                            jsonSorted = new JObject(orderedList);
#endif
                        }
                    }
#if SORT
                    //   Re-write by order 
                    var sortedJsonString = JsonConvert.SerializeObject(jsonSorted, Formatting.Indented);
                    File.WriteAllText(path, sortedJsonString, Encoding.UTF8);
#endif
                }
            }
            catch (Exception ex)
            {
                ParseJsonErrors.Add(new ParseJsonError(path, ex));
                TestContext.Progress.WriteLine($"File path = {path} failed to parse with error: {ex.Message}");
            }
        }

        TestContext.Progress.WriteLine($"Found TranslationFiles = {TranslationFiles.Count()}. First path is '{TranslationFiles.FirstOrDefault()?.FilePath}'");

        var searchPatern = @"\.js|\.jsx|\.ts|\.tsx";

        var javascriptFiles = (from wsPath in Workspaces
                               let clientDir = Path.Combine(BasePath, wsPath)
                               from filePath in Utils.GetFiles(clientDir, searchPatern, SearchOption.AllDirectories)
                               where !filePath.Contains(Utils.ConvertPathToOS("dist/"))
                               && !filePath.Contains(Utils.ConvertPathToOS("storybook-static/"))
                               && !filePath.Contains(Utils.ConvertPathToOS("node_modules/"))
                               && !filePath.Contains(".test.js")
                               && !filePath.Contains(".stories.js")
                               && !filePath.Contains(".test.ts")
                               && !filePath.Contains(".stories.ts")
                               && !filePath.Contains(".test.tsx")
                               && !filePath.Contains(".stories.tsx")
                               select Utils.ConvertPathToOS(filePath))
                              .ToList();

        TestContext.Progress.WriteLine($"Found javascriptFiles by *.js(x) filter = {javascriptFiles.Count()}. First path is '{javascriptFiles.FirstOrDefault()}'");

        JavaScriptFiles = new List<JavaScriptFile>();

        var pattern1 = "[.{\\s\\(]t\\(\\s*[\"\'`]([a-zA-Z0-9_.:\\s{}/-]+)[\"\'`]\\s*[\\),]";
        var pattern2 = "i18nKey=\"([a-zA-Z0-9_.:-]+)\"";
        var pattern3 = "tKey:\\s\"([a-zA-Z0-9_.:-]+)\"";
        var pattern4 = "getTitle\\(\"([a-zA-Z0-9_.:-]+)\"\\)";

        var regexp = new Regex($"({pattern1})|({pattern2})|({pattern3})|({pattern4})", RegexOptions.Multiline | RegexOptions.ECMAScript);

        var notTranslatedToastsRegex = new Regex("(?<=toastr.info\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.error\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.success\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.warn\\([\"`\'])(.*)(?=[\"\'`])", RegexOptions.Multiline | RegexOptions.ECMAScript);

        var notTranslatedPropsRegex = new Regex("<[\\w\\n][^>]* (title|placeholder|label|text)={?[\\\"\\'](.*)[\\\"\\']}?", RegexOptions.Multiline | RegexOptions.ECMAScript);

        NotTranslatedToasts = new List<KeyValuePair<string, string>>();
        NotTranslatedProps = new List<KeyValuePair<string, string>>();

        foreach (var path in javascriptFiles)
        {
            var jsFileText = File.ReadAllText(path);

            var toastMatches = notTranslatedToastsRegex.Matches(jsFileText).ToList();

            if (toastMatches.Any())
            {
                foreach (var toastMatch in toastMatches)
                {
                    var found = toastMatch.Value;
                    if (!string.IsNullOrEmpty(found) && !NotTranslatedToasts.Exists(t => t.Value == found))
                        NotTranslatedToasts.Add(new KeyValuePair<string, string>(path, found));
                }
            }

            var propsMatches = notTranslatedPropsRegex.Matches(jsFileText).ToList();

            if (propsMatches.Any())
            {
                foreach (var propsMatch in propsMatches)
                {
                    var found = propsMatch.Value;
                    if (!string.IsNullOrEmpty(found) && !NotTranslatedProps.Exists(t => t.Value == found))
                        NotTranslatedProps.Add(new KeyValuePair<string, string>(path, found));
                }
            }

            var matches = regexp.Matches(jsFileText);

            var translationKeys = matches
                .Select(m => m.Groups[2].Success
                    ? m.Groups[2].Value
                    : m.Groups[4].Success
                        ? m.Groups[4].Value
                        : m.Groups[6].Success
                            ? m.Groups[6].Value
                            : m.Groups[8].Success
                                ? m.Groups[8].Value
                                : null)
                .Where(m => m != null)
                .ToList();

            if (!translationKeys.Any())
                continue;

            var jsFile = new JavaScriptFile(path);

            jsFile.TranslationKeys = translationKeys;

            JavaScriptFiles.Add(jsFile);
        }

        TestContext.Progress.WriteLine($"Found JavaScriptFiles = {JavaScriptFiles.Count()}. First path is '{JavaScriptFiles.FirstOrDefault()?.Path}'");

        ModuleFolders = new List<ModuleFolder>();

        var list = TranslationFiles
            .Select(t => new
            {
                ModulePath = moduleWorkspaces.FirstOrDefault(m => t.FilePath.Contains(m)),
                Language = new LanguageItem
                {
                    Path = t.FilePath,
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
            })
            .ToList();

        TestContext.Progress.WriteLine($"Found moduleTranslations = {moduleTranslations.Count()}. First path is '{moduleTranslations.FirstOrDefault()?.ModulePath}'");

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

        TestContext.Progress.WriteLine($"Found moduleJsTranslatedFiles = {moduleJsTranslatedFiles.Count()}. First path is '{moduleJsTranslatedFiles.FirstOrDefault()?.ModulePath}'");

        foreach (var wsPath in moduleWorkspaces)
        {
            var t = moduleTranslations.FirstOrDefault(t => t.ModulePath == wsPath);
            var j = moduleJsTranslatedFiles.FirstOrDefault(t => t.ModulePath == wsPath);

            if (j == null && t == null)
                continue;

            ModuleFolders.Add(new ModuleFolder
            {
                Path = wsPath,
                AvailableLanguages = t?.Languages,
                AppliedJsTranslationKeys = j?.TranslationKeys
            });
        }

        TestContext.Progress.WriteLine($"Found ModuleFolders = {ModuleFolders.Count()}. First path is '{ModuleFolders.FirstOrDefault()?.Path}'");

        CommonTranslations = TranslationFiles
            .Where(file => file.FilePath.StartsWith(Utils.ConvertPathToOS(Path.Combine(BasePath, "public/locales"))))
            .Select(t => new LanguageItem
            {
                Path = t.FilePath,
                Language = t.Language,
                Translations = t.Translations
            }).ToList();

        TestContext.Progress.WriteLine($"Found CommonTranslations = {CommonTranslations.Count()}. First path is '{CommonTranslations.FirstOrDefault()?.Path}'");

        TestContext.Progress.WriteLine($"Found Md5Excludes = {Md5Excludes.Count} Path to file '{_md5ExcludesPath}'");

        TestContext.Progress.WriteLine($"Found SpellCheckCommonExcludes = {SpellCheckCommonExcludes.Count} Path to file '{_spellCheckCommonExcludesPath}'");

        TestContext.Progress.WriteLine($"Save spell check excludes = {Save} Path to file '{_spellCheckExcludesPath}'");

    }

    [Test]
    [Category("Locales")]
    public void ParseJsonTest()
    {
        Assert.AreEqual(0, ParseJsonErrors.Count, string.Join("\r\n", ParseJsonErrors.Select(e => $"File path = '{e.Path}' failed to parse with error: '{e.Exception.Message}'")));
    }

    public static Tuple<string, string> getPaths(string language)
    {
        const string dictionariesPath = @"../../../dictionaries";
        const string additionalPath = @"../../../additional";

        var path = dictionariesPath;

        switch (language)
        {
            case "fi":
                path = additionalPath;
                break;
            default:
                break;
        }

        var dicPath = Utils.ConvertPathToOS(Path.Combine(path, language, $"{language}.dic"));
        var affPath = Utils.ConvertPathToOS(Path.Combine(path, language, $"{language}.aff"));

        return new Tuple<string, string>(dicPath, affPath);
    }

    [Test]
    [Category("SpellCheck")]
    public void SpellCheckTest()
    {
        var i = 0;
        var errorsCount = 0;
        var message = $"Next keys have spell check issues:\r\n\r\n";

        var list = new List<SpellCheckExclude>();

        var groupByLng = TranslationFiles
        .GroupBy(t => t.Language)
            .Select(g => new
            {
                Language = g.Key,
                Files = g.ToList()
            })
            .ToList();

        foreach (var group in groupByLng)
        {
            try
            {
                var dicPaths = SpellCheck.GetDictionaryPaths(group.Language);

                var spellCheckExclude = new SpellCheckExclude(group.Language);

                using (var dictionaryStream = File.OpenRead(dicPaths.DictionaryPath))
                using (var affixStream = File.OpenRead(dicPaths.AffixPath))
                {
                    var dictionary = WordList.CreateFromStreams(dictionaryStream, affixStream);

                    foreach (var g in group.Files)
                    {
                        foreach (var item in g.Translations)
                        {
                            var result = SpellCheck.HasSpellIssues(item.Value, group.Language, dictionary);

                            if (result.HasProblems)
                            {
                                var incorrectWords = result.SpellIssues
                                    .Where(t => !SpellCheckCommonExcludes
                                    .Exists(e => e.Equals(t.Word, StringComparison.InvariantCultureIgnoreCase)))
                                    .Select(issue => $"'{issue.Word}' " +
                                $"Suggestion: '{issue.Suggestions.FirstOrDefault()}'")
                                    .ToList();

                                if (!incorrectWords.Any())
                                    continue;

                                message += $"{++i}. lng='{group.Language}' file='{g.FilePath}'\r\nkey='{item.Key}' " +
                                $"value='{item.Value}'\r\nIncorrect words:\r\n" +
                                $"{string.Join("\r\n", incorrectWords)}\r\n\r\n";
                                errorsCount++;

                                if (Save)
                                {
                                    foreach (var word in result.SpellIssues
                                    .Where(issue => issue.Suggestions.Any())
                                    .Select(issue => issue.Word))
                                    {
                                        if (!spellCheckExclude.Excludes.Contains(word))
                                        {
                                            spellCheckExclude.Excludes.Add(word);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (Save)
                {
                    spellCheckExclude.Excludes.Sort();

                    list.Add(spellCheckExclude);
                }
            }
            catch (NotSupportedException)
            {
                // Skip not supported
                continue;
            }
        }

        if (Save)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(_spellCheckExcludesPath, json, Encoding.UTF8);
            TestContext.Progress.WriteLine($"File spellcheck-excludes.json has been saved to '{_spellCheckExcludesPath}'");
        }

        Assert.AreEqual(0, errorsCount, message);
    }

    [Test]
    [Category("Locales")]
    public void SingleKeyFilesTest()
    {
        var singleKeyTranslationFiles = TranslationFiles
            .Where(t => t.Language == "en" && t.Translations.Count == 1)
            .ToList();

        Assert.AreEqual(0, singleKeyTranslationFiles.Count, "Translations files with single key:\r\n" + string.Join("\r\n", singleKeyTranslationFiles.Select(d => $"\r\nKey='{d.Translations.First().Key}':\r\n{d.FilePath}'")));
    }

    [Test]
    [Category("Locales")]
    public void DublicatesFilesByMD5HashTest()
    {
        var duplicatesByMD5 = TranslationFiles
            .Where(t => t.Language != "pt-BR")
            .Where(t => !Md5Excludes.Contains(t.Md5Hash))
            .GroupBy(t => t.Md5Hash)
            .Where(grp => grp.Count() > 1)
            .Select(grp => new { Key = grp.Key, Count = grp.Count(), Paths = grp.ToList().Select(f => f.FilePath) })
            .OrderByDescending(itm => itm.Count)
            .ToList();

        Assert.AreEqual(0, duplicatesByMD5.Count, "Dublicates by MD5 hash:\r\n" + string.Join("\r\n", duplicatesByMD5.Select(d => $"\r\nMD5='{d.Key}':\r\n{string.Join("\r\n", d.Paths.Select(p => p))}'")));
    }

    [Test]
    [Category("Locales")]
    public void FullEnDublicatesTest()
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
    [Category("Locales")]
    public void EnDublicatesByContentTest()
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

        Assert.AreEqual(0, duplicates.Count, string.Join(", ", duplicates.Select(d => JObject.FromObject(d).ToString())));
    }

    public static void SaveNotFoundLanguage(string existJsonPath, string notExistJsonPath)
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

        File.WriteAllText(notExistJsonPath, sortedJsonString, Encoding.UTF8);
    }

    [Test]
    [Category("Locales")]
    public void NotAllLanguageTranslatedTest()
    {
        var groupedByLng = TranslationFiles
            .GroupBy(t => t.Language)
            .Select(grp => new { Lng = grp.Key, Count = grp.Count(), Files = grp.ToList() })
            .ToList();

        // Uncomment if new language is needed
        //var newLng = "sk";

        //if (!groupedByLng.Exists(t => t.Lng == newLng))
        //    groupedByLng.Add(new { Lng = newLng, Count = 0, Files = new List<TranslationFile>() });

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
            var enFilePaths = enGroup.Files.Select(f => f.FilePath);

            for (int i = 0; i < incompleteList.Count; i++)
            {
                var lng = incompleteList[i];

                message += $"\r\n\r\n{i}. {lng.Issue}\r\n";

                var lngFilePaths = lng.Files.Select(f => f.FilePath).ToList();

                var notFoundFilePaths = enFilePaths
                    .Select(p => p.Replace(Utils.ConvertPathToOS("/en/"), Utils.ConvertPathToOS($"/{lng.Lng}/")))
                    .Where(p => !lngFilePaths.Contains(p));

                message += string.Join("\r\n", notFoundFilePaths);

                /* Save empty 'EN' keys to not found files */

                /*foreach (var path in notFoundFilePaths)
                {
                    SaveNotFoundLanguage(path.Replace(Utils.ConvertPathToOS($"\\{lng.Lng}\\"), Utils.ConvertPathToOS("\\en\\")), path);
                }*/
            }
        }

        Assert.AreEqual(0, incompleteList.Count, message);
    }

    public static void SaveNotFoundKeys(string pathToJson, List<string> newKeys)
    {
        if (!File.Exists(pathToJson))
            return;

        var jsonTranslation = JObject.Parse(File.ReadAllText(pathToJson));

        var properties = jsonTranslation.Properties().Select(t => t).ToList();

        properties.AddRange(newKeys.Select(k => new JProperty(k, "")));

        properties = properties.OrderBy(t => t.Name).ToList();

        var result = new JObject(properties);

        var sortedJsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

        File.WriteAllText(pathToJson, sortedJsonString, Encoding.UTF8);
    }

    [Test]
    [Category("Locales")]
    public void NotTranslatedKeysTest()
    {
        var message = $"Next languages are not equal 'en' by translated keys count:\r\n\r\n";

        var exists = false;

        var i = 0;

        foreach (var module in ModuleFolders)
        {
            if (module.AvailableLanguages == null)
                continue;

            var enLanguages = module.AvailableLanguages.Where(l => l.Language == "en").ToList();

            var otherLanguages = module.AvailableLanguages.Where(l => l.Language != "en").ToList();

            foreach (var lng in otherLanguages)
            {
                var lngKeys = lng.Translations.Select(f => f.Key).ToList();

                var enKeys = enLanguages.Where(l => l.Path == lng.Path.Replace(Utils.ConvertPathToOS($"/{lng.Language}/"), Utils.ConvertPathToOS("/en/")))
                    .SelectMany(l => l.Translations.Select(f => f.Key))
                    .ToList();

                var notFoundKeys = enKeys.Except(lngKeys).ToList();

                if (!notFoundKeys.Any())
                    continue;

                exists = true;

                message += $"{++i}. Language ('{lng.Language}'={notFoundKeys.Count}/'en'={enKeys.Count}). Path '{lng.Path}' " +
                    $"Not found keys:\r\n\r\n";

                message += string.Join("\r\n", notFoundKeys) + "\r\n\r\n";

                // Save empty not found keys
                //SaveNotFoundKeys(lng.Path, notFoundKeys);
            }
        }

        Assert.AreEqual(false, exists, message);
    }

    [Test]
    [Category("Locales")]
    public void NotFoundKeysTest()
    {
        var allEnKeys = TranslationFiles
             .Where(file => file.Language == "en")
             .SelectMany(item => item.Translations)
             .Select(item => item.Key);

        var allJsTranslationKeys = JavaScriptFiles
            .Where(f => !f.Path.Contains("Banner.js")) // skip Banner.js (translations from firebase)
            .SelectMany(j => j.TranslationKeys)
            .Select(k => k.Substring(k.IndexOf(":") + 1))
            .Distinct();

        var notFoundJsKeys = allJsTranslationKeys.Except(allEnKeys);

        Assert.AreEqual(0, notFoundJsKeys.Count(),
            "Some i18n-keys are not exist in translations in 'en' language: Keys:\r\n{0}",
            string.Join("\r\n", notFoundJsKeys));
    }

    [Test]
    [Category("Locales")]
    public void UselessTranslationKeysTest()
    {
        var allEnKeys = TranslationFiles
             .Where(file => file.Language == "en")
             .SelectMany(item => item.Translations)
             .Select(item => item.Key)
             .Where(k => !k.StartsWith("Culture_"))
             .OrderBy(t => t);

        var allJsTranslationKeys = JavaScriptFiles
            .SelectMany(j => j.TranslationKeys)
            .Select(k => k.Substring(k.IndexOf(":") + 1))
            .Where(k => !k.StartsWith("Culture_"))
            .Distinct()
            .OrderBy(t => t);

        var notFoundi18nKeys = allEnKeys.Except(allJsTranslationKeys);

        Assert.AreEqual(0, notFoundi18nKeys.Count(),
            "Some i18n-keys are not found in js keys:\r\n{0}",
            string.Join("\r\n", notFoundi18nKeys));
    }

    [Test]
    [Category("Locales")]
    public void UselessModuleTranslationKeysTest()
    {
        var notFoundi18nKeys = new List<KeyValuePair<string, List<string>>>();

        var message = $"Some i18n-keys are not found in Module or Common translations: \r\nKeys: \r\n\r\n";

        var index = 0;

        for (int i = 0; i < ModuleFolders.Count; i++)
        {
            var module = ModuleFolders[i];

            if (module.AppliedJsTranslationKeys == null && module.AvailableLanguages != null)
            {
                message += $"{++index}. 'ANY LANGUAGES' '{module.Path}' NOT USED\r\n";

                var list = module.AvailableLanguages
                    .SelectMany(l => l.Translations.Select(t => t.Key).ToList())
                    .ToList();

                notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>("ANY LANGUAGES", list));

                continue;
            }

            var exepts = new List<string> { "Error", "Done", "Warning", "Alert", "Info" };

            var notCommonKeys = module.AppliedJsTranslationKeys
                .Except(exepts)
                .Where(k => !k.StartsWith("Common:"))
                .OrderBy(t => t)
                .ToList();

            var onlyCommonKeys = module.AppliedJsTranslationKeys
                .Except(notCommonKeys)
                .Select(k => k.Replace("Common:", ""))
                .OrderBy(t => t)
                .ToList();

            notCommonKeys = notCommonKeys.Select(k => k.Substring(k.IndexOf(":") + 1)).ToList();

            if (onlyCommonKeys.Any())
            {
                foreach (var lng in CommonTranslations)
                {
                    var list = onlyCommonKeys
                        .Except(lng.Translations.Select(t => t.Key))
                        .ToList();

                    if (!list.Any())
                        continue;

                    message += $"{++index}. '{lng.Language}' '{module.Path}' \r\n {string.Join("\r\n", list)} \r\n";

                    notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>(lng.Language, list));
                }
            }

            if (module.AvailableLanguages == null)
            {
                if (notCommonKeys.Any())
                {
                    var commonEnKeys = CommonTranslations.First(c => c.Language == "en").Translations.Select(t => t.Key).ToList();

                    var list = notCommonKeys
                        .Except(commonEnKeys.Select(k => k))
                        .ToList();

                    if (list.Any())
                    {
                        message += $"{++index}. 'ANY LANGUAGES' '{module.Path}' \r\n {string.Join("\r\n", list)} \r\n";
                        notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>("ANY LANGUAGES", list));
                    }
                }

                continue;
            }

            foreach (var lng in module.AvailableLanguages)
            {
                var list = lng.Translations
                     .Select(t => t.Key)
                     .Except(notCommonKeys)
                     .ToList();

                if (!list.Any())
                    continue;

                message += $"{++index}. '{lng.Language}' '{module.Path}' \r\n {string.Join("\r\n", list)} \r\n";

                notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>(lng.Language, list));
            }
        }

        Assert.AreEqual(0, notFoundi18nKeys.Count, message);
    }

    [Test]
    [Category("Locales")]
    public void NotTranslatedCommonKeysTest()
    {
        var message = $"Some i18n-keys are not found in COMMON translations: \r\nKeys: \r\n\r\n";

        var enLanguageKeys = CommonTranslations
            .Where(l => l.Language == "en")
            .FirstOrDefault()
            .Translations
            .Select(k => k.Key)
            .ToList();

        var otherCommonLanguages = CommonTranslations.Where(l => l.Language != "en");

        var exists = false;

        var i = 0;
        foreach (var lng in otherCommonLanguages)
        {
            var list = enLanguageKeys
                .Except(lng.Translations.Select(t => t.Key))
                .ToList();

            if (!list.Any())
                continue;

            message += $"{++i}. '{lng.Language}' Keys: \r\n {string.Join("\r\n", list)} \r\n";

            exists = true;

            // Save empty not found keys
            //SaveNotFoundKeys(lng.Path, list);
        }

        Assert.AreEqual(false, exists, message);
    }

    public static void UpdateKeys(string pathToJson, List<TranslationItem> newKeys)
    {
        if (!File.Exists(pathToJson) || !newKeys.Any())
            return;

        var jsonTranslation = JObject.Parse(File.ReadAllText(pathToJson));

        var keys = newKeys.Select(k => k.Key).ToList();

        var properties = jsonTranslation.Properties().ToList();

        properties.ForEach(p =>
        {
            var newKey = newKeys.Where(k => k.Key == p.Name).FirstOrDefault();
            if (newKey != null)
                p.Value = newKey.Value;

        });

        var result = new JObject(properties);

        var sortedJsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

        File.WriteAllText(pathToJson, sortedJsonString, Encoding.UTF8);
    }

    public static void RemoveEmptyKeys(string pathToJson, List<string> emptyKeys)
    {
        if (!File.Exists(pathToJson) || !emptyKeys.Any())
            return;

        var jsonTranslation = JObject.Parse(File.ReadAllText(pathToJson));

        var properties = jsonTranslation.Properties().Where(p => !emptyKeys.Contains(p.Name)).ToList();

        var result = new JObject(properties);

        var sortedJsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

        File.WriteAllText(pathToJson, sortedJsonString, Encoding.UTF8);
    }

    public string GetWorkspace(string path)
    {
        var folderName = Directory.GetParent(Path.GetDirectoryName(path)).Name;

        switch (folderName)
        {
            case "Client":
                return Workspaces.Find(w => w.Contains("client"));
            case "Editor":
                return Workspaces.Find(w => w.Contains("editor"));
            case "Login":
                return Workspaces.Find(w => w.Contains("login"));
            default:
                return Path.Combine(BasePath, Utils.ConvertPathToOS("public\\locales"));
        }
    }

    [Test]
    [Category("Locales")]
    public void EmptyValueKeysTest()
    {
        // Uncomment if new keys are available
        /*var newTranslationsBasePath = @"D:\trans";

        var translationFiles = from file in Directory.EnumerateFiles(newTranslationsBasePath, "*.json", SearchOption.AllDirectories)
                               select file;

        var newTranslationFiles = new List<KeyValuePair<string, TranslationFile>>();

        foreach (var path in translationFiles)
        {
            var jsonTranslation = JObject.Parse(File.ReadAllText(path));

            var translationFile = new TranslationFile(path, jsonTranslation.Properties()
                .Select(p => new TranslationItem(p.Name, (string)p.Value))
                .ToList());

            var wsKey = GetWorkspace(path);

            newTranslationFiles.Add(new KeyValuePair<string, TranslationFile>(wsKey, translationFile));
        }*/

        var message = $"Next files have empty keys:\r\n\r\n";

        var exists = false;

        var i = 0;

        foreach (var module in ModuleFolders)
        {
            if (module.AvailableLanguages == null)
                continue;

            foreach (var lng in module.AvailableLanguages)
            {
                var emptyTranslationItems = lng.Translations.Where(f => string.IsNullOrEmpty(f.Value)).ToList();

                if (!emptyTranslationItems.Any())
                    continue;

                exists = true;

                message += $"{++i}. Language '{lng.Language}' (Count: {emptyTranslationItems.Count}). Path '{lng.Path}' " +
                    $"Empty keys:\r\n\r\n";

                var emptyKeys = emptyTranslationItems.Select(t => t.Key).ToList();

                message += string.Join("\r\n", emptyKeys) + "\r\n\r\n";

                // Uncomment if you want to remove empty keys
                //RemoveEmptyKeys(lng.Path, emptyKeys);

                // Uncomment if new keys are available for saving
                /*var fileName = Path.GetFileName(lng.Path);

                var newKeys = newTranslationFiles
                     .Where(d => lng.Path.Contains(d.Key))
                     .Select(d => d.Value)
                     .Where(t => t.Language == lng.Language
                              && t.FileName == fileName)
                     .SelectMany(t => t.Translations.Where(t => emptyKeys.Contains(t.Key)))
                     .ToList();

                UpdateKeys(lng.Path, newKeys);*/
            }
        }

        foreach (var lng in CommonTranslations)
        {
            var emptyTranslationItems = lng.Translations.Where(f => string.IsNullOrEmpty(f.Value)).ToList();

            if (!emptyTranslationItems.Any())
                continue;

            exists = true;

            message += $"{++i}. Language '{lng.Language}' (Count: {emptyTranslationItems.Count}). Path '{lng.Path}' " +
                $"Empty keys:\r\n\r\n";

            var emptyKeys = emptyTranslationItems.Select(t => t.Key).ToList();

            message += string.Join("\r\n", emptyKeys) + "\r\n\r\n";

            // Uncomment if you want to remove empty keys
            //RemoveEmptyKeys(lng.Path, emptyKeys);

            // Uncomment if new keys are available for saving
            /*var newKeys = newTranslationFiles
                 .Select(d => d.Value)
                 .Where(t => t.Language == lng.Language)
                 .SelectMany(t => t.Translations.Where(t => emptyKeys.Contains(t.Key)))
                 .GroupBy(t => t.Key)
                 .Select(g => g.ToList().FirstOrDefault())
                 .ToList();

            UpdateKeys(lng.Path, newKeys);*/
        }

        Assert.AreEqual(false, exists, message);
    }

    [Test]
    [Category("Locales")]
    public void LanguageTranslatedPercentTest()
    {
        var message = $"Next languages translated less then 100%:\r\n\r\n";

        var groupedByLng = TranslationFiles
            .GroupBy(t => t.Language)
            .Select(g => new
            {
                Language = g.Key,
                AllTranslated = g.ToList()
                    .SelectMany(t => t.Translations)
                    .ToList()
            })
            .Select(t => new
            {
                t.Language,
                TotalKeysCount = t.AllTranslated.LongCount(),
                EmptyKeysCount = t.AllTranslated
                    .Where(t => string.IsNullOrEmpty(t.Value))
                    .LongCount()
            })
            .ToList();

        var i = 0;
        var exists = false;

        var expectedTotalKeysCount = groupedByLng.Where(t => t.Language == "en").Single().TotalKeysCount;

        foreach (var lng in groupedByLng)
        {
            if (lng.EmptyKeysCount == 0 && lng.TotalKeysCount == expectedTotalKeysCount)
                continue;

            exists = true;

            var translated = lng.TotalKeysCount == expectedTotalKeysCount
                ? Math.Round(100f - (lng.EmptyKeysCount * 100f / expectedTotalKeysCount), 1)
                : Math.Round(lng.TotalKeysCount * 100f / expectedTotalKeysCount, 1);

            message += $"{++i}. Language '{lng.Language}' translated by '{translated}%'\r\n";
        }

        Assert.AreEqual(false, exists, message);
    }

    [Test]
    [Category("Locales")]
    public void NotTranslatedToastsTest()
    {
        var message = $"Next text not translated in toasts:\r\n\r\n";

        var i = 0;

        NotTranslatedToasts.GroupBy(t => t.Key)
            .Select(g => new
            {
                FilePath = g.Key,
                Values = g.ToList()
            })
            .ToList()
            .ForEach(t =>
            {
                message += $"{++i}. Path='{t.FilePath}'\r\n\r\n{string.Join("\r\n", t.Values.Select(v => v.Value))}\r\n\r\n";
            });

        Assert.AreEqual(0, NotTranslatedToasts.Count, message);
    }

    [Test]
    [Category("Locales")]
    public void NotTranslatedPropsTest()
    {
        var message = $"Next text not translated props (title, placeholder, label, text):\r\n\r\n";

        var i = 0;

        NotTranslatedProps.GroupBy(t => t.Key)
            .Select(g => new
            {
                FilePath = g.Key,
                Values = g.ToList()
            })
            .ToList()
            .ForEach(t =>
            {
                message += $"{++i}. Path='{t.FilePath}'\r\n\r\n{string.Join("\r\n", t.Values.Select(v => v.Value))}\r\n\r\n";
            });

        Assert.AreEqual(0, NotTranslatedProps.Count, message);
    }

    [Test]
    [Category("Locales")]
    public void WrongTranslationVariablesTest()
    {
        var message = $"Next keys have wrong or empty variables:\r\n\r\n";
        var regVariables = new Regex("\\{\\{([^\\{].?[^\\}]+)\\}\\}", RegexOptions.Compiled | RegexOptions.Multiline);

        var groupedByLng = TranslationFiles
            .GroupBy(t => t.Language)
            .Select(g => new
            {
                Language = g.Key,
                TranslationsWithVariables = g.ToList()
                    .SelectMany(t => t.Translations)
                    //.Where(k => k.Value.IndexOf("{{") != -1)
                    .Select(t => new
                    {
                        t.Key,
                        t.Value,
                        Variables = regVariables.Matches(t.Value)
                                    .Select(m => m.Groups[1]?.Value?.Trim().Replace(", lowercase", ""))
                                    .ToList()
                    })
                    .ToList()
            })
            .ToList();

        var enWithVariables = groupedByLng
            .Where(t => t.Language == "en")
            .SelectMany(t => t.TranslationsWithVariables)
            .Where(t => t.Variables.Count > 0)
            .ToList();

        var otherLanguagesWithVariables = groupedByLng
            .Where(t => t.Language != "en")
            .ToList();

        var i = 0;
        var errorsCount = 0;

        foreach (var enKeyWithVariables in enWithVariables)
        {
            foreach (var lng in otherLanguagesWithVariables)
            {
                var lngKey = lng.TranslationsWithVariables
                    .Where(t => t.Key == enKeyWithVariables.Key)
                    .FirstOrDefault();

                if (lngKey == null)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{enKeyWithVariables.Key}' not found\r\n\r\n";
                    errorsCount++;
                    continue;
                }

                if (enKeyWithVariables.Variables.Count != lngKey.Variables.Count)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{lngKey.Key}' has less variables then 'en' language have " +
                        $"(en={enKeyWithVariables.Variables.Count}|{lng.Language}={lngKey.Variables.Count})\r\n" +
                        $"'en': '{enKeyWithVariables.Value}'\r\n'{lng.Language}': '{lngKey.Value}'\r\n\r\n";
                    errorsCount++;
                }

                if (!lngKey.Variables.All(v => enKeyWithVariables.Variables.Contains(v)))
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{lngKey.Key}' has not equals variables of 'en' language have \r\n" +
                        $"'{enKeyWithVariables.Value}' Variables=[{string.Join(",", enKeyWithVariables.Variables)}]\r\n" +
                        $"'{lngKey.Value}' Variables=[{string.Join(",", lngKey.Variables)}]\r\n\r\n";
                    errorsCount++;
                }
            }
        }

        Assert.AreEqual(0, errorsCount, message);
    }

    [Test]
    [Category("Locales")]
    public void WrongTranslationTagsTest()
    {
        var message = $"Next keys have wrong or empty translation's html tags:\r\n\r\n";
        var regString = "<([^>]*)>(\\s*(.+?)\\s*)</([^>/]*)>";

        var regTags = new Regex(regString, RegexOptions.Compiled | RegexOptions.Multiline);

        var groupedByLng = TranslationFiles
            .GroupBy(t => t.Language)
            .Select(g => new
            {
                Language = g.Key,
                TranslationsWithTags = g.ToList()
                    .SelectMany(t => t.Translations)
                    //.Where(k => k.Value.IndexOf("<") != -1)
                    .Select(t => new
                    {
                        t.Key,
                        t.Value,
                        Tags = regTags.Matches(t.Value)
                                    .Select(m => m.Groups[1]?.Value?.Trim())
                                    .ToList()
                    })
                    .ToList()
            })
            .ToList();

        var enWithTags = groupedByLng
            .Where(t => t.Language == "en")
            .SelectMany(t => t.TranslationsWithTags)
            .Where(t => t.Tags.Count > 0)
            .ToList();

        var otherLanguagesWithTags = groupedByLng
            .Where(t => t.Language != "en")
            .ToList();

        var i = 0;
        var errorsCount = 0;

        foreach (var enKeyWithTags in enWithTags)
        {
            foreach (var lng in otherLanguagesWithTags)
            {
                var lngKey = lng.TranslationsWithTags
                    .Where(t => t.Key == enKeyWithTags.Key)
                    .FirstOrDefault();

                if (lngKey == null)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{enKeyWithTags.Key}' not found\r\n\r\n";
                    errorsCount++;
                    continue;
                }

                if (enKeyWithTags.Tags.Count != lngKey.Tags.Count)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{lngKey.Key}' has less tags then 'en' language have " +
                        $"(en={enKeyWithTags.Tags.Count}|{lng.Language}={lngKey.Tags.Count})\r\n" +
                        $"'en': '{enKeyWithTags.Value}'\r\n'{lng.Language}': '{lngKey.Value}'\r\n\r\n";
                    errorsCount++;
                }

                if (!lngKey.Tags.All(v => enKeyWithTags.Tags.Contains(v)))
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{lngKey.Key}' has not equals tags of 'en' language have \r\n" +
                        $"'{enKeyWithTags.Value}' Tags=[{string.Join(",", enKeyWithTags.Tags)}]\r\n" +
                        $"'{lngKey.Value}' Tags=[{string.Join(",", lngKey.Tags)}]\r\n\r\n";
                    errorsCount++;
                }
            }

        }

        /*foreach (var lng in otherLanguagesWithTags)
        {
            foreach (var t in lng.TranslationsWithTags)
            {
                var enKey = enWithTags
                    .Where(en => en.Key == t.Key)
                    .FirstOrDefault();

                if (enKey == null)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has no 'en' language variant (!!!useless key!!!)\r\n\r\n";
                    errorsCount++;
                    continue;
                }

                if (enKey.Tags.Count != t.Tags.Count)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has less tags then 'en' language have " +
                        $"(en={enKey.Tags.Count}|{lng.Language}={t.Tags.Count})\r\n" +
                        $"'en': '{enKey.Value}'\r\n'{lng.Language}': '{t.Value}'\r\n\r\n";
                    errorsCount++;
                }

                if (!t.Tags.All(v => enKey.Tags.Contains(v)))
                {
                    // wrong
                    errorsCount++;
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has not equals tags of 'en' language have\r\n\r\n" +
                        $"Have to be:\r\n'{enKey.Value}'\r\n\r\n{string.Join("\r\n", enKey.Tags)}\r\n\r\n" +
                        $"But in real:\r\n'{t.Value}'\r\n\r\n{string.Join("\r\n", t.Tags)} \r\n\r\n";
                }
            }
        }*/

        Assert.AreEqual(0, errorsCount, message);
    }

    //[Test]
    //[Category("Locales")]
    //public void TranslationsEncodingTest()
    //{
    //    /*//Convert to UTF-8
    //    foreach (var issue in WrongEncodingJsonErrors)
    //    {
    //        if (issue.DetectionDetail.Encoding == null)
    //            continue;

    //        ConvertFileEncoding(issue.Path, issue.Path, issue.DetectionDetail.Encoding, Encoding.UTF8);
    //    }*/

    //    var message = $"Next files have encoding issues:\r\n\r\n";

    //    Assert.AreEqual(0, WrongEncodingJsonErrors.Count,
    //       message + string.Join("\r\n", WrongEncodingJsonErrors
    //            .Select(e => $"File path = '{e.Path}' potentially wrong file encoding: {e.DetectionDetail.EncodingName}")));
    //}

    /// <summary>
    /// Converts a file from one encoding to another.
    /// </summary>
    /// <param name="sourcePath">the file to convert</param>
    /// <param name="destPath">the destination for the converted file</param>
    /// <param name="sourceEncoding">the original file encoding</param>
    /// <param name="destEncoding">the encoding to which the contents should be converted</param>
    //public static void ConvertFileEncoding(string sourcePath, string destPath,
    //                                       Encoding sourceEncoding, Encoding destEncoding)
    //{
    //    // If the destination's parent doesn't exist, create it.
    //    var parent = Path.GetDirectoryName(Path.GetFullPath(destPath));
    //    if (!Directory.Exists(parent))
    //    {
    //        Directory.CreateDirectory(parent);
    //    }
    //    // If the source and destination encodings are the same, just copy the file.
    //    if (sourceEncoding == destEncoding)
    //    {
    //        File.Copy(sourcePath, destPath, true);
    //        return;
    //    }
    //    // Convert the file.
    //    string tempName = null;
    //    try
    //    {
    //        tempName = Path.GetTempFileName();
    //        using (StreamReader sr = new StreamReader(sourcePath, sourceEncoding, false))
    //        {
    //            using (StreamWriter sw = new StreamWriter(tempName, false, destEncoding))
    //            {
    //                int charsRead;
    //                char[] buffer = new char[128 * 1024];
    //                while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
    //                {
    //                    sw.Write(buffer, 0, charsRead);
    //                }
    //            }
    //        }
    //        File.Delete(destPath);
    //        File.Move(tempName, destPath);
    //    }
    //    finally
    //    {
    //        File.Delete(tempName);
    //    }
    //}

    /*[Test]
    public void TempTest()
    {
        var newTranslationsBasePath = @"D:\trans";

        var translationFiles = from file in Directory.EnumerateFiles(newTranslationsBasePath, "*.json", SearchOption.AllDirectories)
                               select file;

        var newTranslationFiles = new List<TranslationFile>();

        foreach (var path in translationFiles)
        {
            var jsonTranslation = JObject.Parse(File.ReadAllText(path));

            var translationFile = new TranslationFile(path, jsonTranslation.Properties()
                .Select(p => new TranslationItem(p.Name, (string)p.Value))
                .ToList());

            newTranslationFiles.Add(translationFile);
        }

        var groupedNewTranslation = newTranslationFiles
                .GroupBy(t => t.Language)
                .Select(g => new
                {
                    Language = g.Key,
                    Translations = g.ToList().SelectMany(t => t.Translations)
                })
                .ToList();

        var enCommonTranslations = CommonTranslations.Where(t => t.Language == "en").ToList();

        foreach (var lng in CommonTranslations.Where(t => t.Language != "en"))
        {
            var emptyTranslationItems = lng.Translations.Where(f => string.IsNullOrEmpty(f.Value)).ToList();

            if (!emptyTranslationItems.Any())
                continue;

            var emptyKeys = emptyTranslationItems.Select(t => t.Key).ToList();

            var enCommonKeyValues = enCommonTranslations
                .SelectMany(t => t.Translations)
                .Where(t => emptyKeys.Contains(t.Key))
                .ToList();

            var newTranslationsKeysByOldEnContent = groupedNewTranslation
                .Where(t => t.Language == "en")
                .SelectMany(t => t.Translations)
                .Where(t => enCommonKeyValues.Exists(c => c.Value == t.Value))
                //.Select(t => new TranslationItem(enCommonKeyValues.Find(c => c.Value == t.Value).Key, t.Value))
                .ToList();

            // Uncomment if new keys are available for saving

            var newKeys = new List<TranslationItem>();

            newTranslationFiles
                .Where(t => t.Language == lng.Language)
                .SelectMany(t => t.Translations)
                .ToList()
                .ForEach(t =>
                {
                    var newItem = newTranslationsKeysByOldEnContent.Where(k => k.Key == t.Key).FirstOrDefault();

                    if (newItem != null)
                    {
                        var value = t.Value;
                        var newCommonKey = enCommonKeyValues.Find(c => c.Value == newItem.Value).Key;

                        if (!newKeys.Exists(k => k.Key == newCommonKey))
                        {
                            newKeys.Add(new TranslationItem(newCommonKey, value));
                        }
                    }
                    //if(newKeys.Exists(k => k.Key))
                });
            //.Where(t => newTranslationsKeysByOldEnContent.Exists(t.Key))
            //.GroupBy(t => t.Key)
            //.Select(g => g.ToList().FirstOrDefault())
            //.ToList();

            if (newKeys.Any())
                UpdateKeys(lng.Path, newKeys);
        }

    }*/
}