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

using System.IO;

using ASC.Common.Web;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace ASC.Data.Storage.DiscStorage;

public class StorageHandler
{
    private readonly string _path;
    private readonly string _module;
    private readonly string _domain;
    private readonly bool _checkAuth;

    public StorageHandler(string path, string module, string domain, bool checkAuth = true)
    {
        _path = path;
        _module = module;
        _domain = domain;
        _checkAuth = checkAuth;
    }

    public async ValueTask InvokeAsync(HttpContext context, TenantManager tenantManager, SecurityContext securityContext, StorageFactory storageFactory, EmailValidationKeyProvider emailValidationKeyProvider)
    {
        var storage = await storageFactory.GetStorageAsync((await tenantManager.GetCurrentTenantAsync()).Id, _module);
        var path = CrossPlatform.PathCombine(_path, GetRouteValue("pathInfo", context).Replace('/', Path.DirectorySeparatorChar));
        var header = context.Request.Query[Constants.QueryHeader].FirstOrDefault() ?? "";
        var auth = context.Request.Query[Constants.QueryAuth].FirstOrDefault() ?? "";
        var storageExpire = storage.GetExpire(_domain);

        if (_checkAuth && !securityContext.IsAuthenticated && !await SecureHelper.CheckSecureKeyHeader(header, path, emailValidationKeyProvider) 
            || _module == "backup" && !securityContext.IsAuthenticated)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        if (storageExpire != TimeSpan.Zero && storageExpire != TimeSpan.MinValue && storageExpire != TimeSpan.MaxValue || !string.IsNullOrEmpty(auth))
        {
            var expire = context.Request.Query[Constants.QueryExpire];
            if (string.IsNullOrEmpty(expire))
            {
                expire = storageExpire.TotalMinutes.ToString(CultureInfo.InvariantCulture);
            }

            var validateResult = await emailValidationKeyProvider.ValidateEmailKeyAsync(path + "." + header + "." + expire, auth ?? "", TimeSpan.FromMinutes(Convert.ToDouble(expire)));
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }

        if (!await storage.IsFileAsync(_domain, path))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        var headers = header.Length > 0 ? header.Split('&').Select(HttpUtility.UrlDecode) : Array.Empty<string>();

        const int bigSize = 5 * 1024 * 1024;
        var fileSize = await storage.GetFileSizeAsync(_domain, path);

        if (storage.IsSupportInternalUri && bigSize < fileSize)
        {
            var uri = await storage.GetInternalUriAsync(_domain, path, TimeSpan.FromMinutes(15), headers);

            //TODO
            //context.Response.Cache.SetAllowResponseInBrowserHistory(false);
            //context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            context.Response.Redirect(uri.ToString());
            return;
        }

        //if (!String.IsNullOrEmpty(context.Request.Query["_"]))
        //{
        //    context.Response.Headers["Cache-Control"] = "public, max-age=31536000";
        //}

        //var etag = await storage.GetFileEtagAsync(_domain, path);

        //if (string.Equals(context.Request.Headers["If-None-Match"], etag))
        //{
        //    context.Response.StatusCode = (int)HttpStatusCode.NotModified;

        //    return;
        //}

        //context.Response.Headers.ETag = etag;

        string encoding = null;

        if (storage is DiscDataStore && await storage.IsFileAsync(_domain, path + ".gz"))
        {
            path += ".gz";
            encoding = "gzip";
        }

        var headersToCopy = new List<string> { "Content-Disposition", "Cache-Control", "Content-Encoding", "Content-Language", "Content-Type", "Expires" };
        foreach (var h in headers)
        {
            var toCopy = headersToCopy.Find(x => h.StartsWith(x));
            if (string.IsNullOrEmpty(toCopy))
            {
                continue;
            }

            context.Response.Headers[toCopy] = h.Substring(toCopy.Length + 1);
        }
                
        try
        {
            context.Response.ContentType = MimeMapping.GetMimeMapping(path);
        }
        catch (Exception)
        {

        }

        if (encoding != null)
        {
            context.Response.Headers["Content-Encoding"] = encoding;
        }


        long offset = 0;
        var length = ProcessRangeHeader(context, fileSize, ref offset);

        context.Response.Headers["Connection"] = "Keep-Alive";
        context.Response.Headers["Content-Length"] = length.ToString(CultureInfo.InvariantCulture);

        await using (var stream = await storage.GetReadStreamAsync(_domain, path, offset))
        {
            var responseBufferingFeature = context.Features.Get<IHttpResponseBodyFeature>();
            responseBufferingFeature?.DisableBuffering();

            await stream.CopyToAsync(context.Response.Body);
        }

        await context.Response.Body.FlushAsync();
        await context.Response.CompleteAsync();
    }

    private long ProcessRangeHeader(HttpContext context, long fullLength, ref long offset)
    {
        if (context == null) throw new ArgumentNullException();
        if (context.Request.Headers["Range"] == StringValues.Empty) return fullLength;

        long endOffset = -1;

        var range = context.Request.Headers["Range"][0].Split(new[] { '=', '-' });
        offset = Convert.ToInt64(range[1]);
        if (range.Count() > 2 && !string.IsNullOrEmpty(range[2]))
        {
            endOffset = Convert.ToInt64(range[2]);
        }
        if (endOffset < 0 || endOffset >= fullLength)
        {
            endOffset = fullLength - 1;
        }

        var length = endOffset - offset + 1;

        if (length <= 0) throw new HttpException(HttpStatusCode.BadRequest, "Wrong Range header");

        if (length < fullLength)
        {
            context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
        }
        context.Response.Headers["Accept-Ranges"] = "bytes";
        context.Response.Headers["Content-Range"] = string.Format(" bytes {0}-{1}/{2}", offset, endOffset, fullLength);

        return length;
    }

    private string GetRouteValue(string name, HttpContext context)
    {
        return (context.GetRouteValue(name) ?? "").ToString();
    }
}

public static class StorageHandlerExtensions
{
    public static IEndpointRouteBuilder RegisterStorageHandler(this IEndpointRouteBuilder builder, string module, string domain, bool publicRoute = false)
    {
        var pathUtils = builder.ServiceProvider.GetService<PathUtils>();
        var virtPath = pathUtils.ResolveVirtualPath(module, domain);
        virtPath = virtPath.TrimStart('/');

        var handler = new StorageHandler(string.Empty, module, domain, !publicRoute);
        var url = virtPath + "{*pathInfo}";

        if (!builder.DataSources.Any(r => r.Endpoints.Any(e => e.DisplayName == url)))
        {
            builder.MapGet(url, handler.InvokeAsync);

            var newUrl = url.Replace("{0}", "{t1}/{t2}/{t3}");

            if (newUrl != url)
            {
                builder.MapGet(url, handler.InvokeAsync);
            }
        }

        return builder;
    }
}
