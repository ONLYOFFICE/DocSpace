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


using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Mail.Core.Dao
{
    public class DisplayImagesAddressDao : BaseDao, IDisplayImagesAddressDao
    {
        public DisplayImagesAddressDao(
            DbContextManager<MailDbContext> dbContext,
            ApiContext apiContext,
            SecurityContext securityContext)
            : base(apiContext, securityContext, dbContext)
        {
        }

        public List<string> GetDisplayImagesAddresses()
        {
            var query = MailDb.MailDisplayImages
                .Where(r => r.Tenant == Tenant)
                .Where(r => r.IdUser == UserId)
                .Select(r => r.Address);

            List<string> addresses = query.ToList();

            return addresses;
        }

        public void AddDisplayImagesAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException(@"Invalid address. Address can't be empty.", "address");

            using var tr = MailDb.Database.BeginTransaction();

            var dbAddress = new MailDisplayImages
            {
                Tenant = Tenant,
                IdUser = UserId,
                Address = address
            };

            MailDb.MailDisplayImages.Add(dbAddress);

            MailDb.SaveChanges();

            tr.Commit();
        }

        public void RemovevDisplayImagesAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException(@"Invalid address. Address can't be empty.", "address");

            using var tr = MailDb.Database.BeginTransaction();

            var range = MailDb.MailDisplayImages
                .Where(r => r.IdUser == UserId && r.Tenant == Tenant && r.Address == address);

            MailDb.MailDisplayImages.RemoveRange(range);

            var count = MailDb.SaveChanges();

            tr.Commit();
        }
    }

    public static class DisplayImagesAddressDaoExtension
    {
        public static DIHelper AddDisplayImagesAddressDaoService(this DIHelper services)
        {
            services.TryAddScoped<DisplayImagesAddressDao>();

            return services;
        }
    }
}