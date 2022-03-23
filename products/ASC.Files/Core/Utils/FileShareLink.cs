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

    public FileShareLink(
        FileUtility fileUtility,
        FilesLinkUtility filesLinkUtility,
        BaseCommonLinkUtility baseCommonLinkUtility,
        Global global,
        FileSecurity fileSecurity)
    {
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _global = global;
        _fileSecurity = fileSecurity;
    }

    public string GetLink<T>(File<T> file, bool withHash = true)
    {
        var url = file.DownloadUrl;

        if (_fileUtility.CanWebView(file.Title))
        {
            url = _filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.ID);
        }

        if (withHash)
        {
            var linkParams = CreateKey(file.ID);
            url += "&" + FilesLinkUtility.DocShareKey + "=" + HttpUtility.UrlEncode(linkParams);
        }

        return _baseCommonLinkUtility.GetFullAbsolutePath(url);
    }

    public string CreateKey<T>(T fileId)
    {
        return Signature.Create(fileId, _global.GetDocDbKey());
    }

    public string Parse(string doc)
    {
        return Signature.Read<string>(doc ?? string.Empty, _global.GetDocDbKey());
    }
    public T Parse<T>(string doc)
    {
        return Signature.Read<T>(doc ?? string.Empty, _global.GetDocDbKey());
    }

    public async Task<(bool EditLink, File<T> File)> CheckAsync<T>(string doc, bool checkRead, IFileDao<T> fileDao)
    {
        var check = await CheckAsync(doc, fileDao);
        var fileShare = check.FileShare;

        return ((!checkRead
                && (fileShare == FileShare.ReadWrite
                    || fileShare == FileShare.CustomFilter
                    || fileShare == FileShare.Review
                    || fileShare == FileShare.FillForms
                    || fileShare == FileShare.Comment))
            || (checkRead && fileShare != FileShare.Restrict), check.File);
    }

    public async Task<(FileShare FileShare, File<T> File)> CheckAsync<T>(string doc, IFileDao<T> fileDao)
    {
        if (string.IsNullOrEmpty(doc))
        {
            return (FileShare.Restrict, null);
        }

        var fileId = Parse<T>(doc);
        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            return (FileShare.Restrict, file);
        }

        var filesSecurity = _fileSecurity;
        if (await filesSecurity.CanEditAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.ReadWrite, file);
        }

        if (await filesSecurity.CanCustomFilterEditAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.CustomFilter, file);
        }

        if (await filesSecurity.CanReviewAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Review, file);
        }

        if (await filesSecurity.CanFillFormsAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.FillForms, file);
        }

        if (await filesSecurity.CanCommentAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Comment, file);
        }

        if (await filesSecurity.CanReadAsync(file, FileConstant.ShareLinkId))
        {
            return (FileShare.Read, file);
        }

        return (FileShare.Restrict, file);
    }
}
