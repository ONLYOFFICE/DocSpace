
using ASC.Common;

using Microsoft.Extensions.Hosting;

namespace ASC.Core.Common.Services.Interfaces
{
    [Singletone]
    public interface IInstanceWorkerInfo<T> where T : IHostedService
    {
        string GetInstanceId(); 
    }
}
