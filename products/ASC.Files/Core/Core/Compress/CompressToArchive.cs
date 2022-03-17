﻿/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace ASC.Web.Files.Core.Compress;

/// <summary>
/// Archives the data stream in the format selected in the settings
/// </summary>
[Scope]
public class CompressToArchive : ICompress
{
    private readonly ICompress _compress;

    internal static readonly string TarExt = ".tar.gz";
    internal static readonly string ZipExt = ".zip";
    private static readonly List<string> _exts = new List<string>(2) { TarExt, ZipExt };

    public CompressToArchive(FilesSettingsHelper filesSettings, CompressToTarGz compressToTarGz, CompressToZip compressToZip)
    {
        _compress = filesSettings.DownloadTarGz
            ? compressToTarGz
            : compressToZip;
    }

    public static string GetExt(IServiceProvider serviceProvider, string ext)
    {
        if (_exts.Contains(ext))
        {
            return ext;
        }

        using var zip = serviceProvider.GetService<CompressToArchive>();

        return zip.ArchiveExtension;
    }

    public void SetStream(Stream stream)
    {
        _compress.SetStream(stream);
    }

    /// <summary>
    /// The record name is created (the name of a separate file in the archive)
    /// </summary>
    /// <param name="title">File name with extension, this name will have the file in the archive</param>
    public void CreateEntry(string title)
    {
        _compress.CreateEntry(title);
    }

    /// <summary>
    /// Transfer the file itself to the archive
    /// </summary>
    /// <param name="readStream">File data</param>
    public void PutStream(Stream readStream)
    {
        _compress.PutStream(readStream);
    }

    /// <summary>
    /// Put an entry on the output stream.
    /// </summary>
    public void PutNextEntry()
    {
        _compress.PutNextEntry();
    }

    /// <summary>
    /// Closes the current entry.
    /// </summary>
    public void CloseEntry()
    {
        _compress.CloseEntry();
    }

    /// <summary>
    /// Resource title (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string Title => _compress.Title;

    /// <summary>
    /// Extension the archive (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string ArchiveExtension => _compress.ArchiveExtension;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _compress.Dispose();
    }
}
