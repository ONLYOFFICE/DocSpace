using System.Security.Claims;
using System.Security.Principal;

namespace ASC.Core.Common.Security
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        public override IIdentity Identity { get; }

        public CustomClaimsPrincipal(ClaimsIdentity claimsIdentity, IIdentity identity) : base(claimsIdentity)
        {
            this.Identity = identity;
        }
    }
}
