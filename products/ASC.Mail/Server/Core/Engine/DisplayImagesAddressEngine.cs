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


using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ASC.Mail.Core.Engine
{
    public class DisplayImagesAddressEngine
    {
        public DbContextManager<MailDbContext> DbContext { get; }

        public int Tenant
        {
            get
            {
                return TenantManager.GetCurrentTenant().TenantId;
            }
        }

        public string UserId
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }

        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public ILog Log { get; }

        public DaoFactory DaoFactory { get; }

        public MailDbContext MailDb { get; }

        public DisplayImagesAddressEngine(
            TenantManager tenantManager,
            SecurityContext securityContext,
            DaoFactory daoFactory,
            IOptionsMonitor<ILog> option)
        {
            TenantManager = tenantManager;
            SecurityContext = securityContext;

            DaoFactory = daoFactory;
            MailDb = DaoFactory.MailDb;

            Log = option.Get("ASC.Mail.DisplayImagesAddressEngine");
        }

        public IEnumerable<string> Get()
        {
            return DaoFactory.DisplayImagesAddressDao.GetDisplayImagesAddresses();
        }

        public void Add(string address)
        {
            DaoFactory.DisplayImagesAddressDao.AddDisplayImagesAddress(address);

        }

        public void Remove(string address)
        {
            DaoFactory.DisplayImagesAddressDao.RemovevDisplayImagesAddress(address);
        }
    }

    public static class DisplayImagesAddressEngineExtension
    {
        public static DIHelper AddDisplayImagesAddressEngineService(this DIHelper services)
        {
            services.TryAddScoped<DisplayImagesAddressEngine>();

            services.AddTenantManagerService()
                .AddSecurityContextService()
                .AddDaoFactoryService();

            return services;
        }
    }
}
