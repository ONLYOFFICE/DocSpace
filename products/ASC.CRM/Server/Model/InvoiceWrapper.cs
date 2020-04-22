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
using ASC.CRM.Classes;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Api.Models;
using ASC.Web.CRM.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Api.CRM.Wrappers
{

    /// <summary>
    ///  Invoice
    /// </summary>
    [DataContract(Name = "invoiceBase", Namespace = "")]
    public class InvoiceBaseWrapper
    {
        public InvoiceBaseWrapper()
        {

        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public InvoiceStatusWrapper Status { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public ApiDateTime IssueDate { get; set; }

        [DataMember]
        public InvoiceTemplateType TemplateType { get; set; }

        [DataMember]
        public ContactBaseWithEmailWrapper Contact { get; set; }

        [DataMember]
        public ContactBaseWithEmailWrapper Consignee { get; set; }

        [DataMember]
        public EntityWrapper Entity { get; set; }

        [DataMember]
        public ApiDateTime DueDate { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public CurrencyInfoWrapper Currency { get; set; }

        [DataMember]
        public decimal ExchangeRate { get; set; }

        [DataMember]
        public string PurchaseOrderNumber { get; set; }

        [DataMember]
        public string Terms { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int FileID { get; set; }

        [DataMember]
        public ApiDateTime CreateOn { get; set; }

        [DataMember]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanDelete { get; set; }
    }

    public class InvoiceBaseWrapperHelper
    {
        public InvoiceBaseWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                           EmployeeWraperHelper employeeWraperHelper,
                           CRMSecurity cRMSecurity,
                           SettingsManager settingsManager,
                           CurrencyProvider currencyProvider,
                           InvoiceStatusWrapperHelper invoiceStatusWrapperHelper,
                           CurrencyInfoWrapperHelper currencyInfoWrapperHelper,
                           DaoFactory daoFactory,
                           ContactWrapperHelper contactWrapperHelper,
                           EntityWrapperHelper entityWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            InvoiceStatusWrapperHelper = invoiceStatusWrapperHelper;
            DaoFactory = daoFactory;
            CurrencyInfoWrapperHelper = currencyInfoWrapperHelper;
            ContactWrapperHelper = contactWrapperHelper;
            EntityWrapperHelper = entityWrapperHelper;
        }

        public EntityWrapperHelper EntityWrapperHelper { get; }
        public ContactWrapperHelper ContactWrapperHelper { get; }
        public CurrencyInfoWrapperHelper CurrencyInfoWrapperHelper { get; }
        public InvoiceStatusWrapperHelper InvoiceStatusWrapperHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public DaoFactory DaoFactory { get; }

        public InvoiceBaseWrapper Get(Invoice invoice)
        {
            
            var result = new InvoiceBaseWrapper
            {
                Id = invoice.ID,
                Status = InvoiceStatusWrapperHelper.Get(invoice.Status),
                Number = invoice.Number,
                IssueDate = ApiDateTimeHelper.Get(invoice.IssueDate),
                TemplateType = invoice.TemplateType,
                DueDate = ApiDateTimeHelper.Get(invoice.DueDate),
                Currency = !String.IsNullOrEmpty(invoice.Currency) ?
                            CurrencyInfoWrapperHelper.Get(CurrencyProvider.Get(invoice.Currency)) :
                            CurrencyInfoWrapperHelper.Get(SettingsManager.Load<CRMSettings>().DefaultCurrency),
                ExchangeRate = invoice.ExchangeRate,
                Language = invoice.Language,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                Terms = invoice.Terms,
                Description = invoice.Description,
                FileID = invoice.FileID,
                CreateOn = ApiDateTimeHelper.Get(invoice.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(invoice.CreateBy),
                CanEdit = CRMSecurity.CanEdit(invoice),
                CanDelete = CRMSecurity.CanDelete(invoice)
            };

            if (invoice.ContactID > 0)
            {
                result.Contact = ContactWrapperHelper.GetContactBaseWithEmailWrapper(DaoFactory.GetContactDao().GetByID(invoice.ContactID));
            }

            if (invoice.ConsigneeID > 0)
            {
                result.Consignee = ContactWrapperHelper.GetContactBaseWithEmailWrapper(DaoFactory.GetContactDao().GetByID(invoice.ConsigneeID));
            }

            if (invoice.EntityID > 0)
            {
                result.Entity = EntityWrapperHelper.Get(invoice.EntityType, invoice.EntityID);
            }

            result.Cost = invoice.GetInvoiceCost(DaoFactory);

            return result;

        }
    }

    public static class InvoiceBaseWrapperHelperExtension
    {
        public static DIHelper AddInvoiceBaseWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceBaseWrapperHelper>();

            return services.AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService()
                           .AddSettingsManagerService()
                           .AddCurrencyProviderService()
                           .AddInvoiceStatusWrapperHelperService();
        }
    }

    /// <summary>
    ///  Invoice
    /// </summary>
    [DataContract(Name = "invoice", Namespace = "")]
    public class InvoiceWrapper : InvoiceBaseWrapper
    {
        public InvoiceWrapper()
        {
        }

        [DataMember]
        public List<InvoiceLineWrapper> InvoiceLines { get; set; }

        public static InvoiceWrapper GetSample()
        {
            return new InvoiceWrapper
            {
                Status = InvoiceStatusWrapper.GetSample(),
                Number = string.Empty,
                IssueDate = ApiDateTime.GetSample(),
                TemplateType = InvoiceTemplateType.Eur,
                Language = string.Empty,
                DueDate = ApiDateTime.GetSample(),
                Currency = CurrencyInfoWrapper.GetSample(),
                ExchangeRate = (decimal)1.00,
                PurchaseOrderNumber = string.Empty,
                Terms = string.Empty,
                Description = string.Empty,
                FileID = -1,
                CreateOn = ApiDateTime.GetSample(),
                CreateBy = EmployeeWraper.GetSample(),
                CanEdit = true,
                CanDelete = true,
                Cost = 0,
                InvoiceLines = new List<InvoiceLineWrapper> { InvoiceLineWrapper.GetSample() }
            };
        }
    }


    public class InvoiceWrapperHelper
    {
        public InvoiceWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                                    EmployeeWraperHelper employeeWraperHelper,
                                    CRMSecurity cRMSecurity,
                                    SettingsManager settingsManager,
                                    CurrencyProvider currencyProvider,
                                    InvoiceStatusWrapperHelper invoiceStatusWrapperHelper,
                                    InvoiceLineWrapperHelper invoiceLineWrapperHelper,
                                    DaoFactory daoFactory,
                                    CurrencyInfoWrapperHelper currencyInfoWrapperHelper,
                                    CurrencyRateInfoWrapperHelper currencyRateInfoWrapperHelper,
                                    ContactWrapperHelper contactWrapperHelper,
                                    EntityWrapperHelper entityWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            InvoiceStatusWrapperHelper = invoiceStatusWrapperHelper;
            DaoFactory = daoFactory;
            InvoiceLineWrapperHelper = invoiceLineWrapperHelper;
            CurrencyInfoWrapperHelper = currencyInfoWrapperHelper;
            CurrencyRateInfoWrapperHelper = currencyRateInfoWrapperHelper;
            ContactWrapperHelper = contactWrapperHelper;
            EntityWrapperHelper = entityWrapperHelper;
        }

        public ContactWrapperHelper ContactWrapperHelper { get; }
        public CurrencyInfoWrapperHelper CurrencyInfoWrapperHelper { get; }
        public CurrencyRateInfoWrapperHelper CurrencyRateInfoWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public InvoiceLineWrapperHelper InvoiceLineWrapperHelper { get; }
        public InvoiceStatusWrapperHelper InvoiceStatusWrapperHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public CRMSecurity CRMSecurity { get; }
        public EntityWrapperHelper EntityWrapperHelper { get; }
        public InvoiceWrapper Get(Invoice invoice)
        {
            var result = new InvoiceWrapper
            {
                Id = invoice.ID,
                Status = InvoiceStatusWrapperHelper.Get(invoice.Status),
                Number = invoice.Number,
                IssueDate = ApiDateTimeHelper.Get(invoice.IssueDate),
                TemplateType = invoice.TemplateType,
                DueDate = ApiDateTimeHelper.Get(invoice.DueDate),
                Currency = !String.IsNullOrEmpty(invoice.Currency) ?
                CurrencyInfoWrapperHelper.Get(CurrencyProvider.Get(invoice.Currency)) :
                CurrencyInfoWrapperHelper.Get(SettingsManager.Load<CRMSettings>().DefaultCurrency),
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
                result.Contact = ContactWrapperHelper.GetContactBaseWithEmailWrapper(DaoFactory.GetContactDao().GetByID(invoice.ContactID));
            }

            if (invoice.ConsigneeID > 0)
            {
                result.Consignee = ContactWrapperHelper.GetContactBaseWithEmailWrapper(DaoFactory.GetContactDao().GetByID(invoice.ConsigneeID));
            }

            if (invoice.EntityID > 0)
            {
                result.Entity = EntityWrapperHelper.Get(invoice.EntityType, invoice.EntityID);
            }

            result.Cost = invoice.GetInvoiceCost(DaoFactory);

            result.InvoiceLines = invoice.GetInvoiceLines(DaoFactory).Select(x => InvoiceLineWrapperHelper.Get(x)).ToList();

            return result;

        }
    }

    public static class InvoiceWrapperHelperExtension
    {
        public static DIHelper AddInvoiceWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceWrapperHelper>();

            return services.AddCurrencyProviderService()
                           .AddSettingsManagerService()
                           .AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService()
                           .AddInvoiceStatusWrapperHelperService();
        }
    }

    /// <summary>
    ///  Invoice Item
    /// </summary>
    [DataContract(Name = "invoiceItem", Namespace = "")]
    public class InvoiceItemWrapper
    {
        public InvoiceItemWrapper()
        {
        }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string StockKeepingUnit { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public CurrencyInfoWrapper Currency { get; set; }

        [DataMember]
        public decimal StockQuantity { get; set; }

        [DataMember]
        public bool TrackInvenory { get; set; }

        [DataMember]
        public InvoiceTaxWrapper InvoiceTax1 { get; set; }

        [DataMember]
        public InvoiceTaxWrapper InvoiceTax2 { get; set; }

        [DataMember]
        public ApiDateTime CreateOn { get; set; }

        [DataMember]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanDelete { get; set; }
    }
    
    public class InvoiceItemWrapperHelper
    {
        public InvoiceItemWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                                    EmployeeWraperHelper employeeWraperHelper,
                                   CRMSecurity cRMSecurity,
                                   SettingsManager settingsManager,
                                   CurrencyProvider currencyProvider,
                                   DaoFactory daoFactory,
                                   CurrencyInfoWrapperHelper currencyInfoWrapperHelper,
                                   InvoiceTaxWrapperHelper invoiceTaxWrapperHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            DaoFactory = daoFactory;
            CurrencyInfoWrapperHelper = currencyInfoWrapperHelper;
            InvoiceTaxWrapperHelper = invoiceTaxWrapperHelper;
        }

        public InvoiceTaxWrapperHelper InvoiceTaxWrapperHelper { get; }

        public CurrencyInfoWrapperHelper CurrencyInfoWrapperHelper { get; }
        public DaoFactory DaoFactory { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public CRMSecurity CRMSecurity { get; }

        public InvoiceItemWrapper Get(InvoiceItem invoiceItem)
        {
            var result =  new InvoiceItemWrapper {                

                Title = invoiceItem.Title,
                StockKeepingUnit = invoiceItem.StockKeepingUnit,
                Description = invoiceItem.Description,
                Price = invoiceItem.Price,
                StockQuantity = invoiceItem.StockQuantity,
                TrackInvenory = invoiceItem.TrackInventory,
                CreateOn = ApiDateTimeHelper.Get(invoiceItem.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(invoiceItem.CreateBy),
                Currency = !String.IsNullOrEmpty(invoiceItem.Currency) ?
                CurrencyInfoWrapperHelper.Get(CurrencyProvider.Get(invoiceItem.Currency)) :
                CurrencyInfoWrapperHelper.Get(SettingsManager.Load<CRMSettings>().DefaultCurrency),
                CanEdit = CRMSecurity.CanEdit(invoiceItem),
                CanDelete = CRMSecurity.CanDelete(invoiceItem)
            };

            if (invoiceItem.InvoiceTax1ID > 0)
            {
                result.InvoiceTax1 = InvoiceTaxWrapperHelper.Get(DaoFactory.GetInvoiceTaxDao().GetByID(invoiceItem.InvoiceTax1ID));
            }
            if (invoiceItem.InvoiceTax2ID > 0)
            {
                result.InvoiceTax2 = InvoiceTaxWrapperHelper.Get(DaoFactory.GetInvoiceTaxDao().GetByID(invoiceItem.InvoiceTax2ID));
            }

            return result;

        }

    }

    public static class InvoiceItemWrapperHelperExtension
    {
        public static DIHelper AddInvoiceItemWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceWrapperHelper>();

            return services.AddCurrencyProviderService()
                           .AddSettingsManagerService()
                           .AddApiDateTimeHelper()
                           .AddEmployeeWraper()
                           .AddCRMSecurityService();
        }
    }

    /// <summary>
    ///  Invoice Tax
    /// </summary>
    [DataContract(Name = "invoiceTax", Namespace = "")]
    public class InvoiceTaxWrapper
    {

        public InvoiceTaxWrapper()
        {
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Rate { get; set; }

        [DataMember]
        public ApiDateTime CreateOn { get; set; }

        [DataMember]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanDelete { get; set; }
    }

    public class InvoiceTaxWrapperHelper
    {
        public InvoiceTaxWrapperHelper(ApiDateTimeHelper apiDateTimeHelper,
                                      EmployeeWraperHelper employeeWraperHelper,
                                      CRMSecurity cRMSecurity)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
        }

        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public CRMSecurity CRMSecurity { get; }

        public InvoiceTaxWrapper Get(InvoiceTax invoiceTax)
        {
            return new InvoiceTaxWrapper
            {
                Id = invoiceTax.ID,
                Name = invoiceTax.Name,
                Description = invoiceTax.Description,
                Rate = invoiceTax.Rate,
                CreateOn = ApiDateTimeHelper.Get(invoiceTax.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(invoiceTax.CreateBy),
                CanEdit = CRMSecurity.CanEdit(invoiceTax),
                CanDelete = CRMSecurity.CanDelete(invoiceTax)
            };
        }
    }

    public static class InvoiceTaxWrapperHelperExtension
    {
        public static DIHelper AddInvoiceTaxWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceTaxWrapperHelper>();

            return services;
        }
    }

    /// <summary>
    ///  Invoice Line
    /// </summary>
    [DataContract(Name = "invoiceLine", Namespace = "")]
    public class InvoiceLineWrapper
    {
        public InvoiceLineWrapper()
        {
        }


        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public int InvoiceID { get; set; }

        [DataMember]
        public int InvoiceItemID { get; set; }

        [DataMember]
        public int InvoiceTax1ID { get; set; }

        [DataMember]
        public int InvoiceTax2ID { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Quantity { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        public static InvoiceLineWrapper GetSample()
        {
            return new InvoiceLineWrapper
            {
                Description = string.Empty,
                Discount = (decimal)0.00,
                InvoiceID = 0,
                InvoiceItemID = 0,
                InvoiceTax1ID = 0,
                InvoiceTax2ID = 0,
                Price = (decimal)0.00,
                Quantity = 0
            };
        }
    }

    public class InvoiceLineWrapperHelper
    {
        public InvoiceLineWrapperHelper()
        {
        }

        public InvoiceLineWrapper Get(InvoiceLine invoiceLine)
        {
            return new InvoiceLineWrapper
            {
                Id = invoiceLine.ID,
                InvoiceID = invoiceLine.InvoiceID,
                InvoiceItemID = invoiceLine.InvoiceItemID,
                InvoiceTax1ID = invoiceLine.InvoiceTax1ID,
                InvoiceTax2ID = invoiceLine.InvoiceTax2ID,
                SortOrder = invoiceLine.SortOrder,
                Description = invoiceLine.Description,
                Quantity = invoiceLine.Quantity,
                Price = invoiceLine.Price,
                Discount = invoiceLine.Discount
            };
        }
    }

    public static class InvoiceLineWrapperHelperExtension
    {
        public static DIHelper AddInvoiceLineWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceLineWrapperHelper>();

            return services;
        }
    }

    /// <summary>
    ///  Invoice Status
    /// </summary>
    [DataContract(Name = "invoiceStatus", Namespace = "")]
    public class InvoiceStatusWrapper
    {
        public InvoiceStatusWrapper()
        {

        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        public static InvoiceStatusWrapper GetSample()
        {
            return new InvoiceStatusWrapper
            {
                Id = (int)InvoiceStatus.Draft,
                Title = InvoiceStatus.Draft.ToLocalizedString()
            };
        }

    }

    public class InvoiceStatusWrapperHelper
    {
        public InvoiceStatusWrapperHelper()
        {
        }

        public InvoiceStatusWrapper Get(InvoiceStatus status)
        {
            return new InvoiceStatusWrapper
            {
                Id = (int)status,
                Title = status.ToLocalizedString()
            };
        }
    }

    public static class InvoiceStatusWrapperHelperExtension
    {
        public static DIHelper AddInvoiceStatusWrapperHelperService(this DIHelper services)
        {
            services.TryAddTransient<InvoiceStatusWrapperHelper>();

            return services;
        }
    }
}
