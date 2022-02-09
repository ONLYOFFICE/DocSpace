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

namespace ASC.Files.Thirdparty.SharePoint
{
    [Scope(Additional = typeof(SharePointDaoSelectorExtension))]
    internal class SharePointDaoSelector : RegexDaoSelectorBase<SharePointProviderInfo>, IDaoSelector
    {
        protected internal override string Name { get => "sharepoint"; }
        protected internal override string Id { get => "spoint"; }
        public SharePointDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
            : base(serviceProvider, daoFactory)
        {
        }

        public IFileDao<string> GetFileDao(string id)
        {
            return base.GetFileDao<SharePointFileDao>(id);
        }

        public IFolderDao<string> GetFolderDao(string id)
        {
            return base.GetFolderDao<SharePointFolderDao>(id);
        }

        public ITagDao<string> GetTagDao(string id)
        {
            return base.GetTagDao<SharePointTagDao>(id);
        }

        public ISecurityDao<string> GetSecurityDao(string id)
        {
            return base.GetSecurityDao<SharePointSecurityDao>(id);
        }

        public override string ConvertId(string id)
        {
            if (id != null)
            {
                var match = Selector.Match(id);
                if (match.Success)
                {
                    return GetInfo(id).ProviderInfo.SpRootFolderId + match.Groups["path"].Value.Replace('|', '/');
                }
                throw new ArgumentException("Id is not a sharepoint id");
            }
            return base.ConvertId(null);
        }
    }

    public class SharePointDaoSelectorExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<SharePointFileDao>();
            services.TryAdd<SharePointFolderDao>();
            services.TryAdd<SharePointTagDao>();
            services.TryAdd<SharePointSecurityDao>();
        }
    }
}