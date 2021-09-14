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
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;

using Microsoft.Extensions.Configuration;

namespace ASC.Web.Files.Helpers
{
    [Scope(Additional = typeof(BaseLoginProviderExtension))]
    public class ThirdpartyConfiguration
    {
        private IConfiguration Configuration { get; }
        private Lazy<BoxLoginProvider> BoxLoginProvider { get; }
        private Lazy<DropboxLoginProvider> DropboxLoginProvider { get; }
        private Lazy<OneDriveLoginProvider> OneDriveLoginProvider { get; }
        private Lazy<DocuSignLoginProvider> DocuSignLoginProvider { get; }
        private Lazy<GoogleLoginProvider> GoogleLoginProvider { get; }

        public ThirdpartyConfiguration(
            IConfiguration configuration,
            ConsumerFactory consumerFactory)
        {
            Configuration = configuration;
            BoxLoginProvider = new Lazy<BoxLoginProvider>(() => consumerFactory.Get<BoxLoginProvider>());
            DropboxLoginProvider = new Lazy<DropboxLoginProvider>(() => consumerFactory.Get<DropboxLoginProvider>());
            OneDriveLoginProvider = new Lazy<OneDriveLoginProvider>(() => consumerFactory.Get<OneDriveLoginProvider>());
            DocuSignLoginProvider = new Lazy<DocuSignLoginProvider>(() => consumerFactory.Get<DocuSignLoginProvider>());
            GoogleLoginProvider = new Lazy<GoogleLoginProvider>(() => consumerFactory.Get<GoogleLoginProvider>());
        }

        private IEnumerable<string> thirdPartyProviders;
        public IEnumerable<string> ThirdPartyProviders
        {
            get
            {
                return thirdPartyProviders ??= (Configuration.GetSection("files:thirdparty:enable").Get<string[]>() ?? new string[] { }).ToList();
            }
        }

        public bool SupportInclusion(IDaoFactory daoFactory)
        {
            var providerDao = daoFactory.ProviderDao;
            if (providerDao == null) return false;

            return SupportBoxInclusion || SupportDropboxInclusion || SupportDocuSignInclusion || SupportGoogleDriveInclusion || SupportOneDriveInclusion || SupportSharePointInclusion || SupportWebDavInclusion || SupportNextcloudInclusion || SupportOwncloudInclusion || SupportkDriveInclusion || SupportYandexInclusion;
        }

        public bool SupportBoxInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("box") && BoxLoginProvider.Value.IsEnabled;
            }
        }

        public bool SupportDropboxInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("dropboxv2") && DropboxLoginProvider.Value.IsEnabled;
            }
        }

        public bool SupportOneDriveInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("onedrive") && OneDriveLoginProvider.Value.IsEnabled;
            }
        }

        public bool SupportSharePointInclusion
        {
            get { return ThirdPartyProviders.Contains("sharepoint"); }
        }

        public bool SupportWebDavInclusion
        {
            get { return ThirdPartyProviders.Contains("webdav"); }
        }

        public bool SupportNextcloudInclusion
        {
            get { return ThirdPartyProviders.Contains("nextcloud"); }
        }

        public bool SupportOwncloudInclusion
        {
            get { return ThirdPartyProviders.Contains("owncloud"); }
        }

        public bool SupportkDriveInclusion
        {
            get { return ThirdPartyProviders.Contains("kdrive"); }
        }

        public bool SupportYandexInclusion
        {
            get { return ThirdPartyProviders.Contains("yandex"); }
        }

        public string DropboxAppKey
        {
            get { return DropboxLoginProvider.Value["dropboxappkey"]; }
        }

        public string DropboxAppSecret
        {
            get { return DropboxLoginProvider.Value["dropboxappsecret"]; }
        }

        public bool SupportDocuSignInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("docusign") && DocuSignLoginProvider.Value.IsEnabled;
            }
        }

        public bool SupportGoogleDriveInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("google") && GoogleLoginProvider.Value.IsEnabled;
            }
        }

        public List<List<string>> GetProviders()
        {
            var result = new List<List<string>>();

            if (SupportBoxInclusion)
            {
                result.Add(new List<string> { "Box", BoxLoginProvider.Value.ClientID, BoxLoginProvider.Value.RedirectUri });
            }

            if (SupportDropboxInclusion)
            {
                result.Add(new List<string> { "DropboxV2", DropboxLoginProvider.Value.ClientID, DropboxLoginProvider.Value.RedirectUri });
            }

            if (SupportGoogleDriveInclusion)
            {
                result.Add(new List<string> { "GoogleDrive", GoogleLoginProvider.Value.ClientID, GoogleLoginProvider.Value.RedirectUri });
            }

            if (SupportOneDriveInclusion)
            {
                result.Add(new List<string> { "OneDrive", OneDriveLoginProvider.Value.ClientID, OneDriveLoginProvider.Value.RedirectUri });
            }

            if (SupportSharePointInclusion)
            {
                result.Add(new List<string> { "SharePoint" });
            }

            if (SupportkDriveInclusion)
            {
                result.Add(new List<string> { "kDrive" });
            }

            if (SupportYandexInclusion)
            {
                result.Add(new List<string> { "Yandex" });
            }

            if (SupportWebDavInclusion)
            {
                result.Add(new List<string> { "WebDav" });
            }

            //Obsolete BoxNet, DropBox, Google, SkyDrive,

            return result;
        }
    }
}