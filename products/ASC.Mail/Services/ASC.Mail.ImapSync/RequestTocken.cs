using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Mail.ImapSync
{
    public class RequestTocken
    {
        public readonly ClientState RequestState;

        public RequestTocken(ClientState requestState)
        {
            RequestState= requestState;
        }
    }
}
