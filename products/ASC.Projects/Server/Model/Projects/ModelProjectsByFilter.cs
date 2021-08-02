using System;

using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Projects
{
    public class ModelProjectsByFilter
    {
        public int Tag{get; set;} 
        public ProjectStatus? Status{get; set;}
        public Guid Participant{get; set;}
        public Guid Manager{get; set;}
        public Guid Departament{get; set;} 
        public bool Follow { get; set; }
    }
}
