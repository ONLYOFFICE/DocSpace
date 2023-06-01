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

    [HttpGet("notification/{type}")]
    public async Task<NotificationSettingsDto> GetSettingsAsync(NotificationType type)
    {
        var isEnabled = await _notificationControllerHelper.GetNotificationStatusAsync(type);

        return new NotificationSettingsDto() { Type = type, IsEnabled = isEnabled };
    }

    [HttpPost("notification")]
    public async Task<NotificationSettingsDto> SetSettingsAsync(NotificationSettingsRequestsDto model)
    {
        await _notificationControllerHelper.SetNotificationStatusAsync(model.Type, model.IsEnabled);

        return _mapper.Map<NotificationSettingsDto>(model);
    }

    [HttpGet("notification/rooms")]
    public RoomsNotificayionSettingsDto GetRoomsNotificationSettings()
    {
        var  settings = _notificationControllerHelper.GetSettings();
        return _mapper.Map<RoomsNotificayionSettingsDto>(settings);
    }

    [HttpPost("notification/rooms")]
    public RoomsNotificayionSettingsDto SetRoomsNotificationStatus(RoomsNotificationsSettingsRequestDto model)
    {
        var settings = _notificationControllerHelper.SetRoomsNotificationStatus(model.RoomsId, model.Mute);
        return _mapper.Map<RoomsNotificayionSettingsDto>(settings);
    }
}
