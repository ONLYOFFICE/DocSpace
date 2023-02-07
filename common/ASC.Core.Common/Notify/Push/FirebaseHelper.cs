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

namespace ASC.Core.Common.Notify.Push;

[Scope]
public class FirebaseHelper
{
    protected readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly ILogger<FirebaseHelper> _logger;
    private readonly IConfiguration _configuration;
    private readonly FirebaseDao _firebaseDao;

    public FirebaseHelper(
        AuthContext authContext,
        UserManager userManager,
        TenantManager tenantManager,
        IConfiguration configuration,
        ILogger<FirebaseHelper> logger,
        FirebaseDao firebaseDao)
    {
        _authContext = authContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _configuration = configuration;
        _logger = logger;
        _firebaseDao = firebaseDao;
    }

    public void SendMessage(NotifyMessage msg)
    {
        var defaultInstance = FirebaseApp.DefaultInstance;
        if (defaultInstance == null)
        {
            try
            {
                var credentials = JsonConvert.SerializeObject(new FirebaseApiKey(_configuration)).Replace("\\\\", "\\");
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(credentials)
                });
            }
            catch (Exception e)
            {
                _logger.ErrorUnexpected(e);
            }
        }

        _tenantManager.SetCurrentTenant(msg.TenantId);

        var user = _userManager.GetUserByUserName(msg.Reciever);

        Guid productID;

        if (!Guid.TryParse(msg.ProductID, out productID))
        {
            return;
        }

        var fireBaseUser = new List<FireBaseUser>();

        if (productID == new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}")) //documents product
        {
            fireBaseUser = _firebaseDao.GetUserDeviceTokens(user.Id, msg.TenantId, PushConstants.PushDocAppName);
        }

        foreach (var fb in fireBaseUser)
        {
            if (fb.IsSubscribed.HasValue && fb.IsSubscribed.Value == true)
            {
                var m = new FirebaseAdminMessaging.Message()
                {
                    Data = new Dictionary<string, string>{
                            { "data", msg.Data }
                        },
                    Token = fb.FirebaseDeviceToken,
                    Notification = new FirebaseAdminMessaging.Notification()
                    {
                        Body = msg.Content
                    }
                };
                FirebaseAdminMessaging.FirebaseMessaging.DefaultInstance.SendAsync(m);
            }
        }
    }

    public FireBaseUser RegisterUserDevice(string fbDeviceToken, bool isSubscribed, string application)
    {
        var userId = _authContext.CurrentAccount.ID;
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return _firebaseDao.RegisterUserDevice(userId, tenantId, fbDeviceToken, isSubscribed, application);
    }

    public FireBaseUser UpdateUser(string fbDeviceToken, bool isSubscribed, string application)
    {
        var userId = _authContext.CurrentAccount.ID;
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return _firebaseDao.UpdateUser(userId, tenantId, fbDeviceToken, isSubscribed, application);
    }
}
