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

public static class JsonManager
{
    public static void Upload(ILogger option, ResourceData resourceData, string fileName, Stream fileStream, string projectName, string moduleName)
    {
        var culture = GetCultureFromFileName(fileName);

        string jsonString;
        using (var reader = new StreamReader(fileStream))
        {
            jsonString = reader.ReadToEnd();
        }

        var jsonObj = new Dictionary<string, string>();

        if (Path.GetExtension(fileName) == ".xml")
        {
            var doc = new XmlDocument();
            doc.LoadXml(jsonString);
            var list = doc.SelectNodes("//resources//string");
            if (list != null)
            {
                try
                {
                    var nodes = list.Cast<XmlNode>().ToList();
                    jsonObj = nodes.ToDictionary(r => r.Attributes["name"].Value, r => r.InnerText);
                }
                catch (Exception e)
                {
                    option.LogError("parse xml " + fileName, e);
                }
            }
        }
        else
        {
            jsonObj = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }

        var fileID = resourceData.AddFile(fileName, projectName, moduleName);
        const string resourceType = "text";
        foreach (var key in jsonObj.Keys)
        {
            var word = new ResWord
            {
                Title = key,
                ValueFrom = jsonObj[key],
                ResFile = new ResFile { FileID = fileID }
            };
            if (culture != "Neutral")
            {
                var neutralKey = new ResWord
                {
                    Title = key,
                    ValueFrom = jsonObj[key],
                    ResFile = new ResFile { FileID = fileID }
                };

                resourceData.GetValueByKey(neutralKey, "Neutral");
                if (string.IsNullOrEmpty(neutralKey.ValueTo)) continue;
            }

            resourceData.AddResource(culture, resourceType, DateTime.UtcNow, word, true, "Console");
        }
    }

    public static bool Export(IServiceProvider serviceProvider, string project, string module, string fName, string language, string exportPath, string key = null)
    {
        var filter = new ResCurrent
        {
            Project = new ResProject { Name = project },
            Module = new ResModule { Name = module },
            Language = new ResCulture { Title = language },
            Word = new ResWord { ResFile = new ResFile { FileName = fName } }
        };

        using var scope = serviceProvider.CreateScope();
        var resourceData = scope.ServiceProvider.GetService<ResourceData>();
        var words = resourceData.GetListResWords(filter, string.Empty).GroupBy(x => x.ResFile.FileID).ToList();

        if (!words.Any())
        {
            Console.WriteLine("Error!!! Can't find appropriate project and module. Possibly wrong names!");
            return false;
        }

        foreach (var fileWords in words)
        {
            var wordsDictionary = new Dictionary<string, object>();
            var firstWord = fileWords.FirstOrDefault();
            var fileName = firstWord == null
                ? module
                : Path.GetFileNameWithoutExtension(firstWord.ResFile.FileName);
            var zipFileName = Path.Combine(exportPath, language == "Neutral" ? "en" : language, $"{fileName}.json");

            var dirName = Path.GetDirectoryName(zipFileName);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            var toAdd = new List<ResWord>();
            if (!string.IsNullOrEmpty(key))
            {
                if (File.Exists(zipFileName))
                {
                    var jObject = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(zipFileName, Encoding.UTF8));
                    foreach (var j in jObject)
                    {
                        toAdd.Add(new ResWord { Title = j.Key, ValueFrom = j.Value.ToString() });
                    }
                }

                if (!toAdd.Any(r => r.Title == key))
                {
                    toAdd.Add(fileWords.FirstOrDefault(r => r.Title == key));
                }
            }
            else
            {
                if (File.Exists(zipFileName))
                {
                    var jObject = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(zipFileName, Encoding.UTF8));
                    foreach (var f in fileWords.Where(word => !wordsDictionary.ContainsKey(word.Title)))
                    {
                        if (jObject.ContainsKey(f.Title))
                        {
                            toAdd.Add(f);
                        }
                    }

                    foreach (var j in jObject)
                    {
                        if (!toAdd.Any(r => r.Title == j.Key))
                        {
                            toAdd.Add(new ResWord { Title = j.Key, ValueFrom = j.Value, ValueTo = j.Value });
                        }
                    }
                }
                else
                {
                    toAdd.AddRange(fileWords.Where(word => !wordsDictionary.ContainsKey(word.Title)));
                }
            }

            foreach (var word in toAdd.Where(r => r != null))
            {
                if (string.IsNullOrEmpty(word.ValueTo)) continue;

                var newVal = word.ValueTo ?? word.ValueFrom;

                if (!string.IsNullOrEmpty(newVal))
                {
                    newVal = newVal.TrimEnd('\n').TrimEnd('\r');
                }

                var newKey = GetKey(word.Title, newVal);
                wordsDictionary.Add(newKey.Keys.First(), newKey.Values.First());
            }

            var serialized = JsonSerializer.Serialize(wordsDictionary.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value),
                new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });

            File.WriteAllText(zipFileName, serialized, Encoding.UTF8);

        }

        return true;
    }

    private static string GetCultureFromFileName(string fileName)
    {
        var culture = "Neutral";
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        if (nameWithoutExtension != null && nameWithoutExtension.Split('.').Length > 1)
        {
            culture = nameWithoutExtension.Split('.')[1];
        }

        return culture;
    }

    private static Dictionary<string, object> GetKey(string title, string val)
    {
        var splited = title.Split('.');
        if (splited.Length == 1) return new Dictionary<string, object>() { { title, val } };

        return new Dictionary<string, object>() { { splited[0], GetKey(title.Substring(title.IndexOf('.') + 1), val) } };
    }
}