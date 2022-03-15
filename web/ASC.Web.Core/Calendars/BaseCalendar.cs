namespace ASC.Web.Core.Calendars
{
    public abstract class BaseCalendar : ICalendar, ICloneable
    {
        protected BaseCalendar(AuthContext authContext, TimeZoneConverter timeZoneConverter)
        {
            this.Context = new CalendarContext();
            this.SharingOptions = new SharingOptions();
            AuthContext = authContext;
            TimeZoneConverter = timeZoneConverter;
        }

        #region ICalendar Members

        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual EventAlertType EventAlertType { get; set; }

        public abstract List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate);
        public abstract List<ITodo> LoadTodos(Guid userId, DateTime utcStartDate, DateTime utcEndDate);

        public virtual SharingOptions SharingOptions { get; set; }

        public virtual TimeZoneInfo TimeZone { get; set; }

        public virtual CalendarContext Context { get; set; }
        protected AuthContext AuthContext { get; }
        protected TimeZoneConverter TimeZoneConverter { get; }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var cal = (BaseCalendar)this.MemberwiseClone();
            cal.Context = (CalendarContext)this.Context.Clone();
            cal.SharingOptions = (SharingOptions)this.SharingOptions.Clone();
            return cal;
        }

        #endregion

        #region IiCalFormatView Members

        public string ToiCalFormat()
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:TeamLab Calendar");
            sb.AppendLine("VERSION:2.0");

            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine($"X-WR-CALNAME:{Name}");
            sb.AppendLine($"X-WR-TIMEZONE:{TimeZoneConverter.WindowsTzId2OlsonTzId(TimeZone.Id)}");
            //tz
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine($"TZID:{TimeZoneConverter.WindowsTzId2OlsonTzId(TimeZone.Id)}");
            sb.AppendLine("END:VTIMEZONE");

            //events
            foreach (var e in LoadEvents(AuthContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue))
            {
                if (e is BaseEvent && e.GetType().GetCustomAttributes(typeof(AllDayLongUTCAttribute), true).Length == 0)
                    (e as BaseEvent).TimeZone = TimeZone;

                sb.AppendLine(e.ToiCalFormat());
            }

            sb.Append("END:VCALENDAR");

            return sb.ToString();
        }

        #endregion
    }
}
