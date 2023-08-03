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

namespace ASC.Web.Api.Controllers.Settings;

public class NotificationController : BaseSettingsController
{
    private readonly NotificationControllerHelper _notificationControllerHelper;
    private readonly IMapper _mapper;

    public NotificationController(
        ApiContext apiContext,
        IMemoryCache memoryCache,
        WebItemManager webItemManager,
        NotificationControllerHelper notificationControllerHelper,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _notificationControllerHelper = notificationControllerHelper;
        _mapper = mapper;
    }

    /// <summary>
    /// Checks if the notification type specified in the request is enabled or not.
    /// </summary>
    /// <category>Notifications</category>
    /// <short>Check notification availability</short>
    /// <param type="ASC.Web.Core.Notify.NotificationType, ASC.Web.Core" method="url" name="type">Notification type (Badges, RoomsActivity, DailyFeed, UsefullTips)</param>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.NotificationSettingsDto, ASC.Web.Api">Notification settings</returns>
    /// <path>api/2.0/settings/notification/{type}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("notification/{type}")]
    public async Task<NotificationSettingsDto> GetSettingsAsync(NotificationType type)
    {
        var isEnabled = await _notificationControllerHelper.GetNotificationStatusAsync(type);

        return new NotificationSettingsDto() { Type = type, IsEnabled = isEnabled };
    }

    /// <summary>
    /// Enables the notification type specified in the request.
    /// </summary>
    /// <category>Notifications</category>
    /// <short>Enable notifications</short>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.NotificationSettingsRequestsDto, ASC.Web.Api" name="inDto">Notification settings request parameters</param>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.NotificationSettingsDto, ASC.Web.Api">Notification settings</returns>
    /// <path>api/2.0/settings/notification</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("notification")]
    public async Task<NotificationSettingsDto> SetSettingsAsync(NotificationSettingsRequestsDto inDto)
    {
        await _notificationControllerHelper.SetNotificationStatusAsync(inDto.Type, inDto.IsEnabled);

        return _mapper.Map<NotificationSettingsDto>(inDto);
    }

    /// <summary>
    /// Returns a list of rooms with the disabled notifications
    /// </summary>
    /// <category>Notifications</category>
    /// <short>Get room notification settings</short>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.RoomsNotificayionSettingsDto, ASC.Web.Api">Room notification settings</returns>
    /// <path>api/2.0/settings/notification/rooms</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("notification/rooms")]
    public RoomsNotificayionSettingsDto GetRoomsNotificationSettings()
    {
        var  settings = _notificationControllerHelper.GetSettings();
        return _mapper.Map<RoomsNotificayionSettingsDto>(settings);
    }

    /// <summary>
    /// Sets a notification status for a room with the ID specified in the request.
    /// </summary>
    /// <category>Notifications</category>
    /// <short>Set room notification status</short>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.RoomsNotificationsSettingsRequestDto, ASC.Web.Api" name="inDto">Request parameters for the room notification settings</param>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.RoomsNotificayionSettingsDto, ASC.Web.Api">Room notification settings</returns>
    /// <path>api/2.0/settings/notification/rooms</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("notification/rooms")]
    public RoomsNotificayionSettingsDto SetRoomsNotificationStatus(RoomsNotificationsSettingsRequestDto inDto)
    {
        var settings = _notificationControllerHelper.SetRoomsNotificationStatus(inDto.RoomsId, inDto.Mute);
        return _mapper.Map<RoomsNotificayionSettingsDto>(settings);
    }
}
