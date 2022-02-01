/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

using ASC.AuditTrail.Models;
using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

using AutoMapper;

namespace ASC.AuditTrail.Data.Repositories
{
    [Scope]
    public class LoginEventsRepository
    {
        private MessagesContext MessagesContext => _lazyMessagesContext.Value;

        private readonly Lazy<MessagesContext> _lazyMessagesContext;
        private readonly IMapper _mapper;

        public LoginEventsRepository(
            DbContextManager<MessagesContext> dbMessagesContext,
            IMapper mapper)
        {
            _lazyMessagesContext = new Lazy<MessagesContext>(() => dbMessagesContext.Value);
            _mapper = mapper;
        }

        public IEnumerable<LoginEvent> GetLast(int tenant, int chunk)
        {
            var query =
                (from b in MessagesContext.LoginEvents
                 from p in MessagesContext.Users.Where(p => b.UserId == p.Id).DefaultIfEmpty()
                 where b.TenantId == tenant
                 orderby b.Date descending
                 select new LoginEventQuery { LoginEvents = b, User = p })
                .Take(chunk);

            return query.AsEnumerable().Select(_mapper.Map<LoginEvent>).ToList();
        }

        public IEnumerable<LoginEvent> Get(int tenant, DateTime fromDate, DateTime to)
        {
            var query =
                from q in MessagesContext.LoginEvents
                from p in MessagesContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
                where q.TenantId == tenant
                where q.Date >= fromDate
                where q.Date <= to
                orderby q.Date descending
                select new LoginEventQuery { LoginEvents = q, User = p };

            return query.AsEnumerable().Select(_mapper.Map<LoginEvent>).ToList();
        }

        public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
        {
            var query = MessagesContext.LoginEvents
                .Where(l => l.TenantId == tenant);

            if (from.HasValue && to.HasValue) query = query.Where(l => l.Date >= from & l.Date <= to);

            return query.Count();
        }
    }
}