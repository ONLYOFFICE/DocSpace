using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Common.Services.Interfaces;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ASC.Common.Services
{
    [Scope]
    public class RegisterInstanceRepository<T> : IRegisterInstanceRepository<T> where T : IHostedService
    {
        private InstanceRegistrationContext _instanceRegistrationContext;

        public RegisterInstanceRepository(DbContextManager<InstanceRegistrationContext> dbContextManager)
        {
            _instanceRegistrationContext = dbContextManager.Value;
        }

        public async Task Add(InstanceRegistrationEntry obj)
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

        public async Task<IEnumerable<InstanceRegistrationEntry>> GetAll()
        {
            return await _instanceRegistrationContext.InstanceRegistrations
                                                    .Where(x => x.WorkerName == typeof(T).Name)
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
}
