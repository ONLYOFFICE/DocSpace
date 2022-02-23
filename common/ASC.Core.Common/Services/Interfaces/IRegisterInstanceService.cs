using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace ASC.Common.Services.Interfaces;

[Scope]
public interface IRegisterInstanceService<T> where T : IHostedService
{
    Task Register(string instanceId);
    Task UnRegister(string instanceId);
    Task<bool> IsActive(string instanceId);
    Task<List<string>> DeleteOrphanInstances();
}