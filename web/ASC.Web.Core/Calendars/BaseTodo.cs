namespace ASC.Web.Core.Calendars
{
    public abstract class BaseTodo : ITodo, ICloneable
    {
        internal TimeZoneInfo TimeZone { get; set; }

        protected BaseTodo()
        {
            this.Context = new TodoContext();
        }

        #region ITodo Members


        public virtual string CalendarId { get; set; }

        public virtual string Description { get; set; }

        public virtual string Id { get; set; }

        public virtual string Uid { get; set; }

        public virtual string Name { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual DateTime UtcStartDate { get; set; }

        public virtual TodoContext Context { get; set; }

        public virtual DateTime Completed { get; set; }


        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var t = (BaseTodo)this.MemberwiseClone();
            t.Context = (TodoContext)this.Context.Clone();
            return t;
        }

        #endregion


        #region IiCalFormatView Members

        public virtual string ToiCalFormat()
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:TODO");

            var id = string.IsNullOrEmpty(this.Uid) ? this.Id : this.Uid;

            sb.AppendLine($"UID:{id}");
            sb.AppendLine($"SUMMARY:{Name}");

            if (!string.IsNullOrEmpty(this.Description))
                sb.AppendLine($"DESCRIPTION:{Description.Replace("\n", "\\n")}");


            if (this.UtcStartDate != DateTime.MinValue)
            {
                var utcStart = UtcStartDate.ToString("yyyyMMdd'T'HHmmss'Z'");
                sb.AppendLine($"DTSTART:{utcStart}");
            }
            if (this.Completed != DateTime.MinValue)
            {
                var completed = Completed.ToString("yyyyMMdd'T'HHmmss'Z'");
                sb.AppendLine($"COMPLETED:{completed}");
            }

            sb.Append("END:TODO");
            return sb.ToString();
        }

        #endregion

    }
}
