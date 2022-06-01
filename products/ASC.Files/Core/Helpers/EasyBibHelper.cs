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

namespace ASC.Web.Files.Helpers;

[Scope]
public class EasyBibHelper : Consumer
{
    public ILogger Logger { get; set; }

    static readonly string _searchBookUrl = "https://worldcat.citation-api.com/query?search=",
                    _searchJournalUrl = "https://crossref.citation-api.com/query?search=",
                    _searchWebSiteUrl = "https://web.citation-api.com/query?search=",
                    _easyBibStyles = "https://api.citation-api.com/2.1/rest/styles";

    public enum EasyBibSource
    {
        book = 0,
        journal = 1,
        website = 2
    }

    public string AppKey => this["easyBibappkey"];
    private readonly RequestHelper _requestHelper;

    public EasyBibHelper() { }

    public EasyBibHelper(
        ILogger<EasyBibHelper> logger,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory factory,
        RequestHelper requestHelper,
        string name,
        int order,
        Dictionary<string, string> props,
        Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, factory, name, order, props, additional)
    {
        Logger = logger;
        _requestHelper = requestHelper;
    }

    public string GetEasyBibCitationsList(int source, string data)
    {
        var uri = "";
        switch (source)
        {
            case 0:
                uri = _searchBookUrl;
                break;
            case 1:
                uri = _searchJournalUrl;
                break;
            case 2:
                uri = _searchWebSiteUrl;
                break;
            default:
                break;
        }
        uri += data;

        const string method = "GET";
        var headers = new Dictionary<string, string>() { };
        try
        {
            return _requestHelper.PerformRequest(uri, "", method, "", headers);
        }
        catch (Exception)
        {
            return "error";
        }

    }

    public string GetEasyBibStyles()
    {

        const string method = "GET";
        var headers = new Dictionary<string, string>() { };
        try
        {
            return _requestHelper.PerformRequest(_easyBibStyles, "", method, "", headers);
        }
        catch (Exception)
        {
            return "error";
        }
    }

    public object GetEasyBibCitation(string data)
    {
        try
        {
            var easyBibappkey = ConsumerFactory.Get<EasyBibHelper>().AppKey;

            var jsonBlogInfo = JObject.Parse(data);
            jsonBlogInfo.Add("key", easyBibappkey);
            var citationData = jsonBlogInfo.ToString();

            const string uri = "https://api.citation-api.com/2.0/rest/cite";
            const string contentType = "application/json";
            const string method = "POST";
            var body = citationData;
            var headers = new Dictionary<string, string>() { };

            return _requestHelper.PerformRequest(uri, contentType, method, body, headers);

        }
        catch (Exception)
        {
            return null;
            throw;
        }

    }
}
