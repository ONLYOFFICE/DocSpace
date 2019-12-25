using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("api_keys")]
    public class ApiKeys
    {
        public int Id { get; set; }

        [Column("access_token")]
        public string AccessToken { get; set; }
    }
}
