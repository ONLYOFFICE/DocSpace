using System.Collections.Generic;

namespace ASC.CRM.ApiModels
{
    public class AddTagToBatchRequestDto
    {
        public IEnumerable<int> Entityid { get; set; }
        public string TagName { get; set; }
    }
}
