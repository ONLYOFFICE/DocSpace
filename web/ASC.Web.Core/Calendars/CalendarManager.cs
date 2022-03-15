namespace ASC.Web.Core.Calendars
{
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
                    this._calendars.Add(calendar);
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
                    this._calendarProviders.Add(provider);
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
                    cals.AddRange(list.FindAll(c => c.SharingOptions.PublicForItem(userId, userManager)));
            }

            cals.AddRange(_calendars.FindAll(c => c.SharingOptions.PublicForItem(userId, userManager)));
            return cals;
        }

    }
}
