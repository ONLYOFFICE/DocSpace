using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit;

namespace ASC.Mail.ImapSync
{
    public class MessageDescriptor
    {
        public MessageFlags? Flags;
        public int Index;
        public UniqueId UniqueId;
        public DateTimeOffset? InternalDate;

        public MessageDescriptor(IMessageSummary message)
        {
            Flags = message.Flags;
            Index = message.Index;
            UniqueId = message.UniqueId;
            InternalDate = message.InternalDate;
        }
    }

    public static class MessageDescriptorExtention
    {
        public static List<MessageDescriptor> ToMessageDescriptorList(this IList<IMessageSummary> messages)
        {
            List<MessageDescriptor> result = messages.Select(x => new MessageDescriptor(x)).ToList();

            return result;
        }
    }
}
