using System;

using ASC.Api.Core;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Milestones
{
    public class ModelMilestoneUpdate
    {
        public string Title{ get; set; } 
        public ApiDateTime Deadline{ get; set; } 
        public bool? IsKey{ get; set; } 
        public MilestoneStatus Status{ get; set; } 
        public bool? IsNotify{ get; set; }
        public string Description{ get; set; }
        public int ProjectID{ get; set; }
        public Guid Responsible{ get; set; } 
        public bool NotifyResponsible { get; set; }
    }
}
