using System;
using System.Collections.Generic;

using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Projects
{
    public class ModelUpdateProject
    {
        public string Title{get; set;}
        public string Description{get; set;} 
        public Guid ResponsibleId{get; set;}
        public string Tags{get; set;}
        public IEnumerable<Guid> Participants{get; set;} 
        public ProjectStatus? Status{get; set;}
        public bool? @Private{get; set;}
        public bool Notify { get; set; }
    }
}
