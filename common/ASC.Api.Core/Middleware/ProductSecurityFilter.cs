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

using CallContext = ASC.Common.Notify.Engine.CallContext;

namespace ASC.Api.Core.Middleware;

[Scope]
public class ProductSecurityFilter : IAsyncResourceFilter
{
    private static readonly IDictionary<string, Guid> _products;
    private readonly ILogger<ProductSecurityFilter> _logger;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly AuthContext _authContext;

    static ProductSecurityFilter()
    {
        var blog = new Guid("6a598c74-91ae-437d-a5f4-ad339bd11bb2");
        var bookmark = new Guid("28b10049-dd20-4f54-b986-873bc14ccfc7");
        var forum = new Guid("853b6eb9-73ee-438d-9b09-8ffeedf36234");
        var news = new Guid("3cfd481b-46f2-4a4a-b55c-b8c0c9def02c");
        var wiki = new Guid("742cf945-cbbc-4a57-82d6-1600a12cf8ca");
        var photo = new Guid("9d51954f-db9b-4aed-94e3-ed70b914e101");

        _products = new Dictionary<string, Guid>
                {
                    { "blog", blog },
                    { "bookmark", bookmark },
                    { "event", news },
                    { "forum", forum },
                    { "photo", photo },
                    { "wiki", wiki },
                    { "birthdays", WebItemManager.BirthdaysProductID },
                    { "community", WebItemManager.CommunityProductID },
                    { "crm", WebItemManager.CRMProductID },
                    { "files", WebItemManager.DocumentsProductID },
                    { "project", WebItemManager.ProjectsProductID },
                    { "calendar", WebItemManager.CalendarProductID },
                    { "mail", WebItemManager.MailProductID },
                };
    }


    public ProductSecurityFilter(
        ILogger<ProductSecurityFilter> logger,
        WebItemSecurity webItemSecurity,
        AuthContext authContext)
    {
        _logger = logger;
        _webItemSecurity = webItemSecurity;
        _authContext = authContext;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (!_authContext.IsAuthenticated)
        {
            await next();
            return;
        }

        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var pid = FindProduct(controllerActionDescriptor);
            if (pid != Guid.Empty)
            {
                if (CallContext.GetData("asc.web.product_id") == null)
                {
                    CallContext.SetData("asc.web.product_id", pid);
                }

                if (!await _webItemSecurity.IsAvailableForMeAsync(pid))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    _logger.WarningPaymentRequired(controllerActionDescriptor.ControllerName, _authContext.CurrentAccount.ID);
                    return;
                }
            }
        }
        await next();
    }

    private static Guid FindProduct(ControllerActionDescriptor method)
    {
        if (method == null || string.IsNullOrEmpty(method.ControllerName))
        {
            return default;
        }

        var name = method.ControllerName.ToLower();
        if (name == "community")
        {
            var url = method.MethodInfo.GetCustomAttribute<HttpMethodAttribute>().Template;
            if (!string.IsNullOrEmpty(url))
            {
                var module = url.Split('/')[0];
                if (_products.TryGetValue(module, out var communityProduct))
                {
                    return communityProduct;
                }
            }
        }

        if (_products.TryGetValue(name, out var product))
        {
            return product;
        }

        return default;
    }
}