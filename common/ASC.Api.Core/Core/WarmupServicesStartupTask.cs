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

namespace ASC.Api.Core.Core;

/// <summary>
///     https://andrewlock.net/reducing-latency-by-pre-building-singletons-in-asp-net-core/
/// </summary>
public class WarmupServicesStartupTask : IStartupTask
{
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _provider;
    public WarmupServicesStartupTask(IServiceCollection services, IServiceProvider provider)
    {
        _services = services;
        _provider = provider;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {      
        var processedFailed = 0;
        var processedSuccessed = 0;
        var startTime = DateTime.UtcNow;

        using (var scope = _provider.CreateScope())
        {
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var logger = scope.ServiceProvider.GetService<ILogger<WarmupServicesStartupTask>>();

            logger.TraceWarmupStarted();

            await tenantManager.SetCurrentTenantAsync("localhost");
            
            foreach (var service in GetServices(_services))
            {
                try
                {
                    scope.ServiceProvider.GetService(service);

                    processedSuccessed++;
                }
                catch (Exception ex)
                {
                    processedFailed++;

                    logger.DebugWarmupFailed(processedFailed, service.FullName, ex.Message);
                }
            }

            var processed = processedSuccessed + processedFailed;

            logger.TraceWarmupFinished(processed,
                                       processedSuccessed,
                                       processedFailed,
                                       (DateTime.UtcNow - startTime).TotalMilliseconds);
        }

    }

    static IEnumerable<Type> GetServices(IServiceCollection services)
    {
        return services
            .Where(descriptor => descriptor.ImplementationType != typeof(WarmupServicesStartupTask))
            .Where(descriptor => descriptor.ServiceType.ContainsGenericParameters == false)
            .Select(descriptor => descriptor.ServiceType)
            .Distinct();
    }
}