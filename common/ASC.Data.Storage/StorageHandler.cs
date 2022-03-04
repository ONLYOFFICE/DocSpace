/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Data.Storage.DiscStorage;

public class StorageHandler
{
    private readonly string _path;
    private readonly string _module;
    private readonly string _domain;
    private readonly bool _checkAuth;
    private readonly IServiceProvider _serviceProvider;

    public StorageHandler(IServiceProvider serviceProvider, string path, string module, string domain, bool checkAuth = true)
    {
        _serviceProvider = serviceProvider;
        _path = path;
        _module = module;
        _domain = domain;
        _checkAuth = checkAuth;
    }

    public Task Invoke(HttpContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<StorageHandlerScope>();
        var (tenantManager, securityContext, storageFactory, emailValidationKeyProvider) = scopeClass;

        if (_checkAuth && !securityContext.IsAuthenticated)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        var storage = storageFactory.GetStorage(tenantManager.GetCurrentTenant().Id.ToString(CultureInfo.InvariantCulture), _module);
        var path = CrossPlatform.PathCombine(_path, GetRouteValue("pathInfo", context).Replace('/', Path.DirectorySeparatorChar));
        var header = context.Request.Query[Constants.QueryHeader].FirstOrDefault() ?? "";

        var auth = context.Request.Query[Constants.QueryAuth].FirstOrDefault() ?? "";
        var storageExpire = storage.GetExpire(_domain);

        if (storageExpire != TimeSpan.Zero && storageExpire != TimeSpan.MinValue && storageExpire != TimeSpan.MaxValue || !string.IsNullOrEmpty(auth))
        {
            var expire = context.Request.Query[Constants.QueryExpire];
            if (string.IsNullOrEmpty(expire)) expire = storageExpire.TotalMinutes.ToString(CultureInfo.InvariantCulture);

            var validateResult = emailValidationKeyProvider.ValidateEmailKey(path + "." + header + "." + expire, auth ?? "", TimeSpan.FromMinutes(Convert.ToDouble(expire)));
            if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }
        }

            return InternalInvoke(context, storage, path, header);
        }

        private async Task InternalInvoke(HttpContext context, IDataStore storage, string path, string header)
        {
            if (!await storage.IsFileAsync(_domain, path))
            {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
        }

        var headers = header.Length > 0 ? header.Split('&').Select(HttpUtility.UrlDecode) : Array.Empty<string>();

        if (storage.IsSupportInternalUri)
        {
                var uri = await storage.GetInternalUriAsync(_domain, path, TimeSpan.FromMinutes(15), headers);

            //TODO
            //context.Response.Cache.SetAllowResponseInBrowserHistory(false);
            //context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            context.Response.Redirect(uri.ToString());
                return;
        }

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

        using (var stream = await storage.GetReadStreamAsync(_domain, path))
        {
            await stream.CopyToAsync(context.Response.Body);
        }

        await context.Response.Body.FlushAsync();
        await context.Response.CompleteAsync();
    }

    private string GetRouteValue(string name, HttpContext context)
    {
        return (context.GetRouteValue(name) ?? "").ToString();
    }
}

[Scope]
public class StorageHandlerScope
{
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly StorageFactory _storageFactory;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;

    public StorageHandlerScope(TenantManager tenantManager, SecurityContext securityContext, StorageFactory storageFactory, EmailValidationKeyProvider emailValidationKeyProvider)
    {
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _storageFactory = storageFactory;
        _emailValidationKeyProvider = emailValidationKeyProvider;
    }
    public void Deconstruct(out TenantManager tenantManager, out SecurityContext securityContext, out StorageFactory storageFactory, out EmailValidationKeyProvider emailValidationKeyProvider)
    {
        tenantManager = _tenantManager;
        securityContext = _securityContext;
        storageFactory = _storageFactory;
        emailValidationKeyProvider = _emailValidationKeyProvider;
    }
}

public static class StorageHandlerExtensions
{
    public static IEndpointRouteBuilder RegisterStorageHandler(this IEndpointRouteBuilder builder, string module, string domain, bool publicRoute = false)
    {
        var pathUtils = builder.ServiceProvider.GetService<PathUtils>();
        var virtPath = pathUtils.ResolveVirtualPath(module, domain);
        virtPath = virtPath.TrimStart('/');

        var handler = new StorageHandler(builder.ServiceProvider, string.Empty, module, domain, !publicRoute);
        var url = virtPath + "{*pathInfo}";

        if (!builder.DataSources.Any(r => r.Endpoints.Any(e => e.DisplayName == url)))
        {
            builder.Map(url, handler.Invoke);

            var newUrl = url.Replace("{0}", "{t1}/{t2}/{t3}");

            if (newUrl != url)
            {
                builder.Map(url, handler.Invoke);
            }
        }

        return builder;
    }
}
