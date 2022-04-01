namespace ASC.Core.Common.Hosting.Interfaces;

[Scope]
public interface IRegisterInstanceManager<T> where T : IHostedService
{
    Task Register(string instanceId);
    Task UnRegister(string instanceId);
    Task<bool> IsActive(string instanceId);
    Task<List<string>> DeleteOrphanInstances();
}