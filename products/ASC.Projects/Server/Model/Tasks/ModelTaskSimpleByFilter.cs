using System;
using System.Threading.Tasks;

using ASC.Api.Core;

namespace ASC.Projects.Model.Tasks
{
    public class ModelTaskSimpleByFilter
    {
        public int ProjectId { get; set; }
        public bool MyProjects { get; set; }
        public int? Milestone { get; set; }
        public bool MyMilestones { get; set; }
        public int Tag { get; set; }
        public TaskStatus? Status { get; set; }
        public bool Follow { get; set; }
        public Guid Departament { get; set; }
        public Guid? Participant { get; set; }
        public Guid Creator { get; set; }
        public ApiDateTime DeadlineStart { get; set; }
        public ApiDateTime DeadlineStop { get; set; }
        public int LastId { get; set; }
    }
}
