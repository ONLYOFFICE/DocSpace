using System;
using System.Collections.Generic;

namespace ASC.People.Models
{
    public class GroupModel
    {
        public Guid Groupid { get; set; }
        public Guid GroupManager { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<Guid> Members { get; set; }
    }
}
