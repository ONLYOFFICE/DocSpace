using System.Collections.Generic;

using ASC.Api.Collections;

namespace ASC.Web.Api.Models
{
    public class WebItemSecurityModel
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<Guid> Subjects { get; set; }
        public IEnumerable<ItemKeyValuePair<string, bool>> Items { get; set; }
    }
}
