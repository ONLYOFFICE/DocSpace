namespace ASC.Core.Common.Security;

public class CustomClaimsPrincipal : ClaimsPrincipal
{
    public override IIdentity Identity { get; }

    public CustomClaimsPrincipal(ClaimsIdentity claimsIdentity, IIdentity identity) : base(claimsIdentity)
    {
        Identity = identity;
    }
}
