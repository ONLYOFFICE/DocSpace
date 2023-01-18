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

    private const string UserItem = Constants.UserItem;

    public UsersModule(
        TenantManager tenantManager,
        WebItemSecurity webItemSecurity,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper) 
        : base(tenantManager, webItemSecurity)
    {
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
    }

    public override string Name => Constants.UsersModule;
    public override string Product => Constants.People;
    public override Guid ProductID => WebItemManager.PeopleProductID;
    protected override string DbId => string.Empty;

    public override Task<IEnumerable<Tuple<Feed.Aggregator.Feed, object>>> GetFeeds(FeedFilter filter)
    {
        var users = _userManager.GetUsers().Where(u => u.LastModified >= filter.Time.From && u.LastModified <= filter.Time.To);

        return Task.FromResult(users.Select(u => new Tuple<Feed.Aggregator.Feed, object>(ToFeed(u), u)));
    }

    public override Task<IEnumerable<int>> GetTenantsWithFeeds(DateTime fromTime)
    {
        return Task.FromResult(_userManager.GetTenantsWithFeeds(fromTime));
    }

    private Feed.Aggregator.Feed ToFeed(UserInfo u)
    {
        var fullName = _displayUserSettingsHelper.GetFullUserName(u);

        var feed = new Feed.Aggregator.Feed(u.Id, u.LastModified, true)
        {
            Item = UserItem,
            ItemId = u.Id.ToString(),
            Product = Product,
            Module = Name,
            Title = fullName,
            ExtraLocation = u.Id.ToString(),
            AdditionalInfo = u.Email,
            AdditionalInfo2 = _userManager.GetUserType(u.Id).ToString(),
            AdditionalInfo3 = u.Status.ToString(),
            AdditionalInfo4 = u.ActivationStatus.ToString(),
            Keywords = fullName,
            Action = u.LastModified > u.CreateDate ? FeedAction.Updated : FeedAction.Created,
        };

        return feed;
    }

    public override bool VisibleFor(Feed.Aggregator.Feed feed, object data, Guid userId)
    {
        var user = data as UserInfo;

        return user.Id != userId && !_userManager.IsUser(userId);
    }

    public override Task VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (_userManager.IsUser(userId))
        {
            return Task.CompletedTask;
        }

        foreach (var row in feed)
        {
            var user = row.Item2 as UserInfo;

            if (user.Id != userId)
            {
                row.Item1.Users.Add(userId);
            }
        }

        return Task.CompletedTask;
    }
}