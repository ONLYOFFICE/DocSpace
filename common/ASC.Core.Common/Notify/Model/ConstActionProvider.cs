namespace ASC.Notify.Model;

public sealed class ConstActionProvider : IActionProvider
{
    private readonly Dictionary<string, INotifyAction> _actions;

    public ConstActionProvider(params INotifyAction[] actions)
    {
        _actions = actions.ToDictionary(a => a.ID);
    }

    public INotifyAction[] GetActions()
    {
        return _actions.Values.ToArray();
    }

    public INotifyAction GetAction(string id)
    {
        _actions.TryGetValue(id, out var action);
        return action;
    }
}
