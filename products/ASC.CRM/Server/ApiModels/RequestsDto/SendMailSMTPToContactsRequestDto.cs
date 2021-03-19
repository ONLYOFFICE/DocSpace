using System;
using System.Collections.Generic;

namespace ASC.CRM.ApiModels
{
    public class SendMailSMTPToContactsRequestDto
    {
        public List<int> FileIDs { get; set; }
        public List<int> ContactIds { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public bool StoreInHistory { get; set; }
    }
}
