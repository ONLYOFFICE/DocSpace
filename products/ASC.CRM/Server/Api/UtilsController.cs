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
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Api.CRM;
using ASC.Common.Threading.Progress;
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Utility;
using ASC.Web.CRM.Classes;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class UtilsController : BaseApiController
    {
        private readonly ExportToCsv _exportToCsv;
        private readonly ImportFromCSV _importFromCSV;
        private readonly Global _global;
        private readonly OrganisationLogoManager _organisationLogoManager;
        private readonly ImportFromCSVManager _importFromCSVManager;
        private readonly InvoiceSetting _invoiceSetting;
        private readonly SettingsManager _settingsManager;
        private readonly CurrencyProvider _currencyProvider;
        private readonly MessageService _messageService;

        public UtilsController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     MessageService messageService,
                     SettingsManager settingsManager,
                     CurrencyProvider currencyProvider,
                     InvoiceSetting invoiceSetting,
                     ImportFromCSVManager importFromCSVManager,
                     OrganisationLogoManager organisationLogoManager,
                     Global global,
                     ImportFromCSV importFromCSV,
                     ExportToCsv exportToCsv,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _messageService = messageService;
            _currencyProvider = currencyProvider;
            _settingsManager = settingsManager;
            _invoiceSetting = invoiceSetting;
            _importFromCSVManager = importFromCSVManager;
            _organisationLogoManager = organisationLogoManager;
            _global = global;
            _importFromCSV = importFromCSV;
            _exportToCsv = exportToCsv;
        }

        /// <summary>
        ///     Returns the list of all currencies currently available on the portal
        /// </summary>
        /// <short>Get currency list</short> 
        /// <category>Common</category>
        /// <returns>
        ///    List of available currencies
        /// </returns>
        [Read(@"settings/currency")]
        public IEnumerable<CurrencyInfoDto> GetAvaliableCurrency()
        {
            return _currencyProvider.GetAll().ConvertAll(item => _mapper.Map<CurrencyInfoDto>(item));
        }

        /// <summary>
        ///     Returns the result of convertation from one currency to another
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromcurrency">Old currency key</param>
        /// <param name="tocurrency">New currency key</param>
        /// <short>Get the result of convertation</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Decimal result of convertation
        /// </returns>
        [Read(@"settings/currency/convert")]
        public Decimal ConvertAmount(Decimal amount, String fromcurrency, String tocurrency)
        {
            return _currencyProvider.MoneyConvert(amount, fromcurrency, tocurrency);
        }

        /// <summary>
        ///     Returns the summary table with rates for selected currency
        /// </summary>
        /// <param name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Currency (Abbreviation)</param>
        /// <short>Get the summary table</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Dictionary of currencies and rates
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        [Read(@"settings/currency/summarytable")]
        public IEnumerable<CurrencyRateInfoDto> GetSummaryTable(String currency)
        {
            var result = new List<CurrencyRateInfoDto>();

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentException();
            }

            var cur = _currencyProvider.Get(currency.ToUpper());

            if (cur == null) throw new ArgumentException();

            var table = _currencyProvider.MoneyConvert(cur).ToList();

            foreach (var row in table)
            {
                var currencyInfoDto = _mapper.Map<CurrencyRateInfoDto>(row.Key);

                currencyInfoDto.Rate = row.Value;

                result.Add(currencyInfoDto);
            }

            return result;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="changeContactStatusGroupAuto" remark="true, false or null">Change contact status group auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    ChangeContactStatusGroupAuto setting value (true, false or null)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/status/settings")]
        public Boolean? UpdateCRMContactStatusSettings(Boolean? changeContactStatusGroupAuto)
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();

            tenantSettings.ChangeContactStatusGroupAuto = changeContactStatusGroupAuto;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.ContactTemperatureLevelSettingsUpdated);

            return changeContactStatusGroupAuto;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="writeMailToHistoryAuto" remark="true or false">Write mail to history auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    WriteMailToHistoryAuto setting value (true or false)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/mailtohistory/settings")]
        public Boolean UpdateCRMWriteMailToHistorySettings(Boolean writeMailToHistoryAuto)
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();

            tenantSettings.WriteMailToHistoryAuto = writeMailToHistoryAuto;

            _settingsManager.Save<CrmSettings>(tenantSettings);
            //MessageService.Send( MessageAction.ContactTemperatureLevelSettingsUpdated);

            return writeMailToHistoryAuto;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="addTagToContactGroupAuto" remark="true, false or null">add tag to contact group auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    AddTagToContactGroupAuto setting value (true, false or null)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/tag/settings")]
        public Boolean? UpdateCRMContactTagSettings(Boolean? addTagToContactGroupAuto)
        {
            var tenantSettings = _settingsManager.Load<CrmSettings>();
            tenantSettings.AddTagToContactGroupAuto = addTagToContactGroupAuto;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.ContactsTagSettingsUpdated);

            return addTagToContactGroupAuto;
        }


        /// <summary>
        ///    Set IsConfiguredPortal tenant setting and website contact form key specified in the request
        /// </summary>
        /// <short>Set tenant settings</short> 
        /// <category>Common</category>
        /// <returns>
        ///    IsConfiguredPortal setting value (true or false)
        /// </returns>
        [Update(@"settings")]
        public Boolean SetIsPortalConfigured(Boolean? configured, Guid? webFormKey)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var tenantSettings = _settingsManager.Load<CrmSettings>();

            tenantSettings.IsConfiguredPortal = configured ?? true;
            tenantSettings.WebFormKey = webFormKey ?? Guid.NewGuid();

            _settingsManager.Save<CrmSettings>(tenantSettings);

            return tenantSettings.IsConfiguredPortal;
        }

        /// <summary>
        ///  Save organisation company name
        /// </summary>
        /// <param name="companyName">Organisation company name</param>
        /// <short>Save organisation company name</short>
        /// <category>Organisation</category>
        /// <returns>Organisation company name</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/organisation/base")]
        public String UpdateOrganisationSettingsCompanyName(String companyName)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var tenantSettings = _settingsManager.Load<CrmSettings>();

            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = _invoiceSetting.DefaultSettings;
            }
            tenantSettings.InvoiceSetting.CompanyName = companyName;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.OrganizationProfileUpdatedCompanyName, companyName);

            return companyName;
        }

        /// <summary>
        ///  Save organisation company address
        /// </summary>
        /// <param name="street">Organisation company street/building/apartment address</param>
        /// <param name="city">City</param>
        /// <param name="state">State</param>
        /// <param name="zip">Zip</param>
        /// <param name="country">Country</param>
        /// <short>Save organisation company address</short>
        /// <category>Organisation</category>
        /// <returns>Returns a JSON object with the organization company address details</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/organisation/address")]
        public String UpdateOrganisationSettingsCompanyAddress(String street, String city, String state, String zip, String country)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var tenantSettings = _settingsManager.Load<CrmSettings>();

            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = _invoiceSetting.DefaultSettings;
            }

            var companyAddress = JsonSerializer.Serialize(new
            {
                type = nameof(AddressCategory.Billing),
                street,
                city,
                state,
                zip,
                country
            });

            tenantSettings.InvoiceSetting.CompanyAddress = companyAddress;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.OrganizationProfileUpdatedAddress);

            return companyAddress;
        }

        /// <summary>
        ///  Save organisation logo
        /// </summary>
        /// <param name="reset">Reset organisation logo</param>
        /// <short>Save organisation logo</short>
        /// <category>Organisation</category>
        /// <returns>Organisation logo ID</returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="Exception"></exception>
        [Update(@"settings/organisation/logo")]
        public Task<Int32> UpdateOrganisationSettingsLogoAsync(bool reset)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return InternalUpdateOrganisationSettingsLogoAsync(reset);
        }

        private async Task<Int32> InternalUpdateOrganisationSettingsLogoAsync(bool reset)
        {
            int companyLogoID;

            if (!reset)
            {
                companyLogoID = await _organisationLogoManager.TryUploadOrganisationLogoFromTmpAsync(_daoFactory);
                if (companyLogoID == 0)
                {
                    throw new Exception("Downloaded image not found");
                }
            }
            else
            {
                companyLogoID = 0;
            }

            var tenantSettings = _settingsManager.Load<CrmSettings>();

            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = _invoiceSetting.DefaultSettings;
            }
            tenantSettings.InvoiceSetting.CompanyLogoID = companyLogoID;

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.OrganizationProfileUpdatedInvoiceLogo);

            return companyLogoID;
        }

        /// <summary>
        ///  Get organisation logo in base64 format  (if 'id' is 0 then take current logo)
        /// </summary>
        /// <param name="id">organisation logo id</param>
        /// <short>Get organisation logo</short>
        /// <category>Organisation</category>
        /// <returns>Organisation logo content in base64</returns>
        /// <exception cref="Exception"></exception>
        [Read(@"settings/organisation/logo")]
        public String GetOrganisationSettingsLogo(int id)
        {
            if (id != 0)
            {
                return _organisationLogoManager.GetOrganisationLogoBase64(id);
            }
            else
            {
                var tenantSettings = _settingsManager.Load<CrmSettings>();

                if (tenantSettings.InvoiceSetting == null)
                {
                    return string.Empty;
                }

                return _organisationLogoManager.GetOrganisationLogoBase64(tenantSettings.InvoiceSetting.CompanyLogoID);
            }
        }

        /// <summary>
        ///  Change Website Contact Form key
        /// </summary>
        /// <short>Change web form key</short>
        /// <category>Common</category>
        /// <returns>Web form key</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/webformkey/change")]
        public string ChangeWebToLeadFormKey()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var tenantSettings = _settingsManager.Load<CrmSettings>();

            tenantSettings.WebFormKey = Guid.NewGuid();

            _settingsManager.Save<CrmSettings>(tenantSettings);

            _messageService.Send(MessageAction.WebsiteContactFormUpdatedKey);

            return tenantSettings.WebFormKey.ToString();
        }

        /// <summary>
        ///  Change default CRM currency
        /// </summary>
        /// <param name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Currency (Abbreviation)</param>
        /// <short>Change currency</short>
        /// <category>Common</category>
        /// <returns>currency</returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [Update(@"settings/currency")]
        public CurrencyInfoDto UpdateCRMCurrency(String currency)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentException();
            }
            currency = currency.ToUpper();
            var cur = _currencyProvider.Get(currency);
            if (cur == null) throw new ArgumentException();

            _global.SaveDefaultCurrencySettings(cur);
            _messageService.Send(MessageAction.CrmDefaultCurrencyUpdated);

            return _mapper.Map<CurrencyInfoDto>(cur);
        }

        /// <visible>false</visible>
        [Create(@"{entityType:regex(contact|opportunity|case|task)}/import/start")]
        public string StartImportFromCSV(
            [FromRoute] string entityType, 
            [FromBody] StartImportFromCSVRequestDto inDto )
        {
            var csvFileURI = inDto.CsvFileURI;
            var jsonSettings = inDto.JsonSettings;

            EntityType entityTypeObj;

            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(csvFileURI) || string.IsNullOrEmpty(jsonSettings)) throw new ArgumentException();
            switch (entityType.ToLower())
            {
                case "contact":
                    entityTypeObj = EntityType.Contact;
                    break;
                case "opportunity":
                    entityTypeObj = EntityType.Opportunity;
                    break;
                case "case":
                    entityTypeObj = EntityType.Case;
                    break;
                case "task":
                    entityTypeObj = EntityType.Task;
                    break;
                default:
                    throw new ArgumentException();
            }

            _importFromCSVManager.StartImport(entityTypeObj, csvFileURI, jsonSettings);

            return "";

        }

        /// <visible>false</visible>
        [Read(@"{entityType:regex(contact|opportunity|case|task)}/import/status")]
        public IProgressItem GetImportFromCSVStatus(string entityType)
        {
            EntityType entityTypeObj;

            if (string.IsNullOrEmpty(entityType)) throw new ArgumentException();
            switch (entityType.ToLower())
            {
                case "contact":
                    entityTypeObj = EntityType.Contact;
                    break;
                case "opportunity":
                    entityTypeObj = EntityType.Opportunity;
                    break;
                case "case":
                    entityTypeObj = EntityType.Case;
                    break;
                case "task":
                    entityTypeObj = EntityType.Task;
                    break;
                default:
                    throw new ArgumentException();
            }

            return _importFromCSV.GetStatus(entityTypeObj);
        }

        /// <visible>false</visible>
        [Read(@"import/samplerow")]
        public Task<String> GetImportFromCSVSampleRowAsync(string csvFileURI, int indexRow, string jsonSettings)
        {
            if (String.IsNullOrEmpty(csvFileURI) || indexRow < 0) throw new ArgumentException();

            return InternalGetImportFromCSVSampleRowAsync(csvFileURI, indexRow, jsonSettings);
        }

        private async Task<String> InternalGetImportFromCSVSampleRowAsync(string csvFileURI, int indexRow, string jsonSettings)
        {
            if (!await _global.GetStore().IsFileAsync("temp", csvFileURI)) throw new ArgumentException();

            var CSVFileStream = await _global.GetStore().GetReadStreamAsync("temp", csvFileURI);

            return _importFromCSV.GetRow(CSVFileStream, indexRow, jsonSettings);
        }

        /// <visible>false</visible>
        [Create(@"import/uploadfake")]
        public Task<FileUploadResult> ProcessUploadFakeAsync([FromBody] ProcessUploadFakeRequestDto inDto)
        {
            var csvFileURI = inDto.CsvFileURI;
            var jsonSettings = inDto.JsonSettings;

            return _importFromCSVManager.ProcessUploadFakeAsync(csvFileURI, jsonSettings);
        }

        /// <visible>false</visible>
        [Read(@"export/status")]
        public IProgressItem GetExportStatus()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return _exportToCsv.GetStatus(false);

        }

        /// <visible>false</visible>
        [Update(@"export/cancel")]
        public IProgressItem CancelExport()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            _exportToCsv.Cancel(false);

            return _exportToCsv.GetStatus(false);

        }

        /// <visible>false</visible>
        [Create(@"export/start")]
        public IProgressItem StartExport()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            _messageService.Send(MessageAction.CrmAllDataExported);

            return _exportToCsv.Start(null, CRMSettingResource.Export + ".zip");
        }

        /// <visible>false</visible>
        [Read(@"export/partial/status")]
        public IProgressItem GetPartialExportStatus()
        {
            return _exportToCsv.GetStatus(true);
        }

        /// <visible>false</visible>
        [Update(@"export/partial/cancel")]
        public IProgressItem CancelPartialExport()
        {

            _exportToCsv.Cancel(true);

            return _exportToCsv.GetStatus(true);

        }

        /// <visible>false</visible>
        [Create(@"export/partial/{entityType:regex(contact|opportunity|case|task|invoiceitem)}/start")]
        public IProgressItem StartPartialExport([FromRoute] string entityType, [FromBody] string base64FilterString)
        {
            if (string.IsNullOrEmpty(base64FilterString)) throw new ArgumentException();

            FilterObject filterObject;
            String fileName;

            switch (entityType.ToLower())
            {
                case "contact":
                    filterObject = new ContactFilterObject(base64FilterString);
                    fileName = CRMContactResource.Contacts + ".csv";
                    _messageService.Send(MessageAction.ContactsExportedToCsv);
                    break;
                case "opportunity":
                    filterObject = new DealFilterObject(base64FilterString);
                    fileName = CRMCommonResource.DealModuleName + ".csv";
                    _messageService.Send(MessageAction.OpportunitiesExportedToCsv);
                    break;
                case "case":
                    filterObject = new CasesFilterObject(base64FilterString);
                    fileName = CRMCommonResource.CasesModuleName + ".csv";
                    _messageService.Send(MessageAction.CasesExportedToCsv);
                    break;
                case "task":
                    filterObject = new TaskFilterObject(base64FilterString);
                    fileName = CRMCommonResource.TaskModuleName + ".csv";
                    _messageService.Send(MessageAction.CrmTasksExportedToCsv);
                    break;
                case "invoiceitem":
                    fileName = CRMCommonResource.ProductsAndServices + ".csv";
                    filterObject = new InvoiceItemFilterObject(base64FilterString);
                    break;
                default:
                    throw new ArgumentException();
            }

            return _exportToCsv.Start(filterObject, fileName);
        }
    }
}