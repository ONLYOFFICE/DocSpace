using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        [SetUp]
        public void Setup()
        {
            var packageJsonPath = Path.Combine(BasePath, @"package.json");
            var jsonPackage = JObject.Parse(File.ReadAllText(packageJsonPath));

            Workspaces = ((JArray)jsonPackage["workspaces"]).Select(p => (string)p).ToList();

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

                translationFile.Translations = jsonTranslation.Properties().Select(p => new TranslationItem(p.Name, (string)p.Value)).ToList();

                TranslationFiles.Add(translationFile);
            }

            var javascriptFiles = (from wsPath in Workspaces
                                   let clientDir = Path.Combine(BasePath, wsPath)
                                   from file in Directory.EnumerateFiles(clientDir, "*.js", SearchOption.AllDirectories)
                                   where file.Contains("src\\")
                                   select file)
                                  .ToList();

            javascriptFiles.AddRange(from wsPath in Workspaces
                                     let clientDir = Path.Combine(BasePath, wsPath)
                                     from file in Directory.EnumerateFiles(clientDir, "*.jsx", SearchOption.AllDirectories)
                                     where file.Contains("src\\")
                                     select file);

            JavaScriptFiles = new List<JavaScriptFile>();

            var pattern1 = "[.{\\s\\(]t\\(\\s*[\"\'`]([a-zA-Z0-9_.:_\\$\\s{}/_-]+)[\"\'`]\\s*[\\),]";
            var pattern2 = "i18nKey=\"([a-zA-Z0-9_.-]+)\"";

            var regexp = new Regex($"({pattern1})|({pattern2})", RegexOptions.Multiline | RegexOptions.ECMAScript);

            foreach (var path in javascriptFiles)
            {
                var jsFileText = File.ReadAllText(path);

                var matches = regexp.Matches(jsFileText);

                var translationKeys = matches.Select(m => m.Groups[2].Value == "" ? m.Groups[4].Value : m.Groups[2].Value).ToList();

                if (!translationKeys.Any())
                    continue;

                var jsFile = new JavaScriptFile(path);

                jsFile.TranslationKeys = translationKeys;

                JavaScriptFiles.Add(jsFile);
            }
        }

        [Test]
        public void DublicatesByContentTest()
        {
            var allDuplicates = TranslationFiles
                .Where(file => file.Path.Contains("public\\locales\\en"))
                .SelectMany(item => item.Translations)
                .GroupBy(t => t.Value)
                .Where(grp => grp.Count() > 1)
                .Select(grp => new { Key = grp.Key, Count = grp.Count(), Grouped = grp.ToList() })
                //.Where(grp => grp.Grouped.GroupBy(n => n.Key).Any(c => c.Count() > 1))
                .OrderByDescending(itm => itm.Count)
                .ToList();

            Assert.AreEqual(0, allDuplicates.Count);
        }

        [Test]
        public void NotAllLanguageTranslatedTest()
        {
            var groupedByLng = TranslationFiles
                .GroupBy(t => t.Language)
                .Select(grp => new { Lng = grp.Key, Count = grp.Count() })
                .ToList();

            var enGroup = groupedByLng.Find(f => f.Lng == "en");

            foreach (var lng in groupedByLng.Where(g => g.Lng != "en"))
            {
                Assert.AreEqual(enGroup.Count, lng.Count, "language '{0}' is not equals 'en' by translated files count", lng.Lng);
            }

            //Assert.AreEqual(true, groupedByLng.All(g => g.Count == enGroup.Count));
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
                })
                .ToList();

            var enGroup = groupedByLng.Find(f => f.Lng == "en");

            var expectedCount = enGroup.Keys.Count();

            foreach (var lng in groupedByLng.Where(g => g.Lng != "en"))
            {
                Assert.AreEqual(expectedCount, lng.Keys.Count(), "language '{0}' is not equals 'en' by translated keys count", lng.Lng);
            }

            //Assert.AreEqual(true, groupedByLng.All(g => g.Keys.Count() == enGroup.Keys.Count()));
        }

        [Test]
        public void NotFoundKeysTest()
        {
            var allEnKeys = TranslationFiles
                 .Where(file => file.Path.Contains("public\\locales\\en"))
                 .SelectMany(item => item.Translations)
                 .Select(item => item.Key);

            var allJsTranslationKeys = JavaScriptFiles
                .SelectMany(j => j.TranslationKeys)
                .Select(k => k.Replace("Common:", ""))
                .Distinct();

            var notFoundJsKeys = allJsTranslationKeys.Except(allEnKeys);

            Assert.AreEqual(0, notFoundJsKeys.Count(), "Some i18n-keys are not exist in translations in 'en' language: Keys: '{0}'", string.Join(", ", notFoundJsKeys));
        }

        [Test]
        public void UselessTranslationKeysTest()
        {
            var allEnKeys = TranslationFiles
                 .Where(file => file.Path.Contains("public\\locales\\en"))
                 .SelectMany(item => item.Translations)
                 .Select(item => item.Key)
                 .Where(k => !k.StartsWith("Culture_"));

            var allJsTranslationKeys = JavaScriptFiles
                .SelectMany(j => j.TranslationKeys)
                .Select(k => k.Replace("Common:", ""))
                .Where(k => !k.StartsWith("Culture_"))
                .Distinct();

            var notFoundi18nKeys = allEnKeys.Except(allJsTranslationKeys);

            Assert.AreEqual(0, notFoundi18nKeys.Count(), "Some i18n-keys are not found in js: Keys: '{0}'", string.Join(", ", notFoundi18nKeys));
        }
    }
}