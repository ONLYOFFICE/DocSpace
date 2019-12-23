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

using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Tenants;
using ASC.VoipService.Twilio;

namespace ASC.VoipService.Dao
{
    public class VoipDao : AbstractDao
    {
        public VoipDao(
            int tenantID,
            DbContextManager<VoipDbContext> dbOptions,
            AuthContext authContext,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            BaseCommonLinkUtility baseCommonLinkUtility,
            ConsumerFactory consumerFactory)
            : base(dbOptions, tenantID)
        {
            AuthContext = authContext;
            TenantUtil = tenantUtil;
            SecurityContext = securityContext;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            ConsumerFactory = consumerFactory;
        }

        public virtual VoipPhone SaveOrUpdateNumber(VoipPhone phone)
        {
            if (!string.IsNullOrEmpty(phone.Number))
            {
                phone.Number = phone.Number.TrimStart('+');
            }

            var voipNumber = new VoipNumber
            {
                Id = phone.Id,
                Number = phone.Number,
                Alias = phone.Alias,
                Settings = phone.Settings.ToString(),
                TenantId = TenantID
            };

            VoipDbContext.VoipNumbers.Add(voipNumber);
            VoipDbContext.SaveChanges();

            return phone;
        }

        public virtual void DeleteNumber(string phoneId = "")
        {
            var number = VoipDbContext.VoipNumbers.Where(r => r.Id == phoneId && r.TenantId == TenantID).FirstOrDefault();
            VoipDbContext.VoipNumbers.Remove(number);
            VoipDbContext.SaveChanges();
        }

        public virtual IEnumerable<VoipPhone> GetNumbers(params string[] ids)
        {
            var numbers = VoipDbContext.VoipNumbers.Where(r => r.TenantId == TenantID);

            if (ids.Any())
            {
                numbers = numbers.Where(r => ids.Any(a => a == r.Number || a == r.Id));
            }

            return numbers.ToList().ConvertAll(ToPhone);
        }

        public VoipPhone GetNumber(string id)
        {
            return GetNumbers(id.TrimStart('+')).FirstOrDefault();
        }

        public virtual VoipPhone GetCurrentNumber()
        {
            return GetNumbers().FirstOrDefault(r => r.Caller != null);
        }


        public VoipCall SaveOrUpdateCall(VoipCall call)
        {
            var voipCall = new DbVoipCall
            {
                TenantId = TenantID,
                Id = call.Id,
                NumberFrom = call.From,
                NumberTo = call.To,
                ContactId = call.ContactId
            };

            if (!string.IsNullOrEmpty(call.ParentID))
            {
                voipCall.ParentCallId = call.ParentID;
            }

            if (call.Status.HasValue)
            {
                voipCall.Status = (int)call.Status.Value;
            }

            if (!call.AnsweredBy.Equals(Guid.Empty))
            {
                voipCall.AnsweredBy = call.AnsweredBy;
            }

            if (call.DialDate == DateTime.MinValue)
            {
                call.DialDate = DateTime.UtcNow;
            }

            voipCall.DialDate = TenantUtil.DateTimeToUtc(call.DialDate);

            if (call.DialDuration > 0)
            {
                voipCall.DialDuration = call.DialDuration;
            }

            if (call.Price > decimal.Zero)
            {
                voipCall.Price = call.Price;
            }

            if (call.VoipRecord != null)
            {
                if (!string.IsNullOrEmpty(call.VoipRecord.Id))
                {
                    voipCall.RecordSid = call.VoipRecord.Id;
                }

                if (!string.IsNullOrEmpty(call.VoipRecord.Uri))
                {
                    voipCall.RecordUrl = call.VoipRecord.Uri;
                }

                if (call.VoipRecord.Duration != 0)
                {
                    voipCall.RecordDuration = call.VoipRecord.Duration;
                }

                if (call.VoipRecord.Price != default)
                {
                    voipCall.RecordPrice = call.VoipRecord.Price;
                }
            }

            VoipDbContext.VoipCalls.Add(voipCall);
            VoipDbContext.SaveChanges();

            return call;
        }

        public IEnumerable<VoipCall> GetCalls(VoipCallFilter filter)
        {
            var query = GetCallsQuery(filter);

            if (filter.SortByColumn != null)
            {
                query.OrderBy(filter.SortByColumn, filter.SortOrder);
            }

            query = query.Skip((int)filter.Offset);
            query = query.Take((int)filter.Max * 3);

            var calls = query.ToList().ConvertAll(ToCall);

            calls = calls.GroupJoin(calls, call => call.Id, h => h.ParentID, (call, h) =>
            {
                call.ChildCalls.AddRange(h);
                return call;
            }).Where(r => string.IsNullOrEmpty(r.ParentID)).ToList();

            return calls;
        }

        public VoipCall GetCall(string id)
        {
            return GetCalls(new VoipCallFilter { Id = id }).FirstOrDefault();
        }

        public int GetCallsCount(VoipCallFilter filter)
        {
            return GetCallsQuery(filter).Where(r => r.DbVoipCall.ParentCallId == "").Count();
        }

        public IEnumerable<VoipCall> GetMissedCalls(Guid agent, long count = 0, DateTime? from = null)
        {
            var query = GetCallsQuery(new VoipCallFilter { Agent = agent, SortBy = "date", SortOrder = true, Type = "missed" });

            if (from.HasValue)
            {
                query = query.Where(r => r.DbVoipCall.DialDate >= TenantUtil.DateTimeFromUtc(from.Value));
            }

            if (count != 0)
            {
                query = query.Take((int)count);
            }

            var a = query.Select(ca => new
            {
                dbVoipCall = ca,
                tmpDate = VoipDbContext.VoipCalls
                .Where(tmp => tmp.TenantId == ca.DbVoipCall.TenantId)
                .Where(tmp => tmp.NumberFrom == ca.DbVoipCall.NumberFrom || tmp.NumberTo == ca.DbVoipCall.NumberFrom)
                .Where(tmp => tmp.Status <= (int)VoipCallStatus.Missed)
                .Max(tmp => tmp.DialDate)
            }).Where(r => r.dbVoipCall.DbVoipCall.DialDate >= r.tmpDate || r.tmpDate == default);

            return a.ToList().ConvertAll(r => ToCall(r.dbVoipCall));
        }

        private IQueryable<CallContact> GetCallsQuery(VoipCallFilter filter)
        {
            var q = VoipDbContext.VoipCalls
                .Where(r => r.TenantId == TenantID);

            if (!string.IsNullOrEmpty(filter.Id))
            {
                q = q.Where(r => r.Id == filter.Id || r.ParentCallId == filter.Id);
            }

            if (filter.ContactID.HasValue)
            {
                q = q.Where(r => r.ContactId == filter.ContactID.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                q = q.Where(r => r.Id.StartsWith(filter.SearchText));
            }

            if (filter.TypeStatus.HasValue)
            {
                q = q.Where(r => r.Status == filter.TypeStatus.Value);
            }

            if (filter.FromDate.HasValue)
            {
                q = q.Where(r => r.DialDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                q = q.Where(r => r.DialDate <= filter.ToDate.Value);
            }

            if (filter.Agent.HasValue)
            {
                q = q.Where(r => r.AnsweredBy == filter.Agent.Value);
            }

            return q
                .GroupBy(r => r.Id, r => r)
                .Join(
                    VoipDbContext.CrmContact.DefaultIfEmpty(),
                    r => r.FirstOrDefault().ContactId,
                    c => c.Id,
                    (call, contact) => new CallContact { DbVoipCall = call.FirstOrDefault(), CrmContact = contact })
                ;
        }

        class CallContact
        {
            public DbVoipCall DbVoipCall { get; set; }
            public CrmContact CrmContact { get; set; }
        }

        #region Converters

        private VoipPhone ToPhone(VoipNumber r)
        {
            return GetProvider().GetPhone(r);
        }

        private VoipCall ToCall(CallContact dbVoipCall)
        {
            var call = new VoipCall
            {
                Id = dbVoipCall.DbVoipCall.Id,
                ParentID = dbVoipCall.DbVoipCall.ParentCallId,
                From = dbVoipCall.DbVoipCall.NumberFrom,
                To = dbVoipCall.DbVoipCall.NumberTo,
                AnsweredBy = dbVoipCall.DbVoipCall.AnsweredBy,
                DialDate = TenantUtil.DateTimeFromUtc(dbVoipCall.DbVoipCall.DialDate),
                DialDuration = dbVoipCall.DbVoipCall.DialDuration,
                Price = dbVoipCall.DbVoipCall.Price,
                Status = (VoipCallStatus)dbVoipCall.DbVoipCall.Status,
                VoipRecord = new VoipRecord
                {
                    Id = dbVoipCall.DbVoipCall.RecordSid,
                    Uri = dbVoipCall.DbVoipCall.RecordUrl,
                    Duration = dbVoipCall.DbVoipCall.RecordDuration,
                    Price = dbVoipCall.DbVoipCall.RecordPrice
                },
                ContactId = dbVoipCall.CrmContact.Id,
                ContactIsCompany = dbVoipCall.CrmContact.IsCompany,
            };

            if (call.ContactId != 0)
            {
                call.ContactTitle = call.ContactIsCompany
                                        ? dbVoipCall.CrmContact.CompanyName
                                        : dbVoipCall.CrmContact.FirstName == null || dbVoipCall.CrmContact.LastName == null ? null : string.Format("{0} {1}", dbVoipCall.CrmContact.FirstName, dbVoipCall.CrmContact.LastName);
            }

            return call;
        }

        public Consumer Consumer
        {
            get { return ConsumerFactory.GetByKey("twilio"); }
        }

        public TwilioProvider GetProvider()
        {
            return new TwilioProvider(Consumer["twilioAccountSid"], Consumer["twilioAuthToken"], AuthContext, TenantUtil, SecurityContext, BaseCommonLinkUtility);
        }

        public bool ConfigSettingsExist
        {
            get
            {
                return !string.IsNullOrEmpty(Consumer["twilioAccountSid"]) &&
                       !string.IsNullOrEmpty(Consumer["twilioAuthToken"]);
            }
        }

        public AuthContext AuthContext { get; }
        public TenantUtil TenantUtil { get; }
        public SecurityContext SecurityContext { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public ConsumerFactory ConsumerFactory { get; }

        #endregion
    }
}