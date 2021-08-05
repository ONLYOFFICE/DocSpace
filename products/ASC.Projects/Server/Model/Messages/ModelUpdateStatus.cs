using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Messages
{
    public class ModelUpdateStatus
    {
        public MessageStatus Status { get; set; }
    }
}
