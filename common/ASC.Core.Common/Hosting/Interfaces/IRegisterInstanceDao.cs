using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Hosting.Interfaces;

[Scope]
public interface IRegisterInstanceDao<T> where T : IHostedService
{
    Task AddOrUpdate(InstanceRegistration obj);
    Task<IEnumerable<InstanceRegistration>> GetAll();
    Task Delete(string instanceId);
}
