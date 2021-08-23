using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class RelationshipEventCreateTextFileRequestDto
    {
        public String Title { get; set; }
        public String Content { get; set; }
    }
}
