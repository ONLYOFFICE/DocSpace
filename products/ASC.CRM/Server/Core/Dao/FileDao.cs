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
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.CRM.Core.EF;
using ASC.Files.Core;
using ASC.Web.Files.Api;

using AutoMapper;

using Microsoft.Extensions.Options;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class FileDao : AbstractDao
    {
        private FilesIntegration _filesIntegration;
        public FileDao(FilesIntegration filesIntegration,
                       DbContextManager<CrmDbContext> dbContextManager,
                       DbContextManager<TenantDbContext> dbContextManager1,
                       TenantManager tenantManager,
                       SecurityContext securityContext,
                       IOptionsMonitor<ILog> logger,
                       ICache ascCache,
                       IMapper mapper) :
            base(dbContextManager,
                dbContextManager1,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _filesIntegration = filesIntegration;
        }


        public File<int> GetFile(int id, int version)
        {
            var dao = _filesIntegration.DaoFactory.GetFileDao<int>();

            var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);

            return file;
        }

        public void DeleteFile(int id)
        {
            var dao = _filesIntegration.DaoFactory.GetFileDao<int>();

            dao.DeleteFile(id);
        }

        public int GetRoot()
        {
            return _filesIntegration.RegisterBunch<int>("crm", "crm_common", "");
        }

        public int GetMy()
        {
            return _filesIntegration.RegisterBunch<int>("files", "my", _securityContext.CurrentAccount.ID.ToString());
        }

        public File<int> SaveFile(File<int> file, System.IO.Stream stream)
        {
            var dao = _filesIntegration.DaoFactory.GetFileDao<int>();

            return dao.SaveFile(file, stream);
        }

        public List<int> GetEventsByFile(int id)
        {
            var tagdao = _filesIntegration.DaoFactory.GetTagDao<int>();

            var tags = tagdao.GetTags(id, FileEntryType.File, TagType.System).ToList().FindAll(tag => tag.TagName.StartsWith("RelationshipEvent_"));

            return tags.Select(item => Convert.ToInt32(item.TagName.Split(new[] { '_' })[1])).ToList();
        }

    }

}