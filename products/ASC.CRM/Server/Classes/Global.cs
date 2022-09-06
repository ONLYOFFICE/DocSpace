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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.CRM.Core;
using ASC.CRM.Resources;
using ASC.Data.Storage;
using ASC.Web.Core.Files;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.Configuration;

using SixLabors.ImageSharp.Formats;

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class Global
    {

        private IConfiguration _configuration;
        private SettingsManager _settingsManager;
        protected int _tenantID;
        private FilesLinkUtility _filesLinkUtility;
        private SetupInfo _setupInfo;
        private SecurityContext _securityContext;
        private StorageFactory _storageFactory;
        private CrmSecurity _crmSecurity;

        public Global(StorageFactory storageFactory,
                      SecurityContext securityContext,
                      SetupInfo setupInfo,
                      FilesLinkUtility filesLinkUtility,
                      CrmSecurity crmSecurity,
                      TenantManager tenantManager,
                      SettingsManager settingsManager,
                      IConfiguration configuration
                      )
        {
            _storageFactory = storageFactory;
            _filesLinkUtility = filesLinkUtility;
            _setupInfo = setupInfo;
            _securityContext = securityContext;
            _crmSecurity = crmSecurity;
            _tenantID = tenantManager.GetCurrentTenant().Id;
            _settingsManager = settingsManager;
            _configuration = configuration;
        }


        public static readonly int EntryCountOnPage = 25;
        public static readonly int VisiblePageCount = 10;

        public static readonly int MaxCustomFieldSize = 150;
        public static readonly int MaxCustomFieldRows = 25;
        public static readonly int MaxCustomFieldCols = 150;

        public static readonly int DefaultCustomFieldSize = 40;
        public static readonly int DefaultCustomFieldRows = 2;
        public static readonly int DefaultCustomFieldCols = 40;

        public static readonly int MaxHistoryEventCharacters = 65000;
        public static readonly decimal MaxInvoiceItemPrice = (decimal)99999999.99;


        public IDataStore GetStore()
        {
            return _storageFactory.GetStorage(_tenantID.ToString(), "crm");
        }

        public IDataStore GetStoreTemplate()
        {
            return _storageFactory.GetStorage(String.Empty, "crm_template");
        }

        public bool CanCreateProjects()
        {
            throw new NotImplementedException();

            //try
            //{
            //    var apiUrl = String.Format("{0}project/securityinfo.json", SetupInfo.WebApiBaseUrl);

            //    var cacheKey = String.Format("{0}-{1}", SecurityContext.CurrentAccount.ID, apiUrl);

            //    bool canCreateProject = false;

            //    //if (HttpRuntime.Cache[cacheKey] != null)
            //    //    return Convert.ToBoolean(HttpRuntime.Cache[cacheKey]);

            //    //var apiServer = new Api.ApiServer();

            //    //var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))))["response"];

            //    //if (responseApi.HasValues)
            //    //    canCreateProject = Convert.ToBoolean(responseApi["canCreateProject"].Value<String>());
            //    //else
            //    //    canCreateProject = false;

            //    //HttpRuntime.Cache.Remove(cacheKey);
            //    //HttpRuntime.Cache.Insert(cacheKey, canCreateProject, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
            //    //                  TimeSpan.FromMinutes(5));

            //    return canCreateProject;

            //}
            //catch
            //{
            //    return false;
            //}

        }

        public bool CanDownloadInvoices
        {
            get
            {
                var value = _configuration["crm:invoice:download:enable"];

                if (string.IsNullOrEmpty(value)) return false;

                bool canDownloadFiles = Convert.ToBoolean(value);
                if (canDownloadFiles && string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl))
                {
                    canDownloadFiles = false;
                }

                return canDownloadFiles;
            }
        }

        public bool CanCreateReports
        {
            get
            {
                return !string.IsNullOrEmpty(_filesLinkUtility.DocServiceDocbuilderUrl) && _crmSecurity.IsAdmin;
            }
        }

        public void SaveDefaultCurrencySettings(CurrencyInfo currency)
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();

            tenantSettings.DefaultCurrency = currency.Abbreviation;
            _settingsManager.Save<CrmSettings>(tenantSettings);
        }


        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static byte[] ToByteArray(Stream inputStream)
        {
            var br = new MemoryStream();
            var data = new byte[1024];
            int readed;

            while ((readed = inputStream.Read(data, 0, data.Length)) > 0)
            {
                br.Write(data, 0, readed);
            }
            br.Close();
            return br.ToArray();
        }

        public static string GetImgFormatName(IImageFormat format)
        {
            return format.Name.ToLower();
        }

        private static readonly string[] Formats = new[]
                                                       {
                                                           "o",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                                                           "yyyy-MM-ddTHH:mm:ss"
                                                       };

        public static DateTime ApiDateTimeParse(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");

            if (data.Length < 7) throw new ArgumentException(CRMErrorsResource.DateTimeFormatInvalid);

            DateTime dateTime;
            if (DateTime.TryParseExact(data, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                return new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);
            }
            throw new ArgumentException(CRMErrorsResource.DateTimeFormatInvalid);
        }

        public static JsonDocument JObjectParseWithDateAsString(string data)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(data));

            Utf8JsonReader reader = new Utf8JsonReader(readOnlySpan);

            JsonDocument result;

            if (JsonDocument.TryParseValue(ref reader, out result))
            {
                return result;
            }

            return null;
        }
    }
}