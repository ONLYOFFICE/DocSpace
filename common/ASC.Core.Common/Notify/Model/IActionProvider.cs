namespace ASC.Notify.Model;

public interface IActionProvider
{
    INotifyAction GetAction(string id);
    INotifyAction[] GetActions();
}
