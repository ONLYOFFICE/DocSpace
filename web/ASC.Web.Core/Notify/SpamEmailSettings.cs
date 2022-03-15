namespace ASC.Web.Studio.Core.Notify
{
    [Serializable]
    public class SpamEmailSettings : ISettings
    {
        public int MailsSendedCount { get; set; }

        public DateTime MailsSendedDate { get; set; }

        public Guid ID
        {
            get { return new Guid("{A9819A62-60AF-48E3-989C-08259772FA57}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new SpamEmailSettings
            {
                MailsSendedCount = 0,
                MailsSendedDate = DateTime.UtcNow.AddDays(-2)
            };
        }

        public int MailsSended
        {
            get { return GetCount(); }
            set
            {
                MailsSendedDate = DateTime.UtcNow.Date;
                MailsSendedCount = value;
            }
        }

        private int GetCount()
        {
            return MailsSendedDate.Date < DateTime.UtcNow.Date
                       ? 0
                       : MailsSendedCount;
        }
    }
}