using ASC.Data.Storage.Encryption;

namespace ASC.Web.Api.Models
{
    public class EncryptionSettingsModel
    {
        public string Password { get; set; }

        public EncryprtionStatus Status { get; set; }

        public bool NotifyUsers { get; set; }

        public string ServerRootPath { get; set; }
    }
}
