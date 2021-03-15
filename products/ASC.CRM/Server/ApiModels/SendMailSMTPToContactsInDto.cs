using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.CRM.ApiModels
{
    public class SendMailSMTPToContactsInDto
    {
        public List<int> FileIDs { get; set; }
        public List<int> ContactIds { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public bool StoreInHistory { get; set; }
    }
}
