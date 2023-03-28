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
public class WordpressToken
{
    private readonly TokenHelper _tokenHelper;
    private readonly ConsumerFactory _consumerFactory;

    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public const string AppAttr = "wordpress";

    public WordpressToken(TokenHelper tokenHelper, ConsumerFactory consumerFactory, OAuth20TokenHelper oAuth20TokenHelper)
    {
        _tokenHelper = tokenHelper;
        _consumerFactory = consumerFactory;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public async Task<OAuth20Token> GetTokenAsync()
    {
        return await _tokenHelper.GetTokenAsync(AppAttr);
    }

    public async Task SaveTokenAsync(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        await _tokenHelper.SaveTokenAsync(new Token(token, AppAttr));
    }

    public async Task<OAuth20Token> SaveTokenFromCodeAsync(string code)
    {
        var token = _oAuth20TokenHelper.GetAccessToken<WordpressLoginProvider>(_consumerFactory, code);
        ArgumentNullException.ThrowIfNull(token);

        await _tokenHelper.SaveTokenAsync(new Token(token, AppAttr));

        return token;
    }

    public async Task DeleteTokenAsync(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        await _tokenHelper.DeleteTokenAsync(AppAttr);
    }
}

[Singletone]
public class WordpressHelper
{
    private readonly ILogger<WordpressHelper> _logger;
    private readonly RequestHelper _requestHelper;

    public enum WordpressStatus
    {
        draft = 0,
        publish = 1
    }

    public WordpressHelper(ILogger<WordpressHelper> logger, RequestHelper requestHelper)
    {
        _logger = logger;
        _requestHelper = requestHelper;
    }

    public string GetWordpressMeInfo(string token)
    {
        try
        {
            return WordpressLoginProvider.GetWordpressMeInfo(_requestHelper, token);
        }
        catch (Exception ex)
        {
            _logger.ErrorGetWordpressInfo(ex);

            return string.Empty;
        }

    }

    public bool CreateWordpressPost(string title, string content, int status, string blogId, OAuth20Token token)
    {
        try
        {
            var wpStatus = ((WordpressStatus)status).ToString();
            WordpressLoginProvider.CreateWordpressPost(_requestHelper, title, content, wpStatus, blogId, token);

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorCreateWordpressPost(ex);

            return false;
        }
    }
}
