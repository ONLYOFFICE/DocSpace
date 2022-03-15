namespace ASC.Web.Core.Security
{
    public abstract class SecurityAttribute : Attribute
    {
        public abstract bool CheckAuthorization(HttpContext context);
    }
}