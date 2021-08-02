using System;
using System.Collections.Generic;

namespace ASC.Projects.Model.Projects
{
    public class ModelUpdateTeam
    {
        public IEnumerable<Guid> Participants { get; set; }
        public bool Notify { get; set; }
    }
}
