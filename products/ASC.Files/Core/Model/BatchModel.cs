using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Api.Collections;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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


    public class DeleteBatchModelBinder : BaseBatchModelBinder
    {
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            base.BindModelAsync(bindingContext);
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var baseResult = bindingContext.Result.Model as BaseBatchModel;

            var result = new DeleteBatchModel();

            result.FileIds = baseResult.FileIds;
            result.FolderIds = baseResult.FolderIds;

            var modelName = nameof(result.DeleteAfter);
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                if (bool.TryParse(valueProviderResult.FirstValue, out var deleteAfter))
                {
                    result.DeleteAfter = deleteAfter;
                }
            }

            modelName = nameof(result.Immediately);
            valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                if (bool.TryParse(valueProviderResult.FirstValue, out var immediately))
                {
                    result.Immediately = immediately;
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }

    public class BaseBatchModelBinder : IModelBinder
    {
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var result = new BaseBatchModel();

            result.FileIds = ParseQuery(bindingContext, nameof(result.FileIds));
            result.FolderIds = ParseQuery(bindingContext, nameof(result.FolderIds));

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }

        internal List<JsonElement> ParseQuery(ModelBindingContext bindingContext, string modelName)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                return valueProviderResult.Select(BatchModel.ParseQueryParam).ToList();
            }

            return new List<JsonElement>();
        }
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
