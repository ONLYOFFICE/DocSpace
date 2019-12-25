using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("greylisting_whitelist")]
    public class GreyListingWhiteList
    {
        public string Comment { get; set; }
        public string Source { get; set; }
    }
}
