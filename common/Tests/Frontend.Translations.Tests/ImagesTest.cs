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
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using Frontend.Tests;
using Frontend.Tests.Models;

using NUnit.Framework;

namespace Frontend.Tests;

public class ImagesTest
{
    public static string BasePath
    {
        get
        {
            return Environment.GetEnvironmentVariable("BASE_DIR") ?? Path.GetFullPath(Utils.ConvertPathToOS("../../../../../../"));
        }
    }

    public List<string> Workspaces { get; set; }
    public List<ImageFile> ImageFiles { get; set; }
    public List<JavaScriptFile> JavaScriptFiles { get; set; }
    public Dictionary<string, string> HashErrorFiles { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        HashErrorFiles = new Dictionary<string, string>();

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

        Workspaces.Add(Utils.ConvertPathToOS("public/images"));

        //var searchPaterns = new string[] { "*.svg", "*.png", "*.jpg", "*.ico", "*.jpeg" };
        var searchPatern = @"\.svg|\.png|\.jpg|\.ico|\.jpeg";

        var imageFiles = from wsPath in Workspaces
                               let clientDir = Path.Combine(BasePath, wsPath)
                               from filePath in Utils.GetFiles(clientDir, searchPatern, SearchOption.AllDirectories)
                               where filePath.Contains(Utils.ConvertPathToOS("public/images/"))
                               select Path.GetFullPath(filePath);

        TestContext.Progress.WriteLine($"Base path = {BasePath}");
        TestContext.Progress.WriteLine($"Found imageFiles by '{searchPatern}' filter = {imageFiles.Count()}. First path is '{imageFiles.FirstOrDefault()}'");

        ImageFiles = new List<ImageFile>();

        foreach (var path in imageFiles)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        var md5hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                        ImageFiles.Add(new ImageFile(path, md5hash));

                    }
                }
            }
            catch (Exception ex)
            {
                HashErrorFiles.Add(path, ex.Message);
                TestContext.Progress.WriteLine($"File path = {path} failed to parse with error: {ex.Message}");
            }
        }

        TestContext.Progress.WriteLine($"Found TranslationFiles = {ImageFiles.Count()}. First path is '{ImageFiles.FirstOrDefault()?.FilePath}'");

        searchPatern = @"\.js|\.jsx|\.ts|\.tsx";

        var javascriptFiles = (from wsPath in Workspaces
                               let clientDir = Path.Combine(BasePath, wsPath)
                               from filePath in Utils.GetFiles(clientDir, searchPatern, SearchOption.AllDirectories)
                               where !filePath.Contains(Utils.ConvertPathToOS("dist/"))
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

        /*var pattern1 = "[.{\\s\\(]t\\(\\s*[\"\'`]([a-zA-Z0-9_.:\\s{}/-]+)[\"\'`]\\s*[\\),]";
        var pattern2 = "i18nKey=\"([a-zA-Z0-9_.:-]+)\"";
        var pattern3 = "tKey:\\s\"([a-zA-Z0-9_.:-]+)\"";
        var pattern4 = "getTitle\\(\"([a-zA-Z0-9_.:-]+)\"\\)";

        var regexp = new Regex($"({pattern1})|({pattern2})|({pattern3})|({pattern4})", RegexOptions.Multiline | RegexOptions.ECMAScript);

        var notTranslatedToastsRegex = new Regex("(?<=toastr.info\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.error\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.success\\([\"`\'])(.*)(?=[\"\'`])" +
            "|(?<=toastr.warn\\([\"`\'])(.*)(?=[\"\'`])", RegexOptions.Multiline | RegexOptions.ECMAScript);

        NotTranslatedToasts = new List<KeyValuePair<string, string>>();

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

        TestContext.Progress.WriteLine($"Found _md5Excludes = {_md5Excludes.Count()} Path to file '{_md5ExcludesPath}'");*/

    }

    [Test]
    [Category("FastRunning")]
    public void ParseMd5Test()
    {
        Assert.AreEqual(0, HashErrorFiles.Count, string.Join("\r\n", HashErrorFiles.Select(e => $"File path = '{e.Key}' failed to parse with error: '{e.Value}'")));
    }

    [Test]
    [Category("FastRunning")]
    public void DublicatesFilesByMD5HashTest()
    {
        var duplicatesByMD5 = ImageFiles
            .GroupBy(t => t.Md5Hash)
            .Where(grp => grp.Count() > 1)
            .Select(grp => new { Key = grp.Key, Count = grp.Count(), Paths = grp.ToList().Select(f => f.FilePath) })
            .OrderByDescending(itm => itm.Count)
            .ToList();

        Assert.AreEqual(0, duplicatesByMD5.Count, "Dublicates by MD5 hash:\r\n" + string.Join("\r\n", duplicatesByMD5.Select(d => $"\r\nMD5='{d.Key}':\r\n{string.Join("\r\n", d.Paths.Select(p => p))}'")));
    }

    /*[Test]
    [Category("FastRunning")]
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
    [Category("FastRunning")]
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
    [Category("FastRunning")]
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
    [Category("FastRunning")]
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

            var notCommonKeys = module.AppliedJsTranslationKeys
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
                    message += $"{++index}. 'ANY LANGUAGES' '{module.Path}' \r\n {string.Join("\r\n", notCommonKeys)} \r\n";

                    notFoundi18nKeys.Add(new KeyValuePair<string, List<string>>("ANY LANGUAGES", notCommonKeys));
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
    [Category("FastRunning")]
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
    [Category("FastRunning")]
    public void EmptyValueKeysTest()
    {
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

        }

        Assert.AreEqual(false, exists, message);
    }

    [Test]
    [Category("FastRunning")]
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
    [Category("FastRunning")]
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
    [Category("FastRunning")]
    public void WrongTranslationVariablesTest()
    {
        var message = $"Next keys have wrong variables:\r\n\r\n";
        var regVariables = new Regex("\\{\\{([^\\{].?[^\\}]+)\\}\\}", RegexOptions.Compiled | RegexOptions.Multiline);

        var groupedByLng = TranslationFiles
            .GroupBy(t => t.Language)
            .Select(g => new
            {
                Language = g.Key,
                TranslationsWithVariables = g.ToList()
                    .SelectMany(t => t.Translations)
                    .Where(k => k.Value.IndexOf("{{") != -1)
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
            .ToList();

        var otherLanguagesWithVariables = groupedByLng
            .Where(t => t.Language != "en")
            .ToList();

        var i = 0;
        var errorsCount = 0;

        foreach (var lng in otherLanguagesWithVariables)
        {
            foreach (var t in lng.TranslationsWithVariables)
            {
                var enKey = enWithVariables
                    .Where(en => en.Key == t.Key)
                    .FirstOrDefault();

                if (enKey == null)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has no 'en' language variant (!!!useless key!!!)\r\n\r\n";
                    errorsCount++;
                    continue;
                }

                if (enKey.Variables.Count != t.Variables.Count)
                {
                    // wrong
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has less variables then 'en' language have " +
                        $"(en={enKey.Variables.Count}|{lng.Language}={t.Variables.Count})\r\n" +
                        $"'en': '{enKey.Value}'\r\n'{lng.Language}': '{t.Value}'\r\n\r\n";
                    errorsCount++;
                }

                if (!t.Variables.All(v => enKey.Variables.Contains(v)))
                {
                    // wrong
                    errorsCount++;
                    message += $"{++i}. lng='{lng.Language}' key='{t.Key}' has not equals variables of 'en' language have\r\n\r\n" +
                        $"Have to be:\r\n'{enKey.Value}'\r\n\r\n{string.Join("\r\n", enKey.Variables)}\r\n\r\n" +
                        $"But in real:\r\n'{t.Value}'\r\n\r\n{string.Join("\r\n", t.Variables)} \r\n\r\n";
                }
            }
        }

        Assert.AreEqual(0, errorsCount, message);
    }*/

}