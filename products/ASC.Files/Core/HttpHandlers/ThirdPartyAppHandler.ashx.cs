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

namespace ASC.Web.Files.HttpHandlers;

public class ThirdPartyAppHandler
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ThirdPartyAppHandler(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var thirdPartyAppHandlerService = scope.ServiceProvider.GetService<ThirdPartyAppHandlerService>();
        await thirdPartyAppHandlerService.InvokeAsync(context);
        await _next.Invoke(context);
    }
}

[Scope]
public class ThirdPartyAppHandlerService
{
    private readonly AuthContext _authContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ILog _logger;

    public string HandlerPath { get; set; }

    public ThirdPartyAppHandlerService(
        IOptionsMonitor<ILog> optionsMonitor,
        AuthContext authContext,
        BaseCommonLinkUtility baseCommonLinkUtility,
        CommonLinkUtility commonLinkUtility)
    {
        _authContext = authContext;
        _commonLinkUtility = commonLinkUtility;
        _logger = optionsMonitor.CurrentValue;
        HandlerPath = baseCommonLinkUtility.ToAbsolute("~/thirdpartyapp");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.Debug("ThirdPartyApp: handler request - " + context.Request.Url());

        var message = string.Empty;

        try
        {
            var app = ThirdPartySelector.GetApp(context.Request.Query[ThirdPartySelector.AppAttr]);
            _logger.Debug("ThirdPartyApp: app - " + app);

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
            _logger.Error("ThirdPartyApp", e);
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
