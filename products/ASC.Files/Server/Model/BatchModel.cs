using System.Collections.Generic;

using ASC.Api.Collections;
using ASC.Web.Files.Services.WCFService.FileOperations;

namespace ASC.Files.Model
{
    public class BaseBatchModel<T>
    {
        public IEnumerable<T> FolderIds { get; set; }
        public IEnumerable<T> FileIds { get; set; }
        public BaseBatchModel()
        {
            FolderIds = new List<T>();
            FileIds = new List<T>();
        }
    }

    public class DownloadModel : BaseBatchModel<object>
    {
        public IEnumerable<ItemKeyValuePair<string, string>> FileConvertIds { get; set; }
        public DownloadModel() : base()
        {
            FileConvertIds = new List<ItemKeyValuePair<string, string>>();
        }
    }

    public class DeleteBatchModel : BaseBatchModel<object>
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class BatchModel : BaseBatchModel<object>
    {
        public object DestFolderId { get; set; }
        public FileConflictResolveType ConflictResolveType { get; set; }
        public bool DeleteAfter { get; set; }
    }
}
