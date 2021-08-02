using System;
using System.Collections.Generic;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Tasks
{
    public class ModelUpdateProjectTask
    {
        public string Description { get; set; }
        public ApiDateTime Deadline { get; set; }
        public ApiDateTime StartDate { get; set; }
        public TaskPriority? Priority { get; set; }
        public string Title { get; set; }
        public int Milestoneid { get; set; }
        public IEnumerable<Guid> Responsibles { get; set; }
        public int? ProjectID { get; set; }
        public bool Notify { get; set; }
        public TaskStatus? Status { get; set; }
        public int? Progress { get; set; }
    }
}
