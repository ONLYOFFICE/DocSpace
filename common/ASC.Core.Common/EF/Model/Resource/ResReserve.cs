using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_reserve")]
    public class ResReserve
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string Title { get; set; }
        public string CultureTitle { get; set; }
        public string TextValue { get; set; }
        public int Flag { get; set; }
    }
}
