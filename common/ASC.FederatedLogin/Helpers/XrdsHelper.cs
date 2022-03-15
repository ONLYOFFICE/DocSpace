namespace ASC.FederatedLogin.Helpers;

public static class XrdsHelper
{
    internal static async Task RenderXrdsAsync(HttpResponse responce, string location, string iconlink)
    {
        var xrds =
            @"<xrds:XRDS xmlns:xrds=""xri://$xrds"" xmlns:openid=""http://openid.net/xmlns/1.0"" " +
            @"xmlns=""xri://$xrd*($v*2.0)""><XRD><Service " +
            @"priority=""1""><Type>http://specs.openid.net/auth/2.0/return_to</Type><URI " +
            $@"priority=""1"">{location}</URI></Service><Service><Type>http://specs.openid.net/extensions/ui/icon</Type><UR" +
            $@"I>{iconlink}</URI></Service></XRD></xrds:XRDS>";

        await responce.WriteAsync(xrds);
    }

    //TODO
    //public static void AppendXrdsHeader()
    //{
    //    AppendXrdsHeader("~/openidlogin.ashx");
    //}

    //public static void AppendXrdsHeader(string handlerVirtualPath)
    //{
    //    Common.HttpContext.Current.Response.Headers.Append(
    //        "X-XRDS-Location",
    //        new Uri(Common.HttpContext.Current.Request.GetUrlRewriter().ToString(), 
    //        Common.HttpContext.Current.Response.ApplyAppPathModifier(handlerVirtualPath)).AbsoluteUri);
    //}
}
