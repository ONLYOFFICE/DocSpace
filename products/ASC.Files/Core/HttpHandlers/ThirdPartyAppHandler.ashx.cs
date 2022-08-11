// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Files.HttpHandlers;

public class ThirdPartyAppHandler
{
    public static string HandlerPath = "~/ThirdPartyApp";

    public ThirdPartyAppHandler(RequestDelegate next)
    {
    }

    public async Task Invoke(HttpContext context, ThirdPartyAppHandlerService thirdPartyAppHandlerService)
    {
        await thirdPartyAppHandlerService.InvokeAsync(context);
    }
}

[Scope]
public class ThirdPartyAppHandlerService
{
    private readonly AuthContext _authContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ILogger<ThirdPartyAppHandlerService> _log;
    private readonly ThirdPartySelector _thirdPartySelector;
    public string HandlerPath { get; set; }

    public ThirdPartyAppHandlerService(
        ILogger<ThirdPartyAppHandlerService> logger,
        AuthContext authContext,
        BaseCommonLinkUtility baseCommonLinkUtility,
        CommonLinkUtility commonLinkUtility,
        ThirdPartySelector thirdPartySelector)
    {
        _authContext = authContext;
        _commonLinkUtility = commonLinkUtility;
        _log = logger;
        _thirdPartySelector = thirdPartySelector;
        HandlerPath = baseCommonLinkUtility.ToAbsolute(ThirdPartyAppHandler.HandlerPath);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _log.DebugThirdPartyAppHandlerRequest(context.Request.Url());

        var message = string.Empty;

        try
        {
            var app = _thirdPartySelector.GetApp(context.Request.Query[ThirdPartySelector.AppAttr]);
            _log.DebugThirdPartyAppApp(app);

            if (await app.RequestAsync(context))
            {
                return;
            }
        }
        catch (ThreadAbortException)
        {
            //Thats is responce ending
            return;
        }
        catch (Exception e)
        {
            _log.ErrorThirdPartyApp(e);
            message = e.Message;
        }

        if (string.IsNullOrEmpty(message))
        {
            if ((context.Request.Query["error"].FirstOrDefault() ?? "").Equals("access_denied", StringComparison.InvariantCultureIgnoreCase))
            {
                message = context.Request.Query["error_description"].FirstOrDefault() ?? FilesCommonResource.AppAccessDenied;
            }
        }

        var redirectUrl = _commonLinkUtility.GetDefault();
        if (!string.IsNullOrEmpty(message))
        {
            redirectUrl += _authContext.IsAuthenticated ? "#error/" : "?m=";
            redirectUrl += HttpUtility.UrlEncode(message);
        }
        context.Response.Redirect(redirectUrl, true);
    }
}

public static class ThirdPartyAppHandlerExtention
{
    public static IApplicationBuilder UseThirdPartyAppHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ThirdPartyAppHandler>();
    }
}
