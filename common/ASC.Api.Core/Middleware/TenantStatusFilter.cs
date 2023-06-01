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

namespace ASC.Api.Core.Middleware;

[Scope]
public class TenantStatusFilter : IAsyncResourceFilter
{
    private readonly TenantManager _tenantManager;
    private readonly ILogger<TenantStatusFilter> _logger;
    private readonly string[] _passthroughtRequestEndings = new[] { "preparation-portal", "getrestoreprogress", "settings", "settings.json", "colortheme.json", "logos.json", "build.json", "@self.json" }; //TODO add or update when "preparation-portal" will be done


    public TenantStatusFilter(ILogger<TenantStatusFilter> logger, TenantManager tenantManager)
    {
        _logger = logger;
        _tenantManager = tenantManager;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var tenant = _tenantManager.GetCurrentTenant(false);
        if (tenant == null)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
            _logger.WarningTenantNotFound();
            return;
        }

        if (tenant.Status == TenantStatus.RemovePending || tenant.Status == TenantStatus.Suspended)
        {
            if (tenant.Status == TenantStatus.Suspended &&
                context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor &&
                controllerActionDescriptor.EndpointMetadata.OfType<AllowSuspendedAttribute>().Any())
            {
                await next();
                return;
            }

            context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
            _logger.WarningTenantIsNotRemoved(tenant.Id);
            return;
        }

        if (tenant.Status == TenantStatus.Transfering || tenant.Status == TenantStatus.Restoring)
        {
            if (_passthroughtRequestEndings.Any(path => context.HttpContext.Request.Path.ToString().EndsWith(path, StringComparison.InvariantCultureIgnoreCase)))
            {
                await next();
                return;
            }

            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            _logger.WarningTenantStatus(tenant.Id, tenant.Status);
            return;
        }
        await next();
    }
}
