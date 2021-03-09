/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Resources;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Resource.Manager
{
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
                var toAddFiles = new Dictionary<string,ResXFileRef>();

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
                                    if (!keys.Any(r=> r.EndsWith("*") && k.StartsWith(r.Replace("*", ""))) && (!k.Contains("_") || k.StartsWith("subject_") || k.StartsWith("pattern_")))
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

                foreach(var f in toAddFiles)
                {
                    resXResourceWriter.AddResource(new ResXDataNode(f.Key, f.Value));
                }

                resXResourceWriter.Generate();
                resXResourceWriter.Close();
            }

            return true;
        }
    }
}
