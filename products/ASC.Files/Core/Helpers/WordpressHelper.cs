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


using System;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Files.ThirdPartyApp;

using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Helpers
{
    [Scope]
    public class WordpressToken
    {
        public ILog Log { get; set; }
        private TokenHelper TokenHelper { get; }
        public ConsumerFactory ConsumerFactory { get; }

        private readonly OAuth20TokenHelper _oAuth20TokenHelper;

        public const string AppAttr = "wordpress";

        public WordpressToken(IOptionsMonitor<ILog> optionsMonitor, TokenHelper tokenHelper, ConsumerFactory consumerFactory, OAuth20TokenHelper oAuth20TokenHelper)
        {
            Log = optionsMonitor.CurrentValue;
            TokenHelper = tokenHelper;
            ConsumerFactory = consumerFactory;
            _oAuth20TokenHelper = oAuth20TokenHelper;
        }

        public OAuth20Token GetToken()
        {
            return TokenHelper.GetToken(AppAttr);
        }

        public void SaveToken(OAuth20Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            TokenHelper.SaveToken(new Token(token, AppAttr));
        }

        public OAuth20Token SaveTokenFromCode(string code)
        {
            var token = _oAuth20TokenHelper.GetAccessToken<WordpressLoginProvider>(ConsumerFactory, code);
            if (token == null) throw new ArgumentNullException("token");
            TokenHelper.SaveToken(new Token(token, AppAttr));
            return token;
        }

        public void DeleteToken(OAuth20Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            TokenHelper.DeleteToken(AppAttr);

        }
    }

    [Singletone]
    public class WordpressHelper
    {
        public ILog Log { get; set; }
        public RequestHelper RequestHelper { get; }

        public enum WordpressStatus
        {
            draft = 0,
            publish = 1
        }

        public WordpressHelper(IOptionsMonitor<ILog> optionsMonitor, RequestHelper requestHelper)
        {
            Log = optionsMonitor.CurrentValue;
            RequestHelper = requestHelper;
        }

        public string GetWordpressMeInfo(string token)
        {
            try
            {
                return WordpressLoginProvider.GetWordpressMeInfo(RequestHelper, token);
            }
            catch (Exception ex)
            {
                Log.Error("Get Wordpress info about me ", ex);
                return "";
            }

        }

        public bool CreateWordpressPost(string title, string content, int status, string blogId, OAuth20Token token)
        {
            try
            {
                var wpStatus = ((WordpressStatus)status).ToString();
                WordpressLoginProvider.CreateWordpressPost(RequestHelper, title, content, wpStatus, blogId, token);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Create Wordpress post ", ex);
                return false;
            }
        }
    }
}