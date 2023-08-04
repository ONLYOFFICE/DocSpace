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

namespace ASC.Feed.Aggregator.Modules;

public abstract class FeedModule : IFeedModule
{
    public abstract string Name { get; }
    public abstract string Product { get; }
    public abstract Guid ProductID { get; }
    protected abstract string DbId { get; }

    protected readonly TenantManager _tenantManager;
    protected readonly WebItemSecurity _webItemSecurity;

    protected FeedModule(TenantManager tenantManager, WebItemSecurity webItemSecurity)
    {
        _tenantManager = tenantManager;
        _webItemSecurity = webItemSecurity;
    }

    public abstract Task<IEnumerable<Tuple<Feed, object>>> GetFeeds(FeedFilter filter);

    public abstract Task<IEnumerable<int>> GetTenantsWithFeeds(DateTime fromTime);

    public virtual async Task VisibleForAsync(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (!await _webItemSecurity.IsAvailableForUserAsync(ProductID, userId))
        {
            return;
        }

        foreach (var tuple in feed)
        {
            if (await VisibleForAsync(tuple.Item1.Feed, tuple.Item2, userId))
            {
                tuple.Item1.Users.Add(userId);
            }
        }
    }

    public virtual async Task<bool> VisibleForAsync(Feed feed, object data, Guid userId)
    {
        return await _webItemSecurity.IsAvailableForUserAsync(ProductID, userId);
    }

    protected static Guid ToGuid(object guid)
    {
        try
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }
        catch (Exception)
        {
            return Guid.Empty;
        }
    }

    protected string GetGroupId(string item, Guid author, DateTime date, string rootId = null, int action = -1)
    {
        var time = date.ToString("g");

        if (rootId == null)
        {
            return $"{item}_{author}_{time}";
        }
        
        if (action == -1)
        {
            return $"{item}_{author}_{time}_{rootId}";
        }
        
        return $"{item}_{author}_{time}_{rootId}_{action}";
    }
}
