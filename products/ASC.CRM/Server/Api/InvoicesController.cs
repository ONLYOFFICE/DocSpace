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
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Api.Documents;
using ASC.Common.Web;
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;

using ASC.CRM.Classes;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Classes;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class InvoicesController : BaseApiController
    {

        private readonly PdfQueueWorker _pdfQueueWorker;
        private readonly Global _global;
        private readonly FileWrapperHelper _fileWrapperHelper;
        private readonly PdfCreator _pdfCreator;
        private readonly SettingsManager _settingsManager;
        private readonly ApiDateTimeHelper _apiDateTimeHelper;
        private readonly ApiContext _apiContext;
        private readonly MessageService _messageService;
        private readonly MessageTarget _messageTarget;
        private readonly CurrencyProvider _currencyProvider;

        public InvoicesController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     ApiContext apiContext,
                     MessageTarget messageTarget,
                     MessageService messageService,
                     ApiDateTimeHelper apiDateTimeHelper,
                     SettingsManager settingsManager,
                     FileWrapperHelper fileWrapperHelper,
                     PdfCreator pdfCreator,
                     Global global,
                     PdfQueueWorker pdfQueueWorker,
                     CurrencyProvider currencyProvider,
                     IMapper mapper)
            : base(daoFactory, crmSecurity, mapper)
        {
            _apiContext = apiContext;
            _messageTarget = messageTarget;
            _messageService = messageService;
            _apiDateTimeHelper = apiDateTimeHelper;
            _settingsManager = settingsManager;
            _pdfCreator = pdfCreator;
            _fileWrapperHelper = fileWrapperHelper;
            _global = global;
            _pdfQueueWorker = pdfQueueWorker;
            _mapper = mapper;
            _currencyProvider = currencyProvider;
        }


        /// <summary>
        ///  Returns the detailed information about the invoice with the ID specified in the request
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <short>Get invoice by ID</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        [Read(@"invoice/{invoiceid:int}")]
        public InvoiceDto GetInvoiceByID(int invoiceid)
        {
            if (invoiceid <= 0) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceid);
            if (invoice == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(invoice))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            return _mapper.Map<InvoiceDto>(invoice);
        }

        /// <summary>
        ///  Returns the detailed information about the invoice sample
        /// </summary>
        /// <short>Get invoice sample</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        [Read(@"invoice/sample")]
        public InvoiceDto GetInvoiceSample()
        {
            var crmSettings = _settingsManager.Load<CrmSettings>();
            var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency);

            var sample = InvoiceDto.GetSample();

            sample.Number = _daoFactory.GetInvoiceDao().GetNewInvoicesNumber();
            sample.Terms = _daoFactory.GetInvoiceDao().GetSettings().Terms ?? string.Empty;
            sample.IssueDate = _apiDateTimeHelper.Get(DateTime.UtcNow);
            sample.DueDate = _apiDateTimeHelper.Get(DateTime.UtcNow.AddDays(30));
            sample.CreateOn = _apiDateTimeHelper.Get(DateTime.UtcNow);

            sample.Currency = _mapper.Map<CurrencyInfoDto>(defaultCurrency);

            sample.InvoiceLines.First().Quantity = 1;

            return sample;
        }

        /// <summary>
        ///  Returns the json data of the invoice with the ID specified in the request
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <short>Get invoice json data</short> 
        /// <category>Invoices</category>
        /// <returns>Json Data</returns>
        [Read(@"invoice/jsondata/{invoiceid:int}")]
        public string GetInvoiceJsonData(int invoiceid)
        {
            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceid);
            if (invoice == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(invoice))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            return invoice.JsonData;
        }

        /// <summary>
        ///   Returns the list of invoices matching the creteria specified in the request
        /// </summary>
        /// <param name="status">Invoice status</param>
        /// <param name="issueDateFrom">Invoice issue date from</param>
        /// <param name="issueDateTo">Invoice issue date to</param>
        /// <param name="dueDateFrom">Invoice due date from</param>
        /// <param name="dueDateTo">Invoice due date to</param>
        /// <param name="entityType">Invoice entity type</param>
        /// <param name="entityid">Invoice entity ID</param>
        /// <param name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Invoice currency</param>
        /// <short>Get invoice list</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice list</returns>
        [Read(@"invoice/filter")]
        public IEnumerable<InvoiceBaseDto> GetInvoices(
            InvoiceStatus? status,
            ApiDateTime issueDateFrom,
            ApiDateTime issueDateTo,
            ApiDateTime dueDateFrom,
            ApiDateTime dueDateTo,
            String entityType,
            int entityid,
            String currency
            )
        {
            if (!String.IsNullOrEmpty(entityType) && !(
                                                          string.Equals(entityType, "contact", StringComparison.CurrentCultureIgnoreCase) ||
                                                          string.Equals(entityType, "opportunity", StringComparison.CurrentCultureIgnoreCase) ||
                                                          string.Equals(entityType, "case", StringComparison.CurrentCultureIgnoreCase)))
                throw new ArgumentException();

            IEnumerable<InvoiceBaseDto> result;

            InvoiceSortedByType sortBy;

            OrderBy invoiceOrderBy;

            var searchString = _apiContext.FilterValue;

            if (InvoiceSortedByType.TryParse(_apiContext.SortBy, true, out sortBy))
            {
                invoiceOrderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
            }
            else if (String.IsNullOrEmpty(_apiContext.SortBy))
            {
                invoiceOrderBy = new OrderBy(InvoiceSortedByType.Number, true);
            }
            else
            {
                invoiceOrderBy = null;
            }

            var fromIndex = (int)_apiContext.StartIndex;
            var count = (int)_apiContext.Count;

            if (invoiceOrderBy != null)
            {
                result = ToListInvoiceBaseDtos(
                    _daoFactory.GetInvoiceDao().GetInvoices(
                        searchString,
                        status,
                        issueDateFrom, issueDateTo,
                        dueDateFrom, dueDateTo,
                        ToEntityType(entityType), entityid,
                        currency,
                        fromIndex, count,
                        invoiceOrderBy));

                _apiContext.SetDataPaginated();
                _apiContext.SetDataFiltered();
                _apiContext.SetDataSorted();
            }
            else
            {
                result = ToListInvoiceBaseDtos(
                    _daoFactory.GetInvoiceDao().GetInvoices(
                        searchString,
                        status,
                        issueDateFrom, issueDateTo,
                        dueDateFrom, dueDateTo,
                        ToEntityType(entityType), entityid,
                        currency,
                        0,
                        0,
                        null));
            }

            int totalCount;

            if (result.Count() < count)
            {
                totalCount = fromIndex + result.Count();
            }
            else
            {
                totalCount = _daoFactory.GetInvoiceDao().GetInvoicesCount(
                    searchString,
                    status,
                    issueDateFrom, issueDateTo,
                    dueDateFrom, dueDateTo,
                    ToEntityType(entityType), entityid,
                    currency);
            }

            _apiContext.SetTotalCount(totalCount);

            return result;
        }

        /// <summary>
        ///  Returns the list of all invoices associated with the entity with the ID and type specified in the request
        /// </summary>
        /// <param name="entityType">Invoice entity type</param>
        /// <param name="entityid">Invoice entity ID</param>
        /// <short>Get entity invoices</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice list</returns>
        [Read(@"{entityType:regex(contact|person|company|opportunity)}/invoicelist/{entityid:int}")]
        public IEnumerable<InvoiceBaseDto> GetEntityInvoices(String entityType, int entityid)
        {
            if (String.IsNullOrEmpty(entityType) || entityid <= 0) throw new ArgumentException();

            return ToListInvoiceBaseDtos(_daoFactory.GetInvoiceDao().GetEntityInvoices(ToEntityType(entityType), entityid));
        }

        /// <summary>
        ///   Updates the status of invoices with the IDs specified in the request
        /// </summary>
        /// <param name="invoiceids">Invoice ID list</param>
        /// <param name="status">Status</param>
        /// <short>Update invoice group status</short> 
        /// <category>Invoices</category>
        /// <returns>KeyValuePair of Invoices and InvoiceItems</returns>
        [Update(@"invoice/status/{status:int}")]
        public KeyValuePair<IEnumerable<InvoiceBaseDto>, IEnumerable<InvoiceItemDto>> UpdateInvoiceBatchStatus(
            int[] invoiceids,
            InvoiceStatus status
            )
        {
            if (invoiceids == null || !invoiceids.Any()) throw new ArgumentException();

            var oldInvoices = _daoFactory.GetInvoiceDao().GetByID(invoiceids).Where(_crmSecurity.CanAccessTo).ToList();

            var updatedInvoices = _daoFactory.GetInvoiceDao().UpdateInvoiceBatchStatus(oldInvoices.ToList().Select(i => i.ID).ToArray(), status);

            // detect what really changed
            var realUpdatedInvoices = updatedInvoices
                .Select(t => oldInvoices.FirstOrDefault(x => x.ID == t.ID && x.Status != t.Status))
                .Where(inv => inv != null)
                .ToList();

            if (realUpdatedInvoices.Any())
            {
                _messageService.Send(MessageAction.InvoicesUpdatedStatus, _messageTarget.Create(realUpdatedInvoices.Select(x => x.ID)), realUpdatedInvoices.Select(x => x.Number), status.ToLocalizedString());
            }

            var invoiceItemsUpdated = new List<InvoiceItem>();

            if (status == InvoiceStatus.Sent || status == InvoiceStatus.Rejected)
            {
                var invoiceItemsAll = _daoFactory.GetInvoiceItemDao().GetAll();
                var invoiceItemsWithTrackInventory = invoiceItemsAll.Where(item => item.TrackInventory).ToList();

                if (status == InvoiceStatus.Sent && invoiceItemsWithTrackInventory != null && invoiceItemsWithTrackInventory.Count != 0)
                {
                    foreach (var inv in updatedInvoices)
                    {
                        if (inv.Status == InvoiceStatus.Sent)
                        {
                            //could be changed
                            var oldInv = oldInvoices.FirstOrDefault(i => i.ID == inv.ID);
                            if (oldInv != null && oldInv.Status == InvoiceStatus.Draft)
                            {
                                //was changed to Sent
                                var invoiceLines = _daoFactory.GetInvoiceLineDao().GetInvoiceLines(inv.ID);

                                foreach (var line in invoiceLines)
                                {
                                    var item = invoiceItemsWithTrackInventory.FirstOrDefault(ii => ii.ID == line.InvoiceItemID);
                                    if (item != null)
                                    {
                                        item.StockQuantity -= line.Quantity;
                                        _daoFactory.GetInvoiceItemDao().SaveOrUpdateInvoiceItem(item);
                                        var oldItem = invoiceItemsUpdated.Find(i => i.ID == item.ID);
                                        if (oldItem != null)
                                        {
                                            invoiceItemsUpdated.Remove(oldItem);
                                        }
                                        invoiceItemsUpdated.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }

                if (status == InvoiceStatus.Rejected && invoiceItemsWithTrackInventory != null && invoiceItemsWithTrackInventory.Count != 0)
                {
                    foreach (var inv in updatedInvoices)
                    {
                        if (inv.Status == InvoiceStatus.Rejected)
                        {
                            //could be changed
                            var oldInv = oldInvoices.FirstOrDefault(i => i.ID == inv.ID);
                            if (oldInv != null && oldInv.Status == InvoiceStatus.Sent)
                            {
                                //was changed from Sent to Rejectes
                                var invoiceLines = _daoFactory.GetInvoiceLineDao().GetInvoiceLines(inv.ID);

                                foreach (var line in invoiceLines)
                                {
                                    var item = invoiceItemsWithTrackInventory.FirstOrDefault(ii => ii.ID == line.InvoiceItemID);
                                    if (item != null)
                                    {
                                        item.StockQuantity += line.Quantity;

                                        _daoFactory.GetInvoiceItemDao().SaveOrUpdateInvoiceItem(item);

                                        var oldItem = invoiceItemsUpdated.Find(i => i.ID == item.ID);

                                        if (oldItem != null)
                                        {
                                            invoiceItemsUpdated.Remove(oldItem);
                                        }

                                        invoiceItemsUpdated.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var listInvoiceBaseDtos = ToListInvoiceBaseDtos(updatedInvoices);

            return new KeyValuePair<IEnumerable<InvoiceBaseDto>, IEnumerable<InvoiceItemDto>>(
                listInvoiceBaseDtos,
                _mapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(invoiceItemsUpdated));
        }

        /// <summary>
        ///   Delete the invoice with the ID specified in the request
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <short>Delete invoice</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        [Delete(@"invoice/{invoiceid:int}")]
        public InvoiceBaseDto DeleteInvoice(int invoiceid)
        {
            if (invoiceid <= 0) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().DeleteInvoice(invoiceid);

            if (invoice == null) throw new ItemNotFoundException();

            _messageService.Send(MessageAction.InvoiceDeleted, _messageTarget.Create(invoice.ID), invoice.Number);

            return _mapper.Map<InvoiceBaseDto>(invoice);
        }

        /// <summary>
        ///   Deletes the group of invoices with the IDs specified in the request
        /// </summary>
        /// <param name="invoiceids">Invoice ID list</param>
        /// <short>Delete invoice group</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice list</returns>
        [Delete(@"invoice")]
        public IEnumerable<InvoiceBaseDto> DeleteBatchInvoices(IEnumerable<int> invoiceids)
        {
            if (invoiceids == null || !invoiceids.Any()) throw new ArgumentException();

            var invoices = _daoFactory.GetInvoiceDao().DeleteBatchInvoices(invoiceids.ToArray());
            _messageService.Send(MessageAction.InvoicesDeleted, _messageTarget.Create(invoices.Select(x => x.ID)), invoices.Select(x => x.Number));

            return ToListInvoiceBaseDtos(invoices);
        }

        /// <summary>
        ///  Creates the invoice with the parameters (contactId, consigneeId, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="number">Invoice number</param>
        /// <param optional="false" name="issueDate">Invoice issue date</param>
        /// <param optional="true" name="templateType">Invoice template type</param>
        /// <param optional="false" name="contactId">Invoice contact ID</param>
        /// <param optional="true" name="consigneeId">Invoice consignee ID</param>
        /// <param optional="true" name="entityId">Invoice entity ID</param>
        /// <param optional="true" name="billingAddressID">Invoice billing address ID</param>
        /// <param optional="true" name="deliveryAddressID">Invoice delivery address ID</param>
        /// <param optional="false" name="dueDate">Invoice due date</param>
        /// <param optional="false" name="language">Invoice language</param>
        /// <param optional="false" name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Invoice currency</param>
        /// <param optional="false" name="exchangeRate">Invoice exchange rate</param>
        /// <param optional="true" name="purchaseOrderNumber">Invoice purchase order number</param>
        /// <param optional="false" name="terms">Invoice terms</param>
        /// <param optional="true" name="description">Invoice description</param>
        /// <param optional="false" name="invoiceLines">Invoice lines list</param>
        /// <short>Create invoice</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        /// <example>
        /// <![CDATA[
        /// 
        /// Data transfer in application/json format:
        /// 
        /// data: {
        ///    number: "invoice000001",
        ///    issueDate: "2015-06-01T00:00:00",
        ///    contactId: 10,
        ///    dueDate: "2025-06-01T00:00:00",
        ///    language: "es-ES",
        ///    currency: "rub",
        ///    exchangeRate: 54.32,
        ///    terms: "Terms for this invoice",
        ///    invoiceLines:
        ///    [{
        ///          invoiceItemID: 1,
        ///          invoiceTax1ID: 1,
        ///          invoiceTax2ID: 2,
        ///          description: "description for invoice line 1",
        ///          quantity: 100,
        ///          price: 7.7,
        ///          discount: 25
        ///    }]  
        /// }
        /// 
        /// where invoiceItemID, invoiceTax1ID, invoiceTax2ID - ids of the real existing invoice item and invoice taxes,
        /// contactId - id of the existing contact
        /// 
        /// ]]>
        /// </example>
        [Create(@"invoice")]
        public InvoiceDto CreateInvoice(
            CreateOrUpdateInvoiceRequestDto inDto
            )
        {
            string number = inDto.Number;

            ApiDateTime issueDate = inDto.IssueDate;
            int templateType = inDto.TemplateType;
            int contactId = inDto.ContactId;
            int consigneeId = inDto.ConsigneeId;
            int entityId = inDto.EntityId;
            int billingAddressID = inDto.BillingAddressID;
            int deliveryAddressID = inDto.DeliveryAddressID;
            ApiDateTime dueDate = inDto.DueDate;
            string language = inDto.Language;
            string currency = inDto.Currency;
            decimal exchangeRate = inDto.ExchangeRate;
            string purchaseOrderNumber = inDto.PurchaseOrderNumber;
            string terms = inDto.Terms;
            string description = inDto.Description;
            IEnumerable<InvoiceLine> invoiceLines = inDto.InvoiceLines;

            var invoiceLinesList = invoiceLines != null ? invoiceLines.ToList() : new List<InvoiceLine>();
            if (!invoiceLinesList.Any() || !IsLinesForInvoiceCorrect(invoiceLinesList)) throw new ArgumentException();

            var invoice = new Invoice
            {
                Status = InvoiceStatus.Draft,
                Number = number,
                IssueDate = issueDate,
                TemplateType = (InvoiceTemplateType)templateType,
                ContactID = contactId,
                ConsigneeID = consigneeId,
                EntityType = EntityType.Opportunity,
                EntityID = entityId,
                DueDate = dueDate,
                Language = language,
                Currency = !String.IsNullOrEmpty(currency) ? currency.ToUpper() : null,
                ExchangeRate = exchangeRate,
                PurchaseOrderNumber = purchaseOrderNumber,
                Terms = terms,
                Description = description
            };

            _crmSecurity.DemandCreateOrUpdate(invoice);

            if (billingAddressID > 0)
            {
                var address = _daoFactory.GetContactInfoDao().GetByID(billingAddressID);
                if (address == null || address.InfoType != ContactInfoType.Address || address.Category != (int)AddressCategory.Billing || address.ContactID != contactId)
                    throw new ArgumentException();
            }

            if (deliveryAddressID > 0)
            {
                var address = _daoFactory.GetContactInfoDao().GetByID(deliveryAddressID);
                if (address == null || address.InfoType != ContactInfoType.Address || address.Category != (int)AddressCategory.Postal || address.ContactID != consigneeId)
                    throw new ArgumentException();
            }


            invoice.ID = _daoFactory.GetInvoiceDao().SaveOrUpdateInvoice(invoice);

            CreateInvoiceLines(invoiceLinesList, invoice);

            _daoFactory.GetInvoiceDao().UpdateInvoiceJsonData(invoice, billingAddressID, deliveryAddressID);

            return _mapper.Map<InvoiceDto>(invoice);
        }


        private bool IsLinesForInvoiceCorrect(List<InvoiceLine> invoiceLines)
        {
            foreach (var line in invoiceLines)
            {
                if (line.InvoiceItemID <= 0 ||
                    line.Quantity < 0 || line.Price < 0 ||
                    line.Discount < 0 || line.Discount > 100 ||
                    line.InvoiceTax1ID < 0 || line.InvoiceTax2ID < 0)
                    return false;
                if (!_daoFactory.GetInvoiceItemDao().IsExist(line.InvoiceItemID))
                    return false;

                if (line.InvoiceTax1ID > 0 && !_daoFactory.GetInvoiceTaxDao().IsExist(line.InvoiceTax1ID))
                    return false;

                if (line.InvoiceTax2ID > 0 && !_daoFactory.GetInvoiceTaxDao().IsExist(line.InvoiceTax2ID))
                    return false;
            }
            return true;
        }

        private List<InvoiceLine> CreateInvoiceLines(List<InvoiceLine> invoiceLines, Invoice invoice)
        {
            var result = new List<InvoiceLine>();
            for (var i = 0; i < invoiceLines.Count; i++)
            {
                var line = new InvoiceLine
                {
                    ID = 0,
                    InvoiceID = invoice.ID,
                    InvoiceItemID = invoiceLines[i].InvoiceItemID,
                    InvoiceTax1ID = invoiceLines[i].InvoiceTax1ID,
                    InvoiceTax2ID = invoiceLines[i].InvoiceTax2ID,
                    SortOrder = i,
                    Description = invoiceLines[i].Description,
                    Quantity = invoiceLines[i].Quantity,
                    Price = invoiceLines[i].Price,
                    Discount = Convert.ToInt32(invoiceLines[i].Discount)
                };

                line.ID = _daoFactory.GetInvoiceLineDao().SaveOrUpdateInvoiceLine(line);
                result.Add(line);
            }
            return result;
        }

        /// <summary>
        ///   Updates the selected invoice with the parameters (contactId, consigneeId, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="id">Invoice ID</param>
        /// <param optional="false" name="issueDate">Invoice issue date</param>
        /// <param optional="true" name="templateType">Invoice template type</param>
        /// <param optional="false" name="contactId">Invoice contact ID</param>
        /// <param optional="true" name="consigneeId">Invoice consignee ID</param>
        /// <param optional="true" name="entityId">Invoice entity ID</param>
        /// <param optional="true" name="billingAddressID">Invoice billing address ID</param>
        /// <param optional="true" name="deliveryAddressID">Invoice delivery address ID</param>
        /// <param name="dueDate">Invoice due date</param>
        /// <param optional="false" name="language">Invoice language</param>
        /// <param optional="false" name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Invoice currency</param>
        /// <param optional="false" name="exchangeRate">Invoice exchange rate</param>
        /// <param optional="true" name="purchaseOrderNumber">Invoice purchase order number</param>
        /// <param optional="false" name="terms">Invoice terms</param>
        /// <param optional="true" name="description">Invoice description</param>
        /// <param optional="false" name="invoiceLines">Invoice lines list</param>
        /// <short>Update invoice</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        /// <example>
        /// <![CDATA[
        /// 
        /// Data transfer in application/json format:
        /// 
        /// data: {
        ///    id: 5,
        ///    issueDate: "2015-06-01T00:00:00",
        ///    contactId: 10,
        ///    dueDate: "2025-06-01T00:00:00",
        ///    language: "es-ES",
        ///    currency: "rub",
        ///    exchangeRate: 54.32,
        ///    terms: "Terms for this invoice",
        ///    invoiceLines:
        ///    [{
        ///          invoiceItemID: 1,
        ///          invoiceTax1ID: 1,
        ///          invoiceTax2ID: 2,
        ///          description: "description for invoice line 1",
        ///          quantity: 100,
        ///          price: 7.7,
        ///          discount: 25
        ///    }]
        /// }
        /// 
        /// where invoiceItemID, invoiceTax1ID, invoiceTax2ID - ids of the real existing invoice item and invoice taxes,
        /// contactId - id of the existing contact
        /// 
        /// ]]>
        /// </example>
        [Update(@"invoice/{id:int}")]
        public InvoiceDto UpdateInvoice(
            int id,
            CreateOrUpdateInvoiceRequestDto inDto)

        {
            ApiDateTime issueDate = inDto.IssueDate;
            int templateType = inDto.TemplateType;
            int contactId = inDto.ContactId;
            int consigneeId = inDto.ConsigneeId;
            int entityId = inDto.EntityId;
            int billingAddressID = inDto.BillingAddressID;
            int deliveryAddressID = inDto.DeliveryAddressID;
            ApiDateTime dueDate = inDto.DueDate;
            string language = inDto.Language;
            string currency = inDto.Currency;
            decimal exchangeRate = inDto.ExchangeRate;
            string purchaseOrderNumber = inDto.PurchaseOrderNumber;
            string terms = inDto.Terms;
            string description = inDto.Description;
            IEnumerable<InvoiceLine> invoiceLines = inDto.InvoiceLines;

            var invoiceLinesList = invoiceLines != null ? invoiceLines.ToList() : new List<InvoiceLine>();
            if (!invoiceLinesList.Any() || !IsLinesForInvoiceCorrect(invoiceLinesList)) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByID(id);
            if (invoice == null || !_crmSecurity.CanEdit(invoice)) throw new ItemNotFoundException();

            invoice.IssueDate = issueDate;
            invoice.TemplateType = (InvoiceTemplateType)templateType;
            invoice.ContactID = contactId;
            invoice.ConsigneeID = consigneeId;
            invoice.EntityType = EntityType.Opportunity;
            invoice.EntityID = entityId;
            invoice.DueDate = dueDate;
            invoice.Language = language;
            invoice.Currency = !String.IsNullOrEmpty(currency) ? currency.ToUpper() : null; ;
            invoice.ExchangeRate = exchangeRate;
            invoice.PurchaseOrderNumber = purchaseOrderNumber;
            invoice.Terms = terms;
            invoice.Description = description;
            invoice.JsonData = null;

            _crmSecurity.DemandCreateOrUpdate(invoice);

            if (billingAddressID > 0)
            {
                var address = _daoFactory.GetContactInfoDao().GetByID(billingAddressID);
                if (address == null || address.InfoType != ContactInfoType.Address || address.Category != (int)AddressCategory.Billing || address.ContactID != contactId)
                    throw new ArgumentException();
            }

            if (deliveryAddressID > 0)
            {
                var address = _daoFactory.GetContactInfoDao().GetByID(deliveryAddressID);
                if (address == null || address.InfoType != ContactInfoType.Address || address.Category != (int)AddressCategory.Postal || address.ContactID != consigneeId)
                    throw new ArgumentException();
            }

            _daoFactory.GetInvoiceDao().SaveOrUpdateInvoice(invoice);


            _daoFactory.GetInvoiceLineDao().DeleteInvoiceLines(invoice.ID);
            CreateInvoiceLines(invoiceLinesList, invoice);

            _daoFactory.GetInvoiceDao().UpdateInvoiceJsonData(invoice, billingAddressID, deliveryAddressID);

            if (_global.CanDownloadInvoices)
            {
                _pdfQueueWorker.StartTask(invoice.ID);
            }

            return _mapper.Map<InvoiceDto>(invoice);
        }

        /// <summary>
        ///  Returns the pdf file associated with the invoice with the ID specified in the request
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <short>Get invoice pdf file</short> 
        /// <category>Invoices</category>
        /// <returns>File</returns>
        [Read(@"invoice/{invoiceid:int}/pdf")]
        public Task<FileWrapper<int>> GetInvoicePdfExistOrCreateAsync(int invoiceid)
        {
            if (invoiceid <= 0) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceid);
            if (invoice == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(invoice))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            return internalGetInvoicePdfExistOrCreateAsync(invoice);
        }

        private async Task<FileWrapper<int>> internalGetInvoicePdfExistOrCreateAsync(Invoice invoice)
        {
            return await _fileWrapperHelper.GetAsync(await GetInvoicePdfExistingOrCreateAsync(invoice));
        }

        private async Task<ASC.Files.Core.File<int>> GetInvoicePdfExistingOrCreateAsync(ASC.CRM.Core.Entities.Invoice invoice)
        {
            var existingFile = invoice.GetInvoiceFile(_daoFactory);

            if (existingFile != null)
            {
                return existingFile;
            }
            else
            {
                var newFile = await _pdfCreator.CreateFileAsync(invoice, _daoFactory);

                invoice.FileID = Int32.Parse(newFile.ID.ToString());

                _daoFactory.GetInvoiceDao().UpdateInvoiceFileID(invoice.ID, invoice.FileID);

                _daoFactory.GetRelationshipEventDao().AttachFiles(invoice.ContactID, invoice.EntityType, invoice.EntityID, new[] { invoice.FileID });

                return newFile;
            }
        }


        /// <summary>
        ///  Returns information about the generation of the pdf file of the invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <param name="storageUrl">Storage Url</param>
        /// <param name="revisionId">Revision ID</param>
        /// <short>Check invoice pdf file</short> 
        /// <category>Invoices</category>
        /// <returns>ConverterData</returns>
        [Create(@"invoice/converter/data")]
        public Task<ConverterData> CreateInvoiceConverterDataAsync(
    [FromBody] CreateInvoiceConverterDataRequestDto inDto)
        {
            var invoiceId = inDto.InvoiceId;
            var storageUrl = inDto.StorageUrl;
            var revisionId = inDto.RevisionId;

            if (invoiceId <= 0) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceId);
            if (invoice == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(invoice))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            var converterData = new ConverterData
            {
                StorageUrl = storageUrl,
                RevisionId = revisionId,
                InvoiceId = invoiceId
            };

            var existingFile = invoice.GetInvoiceFile(_daoFactory);
            if (existingFile != null)
            {
                converterData.FileId = invoice.FileID;
                return System.Threading.Tasks.Task.FromResult(converterData);
            }

            return InternalCreateInvoiceConverterDataAsync(converterData, invoice);
        }

        private async Task<ConverterData> InternalCreateInvoiceConverterDataAsync(ConverterData converterData, Invoice invoice)
        {
            var storageUrl = converterData.StorageUrl;
            var revisionId = converterData.RevisionId;

            if (string.IsNullOrEmpty(storageUrl) || string.IsNullOrEmpty(revisionId))
            {
                return await _pdfCreator.StartCreationFileAsync(invoice);
            }
            else
            {
                var convertedFile = await _pdfCreator.GetConvertedFileAsync(converterData, _daoFactory);
                if (convertedFile != null)
                {
                    invoice.FileID = Int32.Parse(convertedFile.ID.ToString());
                    _daoFactory.GetInvoiceDao().UpdateInvoiceFileID(invoice.ID, invoice.FileID);
                    _daoFactory.GetRelationshipEventDao().AttachFiles(invoice.ContactID, invoice.EntityType, invoice.EntityID, new[] { invoice.FileID });

                    converterData.FileId = invoice.FileID;
                    return converterData;
                }
                else
                {
                    return converterData;
                }
            }
        }

        /// <summary>
        ///  Returns the existence of the invoice with the Number specified in the request
        /// </summary>
        /// <param name="number">Invoice number</param>
        /// <short>Check invoice existence by number</short> 
        /// <category>Invoices</category>
        /// <returns>IsExist</returns>
        [Read(@"invoice/bynumber/exist")]
        public Boolean GetInvoiceByNumberExistence(string number)
        {
            if (String.IsNullOrEmpty(number)) throw new ArgumentException();

            return _daoFactory.GetInvoiceDao().IsExist(number);

        }

        /// <summary>
        ///  Returns the detailed information about the invoice with the Number specified in the request
        /// </summary>
        /// <param name="number">Invoice number</param>
        /// <short>Get invoice by number</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice</returns>
        [Read(@"invoice/bynumber")]
        public InvoiceDto GetInvoiceByNumber(string number)
        {
            if (String.IsNullOrEmpty(number)) throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByNumber(number);
            if (invoice == null) throw new ItemNotFoundException();

            if (!_crmSecurity.CanAccessTo(invoice))
            {
                throw _crmSecurity.CreateSecurityException();
            }

            return _mapper.Map<InvoiceDto>(invoice);
        }

        /// <summary>
        ///   Returns the list of invoice items matching the creteria specified in the request
        /// </summary>
        /// <param name="status">Status</param>
        /// <param optional="true" name="inventoryStock">InventoryStock</param>
        /// <short>Get invoice item list</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceItem list</returns>
        [Read(@"invoiceitem/filter")]
        public IEnumerable<InvoiceItemDto> GetInvoiceItems(int status, bool? inventoryStock)
        {
            IEnumerable<InvoiceItemDto> result;

            InvoiceItemSortedByType sortBy;

            OrderBy invoiceOrderBy;

            var searchString = _apiContext.FilterValue;

            if (InvoiceItemSortedByType.TryParse(_apiContext.SortBy, true, out sortBy))
            {
                invoiceOrderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
            }
            else if (String.IsNullOrEmpty(_apiContext.SortBy))
            {
                invoiceOrderBy = new OrderBy(InvoiceItemSortedByType.Name, true);
            }
            else
            {
                invoiceOrderBy = null;
            }

            var fromIndex = (int)_apiContext.StartIndex;
            var count = (int)_apiContext.Count;

            if (invoiceOrderBy != null)
            {
                var resultFromDao = _daoFactory.GetInvoiceItemDao().GetInvoiceItems(
                    searchString,
                    status,
                    inventoryStock,
                    fromIndex, count,
                    invoiceOrderBy);

                result = _mapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(resultFromDao);

                _apiContext.SetDataPaginated();
                _apiContext.SetDataFiltered();
                _apiContext.SetDataSorted();
            }
            else
            {
                var resultFromDao = _daoFactory.GetInvoiceItemDao().GetInvoiceItems(
                     searchString,
                     status,
                     inventoryStock,
                     0, 0,
                     null);

                result = _mapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(resultFromDao);

            }

            int totalCount;

            if (result.Count() < count)
            {
                totalCount = fromIndex + result.Count();
            }
            else
            {
                totalCount = _daoFactory.GetInvoiceItemDao().GetInvoiceItemsCount(
                    searchString,
                    status,
                    inventoryStock);
            }

            _apiContext.SetTotalCount(totalCount);

            return result;
        }

        /// <summary>
        ///  Returns the detailed information about the invoice item with the ID specified in the request
        /// </summary>
        /// <param name="invoiceitemid">Invoice Item ID</param>
        /// <short>Get invoice item by ID</short> 
        /// <category>Invoices</category>
        /// <returns>Invoice Item</returns>
        [Read(@"invoiceitem/{invoiceitemid:int}")]
        public InvoiceItemDto GetInvoiceItemByID(int invoiceitemid)
        {
            if (invoiceitemid <= 0) throw new ArgumentException();

            var invoiceItem = _daoFactory.GetInvoiceItemDao().GetByID(invoiceitemid);
            if (invoiceItem == null) throw new ItemNotFoundException();

            return _mapper.Map<InvoiceItemDto>(invoiceItem);
        }

        /// <summary>
        ///  Creates the invoice line with the parameters (invoiceId, invoiceItemId, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="invoiceId">Invoice ID</param>
        /// <param optional="false" name="invoiceItemId">Invoice item ID</param>
        /// <param optional="true" name="invoiceTax1Id">First invoice tax ID</param>
        /// <param optional="true" name="invoiceTax2Id">Second invoice tax ID</param>
        /// <param optional="true" name="sortOrder">Sort Order</param>
        /// <param optional="true" name="description">Description</param>
        /// <param optional="true" name="quantity">Quantity</param>
        /// <param optional="true" name="price">Price</param>
        /// <param optional="true" name="discount">Discount</param>
        /// <short>Create invoice line</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceLine</returns>
        [Create(@"invoiceline")]
        public InvoiceLineDto CreateInvoiceLine(
            CreateOrUpdateInvoiceLineRequestDto inDto
            )
        {
            int invoiceId = inDto.InvoiceId;
            int invoiceItemId = inDto.InvoiceItemId;
            int invoiceTax1Id = inDto.InvoiceTax1Id;
            int invoiceTax2Id = inDto.InvoiceTax2Id;
            int sortOrder = inDto.SortOrder;
            string description = inDto.Description;
            int quantity = inDto.Quantity;
            decimal price = inDto.Price;
            int discount = inDto.Discount;

            var invoiceLine = new InvoiceLine
            {
                InvoiceID = invoiceId,
                InvoiceItemID = invoiceItemId,
                InvoiceTax1ID = invoiceTax1Id,
                InvoiceTax2ID = invoiceTax2Id,
                SortOrder = sortOrder,
                Description = description,
                Quantity = quantity,
                Price = price,
                Discount = discount
            };

            if (invoiceId <= 0)
                throw new ArgumentException();

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceId);

            _crmSecurity.DemandCreateOrUpdate(invoiceLine, invoice);

            invoiceLine.ID = _daoFactory.GetInvoiceLineDao().SaveOrUpdateInvoiceLine(invoiceLine);

            _daoFactory.GetInvoiceDao().UpdateInvoiceJsonDataAfterLinesUpdated(invoice);

            if (_global.CanDownloadInvoices)
            {
                _pdfQueueWorker.StartTask(invoice.ID);
            }

            return _mapper.Map<InvoiceLineDto>(invoiceLine);
        }

        /// <summary>
        ///   Updates the selected invoice line with the parameters (invoiceId, invoiceItemId, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="id">Line ID</param>
        /// <param optional="false" name="invoiceId">Invoice ID</param>
        /// <param optional="false" name="invoiceItemId">Invoice item ID</param>
        /// <param optional="true" name="invoiceTax1Id">First invoice tax ID</param>
        /// <param optional="true" name="invoiceTax2Id">Second invoice tax ID</param>
        /// <param optional="true" name="sortOrder">Sort Order</param>
        /// <param optional="true" name="description">Description</param>
        /// <param optional="true" name="quantity">Quantity</param>
        /// <param optional="true" name="price">Price</param>
        /// <param optional="true" name="discount">Discount</param>
        /// <short>Update invoice line</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceLine</returns>
        [Update(@"invoiceline/{id:int}")]
        public InvoiceLineDto UpdateInvoiceLine(int id, CreateOrUpdateInvoiceLineRequestDto inDto)
        {
            int invoiceId = inDto.InvoiceId;
            int invoiceItemId = inDto.InvoiceItemId;
            int invoiceTax1Id = inDto.InvoiceTax1Id;
            int invoiceTax2Id = inDto.InvoiceTax2Id;
            int sortOrder = inDto.SortOrder;
            string description = inDto.Description;
            int quantity = inDto.Quantity;
            decimal price = inDto.Price;
            int discount = inDto.Discount;

            if (invoiceId <= 0)
                throw new ArgumentException();

            var invoiceLine = _daoFactory.GetInvoiceLineDao().GetByID(id);

            if (invoiceLine == null || invoiceLine.InvoiceID != invoiceId) throw new ItemNotFoundException();


            invoiceLine.InvoiceID = invoiceId;
            invoiceLine.InvoiceItemID = invoiceItemId;
            invoiceLine.InvoiceTax1ID = invoiceTax1Id;
            invoiceLine.InvoiceTax2ID = invoiceTax2Id;
            invoiceLine.SortOrder = sortOrder;
            invoiceLine.Description = description;
            invoiceLine.Quantity = quantity;
            invoiceLine.Price = price;
            invoiceLine.Discount = discount;

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceId);
            _crmSecurity.DemandCreateOrUpdate(invoiceLine, invoice);

            _daoFactory.GetInvoiceLineDao().SaveOrUpdateInvoiceLine(invoiceLine);

            _daoFactory.GetInvoiceDao().UpdateInvoiceJsonDataAfterLinesUpdated(invoice);

            if (_global.CanDownloadInvoices)
            {
                _pdfQueueWorker.StartTask(invoice.ID);
            }

            return _mapper.Map<InvoiceLineDto>(invoiceLine);
        }

        /// <summary>
        ///    Deletes the invoice line with the ID specified in the request
        /// </summary>
        /// <param optional="false" name="id">Line ID</param>
        /// <short>Delete invoice line</short> 
        /// <category>Invoices</category>
        /// <returns>Line ID</returns>
        [Delete(@"invoiceline/{id:int}")]
        public int DeleteInvoiceLine(int id)
        {
            var invoiceLine = _daoFactory.GetInvoiceLineDao().GetByID(id);
            if (invoiceLine == null) throw new ItemNotFoundException();
            if (!_daoFactory.GetInvoiceLineDao().CanDelete(invoiceLine.ID)) throw new Exception("Can't delete invoice line");

            var invoice = _daoFactory.GetInvoiceDao().GetByID(invoiceLine.InvoiceID);
            if (invoice == null) throw new ItemNotFoundException();
            if (!_crmSecurity.CanEdit(invoice)) throw _crmSecurity.CreateSecurityException();

            _daoFactory.GetInvoiceLineDao().DeleteInvoiceLine(id);

            _daoFactory.GetInvoiceDao().UpdateInvoiceJsonDataAfterLinesUpdated(invoice);

            if (_global.CanDownloadInvoices)
            {
                _pdfQueueWorker.StartTask(invoice.ID);
            }

            return id;
        }

        /// <summary>
        ///  Creates the invoice item with the parameters (title, description, price, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="title">Item title</param>
        /// <param optional="true" name="description">Item description</param>
        /// <param optional="false" name="price">Item price</param>
        /// <param optional="true" name="sku">Item stock keeping unit</param>
        /// <param optional="true" name="quantity">Item quantity</param>
        /// <param optional="true" name="stockQuantity">Item stock quantity</param>
        /// <param optional="true" name="trackInventory">Track inventory</param>
        /// <param optional="true" name="invoiceTax1id">Item first invoice tax ID</param>
        /// <param optional="true" name="invoiceTax2id">Item second invoice tax ID</param>
        /// <short>Create invoice item</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceItem</returns>
        [Create(@"invoiceitem")]
        public InvoiceItemDto CreateInvoiceItem(
            CreateOrUpdateInvoiceItemRequestDto inDto
            )
        {
            string title = inDto.Title;
            string description = inDto.Description;
            decimal price = inDto.Price;
            string sku = inDto.Sku;
            int quantity = inDto.Quantity;
            int stockQuantity = inDto.StockQuantity;
            bool trackInventory = inDto.TrackInventory;
            int invoiceTax1id = inDto.InvoiceTax1id;
            int invoiceTax2id = inDto.InvoiceTax2id;

            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (String.IsNullOrEmpty(title) || price <= 0) throw new ArgumentException();

            var invoiceItem = new InvoiceItem
            {
                Title = title,
                Description = description,
                Price = price,
                StockKeepingUnit = sku,
                StockQuantity = stockQuantity,
                TrackInventory = trackInventory,
                InvoiceTax1ID = invoiceTax1id,
                InvoiceTax2ID = invoiceTax2id
            };

            invoiceItem = _daoFactory.GetInvoiceItemDao().SaveOrUpdateInvoiceItem(invoiceItem);

            _messageService.Send(MessageAction.InvoiceItemCreated, _messageTarget.Create(invoiceItem.ID), invoiceItem.Title);

            return _mapper.Map<InvoiceItemDto>(invoiceItem);
        }

        /// <summary>
        ///   Updates the selected invoice item with the parameters (title, description, price, etc.) specified in the request
        /// </summary>
        /// <param optional="false" name="id">Item ID</param>
        /// <param optional="false" name="title">Item title</param>
        /// <param optional="true" name="description">Item description</param>
        /// <param optional="false" name="price">Item price</param>
        /// <param optional="true" name="sku">Item stock keeping unit</param>
        /// <param optional="true" name="quantity">Item quantity</param>
        /// <param optional="true" name="stockQuantity">Item stock quantity</param>
        /// <param optional="true" name="trackInventory">Track inventory</param>
        /// <param optional="true" name="invoiceTax1id">Item first invoice tax ID</param>
        /// <param optional="true" name="invoiceTax2id">Item second invoice tax ID</param>
        /// <short>Update invoice item</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceItem</returns>
        [Update(@"invoiceitem/{id:int}")]
        public InvoiceItemDto UpdateInvoiceItem(int id,
         CreateOrUpdateInvoiceItemRequestDto inDto
            )
        {
            string title = inDto.Title;
            string description = inDto.Description;
            decimal price = inDto.Price;
            string sku = inDto.Sku;
            int quantity = inDto.Quantity;
            int stockQuantity = inDto.StockQuantity;
            bool trackInventory = inDto.TrackInventory;
            int invoiceTax1id = inDto.InvoiceTax1id;
            int invoiceTax2id = inDto.InvoiceTax2id;

            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (id <= 0 || String.IsNullOrEmpty(title) || price <= 0) throw new ArgumentException();

            if (!_daoFactory.GetInvoiceItemDao().IsExist(id)) throw new ItemNotFoundException();

            var invoiceItem = new InvoiceItem
            {
                ID = id,
                Title = title,
                Description = description,
                Price = price,
                StockKeepingUnit = sku,
                StockQuantity = stockQuantity,
                TrackInventory = trackInventory,
                InvoiceTax1ID = invoiceTax1id,
                InvoiceTax2ID = invoiceTax2id
            };

            invoiceItem = _daoFactory.GetInvoiceItemDao().SaveOrUpdateInvoiceItem(invoiceItem);
            _messageService.Send(MessageAction.InvoiceItemUpdated, _messageTarget.Create(invoiceItem.ID), invoiceItem.Title);

            return _mapper.Map<InvoiceItemDto>(invoiceItem);
        }

        /// <summary>
        ///    Deletes the invoice item with the ID specified in the request
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <short>Delete invoice item</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceItem</returns>
        [Delete(@"invoiceitem/{id:int}")]
        public InvoiceItemDto DeleteInvoiceItem(int id)
        {
            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (id <= 0) throw new ArgumentException();

            var invoiceItem = _daoFactory.GetInvoiceItemDao().DeleteInvoiceItem(id);
            if (invoiceItem == null) throw new ItemNotFoundException();

            _messageService.Send(MessageAction.InvoiceItemDeleted, _messageTarget.Create(invoiceItem.ID), invoiceItem.Title);

            return _mapper.Map<InvoiceItemDto>(invoiceItem);

        }

        /// <summary>
        ///   Deletes the group of invoice items with the IDs specified in the request
        /// </summary>
        /// <param name="ids">Item ID list</param>
        /// <short>Delete Invoice item group</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceItem list</returns>
        [Delete(@"invoiceitem")]
        public IEnumerable<InvoiceItemDto> DeleteBatchItems(IEnumerable<int> ids)
        {
            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (ids == null) throw new ArgumentException();
            ids = ids.Distinct();

            var items = _daoFactory.GetInvoiceItemDao().DeleteBatchInvoiceItems(ids.ToArray());

            _messageService.Send(MessageAction.InvoiceItemsDeleted, _messageTarget.Create(ids), items.Select(x => x.Title));

            return _mapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(items);
        }

        /// <summary>
        ///   Returns the list of invoice taxes
        /// </summary>
        /// <short>Get invoice taxes list</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceTax list</returns>
        [Read(@"invoice/tax")]
        public IEnumerable<InvoiceTaxDto> GetInvoiceTaxes()
        {
            var responceFromDao = _daoFactory.GetInvoiceTaxDao().GetAll();

            return _mapper.Map<List<InvoiceTax>, List<InvoiceTaxDto>>(responceFromDao);
        }

        /// <summary>
        ///  Creates the invoice tax with the parameters (name, description, rate) specified in the request
        /// </summary>
        /// <param name="name">Tax name</param>
        /// <param name="description">Tax description</param>
        /// <param name="rate">Tax rate</param>
        /// <short>Create invoice tax</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceTax</returns>
        [Create(@"invoice/tax")]
        public InvoiceTaxDto CreateInvoiceTax(
         [FromBody] CreateOrUpdateInvoiceTaxRequestDto inDto)
        {
            string name = inDto.Name;
            string description = inDto.Description;
            decimal rate = inDto.Rate;

            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (String.IsNullOrEmpty(name)) throw new ArgumentException(CRMInvoiceResource.EmptyTaxNameError);
            if (_daoFactory.GetInvoiceTaxDao().IsExist(name)) throw new ArgumentException(CRMInvoiceResource.ExistTaxNameError);

            var invoiceTax = new InvoiceTax
            {
                Name = name,
                Description = description,
                Rate = rate
            };

            invoiceTax = _daoFactory.GetInvoiceTaxDao().SaveOrUpdateInvoiceTax(invoiceTax);
            _messageService.Send(MessageAction.InvoiceTaxCreated, _messageTarget.Create(invoiceTax.ID), invoiceTax.Name);

            return _mapper.Map<InvoiceTaxDto>(invoiceTax);
        }

        /// <summary>
        ///   Updates the selected invoice tax with the parameters (name, description, rate) specified in the request
        /// </summary>
        /// <param name="id">Tax ID</param>
        /// <param name="name">Tax name</param>
        /// <param name="description">Tax description</param>
        /// <param name="rate">Tax rate</param>
        /// <short>Update invoice tax</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceTax</returns>
        [Update(@"invoice/tax/{id:int}")]
        public InvoiceTaxDto UpdateInvoiceTax(
            int id,
            CreateOrUpdateInvoiceTaxRequestDto inDto)
        {
            string name = inDto.Name;
            string description = inDto.Description;
            decimal rate = inDto.Rate;

            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (id <= 0 || String.IsNullOrEmpty(name)) throw new ArgumentException(CRMInvoiceResource.EmptyTaxNameError);

            if (!_daoFactory.GetInvoiceTaxDao().IsExist(id)) throw new ItemNotFoundException();

            var invoiceTax = new InvoiceTax
            {
                ID = id,
                Name = name,
                Description = description,
                Rate = rate
            };

            invoiceTax = _daoFactory.GetInvoiceTaxDao().SaveOrUpdateInvoiceTax(invoiceTax);
            _messageService.Send(MessageAction.InvoiceTaxUpdated, _messageTarget.Create(invoiceTax.ID), invoiceTax.Name);

            return _mapper.Map<InvoiceTaxDto>(invoiceTax);
        }

        /// <summary>
        ///   Delete the invoice tax with the ID specified in the request
        /// </summary>
        /// <param name="id">Tax ID</param>
        /// <short>Delete invoice tax</short> 
        /// <category>Invoices</category>
        /// <returns>InvoiceTax</returns>
        [Delete(@"invoice/tax/{id:int}")]
        public InvoiceTaxDto DeleteInvoiceTax(int id)
        {
            if (!_crmSecurity.IsAdmin)
            {
                throw _crmSecurity.CreateSecurityException();
            }

            if (id <= 0) throw new ArgumentException();

            var invoiceTax = _daoFactory.GetInvoiceTaxDao().DeleteInvoiceTax(id);
            if (invoiceTax == null) throw new ItemNotFoundException();

            _messageService.Send(MessageAction.InvoiceTaxDeleted, _messageTarget.Create(invoiceTax.ID), invoiceTax.Name);

            return _mapper.Map<InvoiceTaxDto>(invoiceTax);
        }

        /// <summary>
        ///  Get default invoice settings
        /// </summary>
        /// <short>Get default invoice settings</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceSetting</returns>
        [Read(@"invoice/settings")]
        public InvoiceSetting GetSettings()
        {
            return _daoFactory.GetInvoiceDao().GetSettings();
        }

        /// <summary>
        ///  Save default invoice number
        /// </summary>
        /// <param name="autogenerated">Is autogenerated</param>
        /// <param name="prefix">Prefix</param>
        /// <param name="number">Number</param>
        /// <short>Save default invoice number</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceSetting</returns>
        [Update(@"invoice/settings/name")]
        public InvoiceSetting SaveNumberSettings(
           SaveNumberSettingsRequestDto inDto
            )
        {
            var autogenerated = inDto.AutoGenerated;
            var number = inDto.Number;
            var prefix = inDto.Prefix;

            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            if (autogenerated && string.IsNullOrEmpty(number))
                throw new ArgumentException();

            if (autogenerated && _daoFactory.GetInvoiceDao().IsExist(prefix + number))
                throw new ArgumentException();

            var invoiceSetting = GetSettings();

            invoiceSetting.Autogenerated = autogenerated;
            invoiceSetting.Prefix = prefix;
            invoiceSetting.Number = number;

            var settings = _daoFactory.GetInvoiceDao().SaveInvoiceSettings(invoiceSetting);
            _messageService.Send(MessageAction.InvoiceNumberFormatUpdated);

            return settings;
        }

        /// <summary>
        ///  Save default invoice terms
        /// </summary>
        /// <param name="terms">Terms</param>
        /// <short>Save default invoice terms</short>
        /// <category>Invoices</category>
        /// <returns>InvoiceSetting</returns>
        [Update(@"invoice/settings/terms")]
        public InvoiceSetting SaveTermsSettings(string terms)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var invoiceSetting = GetSettings();

            invoiceSetting.Terms = terms;

            var result = _daoFactory.GetInvoiceDao().SaveInvoiceSettings(invoiceSetting);
            _messageService.Send(MessageAction.InvoiceDefaultTermsUpdated);

            return result;
        }

        /// <visible>false</visible>
        [Update(@"invoice/{invoiceid:int}/creationdate")]
        public void SetInvoiceCreationDate(int invoiceid, ApiDateTime creationDate)
        {
            var dao = _daoFactory.GetInvoiceDao();
            var invoice = dao.GetByID(invoiceid);

            if (invoice == null || !_crmSecurity.CanAccessTo(invoice))
                throw new ItemNotFoundException();

            dao.SetInvoiceCreationDate(invoiceid, creationDate);
        }

        /// <visible>false</visible>
        [Update(@"invoice/{invoiceid:int}/lastmodifeddate")]
        public void SetInvoiceLastModifedDate(int invoiceid, ApiDateTime lastModifedDate)
        {
            var dao = _daoFactory.GetInvoiceDao();
            var invoice = dao.GetByID(invoiceid);

            if (invoice == null || !_crmSecurity.CanAccessTo(invoice))
            {
                throw new ItemNotFoundException();
            }

            dao.SetInvoiceLastModifedDate(invoiceid, lastModifedDate);
        }

        private IEnumerable<InvoiceBaseDto> ToListInvoiceBaseDtos(ICollection<Invoice> items)
        {
            if (items == null || items.Count == 0) return new List<InvoiceDto>();

            var result = new List<InvoiceBaseDto>();

            var contactIDs = items.Select(item => item.ContactID);

            contactIDs.ToList().AddRange(items.Select(item => item.ConsigneeID));

            var contacts = _daoFactory.GetContactDao().GetContacts(contactIDs.Distinct().ToArray())
                                     .ToDictionary(item => item.ID, x => _mapper.Map<ContactBaseWithEmailDto>(x));

            foreach (var invoice in items)
            {
                var invoiceDto = _mapper.Map<InvoiceBaseDto>(invoice);

                if (contacts.ContainsKey(invoice.ContactID))
                {
                    invoiceDto.Contact = contacts[invoice.ContactID];
                }

                if (contacts.ContainsKey(invoice.ConsigneeID))
                {
                    invoiceDto.Consignee = contacts[invoice.ContactID];
                }

                if (invoice.EntityID > 0)
                {
                    invoiceDto.Entity = ToEntityDto(invoice.EntityType, invoice.EntityID); //Need to optimize
                }

                invoiceDto.Cost = invoice.GetInvoiceCost(_daoFactory);

                result.Add(invoiceDto);
            }

            return result;
        }

        private EntityDto ToEntityDto(EntityType entityType, int entityID)
        {
            if (entityID == 0) return null;

            var result = new EntityDto
            {
                EntityId = entityID
            };

            switch (entityType)
            {
                case EntityType.Case:
                    var caseObj = _daoFactory.GetCasesDao().GetByID(entityID);
                    if (caseObj == null)
                        return null;

                    result.EntityType = "case";
                    result.EntityTitle = caseObj.Title;

                    break;
                case EntityType.Opportunity:
                    var dealObj = _daoFactory.GetDealDao().GetByID(entityID);

                    if (dealObj == null)
                        return null;

                    result.EntityType = "opportunity";
                    result.EntityTitle = dealObj.Title;

                    break;
                default:
                    return null;
            }

            return result;
        }

    }
}