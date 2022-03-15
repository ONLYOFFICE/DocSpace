namespace ASC.Web.Core.Helpers
{
    [Scope]
    public class AuthorizationHelper
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private PasswordHasher PasswordHasher { get; }

        public AuthorizationHelper(
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager,
            SecurityContext securityContext,
            PasswordHasher passwordHasher)
        {
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            SecurityContext = securityContext;
            PasswordHasher = passwordHasher;
        }

        public bool ProcessBasicAuthorization(out string authCookie)
        {
            authCookie = null;
            try
            {
                //Try basic
                var authorization = HttpContextAccessor.HttpContext.Request.Cookies["asc_auth_key"] ?? HttpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorization))
                {
                    return false;
                }

                authorization = authorization.Trim();
                if (0 <= authorization.IndexOf("Basic", 0))
                {
                    var arr = Encoding.ASCII.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(new[] { ':' });
                    var username = arr[0];
                    var password = arr[1];
                    var u = UserManager.GetUserByEmail(username);
                    if (u != null && u.Id != ASC.Core.Users.Constants.LostUser.Id)
                    {
                        var passwordHash = PasswordHasher.GetClientPassword(password);
                        authCookie = SecurityContext.AuthenticateMe(u.Email, passwordHash);
                    }
                }
                else if (0 <= authorization.IndexOf("Bearer", 0))
                {
                    authorization = authorization.Substring("Bearer ".Length);
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
                else
                {
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
            }
            catch (Exception) { }
            return SecurityContext.IsAuthenticated;
        }
    }
}