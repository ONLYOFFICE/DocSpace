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

using System.Text;
using System.Xml;

using NUnit.Framework;

using ResXManager.Infrastructure;

namespace Backend.Translations.Tests;

public class Tests
{
    private Dictionary<FileInfo, IEnumerable<FileInfo>> _resources;
    private readonly HashSet<string> _excludedDirectories = new(new[] { "bin", "obj", "node_modules", "thirdparty", "migration" }, StringComparer.OrdinalIgnoreCase);

    [SetUp]
    public void Setup()
    {
        var basePath = Environment.GetEnvironmentVariable("BASE_DIR") ?? Path.GetFullPath("../../../../../../");

        var directory = new DirectoryInfo(basePath);
        var resources = GetResources(directory);
        var neutralresources = resources.Where(f => IsNeutral(f.Name));
        _resources = new Dictionary<FileInfo, IEnumerable<FileInfo>>();
        foreach (var resource in neutralresources)
        {
            var nameWithoutExt = resource.FullName.Substring(0, resource.FullName.Length - 5);
            _resources.Add(resource, resources.Where(r => r.FullName.StartsWith(nameWithoutExt)));
        }
    }

    private bool IsNeutral(string fileName)
    {
        var split = fileName.Split('.');
        if (split.Length == 2)
        {
            return true;
        }
        else
        {
            var valid = CultureHelper.IsValidCultureName(split[split.Length - 2]);
            return !valid;
        }
    }

    private IEnumerable<FileInfo> GetResources(DirectoryInfo directory)
    {
        foreach (var file in directory.EnumerateFiles())
        {
            if (IsResourceFile(file.Name))
            {
                yield return file;
            }
            else
            {
                continue;
            }
        }

        foreach (var subDirectory in directory.EnumerateDirectories())
        {
            var name = subDirectory.Name;
            if (name.StartsWith(".", StringComparison.Ordinal) || _excludedDirectories.Contains(name))
                continue;

            foreach (var file in GetResources(subDirectory))
            {
                if (IsResourceFile(file.Name))
                {
                    yield return file;
                }
                else
                {
                    continue;
                }
            }
        }
    }

    private bool IsResourceFile(string filePath, string? extension = null)
    {
        extension ??= Path.GetExtension(filePath);

        if (extension == ".resx")
            return true;

        return false;
    }

    [Test, Order(1)]
    public void LanguageTranslatedPercentTest()
    {
        var all = 0;
        var setByCulture = new Dictionary<string, int>();
        var allExist = true;
        var message = new StringBuilder("Next languages translated less then 100%:\n\n");

        foreach (var pair in _resources)
        {
            var set = new HashSet<string>();
            var dictionary = new Dictionary<FileInfo, HashSet<string>>();
            foreach (var resource in pair.Value)
            {
                var culture = GetCulture(resource.Name);
                var valid = CultureHelper.IsValidCultureName(culture);
                if (valid && !setByCulture.ContainsKey(culture))
                {
                    setByCulture.Add(culture, 0);
                }

                foreach (var entry in CreateTranslateDictionary(resource.FullName))
                {
                    if (!string.IsNullOrEmpty(entry.Value.ToString()))
                    {
                        set.Add(entry.Key.ToString());
                        if (valid)
                        {
                            setByCulture[culture] += 1;
                        }
                    }
                }
            }

            all += set.Count;
        }

        var counter = 0;
        foreach (var pair in setByCulture.Where(r => r.Value != all).OrderByDescending(r => Math.Round((r.Value * 100.0) / all, 1)))
        {
            allExist = false;
            message.AppendLine($"{++counter}. Language '{pair.Key}' translated by '{Math.Round((pair.Value * 100.0) / all, 1)}%'");
        }

        Assert.True(allExist, message.ToString());
    }

    [Test, Order(2)]
    public void NotAllLanguageTranslatedTest()
    {
        var all = new HashSet<string>();
        var groupByFile = new Dictionary<FileInfo, HashSet<string>>();
        var allExist = true;
        var message = new StringBuilder("Next languages are missing from files: \n\n");

        foreach (var pair in _resources)
        {
            var resources = pair.Value;
            var set = new HashSet<string>();
            foreach (var resource in resources)
            {
                var split = resource.Name.Split('.');
                if (split.Length == 2)
                {
                    all.Add("en");
                    set.Add("en");
                }
                else
                {
                    var culture = split[split.Length - 2];
                    var valid = CultureHelper.IsValidCultureName(split[split.Length - 2]);
                    if (valid)
                    {
                        all.Add(culture);
                        set.Add(culture);
                    }
                    else
                    {
                        all.Add("en");
                        set.Add("en");
                    }
                }
            }
            groupByFile.Add(pair.Key, set);
        }

        var counter = 0;

        foreach (var culture in all)
        {
            var notExist = groupByFile.Where(l => !l.Value.Contains(culture)).ToList();

            if (notExist.Any())
            {
                allExist = false;
                message.AppendLine($"{++counter}. Language '{culture}'. Not found files: \n");

                foreach (var f in notExist)
                {
                    message.AppendLine($"{f.Key.FullName}");
                }

                message.AppendLine();
            }
        }

        Assert.True(allExist, message.ToString());
    }

    [Test, Order(3)]
    public void NotTranslatedKeysTest()
    {
        var all = new Dictionary<FileInfo, HashSet<string>>();
        var groupByFile = new Dictionary<FileInfo, Dictionary<FileInfo, HashSet<string>>>();
        var allExist = true;
        var message = new StringBuilder("Next languages do not have all keys:\n\n");

        foreach (var pair in _resources)
        {
            var set = new HashSet<string>();
            var dictionary = new Dictionary<FileInfo, HashSet<string>>();
            foreach (var resource in pair.Value)
            {
                var innerSet = new HashSet<string>();
                foreach (var entry in CreateTranslateDictionary(resource.FullName))
                {
                    if (!string.IsNullOrEmpty(entry.Value.ToString()))
                    {
                        set.Add(entry.Key.ToString());
                        innerSet.Add(entry.Key.ToString());
                    }
                }
                dictionary.Add(resource, innerSet);
            }
            groupByFile.Add(pair.Key, dictionary);
            all.Add(pair.Key, set);
        }

        var counter = 0;

        foreach (var pair in groupByFile)
        {
            foreach (var keyValue in pair.Value)
            {
                var notExist = all[pair.Key].Where(l => !keyValue.Value.Contains(l)).ToList();
                if (notExist.Any())
                {
                    allExist = false;
                    var x = notExist.Count;
                    var y = all[pair.Key].Count();

                    var culture = GetCulture(keyValue.Key.Name);

                    message.AppendLine($"{++counter}. Language ('{culture}'={y - x}/'all keys'={y}). Path '{pair.Key.FullName}' Not found keys:");
                    message.AppendLine();

                    foreach (var key in notExist)
                    {
                        message.AppendLine($"{key}");
                    }

                    message.AppendLine();
                }
            }
        }

        Assert.True(allExist, message.ToString());
    }

    /*[Test, Order(4)]
    public void CompliesToRulePunctuationLead()
    {
        CompliesToRule("The punctuation at the start of the messages doesn't match up:\n", CheckRules.CompliesToRulePunctuationLead);
    }

    [Test, Order(5)]
    public void CompliesToRulePunctuationTail()
    {
        CompliesToRule("The punctuation at the end of the messages doesn't match up:\n", CheckRules.CompliesToRulePunctuationTail);
    }
    */
    [Test, Order(6)]
    public void CompliesToRuleWhiteSpaceLead()
    {
        CompliesToRule("The whitespaces at the start of the sequence don't match up:\n", CheckRules.CompliesToRuleWhiteSpaceLead);
    }

    [Test, Order(7)]
    public void CompliesToRuleWhiteSpaceTail()
    {
        CompliesToRule("The whitespaces at the end of the sequence don't match up:\n", CheckRules.CompliesToRuleWhiteSpaceTail);
    }

    [Test, Order(8)]
    public void CompliesToRuleWhiteStringFormat()
    {
        CompliesToRule("This items contains string format parameter mismatches:\n", CheckRules.CompliesToRuleStringFormat);
    }

    private void CompliesToRule(string message, Func<string, string, bool> compliesToRile)
    {
        var result = new StringBuilder(message);
        var counter = 0;

        var allRuleCheck = true;
        foreach (var pair in _resources)
        {
            var neutral = CreateTranslateDictionary(pair.Key.FullName);
            foreach (var resource in pair.Value)
            {
                var list = new Dictionary<string, (string, string)>();
                foreach (var entry in CreateTranslateDictionary(resource.FullName))
                {
                    if (neutral.TryGetValue(entry.Key, out var value))
                    {
                        if (compliesToRile(value, entry.Value))
                        {
                            list.Add(entry.Key, (value, entry.Value));
                            allRuleCheck = false;
                        }
                    }
                }

                if (list.Count > 0)
                {
                    var culture = GetCulture(resource.Name);

                    result.AppendLine();

                    foreach (var item in list)
                    {
                        result.AppendLine($"{++counter}. {resource.FullName}\n");
                        result.AppendLine($"key:{item.Key}\n");
                        result.AppendLine($"'en':{item.Value.Item1}\n");
                        result.AppendLine($"'{culture}':{item.Value.Item2}\n");
                    }
                }
            }
        }
        Assert.True(allRuleCheck, result.ToString());
    }

    private Dictionary<string, string> CreateTranslateDictionary(string filePath)
    {
        var dictionary = new Dictionary<string, string>();
        var name = "";
        using (var reader = XmlReader.Create(filePath))
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "data" && string.IsNullOrEmpty(reader["type"]))
                        {
                            name = reader["name"] ?? "";
                        }
                        break;
                    case XmlNodeType.Text:
                        if (!string.IsNullOrEmpty(name))
                        {
                            dictionary.Add(name, reader.Value);
                            name = "";
                        }
                        break;
                }
            }
        }

        return dictionary;
    }

    private string GetCulture(string fullName)
    {
        var split = fullName.Split('.');
        if (split.Count() == 3) {
            return split[split.Length - 2];
        }
        else
        {
            return "en";
        }
    }
}