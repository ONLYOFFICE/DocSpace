
using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class CreateContactInfoRequestDto
    {
        public ContactInfoType InfoType { get; set; }
        public string Data { get; set; }
        public bool IsPrimary { get; set; }
        public string Category { get; set; }
    }
}
