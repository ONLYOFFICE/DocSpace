using System;

using ASC.VoipService;

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
