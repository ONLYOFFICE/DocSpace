namespace ASC.Common.Security.Authorizing;

public interface IAction
{
    Guid ID { get; }
    string Name { get; }
    bool AdministratorAlwaysAllow { get; }
    bool Conjunction { get; }
}
