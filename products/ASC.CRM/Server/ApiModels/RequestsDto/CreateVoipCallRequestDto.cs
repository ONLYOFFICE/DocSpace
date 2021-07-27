using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.VoipService;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class CreateVoipCallRequestDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public Guid AnsweredBy { get; set; }
        public VoipCallStatus? Status { get; set; }
        public string ContactId { get; set; }
        public decimal? Price { get; set; }
    }
}
