using ASC.Core.Common.Hosting.Interfaces;

using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Hosting;

[Scope]
public class RegisterInstanceManager<T> : IRegisterInstanceManager<T> where T : IHostedService
{
    private readonly IRegisterInstanceDao<T> _registerInstanceRepository;
    private readonly int _timeUntilUnregisterInSeconds;
    public RegisterInstanceManager(IRegisterInstanceDao<T> registerInstanceRepository,
                                   IConfiguration configuration)
    {
        _registerInstanceRepository = registerInstanceRepository;

        if (!int.TryParse(configuration["core:hosting:timeUntilUnregisterInSeconds"], out _timeUntilUnregisterInSeconds))
        {
            _timeUntilUnregisterInSeconds = 15;
        }
    }
    public async Task Register(string instanceId)
    {
        var instances = await _registerInstanceRepository.GetAll();
        var registeredInstance = instances.FirstOrDefault(x => x.InstanceRegistrationId == instanceId);

        var instance = registeredInstance ?? new InstanceRegistration
        {
            InstanceRegistrationId = instanceId,
            WorkerTypeName = typeof(T).Name
        };

        instance.LastUpdated = DateTime.UtcNow;

        if (instances.Any() && !instances.Any(x => x.IsActive))
        {
            var firstAliceInstance = GetFirstAliveInstance(instances);

            if (firstAliceInstance != null && firstAliceInstance.InstanceRegistrationId == instance.InstanceRegistrationId)
            {
                instance.IsActive = true;
            }
        }

        await _registerInstanceRepository.AddOrUpdate(instance);
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
        var oldRegistrations = instances.Where(IsOrphanInstance).ToList();

        foreach (var instanceRegistration in oldRegistrations)
        {
            await _registerInstanceRepository.Delete(instanceRegistration.InstanceRegistrationId);
        }

        return oldRegistrations.Select(x => x.InstanceRegistrationId).ToList();
    }

    private InstanceRegistration? GetFirstAliveInstance(IEnumerable<InstanceRegistration> instances)
    {
        Func<InstanceRegistration, long> _getTicksCreationService = x => Convert.ToInt64(x.InstanceRegistrationId.Split('_')[1]);

        return instances.Where(x => !IsOrphanInstance(x)).MinBy(_getTicksCreationService);
    }

    private bool IsOrphanInstance(InstanceRegistration obj)
    {
        return obj.LastUpdated.Value.AddSeconds(_timeUntilUnregisterInSeconds) < DateTime.UtcNow;
    }
}