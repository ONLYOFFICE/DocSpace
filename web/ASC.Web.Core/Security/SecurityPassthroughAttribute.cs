namespace ASC.Web.Core.Security
{
    public class SecurityPassthroughAttribute : SecurityAttribute
    {
        public override bool CheckAuthorization(HttpContext context)
        {
            //Always authorized
            return true;
        }
    }
}
