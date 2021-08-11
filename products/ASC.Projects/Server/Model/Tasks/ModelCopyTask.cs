using System;
using System.Collections.Generic;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Tasks
{
    public class ModelCopyTask
    {
        public int ProjectId { get; set; }
        public string Description { get; set; }
        public ApiDateTime Deadline { get; set; }
        public TaskPriority Priority { get; set; }
        public string Title { get; set; }
        public int Milestoneid { get; set; }
        public IEnumerable<Guid> Responsibles { get; set; }
        public bool Notify { get; set; }
        public ApiDateTime StartDate { get; set; }
        public bool CopySubtasks { get; set; }
        public bool CopyFiles { get; set; }
        public bool RemoveOld { get; set; }
    }
}
