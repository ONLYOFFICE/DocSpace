﻿namespace ASC.Core.Common.EF
{
    [Singletone]
    public class MigrationHistory
    {
        private ConcurrentDictionary<Type, bool> _historyStore
            = new ConcurrentDictionary<Type, bool>();

        public bool TryAddMigratedContext(Type contextType) =>
            _historyStore.TryAdd(contextType, true);
    }
}