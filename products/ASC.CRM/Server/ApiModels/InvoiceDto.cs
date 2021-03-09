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

namespace ASC.CRM.ApiModels
{

    /// <summary>
    ///  Invoice
    /// </summary>
    [DataContract(Name = "invoiceBase", Namespace = "")]
    public class InvoiceBaseDto
    {
        public InvoiceBaseDto()
        {

        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public InvoiceStatusDto Status { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public ApiDateTime IssueDate { get; set; }

        [DataMember]
        public InvoiceTemplateType TemplateType { get; set; }

        [DataMember]
        public ContactBaseWithEmailDto Contact { get; set; }

        [DataMember]
        public ContactBaseWithEmailDto Consignee { get; set; }

        [DataMember]
        public EntityDto Entity { get; set; }

        [DataMember]
        public ApiDateTime DueDate { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public CurrencyInfoDto Currency { get; set; }

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

    [Scope]
    public class InvoiceBaseDtoHelper
    {
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
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            InvoiceStatusDtoHelper = invoiceStatusDtoHelper;
            DaoFactory = daoFactory;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
            ContactDtoHelper = contactDtoHelper;
            EntityDtoHelper = entityDtoHelper;
        }

        public EntityDtoHelper EntityDtoHelper { get; }
        public ContactDtoHelper ContactDtoHelper { get; }
        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper { get; }
        public InvoiceStatusDtoHelper InvoiceStatusDtoHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public CRMSecurity CRMSecurity { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public DaoFactory DaoFactory { get; }

        public InvoiceBaseDto Get(Invoice invoice)
        {
            
            var result = new InvoiceBaseDto
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
                CanDelete = CRMSecurity.CanDelete(invoice)
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

            return result;

        }
    }

    /// <summary>
    ///  Invoice
    /// </summary>
    [DataContract(Name = "invoice", Namespace = "")]
    public class InvoiceDto : InvoiceBaseDto
    {
        public InvoiceDto()
        {
        }

        [DataMember]
        public List<InvoiceLineDto> InvoiceLines { get; set; }

        public static InvoiceDto GetSample()
        {
            return new InvoiceDto
            {
                Status = InvoiceStatusDto.GetSample(),
                Number = string.Empty,
                IssueDate = ApiDateTime.GetSample(),
                TemplateType = InvoiceTemplateType.Eur,
                Language = string.Empty,
                DueDate = ApiDateTime.GetSample(),
                Currency = CurrencyInfoDto.GetSample(),
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
                InvoiceLines = new List<InvoiceLineDto> { InvoiceLineDto.GetSample() }
            };
        }
    }

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

    /// <summary>
    ///  Invoice Item
    /// </summary>
    [DataContract(Name = "invoiceItem", Namespace = "")]
    public class InvoiceItemDto
    {
        public InvoiceItemDto()
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
        public CurrencyInfoDto Currency { get; set; }

        [DataMember]
        public decimal StockQuantity { get; set; }

        [DataMember]
        public bool TrackInvenory { get; set; }

        [DataMember]
        public InvoiceTaxDto InvoiceTax1 { get; set; }

        [DataMember]
        public InvoiceTaxDto InvoiceTax2 { get; set; }

        [DataMember]
        public ApiDateTime CreateOn { get; set; }

        [DataMember]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanDelete { get; set; }
    }
    
    [Scope]
    public class InvoiceItemDtoHelper
    {
        public InvoiceItemDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
                                    EmployeeWraperHelper employeeWraperHelper,
                                   CRMSecurity cRMSecurity,
                                   SettingsManager settingsManager,
                                   CurrencyProvider currencyProvider,
                                   DaoFactory daoFactory,
                                   CurrencyInfoDtoHelper currencyInfoDtoHelper,
                                   InvoiceTaxDtoHelper invoiceTaxDtoHelper)
        {
            ApiDateTimeHelper = apiDateTimeHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            CRMSecurity = cRMSecurity;
            SettingsManager = settingsManager;
            CurrencyProvider = currencyProvider;
            DaoFactory = daoFactory;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
            InvoiceTaxDtoHelper = invoiceTaxDtoHelper;
        }

        public InvoiceTaxDtoHelper InvoiceTaxDtoHelper { get; }

        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper { get; }
        public DaoFactory DaoFactory { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public SettingsManager SettingsManager { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public CRMSecurity CRMSecurity { get; }

        public InvoiceItemDto Get(InvoiceItem invoiceItem)
        {
            var result =  new InvoiceItemDto {                

                Title = invoiceItem.Title,
                StockKeepingUnit = invoiceItem.StockKeepingUnit,
                Description = invoiceItem.Description,
                Price = invoiceItem.Price,
                StockQuantity = invoiceItem.StockQuantity,
                TrackInvenory = invoiceItem.TrackInventory,
                CreateOn = ApiDateTimeHelper.Get(invoiceItem.CreateOn),
                CreateBy = EmployeeWraperHelper.Get(invoiceItem.CreateBy),
                Currency = !String.IsNullOrEmpty(invoiceItem.Currency) ?
                CurrencyInfoDtoHelper.Get(CurrencyProvider.Get(invoiceItem.Currency)) :
                CurrencyInfoDtoHelper.Get(SettingsManager.Load<CRMSettings>().DefaultCurrency),
                CanEdit = CRMSecurity.CanEdit(invoiceItem),
                CanDelete = CRMSecurity.CanDelete(invoiceItem)
            };

            if (invoiceItem.InvoiceTax1ID > 0)
            {
                result.InvoiceTax1 = InvoiceTaxDtoHelper.Get(DaoFactory.GetInvoiceTaxDao().GetByID(invoiceItem.InvoiceTax1ID));
            }
            if (invoiceItem.InvoiceTax2ID > 0)
            {
                result.InvoiceTax2 = InvoiceTaxDtoHelper.Get(DaoFactory.GetInvoiceTaxDao().GetByID(invoiceItem.InvoiceTax2ID));
            }

            return result;

        }

    }

    //public static class InvoiceItemDtoHelperExtension
    //{
    //    public static DIHelper AddInvoiceItemDtoHelperService(this DIHelper services)
    //    {
    //        services.TryAddTransient<InvoiceDtoHelper>();
    //        return services.AddCurrencyProviderService()
    //                       .AddSettingsManagerService()
    //                       .AddApiDateTimeHelper()
    //                       .AddEmployeeWraper()
    //                       .AddCRMSecurityService();
    //    }
    //}

    /// <summary>
    ///  Invoice Tax
    /// </summary>
    [DataContract(Name = "invoiceTax", Namespace = "")]
    public class InvoiceTaxDto
    {

        public InvoiceTaxDto()
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

    [Scope]
    public class InvoiceTaxDtoHelper
    {
        public InvoiceTaxDtoHelper(ApiDateTimeHelper apiDateTimeHelper,
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

        public InvoiceTaxDto Get(InvoiceTax invoiceTax)
        {
            return new InvoiceTaxDto
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

    /// <summary>
    ///  Invoice Line
    /// </summary>
    [DataContract(Name = "invoiceLine", Namespace = "")]
    public class InvoiceLineDto
    {
        public InvoiceLineDto()
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

        public static InvoiceLineDto GetSample()
        {
            return new InvoiceLineDto
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

    [Singletone]
    public class InvoiceLineDtoHelper
    {
        public InvoiceLineDtoHelper()
        {
        }

        public InvoiceLineDto Get(InvoiceLine invoiceLine)
        {
            return new InvoiceLineDto
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

    /// <summary>
    ///  Invoice Status
    /// </summary>
    [DataContract(Name = "invoiceStatus", Namespace = "")]
    public class InvoiceStatusDto
    {
        public InvoiceStatusDto()
        {

        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        public static InvoiceStatusDto GetSample()
        {
            return new InvoiceStatusDto
            {
                Id = (int)InvoiceStatus.Draft,
                Title = InvoiceStatus.Draft.ToLocalizedString()
            };
        }

    }

    [Singletone]
    public class InvoiceStatusDtoHelper
    {
        public InvoiceStatusDtoHelper()
        {
        }

        public InvoiceStatusDto Get(InvoiceStatus status)
        {
            return new InvoiceStatusDto
            {
                Id = (int)status,
                Title = status.ToLocalizedString()
            };
        }
    }
}
