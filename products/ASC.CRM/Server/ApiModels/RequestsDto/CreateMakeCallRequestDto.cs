using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateMakeCallRequestDto
    {
        public string To { get; set; }
        public string ContactId { get; set; }
    }
}
