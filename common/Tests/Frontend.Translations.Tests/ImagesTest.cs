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
using System.Text;
using System.Text.RegularExpressions;

using Frontend.Tests;
using Frontend.Tests.Models;

using Microsoft.VisualBasic;

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
    public List<SourceImageFile> SourceImageFiles { get; set; }
    public Dictionary<string, string> HashErrorFiles { get; set; }

    private static Dictionary<ModuleTypes, string> Modules
    {
        get
        {
            return new Dictionary<ModuleTypes, string>() {
                { ModuleTypes.PUBLIC, Path.Combine(BasePath,Utils.ConvertPathToOS("public")) },
                { ModuleTypes.COMMON, Path.Combine(BasePath,Utils.ConvertPathToOS("packages/common")) },
                { ModuleTypes.COMPONENTS, Path.Combine(BasePath,Utils.ConvertPathToOS("packages/components")) },
                { ModuleTypes.CLIENT, Path.Combine(BasePath,Utils.ConvertPathToOS("packages/client")) },
                { ModuleTypes.EDITOR, Path.Combine(BasePath,Utils.ConvertPathToOS("packages/editor")) },
                { ModuleTypes.LOGIN, Path.Combine(BasePath,Utils.ConvertPathToOS("packages/login")) }
            };
        }
    }

    private static ModuleTypes GetModuleType(string path)
    {
        var mType = Modules.First(m => path.Contains(m.Value)).Key;
        return mType;
    }

    [OneTimeSetUp]
    public void Setup()
    {
        TestContext.Progress.WriteLine($"Base path = {BasePath}");

        HashErrorFiles = new Dictionary<string, string>();

        Workspaces = Modules.Values.ToList();

        TestContext.Progress.WriteLine($"Workspaces: {string.Join("\r\n", Workspaces)}");

        //var searchPaterns = new string[] { "*.svg", "*.png", "*.jpg", "*.ico", "*.jpeg" };
        var imageSearchPatern = @"\.svg|\.png|\.jpg|\.ico|\.jpeg";

        var imageFiles = from wsPath in Workspaces
                         from filePath in Utils.GetFiles(wsPath, imageSearchPatern, SearchOption.AllDirectories)
                         where !filePath.Contains(Utils.ConvertPathToOS("dist/")) && !filePath.Contains(Utils.ConvertPathToOS("tests/"))
                         //where filePath.Contains(Utils.ConvertPathToOS("public/images/"))
                         select Path.GetFullPath(filePath);

        //TestContext.Progress.WriteLine($"Found imageFiles by filter '{imageSearchPatern}' count={imageFiles.Count()}. First path is '{imageFiles.FirstOrDefault()}'");

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

                        ImageFiles.Add(new ImageFile(path, GetModuleType(path), md5hash));

                    }
                }
            }
            catch (Exception ex)
            {
                HashErrorFiles.Add(path, ex.Message);
                TestContext.Progress.WriteLine($"File path = {path} failed to parse with error: {ex.Message}");
            }
        }

        TestContext.Progress.WriteLine($"Found ImageFiles by filter '{imageSearchPatern}' count={ImageFiles.Count}. First path is '{ImageFiles.FirstOrDefault()?.FilePath}'");

        var sourceSearchPatern = @"(\.js|\.jsx|\.ts|\.tsx|\.html|\.css|\.scss|\.sass)$";

        var sourceFiles = (from wsPath in Workspaces
                           from filePath in Utils.GetFiles(wsPath, sourceSearchPatern, SearchOption.AllDirectories)
                           where !filePath.Contains(Utils.ConvertPathToOS("dist/"))
                           && !filePath.Contains(Utils.ConvertPathToOS("tests/"))
                           && !filePath.Contains(".test.js")
                           && !filePath.Contains(".stories.js")
                           && !filePath.Contains(".test.ts")
                           && !filePath.Contains(".stories.ts")
                           && !filePath.Contains(".test.tsx")
                           && !filePath.Contains(".stories.tsx")
                           select Utils.ConvertPathToOS(filePath))
                              .ToList();

        //TestContext.Progress.WriteLine($"Found sourceFiles by '{searchPatern}' filter = {sourceFiles.Count()}. First path is '{sourceFiles.FirstOrDefault()}'");

        SourceImageFiles = new List<SourceImageFile>();

        var pattern = $"\"([a-zA-Z0-9_.:/-]+({imageSearchPatern}))\"";

        var regexp = new Regex(pattern, RegexOptions.Multiline | RegexOptions.ECMAScript);

        foreach (var path in sourceFiles)
        {
            var sourceText = File.ReadAllText(path);

            var mType = GetModuleType(path);

            var matches = regexp.Matches(sourceText);

            var images = matches
                .Select(m => m.Groups[1].Success
                    ? m.Groups[1].Value
                    : null)
                .Where(m => m != null)
                .Distinct()
                .Select(p => new ImageFile(p, mType))
                .ToList();

            if (!images.Any())
                continue;

            var sourceImageFile = new SourceImageFile(path, mType);

            sourceImageFile.Images = images;

            SourceImageFiles.Add(sourceImageFile);
        }

        TestContext.Progress.WriteLine($"Found SourceImageFiles by filter '{sourceSearchPatern}' " +
            $"count={SourceImageFiles.Count}. First path is '{SourceImageFiles.FirstOrDefault()?.Path}'");
    }

    [Test]
    [Category("Images")]
    public void ParseMd5Test()
    {
        Assert.AreEqual(0, HashErrorFiles.Count, string.Join("\r\n",
            HashErrorFiles.Select(e => $"File path = '{e.Key}' failed to parse with error: '{e.Value}'")));
    }

    [Test]
    [Category("Images")]
    public void DublicatesFilesByMD5HashTest()
    {
        var duplicatesByMD5 = ImageFiles
            .GroupBy(t => t.Md5Hash)
            .Where(grp => grp.Count() > 1)
            .Select(grp => new { Key = grp.Key, Count = grp.Count(), Paths = grp.ToList().Select(f => f.FilePath) })
            .OrderByDescending(itm => itm.Count)
            .ToList();

        Assert.AreEqual(0, duplicatesByMD5.Count, "Dublicates by MD5 hash:\r\n" +
            string.Join("\r\n", duplicatesByMD5.Select(d => $"\r\nMD5='{d.Key}':\r\n" +
            $"{string.Join("\r\n", d.Paths.Select(p => p))}'")));
    }

    [Test]
    [Category("Images")]
    public void DublicatesFilesByFileNameButDifferentByMD5HashTest()
    {
        var duplicatesByNameWithDifMD5 = ImageFiles
            .Where(i => !i.FilePath.Contains(Utils.ConvertPathToOS("public/images/icons/")))
            .GroupBy(t => t.FileName)
            .Where(grp => grp.Count() > 1)
            .Select(grp => new { Key = grp.Key, Count = grp.Count(), Images = grp.ToList() })
            .Where(g => g.Images.Any(i => i.Md5Hash != g.Images[0].Md5Hash))
            .ToList();

        Assert.AreEqual(0, duplicatesByNameWithDifMD5.Count, "Dublicates by Name but different by MD5 hashs:\r\n" +
            string.Join("\r\n", duplicatesByNameWithDifMD5.Select(d => $"\r\nFileName='{d.Key}':\r\n" +
            $"{string.Join("\r\n", d.Images.Select(p => $"MD5='{p.Md5Hash}' Path='{p.FilePath}'"))}")));
    }

    [Test]
    [Category("Images")]
    public void UselessImagesTest()
    {
        var usedImages = SourceImageFiles
             .SelectMany(item => item.Images.Select(p => p.FileName))
             .Distinct()
             .ToList();

        var existingImages = ImageFiles
            .Select(j => j.FileName)
            .Distinct()
            .ToList();

        var notFoundInSourceImages = existingImages.Except(usedImages).ToList();

        var notFoundPaths = ImageFiles
            .Where(i => notFoundInSourceImages.Contains(i.FileName))
            .Select(i => i.FilePath)
            .ToList();

        Assert.AreEqual(0, notFoundPaths.Count,
            "Some images are not used in source files by name:\r\n{0}",
            string.Join("\r\n", notFoundPaths));
    }

    private static string Capitalize(string str)
    {
        if (str.Length == 0)
            return str;

        if (str.Length == 1)
            return char.ToUpper(str[0]) + "";

        return char.ToUpper(str[0]) + str.Substring(1);
    }

    private static string GetVariableByName(string name)
    {
        var split = name.Split(new char[] { '.', '_', '-' });

        return (split.Length == 1
                ? Capitalize(split[0])
                : string.Join("", split.Select(s => Capitalize(s)))
            ) + "Url";
    }

    [Test]
    [Ignore("Ignore a fixture")]
    public void FixStaticTest()
    {
        var onlyJsFiles = SourceImageFiles
            .Where(f => f.ModuleType == ModuleTypes.CLIENT && f.Path.EndsWith(".js"))
            .ToList();


        foreach (var f in onlyJsFiles)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var i in f.Images.Where(i => i.FilePath.StartsWith("/static/images")))
            {
                dictionary.TryAdd(i.FilePath, GetVariableByName(i.FileName));
            }

            if (!dictionary.Any())
                continue;

            var content = File.ReadAllText(f.Path);

            var sb = new StringBuilder();

            foreach (var item in dictionary)
            {
                content = content.Replace($"=\"{item.Key}\"", "={" + item.Value + "}");
                content = content.Replace($"\"{item.Key}\"", item.Value);

                var query = item.Key.EndsWith(".svg") ? "?url" : "";

                sb.AppendLine($"import {item.Value} from \"{item.Key.Replace("/static/images", "PUBLIC_DIR/images")}{query}\";");
            }

            content = sb.ToString() + content;

            File.WriteAllText(f.Path, content, Encoding.UTF8);
        }

        foreach (var f in onlyJsFiles)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var i in f.Images.Where(i => i.FilePath.StartsWith("static/images")))
            {
                dictionary.TryAdd(i.FilePath, GetVariableByName(i.FileName));
            }

            if (!dictionary.Any())
                continue;

            var content = File.ReadAllText(f.Path);

            var sb = new StringBuilder();

            foreach (var item in dictionary)
            {
                content = content.Replace($"=\"{item.Key}\"", "={" + item.Value + "}");
                content = content.Replace($"\"{item.Key}\"", item.Value);

                var query = item.Key.EndsWith(".svg") ? "?url" : "";

                sb.AppendLine($"import {item.Value} from \"{item.Key.Replace("static/images", "PUBLIC_DIR/images")}{query}\";");
            }

            content = sb.ToString() + content;

            File.WriteAllText(f.Path, content, Encoding.UTF8);
        }

        foreach (var f in onlyJsFiles)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var i in f.Images.Where(i => i.FilePath.StartsWith("images/")))
            {
                dictionary.TryAdd(i.FilePath, GetVariableByName(i.FileName));
            }

            if (!dictionary.Any())
                continue;

            var content = File.ReadAllText(f.Path);

            var sb = new StringBuilder();

            foreach (var item in dictionary)
            {
                content = content.Replace($"=\"{item.Key}\"", "={" + item.Value + "}");
                content = content.Replace($"\"{item.Key}\"", item.Value);

                var query = item.Key.EndsWith(".svg") ? "?url" : "";

                sb.AppendLine($"import {item.Value} from \"{item.Key.Replace("images", "ASSETS_DIR/images")}{query}\";");
            }

            content = sb.ToString() + content;

            File.WriteAllText(f.Path, content, Encoding.UTF8);
        }

        Assert.AreEqual(0, 0);
    }
}