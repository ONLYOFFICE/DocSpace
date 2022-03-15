namespace ASC.Common.Security.Authentication;

[Serializable]
public class SystemAccount : Account, ISystemAccount
{
    public SystemAccount(Guid id, string name, bool authenticated)
        : base(id, name, authenticated) { }
}
