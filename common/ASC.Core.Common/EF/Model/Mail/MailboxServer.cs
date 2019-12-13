using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox_server")]
    public class MailboxServer
    {
        public int Id { get; set; }

        [Column("id_provider")]
        public int IdProvider { get; set; }
        public string Type { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

        [Column("socket_type")]
        public string SocketType { get; set; }
        public string UserName { get; set; }
        public string Authentication { get; set; }

        [Column("is_user_data")]
        public bool IsUserData { get; set; }
    }
}
