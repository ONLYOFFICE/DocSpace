using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox_provider")]
    public class MailboxProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("display_short_name")]
        public string DisplayShortName { get; set; }
        public string Documentation { get; set; }
    }
}
