using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;

using ASC.Core.Common.EF;
using ASC.Core.Common.Hosting.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Hosting;

[Scope]
public class RegisterInstanceDao<T> : IRegisterInstanceDao<T> where T : IHostedService
{
    private InstanceRegistrationContext _instanceRegistrationContext;

    public RegisterInstanceDao(DbContextManager<InstanceRegistrationContext> dbContextManager)
    {
        _instanceRegistrationContext = dbContextManager.Value;
    }

    public async Task Add(InstanceRegistration obj)
    {
        var inst = _instanceRegistrationContext.InstanceRegistrations.Find(obj.InstanceRegistrationId);

        if (inst == null)
        {
           await  _instanceRegistrationContext.AddAsync(obj);

        }
        else
        {
            _instanceRegistrationContext.Update(obj);
        }

        await _instanceRegistrationContext.SaveChangesAsync();
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

        await _instanceRegistrationContext.SaveChangesAsync();
    }

}
