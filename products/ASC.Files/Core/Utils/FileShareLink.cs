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
