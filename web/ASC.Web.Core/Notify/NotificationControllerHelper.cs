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

namespace ASC.Web.Core.Notify;
[Scope]
public class NotificationControllerHelper
{
    private readonly SettingsManager _settingsManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly AuthContext _authContext;

    public NotificationControllerHelper(
        SettingsManager settingsManager,
        StudioNotifyHelper studioNotifyHelper,
        AuthContext authContext)
    {
        _settingsManager = settingsManager;
        _studioNotifyHelper = studioNotifyHelper;
        _authContext = authContext;
    }

    public bool GetNotificationStatus(NotificationType notificationType)
    {
        bool isEnabled;
        switch (notificationType)
        {
            case NotificationType.Badges:
                var settings = _settingsManager.Load<BadgesSettings>();
                return settings.EnableBadges;
            case NotificationType.RoomsActivity:
                isEnabled = _studioNotifyHelper.IsSubscribedToNotify(_authContext.CurrentAccount.ID, Actions.RoomsActivity);
                return isEnabled;
            case NotificationType.DailyFeed:
                isEnabled = _studioNotifyHelper.IsSubscribedToNotify(_authContext.CurrentAccount.ID, Actions.SendWhatsNew);
                return isEnabled;
            case NotificationType.UsefullTips:
                isEnabled = _studioNotifyHelper.IsSubscribedToNotify(_authContext.CurrentAccount.ID, Actions.PeriodicNotify);
                return isEnabled;
            default:
                throw new Exception("Incorrect parameters");
        };
    }

    public void SetSettings(NotificationType notificationType, bool isEnabled)
    {
        switch (notificationType)
        {
            case NotificationType.Badges:
                var settings = new BadgesSettings() { EnableBadges = isEnabled };
                _settingsManager.Save(settings);
                break;
            case NotificationType.RoomsActivity:
                _studioNotifyHelper.SubscribeToNotify(_authContext.CurrentAccount.ID, Actions.RoomsActivity, isEnabled);
                break;
            case NotificationType.DailyFeed:
                _studioNotifyHelper.SubscribeToNotify(_authContext.CurrentAccount.ID, Actions.SendWhatsNew, isEnabled);
                break;
            case NotificationType.UsefullTips:
                _studioNotifyHelper.SubscribeToNotify(_authContext.CurrentAccount.ID, Actions.PeriodicNotify, isEnabled);
                break;
        }
    }
}

public enum NotificationType
{
    Badges = 0,
    RoomsActivity = 1,
    DailyFeed = 2,
    UsefullTips = 3
}
