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

namespace ASC.Web.Files.Classes
{
    [Scope]
    public class PathProvider
    {
        public static readonly string ProjectVirtualPath = "~/Products/Projects/TMDocs.aspx";

        public static readonly string TemplatePath = "/Products/Files/Templates/";

        public static readonly string StartURL = FilesLinkUtility.FilesBaseVirtualPath;

        public readonly string GetFileServicePath;

        private readonly WebImageSupplier _webImageSupplier;
        private readonly IDaoFactory _daoFactory;
        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly FilesLinkUtility _filesLinkUtility;
        private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
        private readonly GlobalStore _globalStore;
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;

        public PathProvider(
            WebImageSupplier webImageSupplier,
            IDaoFactory daoFactory,
            CommonLinkUtility commonLinkUtility,
            FilesLinkUtility filesLinkUtility,
            EmailValidationKeyProvider emailValidationKeyProvider,
            GlobalStore globalStore,
            BaseCommonLinkUtility baseCommonLinkUtility)
        {
            _webImageSupplier = webImageSupplier;
            _daoFactory = daoFactory;
            _commonLinkUtility = commonLinkUtility;
            _filesLinkUtility = filesLinkUtility;
            _emailValidationKeyProvider = emailValidationKeyProvider;
            _globalStore = globalStore;
            _baseCommonLinkUtility = baseCommonLinkUtility;
            GetFileServicePath = _baseCommonLinkUtility.ToAbsolute("~/Products/Files/Services/WCFService/service.svc/");
        }

        public string GetImagePath(string imgFileName)
        {
            return _webImageSupplier.GetAbsoluteWebPath(imgFileName, ProductEntryPoint.ID);
        }

        public string GetFileStaticRelativePath(string fileName)
        {
            var ext = FileUtility.GetFileExtension(fileName);

            return ext switch
            {
                //Attention: Only for ResourceBundleControl
                ".js" => VirtualPathUtility.ToAbsolute("~/Products/Files/js/" + fileName),
                ".ascx" => _baseCommonLinkUtility.ToAbsolute("~/Products/Files/Controls/" + fileName),
                //Attention: Only for ResourceBundleControl
                ".css" => VirtualPathUtility.ToAbsolute("~/Products/Files/App_Themes/default/" + fileName),
                _ => fileName,
            };
        }

        public string GetFileControlPath(string fileName)
        {
            return _baseCommonLinkUtility.ToAbsolute("~/Products/Files/Controls/" + fileName);
        }

        public async Task<string> GetFolderUrlAsync<T>(Folder<T> folder, int projectID = 0)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder), FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            var folderDao = _daoFactory.GetFolderDao<T>();

            switch (folder.RootFolderType)
            {
                case FolderType.BUNCH:
                    if (projectID == 0)
                    {
                        var path = await folderDao.GetBunchObjectIDAsync(folder.RootFolderId);

                        var projectIDFromDao = path.Split('/').Last();

                        if (string.IsNullOrEmpty(projectIDFromDao)) return string.Empty;

                        projectID = Convert.ToInt32(projectIDFromDao);
                    }

                    return _commonLinkUtility.GetFullAbsolutePath(string.Format("{0}?prjid={1}#{2}", ProjectVirtualPath, projectID, folder.ID));
                default:
                    return _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FilesBaseAbsolutePath + "#" + HttpUtility.UrlPathEncode(folder.ID.ToString()));
            }
        }

        public async Task<string> GetFolderUrlByIdAsync<T>(T folderId)
        {
            var folder = await _daoFactory.GetFolderDao<T>().GetFolderAsync(folderId);

            return await GetFolderUrlAsync(folder);
        }

        public string GetFileStreamUrl<T>(File<T> file, string doc = null, bool lastVersion = false)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            }

            //NOTE: Always build path to handler!
            var uriBuilder = new UriBuilder(_commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath));
            var query = uriBuilder.Query;
            query += FilesLinkUtility.Action + "=stream&";
            query += FilesLinkUtility.FileId + "=" + HttpUtility.UrlEncode(file.ID.ToString()) + "&";
            var version = 0;
            if (!lastVersion)
            {
                version = file.Version;
                query += FilesLinkUtility.Version + "=" + file.Version + "&";
            }

            query += FilesLinkUtility.AuthKey + "=" + _emailValidationKeyProvider.GetEmailKey(file.ID.ToString() + version);
            if (!string.IsNullOrEmpty(doc))
            {
                query += "&" + FilesLinkUtility.DocShareKey + "=" + HttpUtility.UrlEncode(doc);
            }

            return uriBuilder.Uri + "?" + query;
        }

        public string GetFileChangesUrl<T>(File<T> file, string doc = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            }

            var uriBuilder = new UriBuilder(_commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath));
            var query = uriBuilder.Query;
            query += $"{FilesLinkUtility.Action}=diff&";
            query += $"{FilesLinkUtility.FileId}={HttpUtility.UrlEncode(file.ID.ToString())}&";
            query += $"{FilesLinkUtility.Version}={file.Version}&";
            query += $"{FilesLinkUtility.AuthKey}={_emailValidationKeyProvider.GetEmailKey(file.ID + file.Version.ToString(CultureInfo.InvariantCulture))}";
            if (!string.IsNullOrEmpty(doc))
            {
                query += $"&{FilesLinkUtility.DocShareKey}={HttpUtility.UrlEncode(doc)}";
            }

            return $"{uriBuilder.Uri}?{query}";
        }

        public async Task<string> GetTempUrlAsync(Stream stream, string ext)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var store = _globalStore.GetStore();
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), ext);
            var path = CrossPlatform.PathCombine("temp_stream", fileName);

            if (await store.IsFileAsync(FileConstant.StorageDomainTmp, path))
            {
                await store.DeleteAsync(FileConstant.StorageDomainTmp, path);
            }

            await store.SaveAsync(
                FileConstant.StorageDomainTmp,
                path,
                stream,
                MimeMapping.GetMimeMapping(ext),
                "attachment; filename=\"" + fileName + "\"");

            var uriBuilder = new UriBuilder(_commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath));
            var query = uriBuilder.Query;
            query += $"{FilesLinkUtility.Action}=tmp&";
            query += $"{FilesLinkUtility.FileTitle}={HttpUtility.UrlEncode(fileName)}&";
            query += $"{FilesLinkUtility.AuthKey}={_emailValidationKeyProvider.GetEmailKey(fileName)}";

            return $"{uriBuilder.Uri}?{query}";
        }

        public string GetEmptyFileUrl(string extension)
        {
            var uriBuilder = new UriBuilder(_commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath));
            var query = uriBuilder.Query;
            query += $"{FilesLinkUtility.Action}=empty&";
            query += $"{FilesLinkUtility.FileTitle}={HttpUtility.UrlEncode(extension)}";

            return $"{uriBuilder.Uri}?{query}";
        }
    }
}