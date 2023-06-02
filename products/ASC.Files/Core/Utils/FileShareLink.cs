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

namespace ASC.Web.Files.Utils;

[Scope]
public class FileShareLink
{
    private readonly FileUtility _fileUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly Global _global;
    private readonly FileSecurity _fileSecurity;
    private readonly FilesSettingsHelper _filesSettingsHelper;

    public FileShareLink(
        FileUtility fileUtility,
        FilesLinkUtility filesLinkUtility,
        BaseCommonLinkUtility baseCommonLinkUtility,
        Global global,
        FileSecurity fileSecurity,
        FilesSettingsHelper filesSettingsHelper)
    {
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _global = global;
        _fileSecurity = fileSecurity;
        _filesSettingsHelper = filesSettingsHelper;
    }

    public async Task<string> GetLinkAsync<T>(File<T> file, bool withHash = true)
    {
        var url = file.DownloadUrl;

        if (_fileUtility.CanWebView(file.Title))
        {
            url = _filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.Id);
        }

        if (withHash)
        {
            var linkParams = await CreateKeyAsync(file.Id);
            url += "&" + FilesLinkUtility.DocShareKey + "=" + HttpUtility.UrlEncode(linkParams);
        }

        return _baseCommonLinkUtility.GetFullAbsolutePath(url);
    }

    public async Task<string> CreateKeyAsync<T>(T fileId)
    {
        return Signature.Create(fileId, await _global.GetDocDbKeyAsync());
    }

    public async Task<string> ParseAsync(string doc)
    {
        return Signature.Read<string>(doc ?? string.Empty, await _global.GetDocDbKeyAsync());
    }
    public async Task<T> ParseAsync<T>(string doc)
    {
        return Signature.Read<T>(doc ?? string.Empty, await _global.GetDocDbKeyAsync());
    }

    public async Task<(bool EditLink, File<T> File, FileShare fileShare)> CheckAsync<T>(string doc, bool checkRead, IFileDao<T> fileDao)
    {
        var check = await CheckAsync(doc, fileDao);
        var fileShare = check.FileShare;

        return ((!checkRead
                && (fileShare == FileShare.ReadWrite
                    || fileShare == FileShare.CustomFilter
                    || fileShare == FileShare.Review
                    || fileShare == FileShare.FillForms
                    || fileShare == FileShare.Comment))
            || (checkRead && fileShare != FileShare.Restrict), check.File, fileShare);
    }

    public async Task<(FileShare FileShare, File<T> File)> CheckAsync<T>(string doc, IFileDao<T> fileDao)
    {
        if (!_filesSettingsHelper.ExternalShare)
        {
            return (FileShare.Restrict, null);
        }

        if (string.IsNullOrEmpty(doc))
        {
            return (FileShare.Restrict, null);
        }

        var fileId = await ParseAsync<T>(doc);

        File<T> file = null;

        if (!EqualityComparer<T>.Default.Equals(fileId, default(T)))
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        if (file == null)
        {
            return (FileShare.Restrict, file);
        }

        if (await _fileSecurity.CanEditAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.ReadWrite, file);
        }

        if (await _fileSecurity.CanCustomFilterEditAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.CustomFilter, file);
        }

        if (await _fileSecurity.CanReviewAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Review, file);
        }

        if (await _fileSecurity.CanFillFormsAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.FillForms, file);
        }

        if (await _fileSecurity.CanCommentAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Comment, file);
        }

        if (await _fileSecurity.CanReadAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Read, file);
        }

        return (FileShare.Restrict, file);
    }
}
