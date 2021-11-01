using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.CRM.Core.Enums;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateContactInfoRequestDto
    {
        public ContactInfoType InfoType { get; set; }
        public string Data { get; set; }
        public bool IsPrimary { get; set; }
        public string Category { get; set; }
    }
}
