using System.Collections.Generic;

namespace ASC.CRM.ApiModels
{
    public class AddTagToBatchCasesRequestDto
    {
        public int ContactId { get; set; }
        public bool? isClosed { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string TagName { get; set; }
    }
}
