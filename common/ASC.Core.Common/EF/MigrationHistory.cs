using System.Collections.Concurrent;

using ASC.Common;

namespace ASC.Core.Common.EF
{
    [Singletone]
    public class MigrationHistory
    {
        private ConcurrentDictionary<string, bool> _historyStore
            = new ConcurrentDictionary<string, bool>();

        public bool TryAddMigratedContext(string contextTypeName)
        {
            return _historyStore.TryAdd(contextTypeName, true);
        }
    }
}
