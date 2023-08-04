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

using Constants = ASC.Feed.Constants;
using FeedModule = ASC.Feed.Aggregator.Modules.FeedModule;

namespace ASC.Files.Service.Core;

public class UsersModule : FeedModule
{
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly TenantUtil _tenantUtil;

    private const string UserItem = Constants.UserItem;

    public UsersModule(
        TenantManager tenantManager,
        WebItemSecurity webItemSecurity,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        TenantUtil tenantUtil) 
        : base(tenantManager, webItemSecurity)
    {
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _tenantUtil = tenantUtil;
    }

    public override string Name => Constants.UsersModule;
    public override string Product => Constants.People;
    public override Guid ProductID => WebItemManager.PeopleProductID;
    protected override string DbId => string.Empty;

    public override async Task<IEnumerable<Tuple<Feed.Aggregator.Feed, object>>> GetFeeds(FeedFilter filter)
    {
        var users = (await _userManager.GetUsersAsync()).Where(u => u.LastModified >= filter.Time.From && u.LastModified <= filter.Time.To).ToAsyncEnumerable();

        return await users.SelectAwait(async u => new Tuple<Feed.Aggregator.Feed, object>(await ToFeedAsync(u), u)).ToListAsync();
    }

    public override async Task<IEnumerable<int>> GetTenantsWithFeeds(DateTime fromTime)
    {
        return await _userManager.GetTenantsWithFeedsAsync(fromTime);
    }

    private async Task<Feed.Aggregator.Feed> ToFeedAsync(UserInfo u)
    {
        var fullName = _displayUserSettingsHelper.GetFullUserName(u);

        var feed = new Feed.Aggregator.Feed(u.Id, _tenantUtil.DateTimeToUtc(u.LastModified), true)
        {
            Item = UserItem,
            ItemId = u.Id.ToString(),
            Product = Product,
            Module = Name,
            Title = fullName,
            ExtraLocation = u.Id.ToString(),
            AdditionalInfo = u.Email,
            AdditionalInfo2 = (await _userManager.GetUserTypeAsync(u.Id)).ToString(),
            AdditionalInfo3 = u.Status.ToString(),
            AdditionalInfo4 = u.ActivationStatus.ToString(),
            Keywords = fullName,
            Action = u.LastModified > u.CreateDate ? FeedAction.Updated : FeedAction.Created,
        };

        return feed;
    }

    public override async Task<bool> VisibleForAsync(Feed.Aggregator.Feed feed, object data, Guid userId)
    {
        var user = data as UserInfo;

        return user.Id != userId && !await _userManager.IsUserAsync(userId);
    }

    public override async Task VisibleForAsync(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (await _userManager.IsUserAsync(userId))
        {
            return;
        }

        foreach (var row in feed)
        {
            var user = row.Item2 as UserInfo;

            if (user.Id != userId)
            {
                row.Item1.Users.Add(userId);
            }
        }
    }
}