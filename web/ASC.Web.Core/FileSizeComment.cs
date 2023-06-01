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

namespace ASC.Web.Studio.Core;

[Scope]
public class FileSizeComment
{
    private readonly TenantExtra _tenantExtra;
    private readonly SetupInfo _setupInfo;

    public FileSizeComment(TenantExtra tenantExtra, SetupInfo setupInfo)
    {
        _tenantExtra = tenantExtra;
        _setupInfo = setupInfo;
    }

    public string FileImageSizeExceptionString
    {
        get { return GetFileSizeExceptionString(_setupInfo.MaxImageUploadSize); }
    }

    public static string GetFileSizeExceptionString(long size)
    {
        return $"{Resource.FileSizeMaxExceed} ({FilesSizeToString(size)}).";
    }

    public static string GetPersonalFreeSpaceExceptionString(long size)
    {
        return $"{Resource.PersonalFreeSpaceException} ({FilesSizeToString(size)}).";
    }

    /// <summary>
    /// The maximum file size is exceeded (1 MB).
    /// </summary>
    public Exception FileImageSizeException
    {
        get { return new TenantQuotaException(FileImageSizeExceptionString); }
    }

    public static Exception GetFileSizeException(long size)
    {
        return new TenantQuotaException(GetFileSizeExceptionString(size));
    }

    public static Exception GetPersonalFreeSpaceException(long size)
    {
        return new TenantQuotaException(GetPersonalFreeSpaceExceptionString(size));
    }

    /// <summary>
    /// The maximum file size is exceeded (25 MB).
    /// </summary>
    public async Task<Exception> GetFileSizeExceptionAsync()
    {
        return new TenantQuotaException(await GetFileSizeExceptionStringAsync());
    }

    public async Task<string> GetFileSizeExceptionStringAsync()
    {
        return GetFileSizeExceptionString(await _tenantExtra.GetMaxUploadSizeAsync());
    }

    /// <summary>
    /// Get note about maximum file size
    /// </summary>
    /// <returns>Note: the file size cannot exceed 25 MB</returns>
    public async Task<string> GetFileSizeNoteAsync()
    {
        return await GetFileSizeNoteAsync(true);
    }

    /// <summary>
    /// Get note about maximum file size
    /// </summary>
    /// <param name="withHtmlStrong">Highlight a word about size</param>
    /// <returns>Note: the file size cannot exceed 25 MB</returns>
    public async Task<string> GetFileSizeNoteAsync(bool withHtmlStrong)
    {
        return await GetFileSizeNoteAsync(Resource.FileSizeNote, withHtmlStrong);
    }

    /// <summary>
    /// Get note about maximum file size
    /// </summary>
    /// <param name="note">Resource fromat of note</param>
    /// <param name="withHtmlStrong">Highlight a word about size</param>
    /// <returns>Note: the file size cannot exceed 25 MB</returns>
    public async Task<string> GetFileSizeNoteAsync(string note, bool withHtmlStrong)
    {
        return
            string.Format(note,
                          FilesSizeToString(await _tenantExtra.GetMaxUploadSizeAsync()),
                          withHtmlStrong ? "<strong>" : string.Empty,
                          withHtmlStrong ? "</strong>" : string.Empty);
    }

    /// <summary>
    /// Get note about maximum file size of image
    /// </summary>
    /// <param name="note">Resource fromat of note</param>
    /// <param name="withHtmlStrong">Highlight a word about size</param>
    /// <returns>Note: the file size cannot exceed 1 MB</returns>
    public string GetFileImageSizeNote(string note, bool withHtmlStrong)
    {
        return
            string.Format(note,
                          FilesSizeToString(_setupInfo.MaxImageUploadSize),
                          withHtmlStrong ? "<strong>" : string.Empty,
                          withHtmlStrong ? "</strong>" : string.Empty);
    }

    /// <summary>
    /// Generates a string the file size
    /// </summary>
    /// <param name="size">Size in bytes</param>
    /// <returns>10 b, 100 Kb, 25 Mb, 1 Gb</returns>
    public static string FilesSizeToString(long size)
    {
        var sizeNames = !string.IsNullOrEmpty(Resource.FileSizePostfix) ? Resource.FileSizePostfix.Split(',') : new[] { "bytes", "KB", "MB", "GB", "TB" };
        var power = 0;

        double resultSize = size;
        if (1024 <= resultSize)
        {
            power = (int)Math.Log(resultSize, 1024);
            power = power < sizeNames.Length ? power : sizeNames.Length - 1;
            resultSize /= Math.Pow(1024d, power);
        }
        return string.Format("{0:#,0.##} {1}", resultSize, sizeNames[power]);
    }
}
