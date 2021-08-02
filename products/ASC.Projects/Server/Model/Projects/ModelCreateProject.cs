using System;
using System.Collections.Generic;

namespace ASC.Projects.Model.Projects
{
    public class ModelCreateProject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ResponsibleId { get; set; }
        public string Tags { get; set; }
        public bool @Private {get; set;}
        public IEnumerable<Guid> Participants { get; set; }
        public bool? Notify { get; set; }
        public IEnumerable<ASC.Projects.Core.Domain.Task> Tasks { get; set; }
        public IEnumerable<ASC.Projects.Core.Domain.Milestone> Milestones { get; set; }
        public bool? NotifyResponsibles { get; set; }
    }
}
