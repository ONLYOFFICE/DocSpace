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

namespace ASC.Web.Api.ApiModel.ResponseDto;

[Scope]
public class QuotaUsageManager
{
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreConfiguration _configuration;
    private readonly AuthContext _authContext;
    private readonly SettingsManager _settingsManager;
    private readonly WebItemManager _webItemManager;
    private readonly CountPaidUserStatistic _countPaidUserStatistic;
    private readonly CountUserStatistic _activeUsersStatistic;

    public QuotaUsageManager(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration configuration,
        AuthContext authContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        CountPaidUserStatistic countPaidUserStatistic,
        CountUserStatistic activeUsersStatistic)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _configuration = configuration;
        _authContext = authContext;
        _settingsManager = settingsManager;
        _webItemManager = webItemManager;
        _countPaidUserStatistic = countPaidUserStatistic;
        _activeUsersStatistic = activeUsersStatistic;
    }

    public async Task<QuotaUsageDto> Get()
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var quota = await _tenantManager.GetCurrentTenantQuotaAsync();
        var quotaRows = (await _tenantManager.FindTenantQuotaRowsAsync(tenant.Id))
            .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty)
            .ToList();

        var result = new QuotaUsageDto
        {
            StorageSize = (ulong)Math.Max(0, quota.MaxTotalSize),
            UsedSize = (ulong)Math.Max(0, quotaRows.Sum(r => r.Counter)),
            MaxRoomAdminsCount = quota.CountRoomAdmin,
            RoomAdminCount = _coreBaseSettings.Personal ? 1 : await _countPaidUserStatistic.GetValueAsync(),
            MaxUsers = _coreBaseSettings.Standalone ? -1 : quota.CountUser,
            UsersCount = _coreBaseSettings.Personal ? 0 : await _activeUsersStatistic.GetValueAsync(),

            StorageUsage = quotaRows
                .Select(x => new QuotaUsage { Path = x.Path.TrimStart('/').TrimEnd('/'), Size = x.Counter, })
                .ToList()
        };

        if (_coreBaseSettings.Personal && SetupInfo.IsVisibleSettings("PersonalMaxSpace"))
        {
            result.UserStorageSize = await _configuration.PersonalMaxSpaceAsync(_settingsManager);

            var webItem = _webItemManager[WebItemManager.DocumentsProductID];
            if (webItem.Context.SpaceUsageStatManager is IUserSpaceUsage spaceUsageManager)
            {
                result.UserUsedSize = await spaceUsageManager.GetUserSpaceUsageAsync(_authContext.CurrentAccount.ID);
            }
        }

        result.MaxFileSize = Math.Min(result.AvailableSize, (ulong)quota.MaxFileSize);

        return result;
    }
}

public class QuotaUsageDto
{
    public ulong StorageSize { get; set; }
    public ulong MaxFileSize { get; set; }
    public ulong UsedSize { get; set; }
    public int MaxRoomAdminsCount { get; set; }
    public int RoomAdminCount { get; set; }

    public ulong AvailableSize
    {
        get { return Math.Max(0, StorageSize > UsedSize ? StorageSize - UsedSize : 0); }
        set { throw new NotImplementedException(); }
    }

    public int AvailableUsersCount
    {
        get { return Math.Max(0, MaxRoomAdminsCount - RoomAdminCount); }
        set { throw new NotImplementedException(); }
    }

    public IList<QuotaUsage> StorageUsage { get; set; }
    public long UserStorageSize { get; set; }
    public long UserUsedSize { get; set; }

    public long UserAvailableSize
    {
        get { return Math.Max(0, UserStorageSize - UserUsedSize); }
        set { throw new NotImplementedException(); }
    }

    public long MaxUsers { get; set; }
    public long UsersCount { get; set; }
}

public class QuotaUsage
{
    public string Path { get; set; }
    public long Size { get; set; }
}