using System;
using System.Collections.Generic;

namespace ASC.People.Models
{
    public class UpdateMembersModel
    {
        public IEnumerable<Guid> UserIds { get; set; }
    }
}
