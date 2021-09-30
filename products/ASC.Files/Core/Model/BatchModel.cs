using System.Collections.Generic;
using System.Text.Json;

using ASC.Api.Collections;
using ASC.Web.Files.Services.WCFService.FileOperations;

namespace ASC.Files.Model
{
    public class BaseBatchModel
    {
        public IEnumerable<JsonElement> FolderIds { get; set; }
        public IEnumerable<JsonElement> FileIds { get; set; }
        public BaseBatchModel()
        {
            FolderIds = new List<JsonElement>();
            FileIds = new List<JsonElement>();
        }
    }

    public class DownloadModel : BaseBatchModel
    {
        public IEnumerable<ItemKeyValuePair<JsonElement, string>> FileConvertIds { get; set; }
        public DownloadModel() : base()
        {
            FileConvertIds = new List<ItemKeyValuePair<JsonElement, string>>();
        }
    }

    public class DeleteBatchModel : BaseBatchModel
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class DeleteModel
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class BatchModel : BaseBatchModel
    {
        public JsonElement DestFolderId { get; set; }
        public FileConflictResolveType ConflictResolveType { get; set; }
        public bool DeleteAfter { get; set; }
    }
}
