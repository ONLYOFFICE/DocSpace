using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Core.Common.Services;

using Microsoft.Extensions.Hosting;

namespace ASC.Common.Services.Interfaces
{
    [Scope]
    public interface IRegisterInstanceRepository<T> where T : IHostedService
    {
        Task Add(InstanceRegistrationEntry obj);
        Task<IEnumerable<InstanceRegistrationEntry>> GetAll();
        Task Delete(string instanceId);
    }
}
