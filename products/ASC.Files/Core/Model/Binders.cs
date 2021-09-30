using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASC.Files.Model
{
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

                return valueProviderResult.Select(ParseQueryParam).ToList();
            }

            if (modelName.EndsWith("[]"))
            {
                return new List<JsonElement>();
            }

            return ParseQuery(bindingContext, $"{modelName}[]");
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

    public class BatchModelBinder : BaseBatchModelBinder
    {
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            base.BindModelAsync(bindingContext);
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var baseResult = bindingContext.Result.Model as BaseBatchModel;

            var result = new BatchModel();

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

            modelName = nameof(result.ConflictResolveType);
            valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                if (Enum.TryParse<FileConflictResolveType>(valueProviderResult.FirstValue, out var conflictResolveType))
                {
                    result.ConflictResolveType = conflictResolveType;
                }
            }

            modelName = nameof(result.DestFolderId);
            valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                result.DestFolderId = BaseBatchModelBinder.ParseQueryParam(valueProviderResult.FirstValue);
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
