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

using ASC.AuditTrail.Mappers;
using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Users;
using ASC.MessagingSystem;

using Newtonsoft.Json;

namespace ASC.AuditTrail
{
    [Scope]
    public class AuditEventsRepository
    {
        private MessageTarget MessageTarget { get; set; }
        private UserFormatter UserFormatter { get; set; }
        private Lazy<AuditTrailContext> LazyAuditTrailContext { get; }
        private AuditTrailContext AuditTrailContext { get => LazyAuditTrailContext.Value; }
        private AuditActionMapper AuditActionMapper { get; }

        public AuditEventsRepository(MessageTarget messageTarget, UserFormatter userFormatter, DbContextManager<AuditTrailContext> dbContextManager, AuditActionMapper auditActionMapper)
        {
            MessageTarget = messageTarget;
            UserFormatter = userFormatter;
            LazyAuditTrailContext = new Lazy<AuditTrailContext>(() => dbContextManager.Value );
            AuditActionMapper = auditActionMapper;
        }

        public IEnumerable<AuditEvent> GetLast(int tenant, int chunk)
        {
            return Get(tenant, null, null, chunk);
        }

        public IEnumerable<AuditEvent> Get(int tenant, DateTime from, DateTime to)
        {
            return Get(tenant, from, to, null);
        }

        private sealed class Query
        {
            public Core.Common.EF.Model.AuditEvent AuditEvent { get; set; }
            public User User { get; set; }
        }

        private IEnumerable<AuditEvent> Get(int tenant, DateTime? fromDate, DateTime? to, int? limit)
        {
            var query =
               from q in AuditTrailContext.AuditEvents
               from p in AuditTrailContext.Users.AsQueryable().Where(p => q.UserId == p.Id).DefaultIfEmpty()
               where q.TenantId == tenant
               orderby q.Date descending
               select new Query { AuditEvent = q, User = p };

            if (fromDate.HasValue && to.HasValue)
            {
                query = query.Where(q => q.AuditEvent.Date >= fromDate && q.AuditEvent.Date <= to);
            }

            if (limit.HasValue)
            {
                query = query.Take((int)limit);
            }

            return query.AsEnumerable().Select(ToAuditEvent).ToList();
        }

        public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Core.Common.EF.Model.AuditEvent> query = AuditTrailContext.AuditEvents
                .AsQueryable()
                .Where(a => a.TenantId == tenant)
                .OrderByDescending(a => a.Date);

            if (from.HasValue && to.HasValue)
            {
                query = query.Where(a => a.Date >= from && a.Date <= to);
            }

            return query.Count();
        }

        private AuditEvent ToAuditEvent(Query query)
        {
            try
            {
                var evt = new AuditEvent
                {
                    Id = query.AuditEvent.Id,
                    IP = query.AuditEvent.Ip,
                    Initiator = query.AuditEvent.Initiator,
                    Browser = query.AuditEvent.Browser,
                    Platform = query.AuditEvent.Platform,
                    Date = query.AuditEvent.Date,
                    TenantId = query.AuditEvent.TenantId,
                    UserId = query.AuditEvent.UserId,
                    Page = query.AuditEvent.Page,
                    Action = query.AuditEvent.Action
                };

                if (query.AuditEvent.Description != null)
                {
                    evt.Description = JsonConvert.DeserializeObject<IList<string>>(
                        query.AuditEvent.Description,
                        new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
                }

                evt.Target = MessageTarget.Parse(query.AuditEvent.Target);

                evt.UserName = (query.User.FirstName != null && query.User.LastName != null) ? UserFormatter.GetUserName(query.User.FirstName, query.User.LastName) :
                    evt.UserId == Core.Configuration.Constants.CoreSystem.ID ? AuditReportResource.SystemAccount :
                        evt.UserId == Core.Configuration.Constants.Guest.ID ? AuditReportResource.GuestAccount :
                            evt.Initiator ?? AuditReportResource.UnknownAccount;

                evt.ActionText = AuditActionMapper.GetActionText(evt);
                evt.ActionTypeText = AuditActionMapper.GetActionTypeText(evt);
                evt.Product = AuditActionMapper.GetProductText(evt);
                evt.Module = AuditActionMapper.GetModuleText(evt);

                return evt;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}