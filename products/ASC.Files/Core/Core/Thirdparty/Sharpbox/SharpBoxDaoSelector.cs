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


using System;

using ASC.Common;
using ASC.Files.Core;
using ASC.Files.Core.Security;

namespace ASC.Files.Thirdparty.Sharpbox
{
    [Scope(Additional = typeof(SharpBoxDaoSelectorExtension))]
    internal class SharpBoxDaoSelector : RegexDaoSelectorBase<SharpBoxProviderInfo>, IDaoSelector
    {
        protected internal override string Name { get => "SharpBox"; }
        protected internal override string Id { get => "sbox"; }

        public SharpBoxDaoSelector(IServiceProvider serviceProvider, IDaoFactory daoFactory)
            : base(serviceProvider, daoFactory)
        {
        }

        public IFileDao<string> GetFileDao(string id)
        {
            return base.GetFileDao<SharpBoxFileDao>(id);
        }

        public IFolderDao<string> GetFolderDao(string id)
        {
            return base.GetFolderDao<SharpBoxFolderDao>(id);
        }

        public ITagDao<string> GetTagDao(string id)
        {
            return base.GetTagDao<SharpBoxTagDao>(id);
        }

        public ISecurityDao<string> GetSecurityDao(string id)
        {
            return base.GetSecurityDao<SharpBoxSecurityDao>(id);
        }
    }

    public static class SharpBoxDaoSelectorExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<SharpBoxFileDao>();
            services.TryAdd<SharpBoxFolderDao>();
            services.TryAdd<SharpBoxTagDao>();
            services.TryAdd<SharpBoxSecurityDao>();
        }
    }
}