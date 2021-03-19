using System;
using System.Collections.Generic;

using ASC.Api.Collections;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateCasesRequestDto
    {
        public string Title { get; set; }
        public IEnumerable<int> Members { get; set; }
        public IEnumerable<ItemKeyValuePair<int, string>> CustomFieldList { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> accessList { get; set; }
        public bool isNotify { get; set; }
    }
}
