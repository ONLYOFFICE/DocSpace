using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.CRM.ApiModels
{
    public class SetAccessToBatchCasesByFilterInDto
    {
        public int Contactid { get; set; }
        public bool? isClosed { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> AccessList { get; set; }

    }

    public class SetAccessToBatchCasesInDto
    {
        public IEnumerable<int> CasesId { get; set; }
        public bool isPrivate { get; set; }
        public IEnumerable<Guid> AccessList { get; set; }
    }
}
