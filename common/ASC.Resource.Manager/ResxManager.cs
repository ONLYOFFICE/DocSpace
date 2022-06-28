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

public class ResxManager
{
    public static bool Export(IServiceProvider serviceProvider, string project, string module, string fName, string language, string exportPath, string key = null)
    {
        var filter = new ResCurrent
        {
            Project = new ResProject { Name = project },
            Module = new ResModule { Name = module },
            Language = new ResCulture { Title = language },
            Word = new ResWord() { ResFile = new ResFile() { FileName = fName } }
        };

        using var scope = serviceProvider.CreateScope();
        var resourceData = scope.ServiceProvider.GetService<ResourceData>();
        var words = resourceData.GetListResWords(filter, string.Empty).GroupBy(x => x.ResFile.FileID).ToList();

        if (!words.Any())
        {
            Console.WriteLine($"db empty file:{fName}, lang:{language}");
            return false;
        }

        foreach (var fileWords in words)
        {
            var wordsDictionary = new Dictionary<string, object>();

            var firstWord = fileWords.FirstOrDefault();
            var fileName = firstWord == null
                ? module
                : Path.GetFileNameWithoutExtension(firstWord.ResFile.FileName);
            var zipFileName = Path.Combine(exportPath, $"{fileName}{(language == "Neutral" ? string.Empty : "." + language)}.resx");
            var dirName = Path.GetDirectoryName(zipFileName);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            var toAdd = new List<ResWord>();
            var toAddFiles = new Dictionary<string, ResXFileRef>();

            if (!string.IsNullOrEmpty(key))
            {
                var keys = key.Split(",").Distinct();

                if (File.Exists(zipFileName))
                {
                    using var resXResourceReader = new ResXResourceReader(zipFileName);
                    resXResourceReader.BasePath = Path.GetDirectoryName(zipFileName);
                    resXResourceReader.UseResXDataNodes = true;

                    foreach (var v in resXResourceReader.OfType<DictionaryEntry>())
                    {
                        var k = v.Key.ToString();
                        var val = v.Value as ResXDataNode;

                        if (keys.Any())
                        {
                            if (val.FileRef != null)
                            {
                                var fileRef = new ResXFileRef(Path.GetFileName(val.FileRef.FileName), val.FileRef.TypeName);
                                toAddFiles.Add(k, fileRef);
                            }
                            else
                            {
                                if (!keys.Any(r => r.EndsWith("*") && k.StartsWith(r.Replace("*", ""))) && (!k.Contains("_") || k.StartsWith("subject_") || k.StartsWith("pattern_")))
                                {
                                    k = keys.FirstOrDefault(r => r == k);
                                }

                                if (k != null)
                                {
                                    var word = fileWords.FirstOrDefault(r => r.Title == k);
                                    if (word != null)
                                    {
                                        toAdd.Add(word);
                                    }
                                    else
                                    {
                                        toAdd.Add(new ResWord() { Title = k, ValueTo = val.GetValue((ITypeResolutionService)null)?.ToString() });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (val.FileRef != null)
                            {
                                var fileRef = new ResXFileRef(Path.GetFileName(val.FileRef.FileName), val.FileRef.TypeName);
                                toAddFiles.Add(k, fileRef);
                            }
                            else
                            {
                                toAdd.Add(new ResWord { Title = k, ValueTo = val.GetValue((ITypeResolutionService)null)?.ToString() });
                            }
                        }
                    }
                }

                foreach (var k in keys)
                {
                    if (!toAdd.Any(r => r.Title == k))
                    {
                        var exists = fileWords.FirstOrDefault(r => r.Title == k);
                        if (exists != null)
                        {
                            toAdd.Add(exists);
                        }
                    }
                }
            }
            else
            {
                if (File.Exists(zipFileName))
                {
                    using var resXResourceReader = new ResXResourceReader(zipFileName);
                    resXResourceReader.BasePath = Path.GetDirectoryName(zipFileName);
                    resXResourceReader.UseResXDataNodes = true;

                    foreach (var v in resXResourceReader.OfType<DictionaryEntry>())
                    {
                        var k = v.Key.ToString();
                        var val = v.Value as ResXDataNode;

                        if (val.FileRef != null)
                        {
                            var fileRef = new ResXFileRef(Path.GetFileName(val.FileRef.FileName), val.FileRef.TypeName);
                            toAddFiles.Add(k, fileRef);
                        }
                    }
                }

                toAdd.AddRange(fileWords.Where(word => !wordsDictionary.ContainsKey(word.Title)));
            }



            using var resXResourceWriter = new ResXResourceWriter(zipFileName);

            foreach (var word in toAdd.Where(r => r != null && (!string.IsNullOrEmpty(r.ValueTo) || language == "Neutral")).OrderBy(x => x.Title))
            {
                resXResourceWriter.AddResource(word.Title, word.ValueTo);
            }

            foreach (var f in toAddFiles)
            {
                resXResourceWriter.AddResource(new ResXDataNode(f.Key, f.Value));
            }

            resXResourceWriter.Generate();
            resXResourceWriter.Close();
        }

        return true;
    }
}
