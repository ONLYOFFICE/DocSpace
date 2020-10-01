using System.Security.Claims;
using System.Security.Principal;

namespace ASC.Core.Common.Security
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        private IIdentity identity { get; set; }

        public override IIdentity Identity
        {
            get
            {
                return identity;
            }
        }

        public CustomClaimsPrincipal(ClaimsIdentity claimsIdentity, IIdentity identity) : base(claimsIdentity)
        {
            this.identity = identity;
        }
    }
}
