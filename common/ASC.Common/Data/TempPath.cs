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

namespace System.IO;

[Singletone]
public class TempPath
{
    private readonly string _tempFolder;

    public TempPath(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        var rootFolder = AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(rootFolder))
        {
            rootFolder = Assembly.GetEntryAssembly().Location;
        }

        _tempFolder = configuration["web:temp"] ?? CrossPlatform.PathCombine(hostEnvironment.ContentRootPath, "temp");
        if (!Path.IsPathRooted(_tempFolder))
        {
            _tempFolder = Path.GetFullPath(Path.Combine(rootFolder, _tempFolder));
        }

        if (!Directory.Exists(_tempFolder))
        {
            Directory.CreateDirectory(_tempFolder);
        }
    }

    public string GetTempPath()
    {
        return _tempFolder;
    }

    public string GetTempFileName(string ext = "")
    {
        FileStream f = null;
        string path;
        var count = 0;

        do
        {
            path = Path.Combine(_tempFolder, Path.GetRandomFileName());

            if (!string.IsNullOrEmpty(ext))
            {
                path = Path.ChangeExtension(path, ext);
            }

            try
            {
                using (f = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    return path;
                }
            }
            catch (IOException ex)
            {
                if (ex.HResult != -2147024816 || count++ > 65536)
                {
                    throw;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                if (count++ > 65536)
                {
                    throw new IOException(ex.Message, ex);
                }
            }
        } while (f == null);

        return path;
    }
}