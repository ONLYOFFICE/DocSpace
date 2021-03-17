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
using System.Linq;

namespace ASC.CRM.ApiModels
{
    [Scope]
    public class InvoiceDtoHelper
    {
        public InvoiceDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                                    EmployeeWraperHelper employeeWraperHelper,
                                    CRMSecurity cRMSecurity,
                                    SettingsManager settingsManager,
                                    CurrencyProvider currencyProvider,
                                    InvoiceStatusDtoHelper invoiceStatusDtoHelper,
                                    InvoiceLineDtoHelper invoiceLineDtoHelper,
                                    DaoFactory daoFactory,
                                    CurrencyInfoDtoHelper currencyInfoDtoHelper,
                                    CurrencyRateInfoDtoHelper currencyRateInfoDtoHelper,
                                    ContactDtoHelper contactDtoHelper,
                                    EntityDtoHelper entityDtoHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            InvoiceStatusDtoHelper = invoiceStatusDtoHelper;
            DaoFactory = daoFactory;
            InvoiceLineDtoHelper = invoiceLineDtoHelper;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
            CurrencyRateInfoDtoHelper = currencyRateInfoDtoHelper;
            ContactDtoHelper = contactDtoHelper;
            EntityDtoHelper = entityDtoHelper;
        }

        public ContactDtoHelper ContactDtoHelper { get; }
        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper { get; }
        public CurrencyRateInfoDtoHelper CurrencyRateInfoDtoHelper { get; }
        public DaoFactory DaoFactory { get; }
        public InvoiceLineDtoHelper InvoiceLineDtoHelper { get; }
        public InvoiceStatusDtoHelper InvoiceStatusDtoHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public CRMSecurity CRMSecurity { get; }
        public EntityDtoHelper EntityDtoHelper { get; }
        public InvoiceDto Get(Invoice invoice)
        {
            var result = new InvoiceDto
            {
                Id = invoice.ID,
                Status = InvoiceStatusDtoHelper.Get(invoice.Status),
                Number = invoice.Number,
                IssueDate = ApiDateTimeHelper.Get(invoice.IssueDate),
                TemplateType = invoice.TemplateType,
                DueDate = ApiDateTimeHelper.Get(invoice.DueDate),
                Currency = !String.IsNullOrEmpty(invoice.Currency) ?
                CurrencyInfoDtoHelper.Get(CurrencyProvider.Get(invoice.Currency)) :
                CurrencyInfoDtoHelper.Get(SettingsManager.Load<CRMSettings>().DefaultCurrency),
                ExchangeRate = invoice.ExchangeRate,
                Language = invoice.Language,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                Terms = invoice.Terms,
                Description = invoice.Description,
                FileID = invoice.FileID,
                CreateOn = ApiDateTimeHelper.Get(invoice.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(invoice.CreateBy),
                CanEdit = CRMSecurity.CanEdit(invoice),
                CanDelete = CRMSecurity.CanDelete(invoice),
            };
                        
            if (invoice.ContactID > 0)
            {
                result.Contact = ContactDtoHelper.GetContactBaseWithEmailDto(DaoFactory.GetContactDao().GetByID(invoice.ContactID));
            }

            if (invoice.ConsigneeID > 0)
            {
                result.Consignee = ContactDtoHelper.GetContactBaseWithEmailDto(DaoFactory.GetContactDao().GetByID(invoice.ConsigneeID));
            }

            if (invoice.EntityID > 0)
            {
                result.Entity = EntityDtoHelper.Get(invoice.EntityType, invoice.EntityID);
            }

            result.Cost = invoice.GetInvoiceCost(DaoFactory);

            result.InvoiceLines = invoice.GetInvoiceLines(DaoFactory).Select(x => InvoiceLineDtoHelper.Get(x)).ToList();

            return result;

        }
    }
}
