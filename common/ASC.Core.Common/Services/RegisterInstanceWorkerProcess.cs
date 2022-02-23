using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Common.Services.Interfaces;
using ASC.Core.Common.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Common.Services;

[Singletone]
public class RegisterInstanceWorkerProcess<T> : BackgroundService, IInstanceWorkerInfo<T> where T : IHostedService
{
    private readonly ILog _logger;
    private IRegisterInstanceService<T> _registerInstanceService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceProvider _serviceProvider;
       
    private static readonly string _instanceId =
        $"{typeof(T).Name}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    public RegisterInstanceWorkerProcess(IOptionsMonitor<ILog> options, 
                                         IServiceProvider serviceProvider, 
                                         IHostApplicationLifetime applicationLifetime)
    {
        _logger = options.CurrentValue;
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }

    public string GetInstanceId()
    {
        return _instanceId;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        _registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceService<T>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _registerInstanceService.Register(_instanceId);
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
            await _registerInstanceService.UnRegister(_instanceId);
            _logger.InfoFormat("UnRegister Instance {instanceName} running at: {time}.", _instanceId, DateTimeOffset.Now);
        }
        catch
        {
            _logger.ErrorFormat("Unable to UnRegister Instance {instanceName} running at: {time}.", _instanceId, DateTimeOffset.Now);
        }

        await base.StopAsync(cancellationToken);
    }
}
