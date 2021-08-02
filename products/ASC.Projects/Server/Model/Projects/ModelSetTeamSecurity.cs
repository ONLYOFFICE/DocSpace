using System;

using ASC.Projects.Core.Domain;

namespace ASC.Projects.Model.Projects
{
    public class ModelSetTeamSecurity
    {
        public Guid UserId { get; set; }
        public ProjectTeamSecurity Security { get; set; }
        public bool Visible { get; set; }
    }
}
