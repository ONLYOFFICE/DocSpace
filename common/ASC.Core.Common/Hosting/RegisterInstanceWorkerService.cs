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
    private IRegisterInstanceManager<T> _registerInstanceService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceProvider _serviceProvider;       
    public static readonly string InstanceId =
        $"{typeof(T).Name}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

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
        using var scope = _serviceProvider.CreateScope();

        _registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceManager<T>>();
     
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _registerInstanceService.Register(InstanceId);
                await _registerInstanceService.DeleteOrphanInstances();

                _logger.InfoFormat("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("Critical error forced worker to shutdown", ex);
                _applicationLifetime.StopApplication();
            }
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _registerInstanceService.UnRegister(InstanceId);
            _logger.InfoFormat("UnRegister Instance {instanceName} running at: {time}.", InstanceId, DateTimeOffset.Now);
        }
        catch
        {
            _logger.ErrorFormat("Unable to UnRegister Instance {instanceName} running at: {time}.", InstanceId, DateTimeOffset.Now);
        }

        await base.StopAsync(cancellationToken);
    }
}
