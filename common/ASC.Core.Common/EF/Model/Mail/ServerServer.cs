using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_server_server")]
    public class ServerServer
    {
        public int Id { get; set; }

        [Column("mx_record")]
        public string MxRecord { get; set; }

        [Column("connection_string")]
        public string ConnectionString { get; set; }

        [Column("server_type")]
        public int ServerType { get; set; }

        [Column("smtp_settings_id")]
        public int SmtpSettingsId { get; set; }

        [Column("imap_settings_id")]
        public int ImapSettingsId { get; set; }
    }
}
