using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Files.Core.Model;
using ASC.Web.Files.Services.WCFService.FileOperations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

namespace ASC.Files.Model
{
    public static class ModelBindingContextExtension
    {
        internal static bool GetFirstValue(this ModelBindingContext bindingContext, string modelName, out string? firstValue)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                firstValue = valueProviderResult.FirstValue;
                return true;
            }

            firstValue = null;
            return false;
        }

        internal static bool GetBoolValue(this ModelBindingContext bindingContext, string modelName, out bool firstValue)
        {
            if (GetFirstValue(bindingContext, modelName, out var deleteAfterValue) &&
                bool.TryParse(deleteAfterValue, out var deleteAfter))
            {
                firstValue = deleteAfter;
                return true;
            }

            firstValue = false;
            return false;
        }

        internal static List<JsonElement> ParseQuery(this ModelBindingContext bindingContext, string modelName)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                return valueProviderResult.Select(ParseQueryParam).ToList();
            }

            if (modelName.EndsWith("[]", StringComparison.Ordinal))
            {
                return new List<JsonElement>();
            }

            return ParseQuery(bindingContext, $"{modelName}[]");
        }

        public static JsonElement ParseQueryParam(string? data)
        {
            if (int.TryParse(data, out _))
            {
                return JsonSerializer.Deserialize<JsonElement>(data);
            }

            return JsonSerializer.Deserialize<JsonElement>($"\"{data}\"");
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

            result.FileIds = bindingContext.ParseQuery(nameof(result.FileIds));
            result.FolderIds = bindingContext.ParseQuery(nameof(result.FolderIds));

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
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

            var result = new DeleteBatchModel();

            var baseResult = bindingContext.Result.Model as BaseBatchModel;

            if (baseResult == null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            result.FileIds = baseResult.FileIds;
            result.FolderIds = baseResult.FolderIds;

            if (bindingContext.GetBoolValue(nameof(result.DeleteAfter), out var deleteAfter))
            {
                result.DeleteAfter = deleteAfter;
            }

            if (bindingContext.GetBoolValue(nameof(result.Immediately), out var immediately))
            {
                result.Immediately = immediately;
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

            var result = new BatchModel();

            var baseResult = bindingContext.Result.Model as BaseBatchModel;

            if (baseResult == null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            result.FileIds = baseResult.FileIds;
            result.FolderIds = baseResult.FolderIds;

            if (bindingContext.GetBoolValue(nameof(result.DeleteAfter), out var deleteAfter))
            {
                result.DeleteAfter = deleteAfter;
            }

            if (bindingContext.GetFirstValue(nameof(result.ConflictResolveType), out var сonflictResolveTypeValue))
            {
                if (Enum.TryParse<FileConflictResolveType>(сonflictResolveTypeValue, out var conflictResolveType))
                {
                    result.ConflictResolveType = conflictResolveType;
                }
            }

            if (bindingContext.GetFirstValue(nameof(result.DestFolderId), out var firstValue))
            {
                result.DestFolderId = ModelBindingContextExtension.ParseQueryParam(firstValue);
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }

    public class InsertFileModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var defaultBindingContext = bindingContext as DefaultModelBindingContext;
            var composite = bindingContext.ValueProvider as CompositeValueProvider;

            if (defaultBindingContext != null && composite != null && composite.Count == 0)
            {
                bindingContext.ValueProvider = defaultBindingContext.OriginalValueProvider;
            }

            var result = new InsertFileModel();

            if (bindingContext.GetBoolValue(nameof(result.CreateNewIfExist), out var createNewIfExist))
            {
                result.CreateNewIfExist = createNewIfExist;
            }

            if (bindingContext.GetBoolValue(nameof(result.KeepConvertStatus), out var keepConvertStatus))
            {
                result.KeepConvertStatus = keepConvertStatus;
            }

            if (bindingContext.GetFirstValue(nameof(result.Title), out var firstValue))
            {
                result.Title = firstValue;
            }

            bindingContext.HttpContext.Request.EnableBuffering();

            bindingContext.HttpContext.Request.Body.Position = 0;

            result.Stream = new MemoryStream();
            await bindingContext.HttpContext.Request.Body.CopyToAsync(result.Stream);
            result.Stream.Position = 0;

            bindingContext.Result = ModelBindingResult.Success(result);
        }
    }

    public class UploadModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var defaultBindingContext = bindingContext as DefaultModelBindingContext;
            var composite = bindingContext.ValueProvider as CompositeValueProvider;

            if (defaultBindingContext != null && composite != null && composite.Count == 0)
            {
                bindingContext.ValueProvider = defaultBindingContext.OriginalValueProvider;
            }

            var result = new UploadModel();

            if (bindingContext.GetBoolValue(nameof(result.CreateNewIfExist), out var createNewIfExist))
            {
                result.CreateNewIfExist = createNewIfExist;
            }

            if (bindingContext.GetBoolValue(nameof(result.KeepConvertStatus), out var keepConvertStatus))
            {
                result.KeepConvertStatus = keepConvertStatus;
            }

            if (bindingContext.GetBoolValue(nameof(result.StoreOriginalFileFlag), out var storeOriginalFileFlag))
            {
                result.StoreOriginalFileFlag = storeOriginalFileFlag;
            }

            if (bindingContext.GetFirstValue(nameof(result.ContentType), out var contentType))
            {
                if (!string.IsNullOrEmpty(contentType))
                {
                    result.ContentType = new ContentType(contentType);
                }
            }

            if (bindingContext.GetFirstValue(nameof(result.ContentDisposition), out var contentDisposition))
            {
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    result.ContentDisposition = new ContentDisposition(contentDisposition);
                }
            }

            bindingContext.HttpContext.Request.EnableBuffering();

            bindingContext.HttpContext.Request.Body.Position = 0;

            result.Stream = new MemoryStream();
            bindingContext.HttpContext.Request.Body.CopyToAsync(result.Stream).Wait();
            result.Stream.Position = 0;

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
