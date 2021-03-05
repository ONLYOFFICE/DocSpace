using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.CRM.Core.Enums;

namespace ASC.CRM.Model
{
    public class SetAccessToBatchContactByFilterInDto
    {
        public IEnumerable<String> Tags { get; set; }
        public int? ContactStage { get; set; }
        public int? ContactType { get; set; }
        public  ContactListViewType ContactListView { get; set; }
        public ApiDateTime FromDate { get; set; }
        public  ApiDateTime ToDate { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> ManagerList { get; set; }
    }

    public class SetAccessToBatchContactInDto
    {
        public IEnumerable<int> ContactID { get; set; }

        public bool isShared { get; set; }

        public IEnumerable<Guid> ManagerList { get; set; }
    }
}
