using System.Collections.Generic;
using System.Text.Json;

using ASC.Api.Documents;

namespace ASC.Files.Model
{
    public class SecurityInfoModel : BaseBatchModel<JsonElement>
    {
        public IEnumerable<FileShareParams> Share { get; set; }
        public bool Notify { get; set; }
        public string SharingMessage { get; set; }
    }
}
