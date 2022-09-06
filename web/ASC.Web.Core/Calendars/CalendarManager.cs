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

namespace ASC.Web.Core.Calendars;

public delegate List<BaseCalendar> GetCalendarForUser(Guid userId);

[Scope]
public class CalendarManager
{
    private readonly List<GetCalendarForUser> _calendarProviders = new List<GetCalendarForUser>();
    private readonly List<BaseCalendar> _calendars = new List<BaseCalendar>();

    public void RegistryCalendar(BaseCalendar calendar)
    {
        lock (this._calendars)
        {
            if (!this._calendars.Exists(c => string.Equals(c.Id, calendar.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                this._calendars.Add(calendar);
            }
        }
    }

    public void UnRegistryCalendar(string calendarId)
    {
        lock (this._calendars)
        {
            this._calendars.RemoveAll(c => string.Equals(c.Id, calendarId, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public void RegistryCalendarProvider(GetCalendarForUser provider)
    {
        lock (this._calendarProviders)
        {
            if (!this._calendarProviders.Exists(p => p.Equals(provider)))
            {
                this._calendarProviders.Add(provider);
            }
        }
    }

    public void UnRegistryCalendarProvider(GetCalendarForUser provider)
    {
        lock (this._calendarProviders)
        {
            this._calendarProviders.RemoveAll(p => p.Equals(provider));
        }
    }

    public BaseCalendar GetCalendarForUser(Guid userId, string calendarId, UserManager userManager)
    {
        return GetCalendarsForUser(userId, userManager).Find(c => string.Equals(c.Id, calendarId, StringComparison.InvariantCultureIgnoreCase));
    }

    public List<BaseCalendar> GetCalendarsForUser(Guid userId, UserManager userManager)
    {
        var cals = new List<BaseCalendar>();
        foreach (var h in _calendarProviders)
        {
            var list = h(userId);
            if (list != null)
            {
                cals.AddRange(list.FindAll(c => c.SharingOptions.PublicForItem(userId, userManager)));
            }
        }

        cals.AddRange(_calendars.FindAll(c => c.SharingOptions.PublicForItem(userId, userManager)));
        return cals;
    }

}
