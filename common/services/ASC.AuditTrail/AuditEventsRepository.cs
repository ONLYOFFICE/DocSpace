/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using ASC.AuditTrail.Mappers;
using System.Linq;
using ASC.Core.Users;
using ASC.MessagingSystem;
using Newtonsoft.Json;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF;
using ASC.Common;

namespace ASC.AuditTrail
{
    public class AuditEventsRepository
    {
        private MessageTarget MessageTarget { get; set; }
        private UserFormatter UserFormatter { get; set; }
        private AuditTrailContext AuditTrailContext { get; }
        private AuditActionMapper AuditActionMapper { get; }

        public AuditEventsRepository(MessageTarget messageTarget, UserFormatter userFormatter, DbContextManager<AuditTrailContext> dbContextManager, AuditActionMapper auditActionMapper)
        {
            MessageTarget = messageTarget;
            UserFormatter = userFormatter;
            AuditTrailContext = dbContextManager.Value;
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

        private class Query
        {
            public Core.Common.EF.Model.AuditEvent AuditEvent { get; set; }
            public User User { get; set; }
        }

        private IEnumerable<AuditEvent> Get(int tenant, DateTime? from, DateTime? to, int? limit)
        {
            var query = AuditTrailContext.AuditEvents.Join(AuditTrailContext.User,
                a => a.UserId,
                u => u.Id,
                (a, u) => new Query { AuditEvent = a, User = u })
                .Where(q => q.AuditEvent.TenantId == tenant)
                .OrderBy(q => q.AuditEvent.Date);

            if (from.HasValue && to.HasValue)
            {
                query = (IOrderedQueryable<Query>)query.Where(q => q.AuditEvent.Date >= from & q.AuditEvent.Date <= to);
            }

            if (limit.HasValue)
            {
                query = (IOrderedQueryable<Query>)query.Take((int)limit);
            }

            return ToAuditEvent(query.ToList());
        }

        public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
        {
            var query = AuditTrailContext.AuditEvents
                .Where(a => a.TenantId == tenant)
                .OrderBy(a => a.Date);

            if (from.HasValue && to.HasValue)
            {
                query = (IOrderedQueryable<Core.Common.EF.Model.AuditEvent>)query.Where(a => a.Date >= from & a.Date <= to);
            }

            return query.Count();
        }

        private IEnumerable<AuditEvent> ToAuditEvent(List<Query> list)
        {
            var auditEvents = new List<AuditEvent>();
            list.ForEach(query =>
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
                            Convert.ToString(query.AuditEvent.Description),
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

                    auditEvents.Add(evt);
                }
                catch (Exception)
                {
                }
            });
            return auditEvents;
        }
    }

    public static class AuditEventsRepositoryExtension
    {
        public static DIHelper AddAuditEventsRepositoryService(this DIHelper services)
        {
            _ = services.TryAddScoped<AuditEventsRepository>();
            return services
                .AddUserFormatter()
                .AddAuditTrailContextService()
                .AddMessageTargetService()
                .AddAuditActionMapperService();
        }
    }
}