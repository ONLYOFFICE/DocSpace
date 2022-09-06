// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

#nullable enable

namespace ASC.Files.Core.ApiModels;

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

    internal static IEnumerable<ItemKeyValuePair<JsonElement, string>> ParseDictionary(this ModelBindingContext bindingContext, string modelName)
    {
        var result = new List<ItemKeyValuePair<JsonElement, string>>();

        for (var i = 0; ; i++)
        {
            var keyProviderResult = bindingContext.ValueProvider.GetValue($"{modelName}[{i}][key]");
            var valueProviderResult = bindingContext.ValueProvider.GetValue($"{modelName}[{i}][value]");

            if (keyProviderResult != ValueProviderResult.None && valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, keyProviderResult);
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                if (!string.IsNullOrEmpty(keyProviderResult.FirstValue) && !string.IsNullOrEmpty(valueProviderResult.FirstValue))
                {
                    result.Add(new ItemKeyValuePair<JsonElement, string> { Key = ParseQueryParam(keyProviderResult.FirstValue), Value = valueProviderResult.FirstValue });
                }
            }
            else
            {
                break;
            }
        }

        return result;
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
        ArgumentNullException.ThrowIfNull(bindingContext);

        var result = new BaseBatchRequestDto();

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
        ArgumentNullException.ThrowIfNull(bindingContext);

        var result = new DeleteBatchRequestDto();

        var baseResult = bindingContext.Result.Model as BaseBatchRequestDto;

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

public class DownloadModelBinder : BaseBatchModelBinder
{
    public override Task BindModelAsync(ModelBindingContext bindingContext)
    {
        base.BindModelAsync(bindingContext);
        ArgumentNullException.ThrowIfNull(bindingContext);

        var result = new DownloadRequestDto();

        var baseResult = bindingContext.Result.Model as BaseBatchRequestDto;

        if (baseResult == null)
        {
            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }

        result.FileIds = baseResult.FileIds;
        result.FolderIds = baseResult.FolderIds;
        result.FileConvertIds = bindingContext.ParseDictionary(nameof(result.FileConvertIds));

        bindingContext.Result = ModelBindingResult.Success(result);

        return Task.CompletedTask;
    }
}

public class BatchModelBinder : BaseBatchModelBinder
{
    public override Task BindModelAsync(ModelBindingContext bindingContext)
    {
        base.BindModelAsync(bindingContext);
        ArgumentNullException.ThrowIfNull(bindingContext);

        var result = new BatchRequestDto();

        var baseResult = bindingContext.Result.Model as BaseBatchRequestDto;

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
            if (FileConflictResolveTypeExtensions.TryParse(сonflictResolveTypeValue, out var conflictResolveType))
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
        ArgumentNullException.ThrowIfNull(bindingContext);

        var defaultBindingContext = bindingContext as DefaultModelBindingContext;
        var composite = bindingContext.ValueProvider as CompositeValueProvider;

        if (defaultBindingContext != null && composite != null && composite.Count == 0)
        {
            bindingContext.ValueProvider = defaultBindingContext.OriginalValueProvider;
        }

        var result = new InsertFileRequestDto();

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
        ArgumentNullException.ThrowIfNull(bindingContext);

        var defaultBindingContext = bindingContext as DefaultModelBindingContext;
        var composite = bindingContext.ValueProvider as CompositeValueProvider;

        if (defaultBindingContext != null && composite != null && composite.Count == 0)
        {
            bindingContext.ValueProvider = defaultBindingContext.OriginalValueProvider;
        }

        var result = new UploadRequestDto();

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
