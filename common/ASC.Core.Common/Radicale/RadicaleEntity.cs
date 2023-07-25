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

public abstract class RadicaleEntity
{
    public string Uid { get; set; }

    protected readonly string _defaultRadicaleUrl;
    protected const string DefaultAddBookName = "11111111-1111-1111-1111-111111111111";
    protected const string ReadonlyAddBookName = "11111111-1111-1111-1111-111111111111-readonly";

    private readonly IConfiguration _configuration;
    private readonly InstanceCrypto _instanceCrypto;

    public RadicaleEntity(IConfiguration configuration, InstanceCrypto instanceCrypto)
    {
        _defaultRadicaleUrl = (configuration["radicale:path"] != null) ? configuration["radicale:path"] : "http://localhost:5232";
        _configuration = configuration;
        _instanceCrypto = instanceCrypto;
    }

    public string GetRadicaleUrl(string url, string email, bool isReadonly = false, bool isCardDav = false, bool isRedirectUrl = false, string entityId = "", string itemID = "")
    {
        string requestUrl;
        var currentUserName = url.StartsWith("http") ? email.ToLower() + "@" + new Uri(url).Host : email.ToLower() + "@" + url;
        var protocolType = (!isCardDav) ? "/caldav/" : "/carddav/";
        var serverUrl = isRedirectUrl ? new Uri(url).Scheme + "://" + new Uri(url).Host + protocolType :
            _defaultRadicaleUrl;
        if (isCardDav)
        {
            var addbookId = isReadonly ? ReadonlyAddBookName : DefaultAddBookName;
            requestUrl = (itemID != "") ? _defaultRadicaleUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + addbookId + "/" + itemID + ".vcf" :
                (isRedirectUrl) ? serverUrl + HttpUtility.UrlEncode(currentUserName) + "/" + addbookId :
                _defaultRadicaleUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + addbookId;
        }
        else
        {
            requestUrl = (itemID != "") ? serverUrl + HttpUtility.UrlEncode(currentUserName) + "/" + entityId + (isReadonly ? "-readonly" : "") +
                                        "/" + HttpUtility.UrlEncode(itemID) + ".ics" :
                                        serverUrl + HttpUtility.UrlEncode(currentUserName) + "/" + entityId + (isReadonly ? "-readonly" : "");
        }
        return requestUrl;
    }

    public string GetSystemAuthorization()
    {   
        if(_configuration["radicale:admin"] == null || _configuration["radicale:admin"] == "")
        {
            return null;
        }
        return _configuration["radicale:admin"] + ":" + _instanceCrypto.Encrypt(_configuration["radicale:admin"]);
    }

    protected string GetData(string sample, string name, string description, string backgroundColor)
    {
        var numbers = Regex.Split(backgroundColor, @"\D+");
        var color = numbers.Length > 4 ? HexFromRGB(int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3])) : "#000000";
        return string.Format(sample, name, color, description);
    }

    private string HexFromRGB(int r, int g, int b)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
    }
}
