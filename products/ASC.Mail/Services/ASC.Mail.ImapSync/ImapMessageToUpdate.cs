using MailKit;

namespace ASC.Mail.ImapSyncService
{
    class ImapMessageToUpdate
    {
        public readonly IMailFolder imapMailFolder;
        public readonly int index;

        public ImapMessageToUpdate(IMailFolder folder, int _index = -1)
        {
            imapMailFolder = folder;
            index = _index;
        }
    }
}
