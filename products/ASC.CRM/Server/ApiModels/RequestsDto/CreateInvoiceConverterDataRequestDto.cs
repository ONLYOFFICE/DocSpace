using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateInvoiceConverterDataRequestDto
    {
        public int InvoiceId { get; set; }
        public string StorageUrl { get; set; }
        public string RevisionId { get; set; }

    }
}
