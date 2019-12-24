using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox")]
    public class Mailbox
    {
        public int Id { get; set; }
        public int Tenant { get; set; }

        [Column("id_user")]
        public string IdUser { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        [Column("is_removed")]
        public bool IsRemoved { get; set; }

        [Column("is_processed")]
        public bool IsProcessed { get; set; }

        [Column("is_server_mailbox")]
        public bool IsServerMailbox { get; set; }

        [Column("is_teamlab_mailbox")]
        public bool IsTeamlabMailbox { get; set; }

        public bool Imap { get; set; }

        [Column("user_online")]
        public bool UserOnline { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("msg_count_last")]
        public int MsgCountLast { get; set; }

        [Column("size_last")]
        public int SizeLast { get; set; }

        [Column("login_delay")]
        public int LoginDelay { get; set; }

        [Column("quota_error")]
        public bool QuotaError { get; set; }

        [Column("imap_intervals")]
        public string ImapIntervals { get; set; }

        [Column("begin_date")]
        public DateTime BeginDate { get; set; }

        [Column("email_in_folder")]
        public string EmailInFolder { get; set; }

        [Column("pop3_password")]
        public string Pop3Password { get; set; }

        [Column("smtp_password")]
        public string SmtpPassword { get; set; }

        [Column("token_type")]
        public int TokenType { get; set; }
        public string Token { get; set; }

        [Column("id_smtp_server")]
        public int IdSmtpServer { get; set; }

        [Column("id_in_server")]
        public int IdInServer { get; set; }

        [Column("date_checked")]
        public DateTime DateChecked { get; set; }

        [Column("date_user_checked")]
        public DateTime DateUserChecked { get; set; }

        [Column("date_login_delay_expires")]
        public DateTime DateLoginDelayExpires { get; set; }

        [Column("date_auth_error")]
        public DateTime? DateAuthError { get; set; }

        [Column("date_created")]
        public DateTime DateCreated { get; set; }

        [Column("date_modified")]
        public DateTime DateModified { get; set; }
    }
}
