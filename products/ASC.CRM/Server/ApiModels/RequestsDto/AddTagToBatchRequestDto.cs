using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class AddTagToBatchRequestDto
    {
        public IEnumerable<int> Entityid { get; set; }
        public string TagName { get; set; }
    }
}
