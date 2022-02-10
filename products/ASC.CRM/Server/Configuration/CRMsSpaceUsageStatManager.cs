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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Resources;
using ASC.Files.Core.EF;
using ASC.Web.Core;

using Microsoft.EntityFrameworkCore;

namespace ASC.Web.CRM.Configuration
{

    [Scope]
    public class CrmSpaceUsageStatManager : SpaceUsageStatManager
    {
        private int _tenantId;
        private Lazy<FilesDbContext> LazyFilesDbContext { get; }
        private FilesDbContext _filesDbContext { get => LazyFilesDbContext.Value; }

        private PathProvider _pathProvider;

        public CrmSpaceUsageStatManager(DbContextManager<FilesDbContext> filesDbContext,
                                        PathProvider pathProvider,
                                        TenantManager tenantManager)
        {
            _pathProvider = pathProvider;
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => filesDbContext.Value);
            _tenantId = tenantManager.GetCurrentTenant().TenantId;
        }


        public override async Task<List<UsageSpaceStatItem>> GetStatDataAsync()
        {
            var spaceUsage = await _filesDbContext.Files.AsQueryable().Join(_filesDbContext.Tree,
                                     x => x.FolderId,
                                     y => y.FolderId,
                                     (x, y) => new { x, y }
                                   )
                              .Join(_filesDbContext.BunchObjects,
                                     x => x.y.ParentId,
                                     y => Convert.ToInt32(y.LeftNode),
                                     (x, y) => new { x, y })
                              .Where(x => x.y.TenantId == _tenantId &&
                                          Microsoft.EntityFrameworkCore.EF.Functions.Like(x.y.RightNode, "crm/crm_common/%"))
                              .SumAsync(x => x.x.x.ContentLength);

            return new List<UsageSpaceStatItem>
            {new UsageSpaceStatItem
                {

                    Name = CRMCommonResource.WholeCRMModule,
                    SpaceUsage = spaceUsage,
                    Url = VirtualPathUtility.ToAbsolute(_pathProvider.StartURL())
                }

            };
        }
    }
}
