using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Mail.ImapSync
{
    public enum ClientState
    {
        Creating,
        Disconected,
        UpdateMessagesList,
        SetFlags,
        MoveMessages,
        ChangeFolder,
        Idle
    }
}
