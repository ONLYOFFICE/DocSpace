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

namespace ASC.Core.Common.Hosting;

[Singletone]
public class RegisterInstanceWorkerService<T> : BackgroundService where T : IHostedService
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _intervalCheckRegisterInstanceInSeconds;
    public static readonly string InstanceId =
        $"{typeof(T).GetFormattedName()}_{DateTime.UtcNow.Ticks}";
    private readonly bool _isSingletoneMode;

    public RegisterInstanceWorkerService(
        ILogger<RegisterInstanceWorkerService<T>> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;

        if (!int.TryParse(configuration["core:hosting:intervalCheckRegisterInstanceInSeconds"], out _intervalCheckRegisterInstanceInSeconds))
        {
            _intervalCheckRegisterInstanceInSeconds = 1;
        }

        if (!bool.TryParse(configuration["core:hosting:singletonMode"], out _isSingletoneMode))
        {
            _isSingletoneMode = true;
        }


        _intervalCheckRegisterInstanceInSeconds = _intervalCheckRegisterInstanceInSeconds * 1000;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_isSingletoneMode)
        {
            _logger.InformationWorkerSingletone();

            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceManager<T>>();

                await registerInstanceService.Register(InstanceId);
                await registerInstanceService.DeleteOrphanInstances();

                _logger.TraceWorkingRunnging(DateTimeOffset.Now);

                await Task.Delay(_intervalCheckRegisterInstanceInSeconds, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.CriticalError(ex);
                _applicationLifetime.StopApplication();
            }
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_isSingletoneMode)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceManager<T>>();

                await registerInstanceService.UnRegister(InstanceId);

                _logger.InformationUnRegister(InstanceId, DateTimeOffset.Now);
            }
            catch
            {
                _logger.ErrorUnableToUnRegister(InstanceId, DateTimeOffset.Now);
            }
        }

        await base.StopAsync(cancellationToken);
    }

}
