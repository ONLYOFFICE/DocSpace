using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace Frontend.Translations.Tests
{
    public class Tests
    {
        public string BasePath
        {
            get
            {
                return "..\\..\\..\\..\\..\\..\\";
            }
        }

        public List<string> Workspaces { get; set; }

        public List<TranslationFile> TranslationFiles { get; set; }

        [SetUp]
        public void Setup()
        {
            var packageJsonPath = Path.Combine(BasePath, @"package.json");
            var jsonPackage = JObject.Parse(File.ReadAllText(packageJsonPath));

            Workspaces = ((JArray)jsonPackage["workspaces"]).Select(p => (string)p).ToList();

            Workspaces.Add("public\\locales");

            var files = from wsPath in Workspaces
                        let clientDir = Path.Combine(BasePath, wsPath)
                        from file in Directory.EnumerateFiles(clientDir, "*.json", SearchOption.AllDirectories)
                        where file.Contains("public\\locales\\en")
                        select file;

            TranslationFiles = new List<TranslationFile>();

            foreach (var file in files)
            {
                var jsonTranslation = JObject.Parse(File.ReadAllText(file));

                var translationFile = new TranslationFile(file);

                translationFile.Translations = jsonTranslation.Properties().Select(p => new TranslationItem(p.Name, (string)p.Value)).ToList();

                TranslationFiles.Add(translationFile);
            }
        }

        [Test]
        public void DublicatesByContentTest()
        {
            var allDuplicates = TranslationFiles.SelectMany(item => item.Translations)
                .GroupBy(t => t.Value)
                .Where(grp => grp.Count() > 1)
                .Select(grp => new { Key = grp.Key, Count = grp.Count() })
                .OrderByDescending(itm => itm.Count)
                .ToList();

            Assert.AreEqual(0, allDuplicates.Count());
        }
    }
}