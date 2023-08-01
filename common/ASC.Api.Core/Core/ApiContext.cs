// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Core;

[Scope]
public class ApiContext : ICloneable
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string[] Fields { get; set; }
    public string[] FilterValues { get; set; }
    public bool FromCache { get; set; }
    public Tenant Tenant => _tenant ??= _tenantManager.GetCurrentTenant(_httpContextAccessor?.HttpContext);
    public long? TotalCount
    {
        set
        {
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(nameof(TotalCount)))
            {
                _httpContextAccessor.HttpContext.Items[nameof(TotalCount)] = value;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add(nameof(TotalCount), value);
            }
        }
    }

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

    internal long SpecifiedCount { get; private set; }
    internal long SpecifiedStartIndex { get; set; }

    private Tenant _tenant;
    private static readonly int _maxCount = 1000;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;

    public ApiContext(IHttpContextAccessor httpContextAccessor, SecurityContext securityContext, TenantManager tenantManager)
    {
        _securityContext = securityContext;
        _tenantManager = tenantManager;
        _httpContextAccessor = httpContextAccessor;
        if (httpContextAccessor.HttpContext == null)
        {
            return;
        }

        Count = _maxCount;
        var query = _httpContextAccessor.HttpContext.Request.Query;
        //Try parse values
        var count = query.GetRequestValue("count");
        if (!string.IsNullOrEmpty(count) && ulong.TryParse(count, out var countParsed))
        {
            //Count specified and valid
            Count = Math.Min((long)countParsed, _maxCount);
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

    public ApiContext SetCount(int count)
    {
        _httpContextAccessor.HttpContext.Items[nameof(Count)] = count;

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

    public async Task AuthByClaimAsync()
    {
        var id = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Sid);
        if (Guid.TryParse(id?.Value, out var userId))
        {
            await _securityContext.AuthenticateMeWithoutCookieAsync(userId);
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
            {
                return values;
            }

            values = query[key];
            if (values.Count > 0)
            {
                if (values.Count == 1) //If it's only one element
                {
                    //Try split
                    if (!string.IsNullOrEmpty(values[0]))
                    {
                        return values[0].Split(',');
                    }
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
        return context?.Fields == null
            || (context.Fields != null
            && context.Fields.Contains(field, StringComparer.InvariantCultureIgnoreCase));
    }
}