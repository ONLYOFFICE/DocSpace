using System;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Milestones
{
    public class ModelMilestoneByFilter
    {
        public int ProjectId { get; set; }
        public int Tag{ get; set; }
        public MilestoneStatus? Status{ get; set; }
        public ApiDateTime DeadlineStart{ get; set; }
        public ApiDateTime DeadlineStop{ get; set; }
        public Guid? TaskResponsible{ get; set; }
        public int LastId{ get; set; }
        public bool MyProjects{ get; set; }
        public Guid MilestoneResponsible { get; set; }
    }
}
