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

[Scope]
public class RadicaleController : BaseSettingsController
{
    private readonly RadicaleClient _radicaleClient;
    private readonly DbRadicale _dbRadicale;
    private readonly CardDavAddressbook _cardDavAddressbook;
    private readonly TenantManager _tenantManager;
    private readonly ILogger<RadicaleController> _logger;
    private readonly InstanceCrypto _crypto;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly WebItemSecurity _webItemSecurity;

    public RadicaleController(
        RadicaleClient radicaleClient,
        DbRadicale dbRadicale,
        CardDavAddressbook cardDavAddressbook,
        TenantManager tenantManager,
        ILogger<RadicaleController> logger,
        InstanceCrypto crypto,
        UserManager userManager,
        AuthContext authContext,
        WebItemSecurity webItemSecurity,
        ApiContext apiContext,
        IMemoryCache memoryCache,
        WebItemManager webItemManager,
        IHttpContextAccessor httpContextAccessor)
        : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _radicaleClient = radicaleClient;
        _dbRadicale = dbRadicale;
        _cardDavAddressbook = cardDavAddressbook;
        _tenantManager = tenantManager;
        _logger = logger;
        _crypto = crypto;
        _userManager = userManager;
        _authContext = authContext;
        _webItemSecurity = webItemSecurity;
    }


    /// <summary>
    /// Creates a CardDav address book for a user with all portal users and returns a link to this address book.
    /// </summary>
    /// <short>
    /// Get a link to the CardDav address book
    /// </short>
    /// <category>CardDav address book</category>
    /// <returns type="ASC.Common.Radicale.DavResponse, ASC.Common.Radicale">CardDav response</returns>
    /// <path>api/2.0/settings/carddavurl</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("carddavurl")]
    public async Task<DavResponse> GetCardDavUrl()
    {

        if (await WebItemManager[WebItemManager.PeopleProductID].IsDisabledAsync(_webItemSecurity, _authContext))
        {
            await DeleteCardDavAddressBook().ConfigureAwait(false);
            throw new MethodAccessException("Method not available");
        }

        var myUri = HttpContext.Request.Url();
        var currUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);
        var userName = currUser.Email.ToLower();
        var currentAccountPaswd = _crypto.Encrypt(userName);
        var cardBuilder = await CardDavAllSerializationAsync(myUri);


        var userAuthorization = userName + ":" + currentAccountPaswd;
        var rootAuthorization = _cardDavAddressbook.GetSystemAuthorization();
        var sharedCardUrl = _cardDavAddressbook.GetRadicaleUrl(myUri.ToString(), userName, true, true, true);
        var getResponse = await _cardDavAddressbook.GetCollection(sharedCardUrl, userAuthorization, myUri.ToString());
        if (getResponse.Completed)
        {
            return new DavResponse()
            {
                Completed = true,
                Data = sharedCardUrl
            };
        }
        else if (getResponse.StatusCode == 404)
        {
            var createResponse = await _cardDavAddressbook.Create("", "", "", sharedCardUrl, rootAuthorization);
            if (createResponse.Completed)
            {
                try
                {
                    await _dbRadicale.SaveCardDavUserAsync(await _tenantManager.GetCurrentTenantIdAsync(), currUser.Id);
                }
                catch (Exception ex)
                {
                    _logger.ErrorWithException(ex);
                }

                await _cardDavAddressbook.UpdateItem(sharedCardUrl, rootAuthorization, cardBuilder, myUri.ToString()).ConfigureAwait(false);
                return new DavResponse()
                {
                    Completed = true,
                    Data = sharedCardUrl
                };
            }

            _logger.Error(createResponse.Error);
            throw new RadicaleException(createResponse.Error);
        }
        else
        {
            _logger.Error(getResponse.Error);
            throw new RadicaleException(getResponse.Error);
        }

    }

    /// <summary>
    /// Deletes a CardDav address book with all portal users.
    /// </summary>
    /// <short>
    /// Delete a CardDav address book
    /// </short>
    /// <category>CardDav address book</category>
    /// <returns type="ASC.Common.Radicale.DavResponse, ASC.Common.Radicale">CardDav response</returns>
    /// <path>api/2.0/settings/deletebook</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("deletebook")]
    public async Task<DavResponse> DeleteCardDavAddressBook()
    {
        var currUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);
        var currentUserEmail = currUser.Email;
        var authorization = _cardDavAddressbook.GetSystemAuthorization();
        var myUri = HttpContext.Request.Url();
        var requestUrlBook = _cardDavAddressbook.GetRadicaleUrl(myUri.ToString(), currentUserEmail, true, true);
        var tenant = await _tenantManager.GetCurrentTenantIdAsync();
        var davRequest = new DavRequest()
        {
            Url = requestUrlBook,
            Authorization = authorization,
            Header = myUri.ToString()
        };

        await _radicaleClient.RemoveAsync(davRequest).ConfigureAwait(false);

        try
        {
            await _dbRadicale.RemoveCardDavUserAsync(tenant, currUser.Id);

            return new DavResponse()
            {
                Completed = true
            };
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            return new DavResponse()
            {
                Completed = false,
                Error = ex.Message
            };
        }


    }

    private async Task<string> CardDavAllSerializationAsync(Uri uri)
    {
        var builder = new StringBuilder();
        var users = await _userManager.GetUsersAsync();

        foreach (var user in users)
        {
            builder.AppendLine(_cardDavAddressbook.GetUserSerialization(ItemFromUserInfo(user)));
        }

        return builder.ToString();
    }

    private static CardDavItem ItemFromUserInfo(UserInfo u)
    {
        return new CardDavItem(u.Id, u.FirstName, u.LastName, u.UserName, u.BirthDate, u.Sex, u.Title, u.Email, u.ContactsList, u.MobilePhone);
    }
}