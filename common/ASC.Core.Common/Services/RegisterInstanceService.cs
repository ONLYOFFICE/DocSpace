using ASC.Common.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using ASC.Core.Common.Services;
using Microsoft.Extensions.Hosting;
using ASC.Core;

namespace ASC.Common.Services;

[Scope]
public class RegisterInstanceService<T> : IRegisterInstanceService<T> where T : IHostedService
{
    private readonly IRegisterInstanceRepository<T> _registerInstanceRepository;
    private readonly int _timeUntilUnregisterInSeconds;
    public RegisterInstanceService(IRegisterInstanceRepository<T> registerInstanceRepository,
                                   IConfiguration configuration)
    {
        _registerInstanceRepository = registerInstanceRepository;

        if (!int.TryParse(configuration["core:TimeUntilUnregisterInSeconds"], out _timeUntilUnregisterInSeconds))
        {
            _timeUntilUnregisterInSeconds = 20;               
        }
    }
    public async Task Register(string instanceId)
    {
        var instances = await _registerInstanceRepository.GetAll();
        var registeredInstance = instances.FirstOrDefault(x => x.InstanceRegistrationId == instanceId);

        var instance = registeredInstance ?? new InstanceRegistrationEntry { InstanceRegistrationId = instanceId, 
                                                                             WorkerName = typeof(T).Name
                                                                            };
            
        instance.LastUpdated = DateTime.UtcNow;
        
        if (!instances.Any() || !instances.Any(x => x.IsActive))
        {
            instance.IsActive = true;
        }

        await _registerInstanceRepository.Add(instance);
    }

    public async Task UnRegister(string instanceId)
    {
        await _registerInstanceRepository.Delete(instanceId);
    }

    public async Task<bool> IsActive(string instanceId)
    {
        var instances = await _registerInstanceRepository.GetAll();
        var instance = instances.FirstOrDefault(x => x.InstanceRegistrationId == instanceId);

        return instance is not null && instance.IsActive;
    }

    public async Task<List<string>> DeleteOrphanInstances()
    {
        var instances = await _registerInstanceRepository.GetAll();
        var oldRegistrations = instances.Where(x => x.LastUpdated.Value.AddSeconds(_timeUntilUnregisterInSeconds) < DateTime.UtcNow).ToList();

        foreach (var instanceRegistration in oldRegistrations)
        {
            await _registerInstanceRepository.Delete(instanceRegistration.InstanceRegistrationId);
        }

        return oldRegistrations.Select(x => x.InstanceRegistrationId).ToList();
    }
}