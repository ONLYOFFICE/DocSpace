using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using ASC.Common.Security.Authentication;

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

        public CustomClaimsPrincipal(ClaimsIdentity claimsIdentity, IIdentity identity): base(claimsIdentity)
        {
            this.identity = identity;
        }
    }
}
