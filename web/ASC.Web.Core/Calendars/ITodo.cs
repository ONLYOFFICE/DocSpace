namespace ASC.Web.Core.Calendars
{

    public class TodoContext : ICloneable
    {
        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public interface ITodo : IICalFormatView
    {
        string Id { get; }
        string Uid { get; }
        string CalendarId { get; }
        string Name { get; }
        string Description { get; }
        Guid OwnerId { get; }
        DateTime UtcStartDate { get; }
        DateTime Completed { get; }
    }
}
