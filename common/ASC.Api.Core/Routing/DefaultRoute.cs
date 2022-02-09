namespace ASC.Web.Api.Routing
{
    public class DefaultRouteAttribute : RouteAttribute
    {
        public static string BaseUrl { get; set; }

        static DefaultRouteAttribute()
        {
            BaseUrl = "api/2.0";
        }

        public DefaultRouteAttribute() : base(BaseUrl)
        {
        }
    }
}
