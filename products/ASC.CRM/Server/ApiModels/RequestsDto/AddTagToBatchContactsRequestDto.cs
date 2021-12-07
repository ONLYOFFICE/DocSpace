using System.Collections.Generic;

using ASC.Api.Core;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class AddTagToBatchContactsRequestDto
    {
        public IEnumerable<string> Tags { get; set; }
        public int ContactStage { get; set; }
        public int ContactType { get; set; }
        public ContactListViewType ContactListView { get; set; }
        public ApiDateTime FromDate { get; set; }
        public ApiDateTime ToDate { get; set; }
        public string TagName { get; set; }
    }
}
