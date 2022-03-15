namespace ASC.Web.Core.Calendars
{
    public abstract class BaseEvent : IEvent, ICloneable
    {
        public virtual TimeZoneInfo TimeZone { get; set; }

        protected BaseEvent()
        {
            this.Context = new EventContext();
            this.AlertType = EventAlertType.Never;
            this.SharingOptions = new SharingOptions();
            this.RecurrenceRule = new RecurrenceRule();
        }

        #region IEvent Members

        public SharingOptions SharingOptions { get; set; }

        public virtual EventAlertType AlertType { get; set; }

        public virtual bool AllDayLong { get; set; }

        public virtual string CalendarId { get; set; }

        public virtual string Description { get; set; }

        public virtual string Id { get; set; }

        public virtual string Uid { get; set; }

        public virtual string Name { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual DateTime UtcEndDate { get; set; }

        public virtual DateTime UtcStartDate { get; set; }

        public virtual DateTime UtcUpdateDate { get; set; }

        public virtual EventContext Context { get; set; }

        public virtual RecurrenceRule RecurrenceRule { get; set; }

        public virtual EventStatus Status { get; set; }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var e = (BaseEvent)this.MemberwiseClone();
            e.Context = (EventContext)this.Context.Clone();
            e.RecurrenceRule = (RecurrenceRule)this.RecurrenceRule.Clone();
            e.SharingOptions = (SharingOptions)this.SharingOptions.Clone();
            return e;
        }

        #endregion


        #region IiCalFormatView Members

        public virtual string ToiCalFormat()
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");

            var id = string.IsNullOrEmpty(Uid) ? Id : Uid;

            sb.AppendLine($"UID:{id}");
            sb.AppendLine($"SUMMARY:{Name}");

            if (!string.IsNullOrEmpty(this.Description))
                sb.AppendLine($"DESCRIPTION:{Description.Replace("\n", "\\n")}");

            if (this.AllDayLong)
            {
                DateTime startDate = this.UtcStartDate, endDate = this.UtcEndDate;
                if (this.TimeZone != null)
                {
                    if (this.UtcStartDate != DateTime.MinValue && startDate.Kind == DateTimeKind.Utc)
                        startDate = startDate.Add(TimeZone.GetOffset());

                    if (this.UtcEndDate != DateTime.MinValue && endDate.Kind == DateTimeKind.Utc)
                        endDate = endDate.Add(TimeZone.GetOffset());
                }

                if (this.UtcStartDate != DateTime.MinValue)
                {
                    var start = startDate.ToString("yyyyMMdd");
                    sb.AppendLine($"DTSTART;VALUE=DATE:{start}");
                }

                if (this.UtcEndDate != DateTime.MinValue)
                {
                    var end = endDate.AddDays(1).ToString("yyyyMMdd");
                    sb.AppendLine($"DTEND;VALUE=DATE:{end}");
                }
            }
            else
            {
                if (this.UtcStartDate != DateTime.MinValue)
                {
                    var utcStart = UtcStartDate.ToString("yyyyMMdd'T'HHmmss'Z'");
                    sb.AppendLine($"DTSTART:{utcStart}");
                }

                if (this.UtcEndDate != DateTime.MinValue)
                {
                    var utcEnd = UtcEndDate.ToString("yyyyMMdd'T'HHmmss'Z'");
                    sb.AppendLine($"DTEND:{utcEnd}");
                }
            }


            if (this.RecurrenceRule != null)
                sb.AppendLine(this.RecurrenceRule.ToiCalFormat());

            sb.Append("END:VEVENT");
            return sb.ToString();
        }

        #endregion

    }
}
