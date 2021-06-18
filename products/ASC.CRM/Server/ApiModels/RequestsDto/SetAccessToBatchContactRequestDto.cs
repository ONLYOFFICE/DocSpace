using System;
using System.Collections.Generic;

using ASC.Api.Core;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.ApiModels
{
    public class SetAccessToBatchContactByFilterRequestDto
    {
        public IEnumerable<String> Tags { get; set; }
        public int? ContactStage { get; set; }
        public int? ContactType { get; set; }
        public ContactListViewType ContactListView { get; set; }
        public ApiDateTime FromDate { get; set; }
        public ApiDateTime ToDate { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> ManagerList { get; set; }
    }

    public class SetAccessToBatchContactRequestDto
    {
        public IEnumerable<int> ContactID { get; set; }

        public bool isShared { get; set; }

        public IEnumerable<Guid> ManagerList { get; set; }
    }
}
