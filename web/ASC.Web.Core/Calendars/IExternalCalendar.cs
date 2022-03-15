namespace ASC.Web.Core.Calendars
{
    public class CalendarContext : ICloneable
    {
        public delegate string GetString();
        public GetString GetGroupMethod { get; set; }
        public string Group { get { return GetGroupMethod != null ? GetGroupMethod() : ""; } }
        public string HtmlTextColor { get; set; }
        public string HtmlBackgroundColor { get; set; }
        public bool CanChangeTimeZone { get; set; }
        public bool CanChangeAlertType { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public interface ICalendar : IICalFormatView
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        Guid OwnerId { get; }
        EventAlertType EventAlertType { get; }
        List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate);
        SharingOptions SharingOptions { get; }
        TimeZoneInfo TimeZone { get; }

        CalendarContext Context { get; }
    }
}
