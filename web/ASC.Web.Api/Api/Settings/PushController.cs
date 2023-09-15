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

using ASC.Web.Api.Controllers.Settings;

namespace ASC.Web.Api.Api.Settings;

public class PushController : BaseSettingsController
{
    private readonly FirebaseHelper _firebaseHelper;

    public PushController(
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        FirebaseHelper firebaseHelper,
        IHttpContextAccessor httpContextAccessor
        ) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _firebaseHelper = firebaseHelper;
    }

    /// <summary>
    /// Saves the Firebase device token specified in the request for the Documents application.
    /// </summary>
    /// <short>Save the Documents Firebase device token</short>
    /// <category>Firebase</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.FirebaseRequestsDto, ASC.Web.Api" name="inDto">Firebase request parameters</param>
    /// <returns type="ASC.Core.Common.EF.FireBaseUser, ASC.Core.Common">FireBase user</returns>
    /// <path>api/2.0/settings/push/docregisterdevice</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("push/docregisterdevice")]
    public async Task<FireBaseUser> DocRegisterPusnNotificationDeviceAsync(FirebaseRequestsDto inDto)
    {
        return await _firebaseHelper.RegisterUserDeviceAsync(inDto.FirebaseDeviceToken, inDto.IsSubscribed, PushConstants.PushDocAppName);
    }

    /// <summary>
    /// Subscribes to the Documents push notification.
    /// </summary>
    /// <short>Subscribe to Documents push notification</short>
    /// <category>Firebase</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.FirebaseRequestsDto, ASC.Web.Api" name="inDto">Firebase request parameters</param>
    /// <returns type="ASC.Core.Common.EF.FireBaseUser, ASC.Core.Common">Firebase user</returns>
    /// <path>api/2.0/settings/push/docsubscribe</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("push/docsubscribe")]
    public async Task<FireBaseUser> SubscribeDocumentsPushNotificationAsync(FirebaseRequestsDto inDto)
    {
        return await _firebaseHelper.UpdateUserAsync(inDto.FirebaseDeviceToken, inDto.IsSubscribed, PushConstants.PushDocAppName);
    }
}