using System;

namespace ASC.CRM.ApiModels
{
    public class GetMailSMTPToContactsPreviewRequestDto
    {
        public String Template { get; set; }
        public int ContactId { get; set; }
    }
}
