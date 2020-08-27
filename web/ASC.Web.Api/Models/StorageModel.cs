using System.Collections.Generic;

using ASC.Api.Collections;

namespace ASC.Web.Api.Models
{
    public class StorageModel
    {
        public string Module { get; set; }
        public IEnumerable<ItemKeyValuePair<string, string>> Props { get; set; }
    }
}
