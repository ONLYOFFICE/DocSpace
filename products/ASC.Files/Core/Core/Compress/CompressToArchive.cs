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

namespace ASC.Web.Files.Core.Compress;

/// <summary>
/// Archives the data stream in the format selected in the settings
/// </summary>
[Scope]
public class CompressToArchive : ICompress
{
    private readonly FilesSettingsHelper _settings;
    private readonly CompressToTarGz _compressToTarGz;
    private readonly CompressToZip _compressToZip;

    internal static readonly string TarExt = ".tar.gz";
    internal static readonly string ZipExt = ".zip";
    private static readonly List<string> _exts = new List<string>(2) { TarExt, ZipExt };

    private ICompress _compress;
    private ICompress Compress
    {
        get
        {
            _compress ??= _settings.DownloadTarGz
                    ? _compressToTarGz
                    : _compressToZip;

            return _compress;
        }
    }

    public CompressToArchive(FilesSettingsHelper filesSettings, CompressToTarGz compressToTarGz, CompressToZip compressToZip)
    {
        _settings = filesSettings;
        _compressToTarGz = compressToTarGz;
        _compressToZip = compressToZip;
    }

    public string GetExt(string ext)
    {
        if (_exts.Contains(ext))
        {
            return ext;
        }

        return ArchiveExtension;
    }

    public void SetStream(Stream stream)
    {
        Compress.SetStream(stream);
    }

    /// <summary>
    /// The record name is created (the name of a separate file in the archive)
    /// </summary>
    /// <param name="title">File name with extension, this name will have the file in the archive</param>
    /// <param name="lastModification"></param>
    public void CreateEntry(string title, DateTime? lastModification = null)
    {
        Compress.CreateEntry(title, lastModification);
    }

    /// <summary>
    /// Transfer the file itself to the archive
    /// </summary>
    /// <param name="readStream">File data</param>
    public async Task PutStream(Stream readStream) => await Compress.PutStream(readStream);

    /// <summary>
    /// Put an entry on the output stream.
    /// </summary>
    public void PutNextEntry()
    {
        Compress.PutNextEntry();
    }

    /// <summary>
    /// Closes the current entry.
    /// </summary>
    public void CloseEntry()
    {
        Compress.CloseEntry();
    }

    /// <summary>
    /// Resource title (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string Title => Compress.Title;

    /// <summary>
    /// Extension the archive (does not affect the work of the class)
    /// </summary>
    /// <returns></returns>
    public string ArchiveExtension => Compress.ArchiveExtension;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        if (_compress != null)
        {
            _compress.Dispose();
        }
    }
}
