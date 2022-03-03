using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Hosting.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.Hosting;

[Singletone]
public class RegisterInstanceWorkerService<T> : BackgroundService where T : IHostedService
{
    private readonly ILog _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceProvider _serviceProvider;       
    public static readonly string InstanceId =
        $"{typeof(T).Name}_{DateTime.UtcNow.Ticks}";

    public RegisterInstanceWorkerService(IOptionsMonitor<ILog> options, 
                                         IServiceProvider serviceProvider, 
                                         IHostApplicationLifetime applicationLifetime)
    {
        _logger = options.CurrentValue;
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }  

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {     
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceManager<T>>();

                await registerInstanceService.Register(InstanceId);
                await registerInstanceService.DeleteOrphanInstances();

                _logger.InfoFormat("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error("Critical error forced worker to shutdown", ex);
                _applicationLifetime.StopApplication();
            }
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceManager<T>>();

            await registerInstanceService.UnRegister(InstanceId);

            _logger.InfoFormat("UnRegister Instance {instanceName} running at: {time}.", InstanceId, DateTimeOffset.Now);
        }
        catch
        {
            _logger.ErrorFormat("Unable to UnRegister Instance {instanceName} running at: {time}.", InstanceId, DateTimeOffset.Now);
        }

        await base.StopAsync(cancellationToken);
    }
}
