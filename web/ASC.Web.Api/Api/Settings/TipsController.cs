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

namespace ASC.Web.Api.Controllers.Settings;

public class TipsController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly AuthContext _authContext;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly SettingsManager _settingsManager;
    private readonly SetupInfo _setupInfo;
    private readonly ILogger _log;
    private readonly IHttpClientFactory _clientFactory;

    public TipsController(
        ILoggerProvider option,
        ApiContext apiContext,
        AuthContext authContext,
        StudioNotifyHelper studioNotifyHelper,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        SetupInfo setupInfo,
        IMemoryCache memoryCache,
        IHttpClientFactory clientFactory,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _log = option.CreateLogger("ASC.Api");
        _authContext = authContext;
        _studioNotifyHelper = studioNotifyHelper;
        _settingsManager = settingsManager;
        _setupInfo = setupInfo;
        _clientFactory = clientFactory;
    }

    [HttpPut("tips")]
    public async Task<TipsSettings> UpdateTipsSettingsAsync(SettingsRequestsDto inDto)
    {
        var settings = new TipsSettings { Show = inDto.Show };
        await _settingsManager.SaveForCurrentUserAsync(settings);

        if (!inDto.Show && !string.IsNullOrEmpty(_setupInfo.TipsAddress))
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_setupInfo.TipsAddress}/tips/deletereaded")
                };

                var data = new NameValueCollection
                {
                    ["userId"] = _authContext.CurrentAccount.ID.ToString(),
                    ["tenantId"] = Tenant.Id.ToString(CultureInfo.InvariantCulture)
                };
                var body = JsonSerializer.Serialize(data);//todo check
                request.Content = new StringContent(body);

                var httpClient = _clientFactory.CreateClient();
                using var response = httpClient.Send(request);

            }
            catch (Exception e)
            {
                _log.ErrorWithException(e);
            }
        }

        return settings;
    }

    [HttpPut("tips/change/subscription")]
    public async Task<bool> UpdateTipsSubscriptionAsync()
    {
        return await StudioPeriodicNotify.ChangeSubscriptionAsync(_authContext.CurrentAccount.ID, _studioNotifyHelper);
    }

    [HttpGet("tips/subscription")]
    public async Task<bool> GetTipsSubscriptionAsync()
    {
        return await _studioNotifyHelper.IsSubscribedToNotifyAsync(_authContext.CurrentAccount.ID, Actions.PeriodicNotify);
    }
}
