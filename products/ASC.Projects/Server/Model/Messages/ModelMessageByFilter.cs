using System;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Messages
{
    public class ModelMessageByFilter
    {
        public int ProjectId { get; set; }
        public int Tag { get; set; }
        public Guid Departament { get; set; }
        public Guid Participant { get; set; }
        public ApiDateTime CreatedStart { get; set; }
        public ApiDateTime CreatedStop { get; set; }
        public int LastId { get; set; }
        public bool MyProjects { get; set; }
        public bool Follow { get; set; }
        public MessageStatus? Status { get; set; }
    }
}
