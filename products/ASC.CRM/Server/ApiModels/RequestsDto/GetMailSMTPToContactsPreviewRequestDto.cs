using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class GetMailSMTPToContactsPreviewRequestDto
    {
        public String Template { get; set; }
        public int ContactId { get; set; }
    }
}
