using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

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
