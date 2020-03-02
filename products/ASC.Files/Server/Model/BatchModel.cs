using System.Collections.Generic;

using ASC.Api.Collections;
using ASC.Web.Files.Services.WCFService.FileOperations;

namespace ASC.Files.Model
{
    public class BaseBatchModel
    {
        public IEnumerable<string> FolderIds { get; set; }
        public IEnumerable<string> FileIds { get; set; }
    }

    public class DownloadModel : BaseBatchModel
    {
        public IEnumerable<ItemKeyValuePair<string, string>> FileConvertIds { get; set; }
    }

    public class DeleteBatchModel : BaseBatchModel
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class BatchModel : BaseBatchModel
    {
        public string DestFolderId { get; set; }
        public FileConflictResolveType ConflictResolveType { get; set; }
        public bool DeleteAfter { get; set; }
    }
}
