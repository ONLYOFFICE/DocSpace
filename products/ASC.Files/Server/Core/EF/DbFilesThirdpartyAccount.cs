using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_account")]
    public class DbFilesThirdpartyAccount : BaseEntity, IDbFile, IDbSearch
    {
        public int Id { get; set; }

        public string Provider { get; set; }

        [Column("customer_title")]
        public string Title { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("folder_type")]
        public FolderType FolderType { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        public string Url { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public override object[] GetKeys() => new object[] { Id };
    };
}
