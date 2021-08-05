using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Core.Common.Caching;

using MailKit;

namespace ASC.Mail.ImapSync
{
    public class ImapAction
    {
        public IMailFolder Folder;
        public UniqueId UniqueId;
        public MailUserAction FolderAction;
    }
}
