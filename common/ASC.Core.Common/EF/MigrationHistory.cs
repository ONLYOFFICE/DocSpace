namespace ASC.Core.Common.EF;

[Singletone]
public class MigrationHistory
{
    private readonly ConcurrentDictionary<Type, bool> _historyStore
        = new ConcurrentDictionary<Type, bool>();

    public bool TryAddMigratedContext(Type contextType)
    {
        return _historyStore.TryAdd(contextType, true);
    }
}
