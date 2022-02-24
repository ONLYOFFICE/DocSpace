using System.Collections.Generic;
using System.Threading.Tasks;

using ASC.Common;

using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Hosting.Interfaces;

[Scope]
public interface IRegisterInstanceDao<T> where T : IHostedService
{
    Task Add(InstanceRegistration obj);
    Task<IEnumerable<InstanceRegistration>> GetAll();
    Task Delete(string instanceId);
}
