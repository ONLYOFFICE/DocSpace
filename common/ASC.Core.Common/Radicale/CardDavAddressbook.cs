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


namespace ASC.Common.Radicale;

[Scope]
public class CardDavAddressbook : RadicaleEntity
{
    private const string StrTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
         "<mkcol xmlns=\"DAV:\" xmlns:C=\"urn: ietf:params:xml: ns: caldav\" xmlns:CR=\"urn: ietf:params:xml: ns: carddav\" xmlns:I=\"http://apple.com/ns/ical/\" xmlns:INF=\"http://inf-it.com/ns/ab/\">" + "" +
         "<set><prop>" +
         "<resourcetype><collection /><CR:addressbook /></resourcetype>" +
         "<displayname>{0}</displayname>" +
         "<INF:addressbook-color>{1}</INF:addressbook-color>" +
         "<CR:addressbook-description>{2}</CR:addressbook-description>" +
         "</prop></set></mkcol>";

    private readonly ILogger<CardDavAddressbook> _logger;
    private readonly RadicaleClient _radicaleClient;
    private readonly DbRadicale _dbRadicale;

    public CardDavAddressbook(
        ILogger<CardDavAddressbook> logger,
        RadicaleClient radicaleClient,
        IConfiguration configuration,
        InstanceCrypto instanceCrypto,
        DbRadicale dbRadicale)
        : base(configuration, instanceCrypto)
    {
        _logger = logger;
        _radicaleClient = radicaleClient;
        _dbRadicale = dbRadicale;
    }

    public async Task<DavResponse> Create(string name, string description, string backgroundColor, string uri, string authorization, bool isReadonly = true)
    {
        var rewriterUri = uri.StartsWith("http") ? uri : "";

        var davRequest = new DavRequest()
        {
            Url = uri,
            Authorization = authorization,
            Header = rewriterUri,
            Data = GetData(StrTemplate, name, description, backgroundColor)
        };

        return await _radicaleClient.CreateAsync(davRequest).ConfigureAwait(false);
    }

    public async Task<DavResponse> Update(string name, string description, string backgroundColor, string uri, string userName, string authorization, bool isReadonly = true)
    {
        var addbookId = isReadonly ? ReadonlyAddBookName : DefaultAddBookName;

        var header = uri.StartsWith("http") ? uri : "";

        var requestUrl = _defaultRadicaleUrl + "/" + HttpUtility.UrlEncode(userName) + "/" + addbookId;

        var davRequest = new DavRequest()
        {
            Url = requestUrl,
            Authorization = authorization,
            Data = GetData(StrTemplate, name, description, backgroundColor),
            Header = header
        };

        return await _radicaleClient.UpdateAsync(davRequest).ConfigureAwait(false);
    }


    public async Task<DavResponse> GetCollection(string url, string authorization, string myUri)
    {
        var path = (new Uri(url).AbsolutePath.StartsWith("/carddav")) ? (new Uri(url).AbsolutePath.Remove(0, 8)) : new Uri(url).AbsolutePath;
        var defaultUrlconn = _defaultRadicaleUrl + path;
        var davRequest = new DavRequest()
        {
            Url = defaultUrlconn,
            Authorization = authorization,
            Header = myUri
        };

        return await _radicaleClient.GetAsync(davRequest).ConfigureAwait(false);
    }

    public async Task<DavResponse> UpdateItem(string url, string authorization, string data, string headerUrl = "")
    {
        var path = (new Uri(url).AbsolutePath.StartsWith("/carddav")) ? (new Uri(url).AbsolutePath.Remove(0, 8)) : new Uri(url).AbsolutePath;
        var requrl = _defaultRadicaleUrl + path;
        var davRequest = new DavRequest()
        {
            Url = requrl,
            Authorization = authorization,
            Header = headerUrl,
            Data = data
        };

        return await _radicaleClient.UpdateItemAsync(davRequest).ConfigureAwait(false);
    }

    public string GetUserSerialization(CardDavItem user)
    {
        var sex = (user.Sex.HasValue) ? user.Sex.Value ? "M" : "W" : string.Empty;

        var builder = new StringBuilder();

        builder.AppendLine("BEGIN:VCARD");
        builder.AppendLine("UID:" + user.ID.ToString());
        builder.AppendLine("N:" + user.LastName + ";" + user.FirstName);
        builder.AppendLine("FN:" + user.FirstName + " " + user.LastName);
        builder.AppendLine("EMAIL:" + user.Email);
        builder.AppendLine("TEL:" + user.MobilePhone);
        builder.AppendLine($"BDAY:{user.BirthDate:s}");
        builder.AppendLine("TITLE:" + user.Title);
        builder.AppendLine("URL:" + "");
        builder.AppendLine("GENDER:" + sex);
        builder.AppendLine($"REV:{DateTime.Now:s}");
        builder.AppendLine("TZ:" + DateTimeOffset.Now.Offset);
        builder.AppendLine("ORG:");
        builder.AppendLine("END:VCARD");

        return builder.ToString();
    }

    public async Task Delete(string uri, Guid userID, string email, int tenantId = 0)
    {
        var authorization = GetSystemAuthorization();
        var deleteUrlBook = GetRadicaleUrl(uri, email.ToLower(), true, true);
        var davRequest = new DavRequest
        {
            Url = deleteUrlBook,
            Authorization = authorization
        };
        try
        {
            await _radicaleClient.RemoveAsync(davRequest);
            await _dbRadicale.RemoveCardDavUserAsync(tenantId, userID);
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
        }
    }

    public async Task UpdateItemForAllAddBooks(List<string> emailList, string uri, CardDavItem user, int tenantId = 0, string changedEmail = null)
    {
        var authorization = GetSystemAuthorization();
        if (changedEmail != null)
        {
            var deleteUrlBook = GetRadicaleUrl(uri, changedEmail.ToLower(), true, true);
            var davRequest = new DavRequest()
            {
                Url = deleteUrlBook,
                Authorization = authorization
            };

            await _radicaleClient.RemoveAsync(davRequest);

            try
            {
                await _dbRadicale.RemoveCardDavUserAsync(tenantId, user.ID);
            }
            catch (Exception ex)
            {
                _logger.ErrorWithException(ex);
            }
        }

        foreach (var email in emailList)
        {
            try
            {
                var currentEmail = email.ToLower();
                var userData = GetUserSerialization(user);
                var requestUrl = GetRadicaleUrl(uri, currentEmail, true, true, itemID: user.ID.ToString());
                await UpdateItem(requestUrl, authorization, userData, uri).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorWithException(ex);
            }
        }
    }
}
