using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using ASC.Api.Collections;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Http;

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

    public class DownloadModel : BaseBatchModel<JsonElement>
    {
        public IEnumerable<ItemKeyValuePair<JsonElement, string>> FileConvertIds { get; set; }
        public DownloadModel() : base()
        {
            FileConvertIds = new List<ItemKeyValuePair<JsonElement, string>>();
        }
    }

    public class DeleteBatchModel : BaseBatchModel<JsonElement>
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class DeleteModel
    {
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class BatchModel : BaseBatchModel<JsonElement>
    {
        public JsonElement DestFolderId { get; set; }
        public FileConflictResolveType ConflictResolveType { get; set; }
        public bool DeleteAfter { get; set; }

        public static BatchModel FromQuery(HttpContext httpContext)
        {
            var result = new BatchModel();
            var query = httpContext.Request.Query;

            var destId = query["DestFolderId"];
            if (destId.Any())
            {
                result.DestFolderId = ParseQueryParam(destId.First());
            }

            var conflictResolveType = query["ConflictResolveType"];
            if (conflictResolveType.Any())
            {
                if (Enum.TryParse<FileConflictResolveType>(conflictResolveType.First(), out var crf))
                {
                    result.ConflictResolveType = crf;
                }
            }

            var deleteAfter = query["DeleteAfter"];
            if (deleteAfter.Any())
            {
                if (bool.TryParse(deleteAfter.First(), out var d))
                {
                    result.DeleteAfter = d;
                }
            }

            var fileIdsQuery = query["FileIds"];
            if (fileIdsQuery.Any())
            {
                var fileIds = new List<JsonElement>();

                foreach (var f in fileIdsQuery)
                {
                    fileIds.Add(ParseQueryParam(f));
                }

                result.FileIds = fileIds;
            }

            var folderIdsQuery = query["FolderIds"];
            if (folderIdsQuery.Any())
            {
                var folderIds = new List<JsonElement>();

                foreach (var f in folderIdsQuery)
                {
                    folderIds.Add(ParseQueryParam(f));
                }

                result.FolderIds = folderIds;
            }

            return result;
        }

        public static JsonElement ParseQueryParam(string data)
        {
            if (int.TryParse(data, out _))
            {
                return JsonSerializer.Deserialize<JsonElement>(data);
            }

            return JsonSerializer.Deserialize<JsonElement>($"\"{data}\"");
        }
    }
}
