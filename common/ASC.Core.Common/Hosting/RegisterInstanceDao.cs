using ASC.Core.Common.Hosting.Interfaces;

using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Hosting;

[Scope]
public class RegisterInstanceDao<T> : IRegisterInstanceDao<T> where T : IHostedService
{
    private readonly ILog _logger;
    private InstanceRegistrationContext _instanceRegistrationContext;

    public RegisterInstanceDao(
        IOptionsMonitor<ILog> options,
        DbContextManager<InstanceRegistrationContext> dbContextManager)
    {
        _logger = options.CurrentValue;
        _instanceRegistrationContext = dbContextManager.Value;
    }

    public async Task AddOrUpdate(InstanceRegistration obj)
    {
        var inst = _instanceRegistrationContext.InstanceRegistrations.Find(obj.InstanceRegistrationId);

        if (inst == null)
        {
           await  _instanceRegistrationContext.AddAsync(obj);
        }

        bool saveFailed;

        do
        {
            saveFailed = false;

            try
            {
                await _instanceRegistrationContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.TraceFormat("DbUpdateConcurrencyException: then updating {instanceName} at {time} time.", obj.InstanceRegistrationId, DateTimeOffset.Now);

                saveFailed = true;

                var entry = ex.Entries.Single();

                if (entry.State == EntityState.Modified)
                    entry.State = EntityState.Added;
            }
        }
        while (saveFailed);
    }

    public async Task<IEnumerable<InstanceRegistration>> GetAll()
    {
        return await _instanceRegistrationContext.InstanceRegistrations
                                                .Where(x => x.WorkerTypeName == typeof(T).Name)
                                                .ToListAsync();
    }

    public async Task Delete(string instanceId)
    {
        var item = _instanceRegistrationContext.InstanceRegistrations.Find(instanceId);

        if (item == null) return;
                
        _instanceRegistrationContext.InstanceRegistrations.Remove(item);

        try
        {
            await _instanceRegistrationContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();

            if (entry.State == EntityState.Deleted)
                entry.State = EntityState.Detached;           
        }   
    }

}
