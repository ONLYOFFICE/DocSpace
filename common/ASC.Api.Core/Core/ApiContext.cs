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
using System.Security.Claims;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;

using Microsoft.AspNetCore.Http;

namespace ASC.Api.Core
{
    [Scope]
    public class ApiContext : ICloneable
    {
        private static int MaxCount = 1000;
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public Tenant tenant;
        public Tenant Tenant { get { return tenant ??= TenantManager.GetCurrentTenant(HttpContextAccessor?.HttpContext); } }

        public ApiContext(IHttpContextAccessor httpContextAccessor, SecurityContext securityContext, TenantManager tenantManager)
        {
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            HttpContextAccessor = httpContextAccessor;
            if (httpContextAccessor.HttpContext == null) return;

            Count = MaxCount;
            var query = HttpContextAccessor.HttpContext.Request.Query;
            //Try parse values
            var count = query.GetRequestValue("count");
            if (!string.IsNullOrEmpty(count) && ulong.TryParse(count, out var countParsed))
            {
                //Count specified and valid
                Count = Math.Min((long)countParsed, MaxCount);
            }

            var startIndex = query.GetRequestValue("startIndex");
            if (startIndex != null && long.TryParse(startIndex, out var startIndexParsed))
            {
                StartIndex = Math.Max(0, startIndexParsed);
                SpecifiedStartIndex = StartIndex;
            }

            var sortOrder = query.GetRequestValue("sortOrder");
            if ("descending".Equals(sortOrder))
            {
                SortDescending = true;
            }

            FilterToType = query.GetRequestValue("type");
            SortBy = query.GetRequestValue("sortBy");
            FilterBy = query.GetRequestValue("filterBy");
            FilterOp = query.GetRequestValue("filterOp");
            FilterValue = query.GetRequestValue("filterValue");
            FilterValues = query.GetRequestArray("filterValue");
            Fields = query.GetRequestArray("fields");

            var updatedSince = query.GetRequestValue("updatedSince");
            if (updatedSince != null)
            {
                UpdatedSince = Convert.ToDateTime(updatedSince);
            }
        }

        public string[] Fields { get; set; }

        public string[] FilterValues { get; set; }

        /// <summary>
        /// Filters responce to specific type from request parameter "type"
        /// </summary>
        /// <remarks>
        /// The type name is retrieved from [DataContractAttribute] name
        /// </remarks>
        public string FilterToType { get; set; }

        /// <summary>
        /// Gets count to get item from collection. Request parameter "count"
        /// </summary>
        /// <remarks>
        /// Don't forget to call _context.SetDataPaginated() to prevent SmartList from filtering response if you fetch data from DB with TOP & COUNT
        /// </remarks>
        public long Count { get; set; }

        /// <summary>
        /// Gets start index to get item from collection. Request parameter "startIndex"
        /// </summary>
        /// <remarks>
        /// Don't forget to call _context.SetDataPaginated() to prevent SmartList from filtering response if you fetch data from DB with TOP & COUNT
        /// </remarks>
        public long StartIndex { get; set; }

        internal long SpecifiedStartIndex { get; set; }

        /// <summary>
        /// Gets field to sort by from request parameter "sortBy"
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets field to filter from request parameter "filterBy"
        /// </summary>
        public string FilterBy { get; set; }

        /// <summary>
        /// Gets filter operation from request parameter "filterOp"
        /// can be one of the following:"contains","equals","startsWith","present"
        /// </summary>
        public string FilterOp { get; set; }

        /// <summary>
        /// Gets value to filter from request parameter "filterValue"
        /// </summary>
        public string FilterValue { get; set; }

        /// <summary>
        /// Sort direction. From request parameter "sortOrder" can be "descending" or "ascending"
        /// Like ...&sortOrder=descending&...
        /// </summary>
        public bool SortDescending { get; set; }

        /// <summary>
        /// Gets value to filter from request parameter "updatedSince"
        /// </summary>
        public DateTime UpdatedSince { get; set; }

        public bool FromCache { get; set; }

        internal long SpecifiedCount { get; private set; }

        /// <summary>
        /// Set mark that data is already paginated and additional filtering is not needed
        /// </summary>
        public ApiContext SetDataPaginated()
        {
            //Count = 0;//We always ask for +1 count so smart list should cut it
            StartIndex = 0;
            return this;
        }

        public ApiContext SetDataSorted()
        {
            SortBy = string.Empty;
            return this;
        }

        public ApiContext SetDataFiltered()
        {
            FilterBy = string.Empty;
            FilterOp = string.Empty;
            FilterValue = string.Empty;
            return this;
        }

        public ApiContext SetTotalCount(long totalCollectionCount)
        {
            TotalCount = totalCollectionCount;
            return this;
        }

        public long? TotalCount
        {
            set
            {
                if (HttpContextAccessor.HttpContext.Items.ContainsKey(nameof(TotalCount)))
                {
                    HttpContextAccessor.HttpContext.Items[nameof(TotalCount)] = value;
                }
                else
                {
                    HttpContextAccessor.HttpContext.Items.Add(nameof(TotalCount), value);
                }
            }
        }

        private SecurityContext SecurityContext { get; }
        private TenantManager TenantManager { get; }

        public ApiContext SetCount(int count)
        {
            HttpContextAccessor.HttpContext.Items[nameof(Count)] = count;
            return this;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("C:{0},S:{1},So:{2},Sd:{3},Fb;{4},Fo:{5},Fv:{6},Us:{7},Ftt:{8}", Count, StartIndex,
                                 SortBy, SortDescending, FilterBy, FilterOp, FilterValue, UpdatedSince.Ticks, FilterToType);
        }

        public void AuthByClaim()
        {
            var id = HttpContextAccessor.HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Sid);
            if (Guid.TryParse(id?.Value, out var userId))
            {
                SecurityContext.AuthenticateMeWithoutCookie(userId);
            }
        }
    }

    public static class QueryExtension
    {
        internal static string[] GetRequestArray(this IQueryCollection query, string key)
        {
            if (query != null)
            {
                var values = query[key + "[]"];
                if (values.Count > 0)
                    return values;

                values = query[key];
                if (values.Count > 0)
                {
                    if (values.Count == 1) //If it's only one element
                    {
                        //Try split
                        if (!string.IsNullOrEmpty(values[0]))
                            return values[0].Split(',');
                    }
                    return values;
                }
            }
            return null;
        }

        public static string GetRequestValue(this IQueryCollection query, string key)
        {
            var reqArray = query.GetRequestArray(key);
            return reqArray?.FirstOrDefault();
        }
    }

    public static class ApiContextExtension
    {
        public static bool Check(this ApiContext context, string field)
        {
            return context?.Fields == null || (context.Fields != null && context.Fields.Contains(field, StringComparer.InvariantCultureIgnoreCase));
        }
    }
}