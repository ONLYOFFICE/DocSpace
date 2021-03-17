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

using ASC.Api.Core;
using ASC.Common;
using ASC.Core.Common.Settings;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Web.Api.Models;
using ASC.Web.CRM.Classes;

using System;

namespace ASC.CRM.ApiModels
{
    [Scope]
    public class InvoiceBaseDtoHelper
    {
        private EntityDtoHelper _entityDtoHelper;
        private ContactDtoHelper _contactDtoHelper;
        private CurrencyInfoDtoHelper _currencyInfoDtoHelper;
        private InvoiceStatusDtoHelper _invoiceStatusDtoHelper;
        private CurrencyProvider _currencyProvider;
        private SettingsManager _settingsManager;
        private CRMSecurity CRMSecurity;
        private ApiDateTimeHelper _apiDateTimeHelper;
        private EmployeeWraperHelper _employeeWraperHelper;
        private DaoFactory _daoFactory;

        public InvoiceBaseDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           SettingsManager settingsManager,
                           CurrencyProvider currencyProvider,
                           InvoiceStatusDtoHelper invoiceStatusDtoHelper,
                           CurrencyInfoDtoHelper currencyInfoDtoHelper,
                           DaoFactory daoFactory,
                           ContactDtoHelper contactDtoHelper,
                           EntityDtoHelper entityDtoHelper)
        {
            _apiDateTimeHelper = apiDateTimeHelper;
            _employeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            _settingsManager = settingsManager;
            _currencyProvider = currencyProvider;
            _invoiceStatusDtoHelper = invoiceStatusDtoHelper;
            _daoFactory = daoFactory;
            _currencyInfoDtoHelper = currencyInfoDtoHelper;
            _contactDtoHelper = contactDtoHelper;
            _entityDtoHelper = entityDtoHelper;
        }  

        public InvoiceBaseDto Get(Invoice invoice)
        {
            
            var result = new InvoiceBaseDto
            {
                Id = invoice.ID,
                Status = _invoiceStatusDtoHelper.Get(invoice.Status),
                Number = invoice.Number,
                IssueDate = _apiDateTimeHelper.Get(invoice.IssueDate),
                TemplateType = invoice.TemplateType,
                DueDate = _apiDateTimeHelper.Get(invoice.DueDate),
                Currency = !String.IsNullOrEmpty(invoice.Currency) ?
                            _currencyInfoDtoHelper.Get(_currencyProvider.Get(invoice.Currency)) :
                            _currencyInfoDtoHelper.Get(_settingsManager.Load<CRMSettings>().DefaultCurrency),
                ExchangeRate = invoice.ExchangeRate,
                Language = invoice.Language,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                Terms = invoice.Terms,
                Description = invoice.Description,
                FileID = invoice.FileID,
                CreateOn = _apiDateTimeHelper.Get(invoice.CreateOn),
                CreateBy = _employeeWraperHelper.Get(invoice.CreateBy),
                CanEdit = CRMSecurity.CanEdit(invoice),
                CanDelete = CRMSecurity.CanDelete(invoice)
            };

            if (invoice.ContactID > 0)
            {
                result.Contact = _contactDtoHelper.GetContactBaseWithEmailDto(_daoFactory.GetContactDao().GetByID(invoice.ContactID));
            }

            if (invoice.ConsigneeID > 0)
            {
                result.Consignee = _contactDtoHelper.GetContactBaseWithEmailDto(_daoFactory.GetContactDao().GetByID(invoice.ConsigneeID));
            }

            if (invoice.EntityID > 0)
            {
                result.Entity = _entityDtoHelper.Get(invoice.EntityType, invoice.EntityID);
            }

            result.Cost = invoice.GetInvoiceCost(_daoFactory);

            return result;

        }
    }
}
