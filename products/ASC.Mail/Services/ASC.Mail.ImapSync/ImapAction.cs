using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit;

namespace ASC.Mail.ImapSync
{
    public class ImapAction
    {
        public IMailFolder Folder;
        public UniqueId UniqueId;
        public Action FolderAction;

        public enum Action
        {
            Nothing,
            SetAsRead,
            SetAsUnread,
            SetAsImportant,
            SetAsNotImpotant,
            SetAsDeleted,
            RemovedFromFolder,
            New
        }
    }
}
