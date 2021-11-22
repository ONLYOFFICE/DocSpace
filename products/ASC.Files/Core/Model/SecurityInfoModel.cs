using System.Collections.Generic;

using ASC.Api.Documents;

namespace ASC.Files.Model
{
    public class SecurityInfoModel : BaseBatchModel
    {
        public IEnumerable<FileShareParams> Share { get; set; }
        public bool Notify { get; set; }
        public string SharingMessage { get; set; }
    }
}
