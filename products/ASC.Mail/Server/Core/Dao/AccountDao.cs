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
using System.Linq;
using System.Collections.Generic;
using ASC.Api.Core;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ASC.Mail.Models;

namespace ASC.Mail.Core.Dao
{
    public class AccountDao : IAccountDao
    {
        public int Tenant
        {
            get
            {
                return ApiContext.Tenant.TenantId;
            }
        }

        public string UserId
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }

        public SecurityContext SecurityContext { get; }

        public ApiContext ApiContext { get; }

        public MailDbContext MailDb { get; }

        public AccountDao(DbContextManager<MailDbContext> dbContext,
            ApiContext apiContext,
            SecurityContext securityContext)
        {
            ApiContext = apiContext;
            SecurityContext = securityContext;

            MailDb = dbContext.Get("mail");
        }

        public List<Account> GetAccounts()
        {
            var accounts = (from mb in MailDb.MailMailbox
                            join signature in MailDb.MailMailboxSignature on mb.Id equals signature.IdMailbox into Signature
                            from sig in Signature.DefaultIfEmpty()
                            join autoreply in MailDb.MailMailboxAutoreply on mb.Id equals autoreply.IdMailbox into Autoreply
                            from reply in Autoreply.DefaultIfEmpty()
                            join address in MailDb.MailServerAddress on mb.Id equals address.IdMailbox into Address
                            from sa in Address.DefaultIfEmpty()
                            join domain in MailDb.MailServerDomain on sa.IdDomain equals domain.Id into Domain
                            from sd in Domain.DefaultIfEmpty()
                            join groupXaddress in MailDb.MailServerMailGroupXMailServerAddress on sa.Id equals groupXaddress.IdAddress into GroupXaddress
                            from sgxa in GroupXaddress.DefaultIfEmpty()
                            join servergroup in MailDb.MailServerMailGroup on sgxa.IdMailGroup equals servergroup.Id into ServerGroup
                            from sg in ServerGroup.DefaultIfEmpty()
                            where mb.Tenant == Tenant && mb.IsRemoved == false && mb.IdUser == UserId
                            orderby sa.IsAlias
                            select new Account
                            {
                                MailboxId = mb.Id,
                                MailboxAddress = mb.Address,
                                MailboxEnabled = mb.Enabled,
                                MailboxAddressName = mb.Name,
                                MailboxQuotaError = mb.QuotaError,
                                MailboxDateAuthError = mb.DateAuthError,
                                MailboxOAuthToken = mb.Token,
                                MailboxIsServerMailbox = mb.IsServerMailbox,
                                MailboxEmailInFolder = mb.EmailInFolder,
                                ServerAddressId = sa.Id,
                                ServerAddressName = sa.Name,
                                ServerAddressIsAlias = sa.IsAlias,
                                ServerDomainId = sd.Id,
                                ServerDomainName = sd.Name,
                                ServerDomainTenant = sd.Tenant,
                                ServerMailGroupId = sg.Id,
                                ServerMailGroupAddress = sg.Address,
                                MailboxSignature = sig != null
                                 ? new MailSignatureData(mb.Id, mb.Tenant, sig.Html, sig.IsActive)
                                 : new MailSignatureData(mb.Id, mb.Tenant, string.Empty, false),
                                MailboxAutoreply = reply != null
                                 ? new MailAutoreplyData(mb.Id, mb.Tenant, reply.TurnOn, reply.OnlyContacts,
                                     reply.TurnOnToDate, reply.FromDate, reply.ToDate, reply.Subject, reply.Html)
                                 : new MailAutoreplyData(mb.Id, mb.Tenant, false, false,
                                     false, DateTime.MinValue, DateTime.MinValue, string.Empty, string.Empty)
                            })
                           .ToList();

            return accounts;

            //return Db.ExecuteList(query)
            //    .ConvertAll(ToAccount);
        }
    }

    public static class AccountDaoExtension
    {
        public static IServiceCollection AddAccountDaoService(this IServiceCollection services)
        {
            services.TryAddScoped<AccountDao>();

            return services;
        }
    }
}