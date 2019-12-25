using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_data")]
    public class ResData
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string Title { get; set; }
        public string CultureTitle { get; set; }
        public string TextValue { get; set; }
        public string Description { get; set; }
        public DateTime TimeChanges { get; set; }
        public string ResourceType { get; set; }
        public int Flag { get; set; }
        public string Link { get; set; }
        public string AuthorLogin { get; set; }
    }
}
