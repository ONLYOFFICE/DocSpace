/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ASC.FederatedLogin.Helpers
{
    public static class XrdsHelper
    {
        internal static async Task RenderXrds(HttpResponse responce, string location, string iconlink)
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
}