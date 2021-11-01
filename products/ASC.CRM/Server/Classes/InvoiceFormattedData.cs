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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

using ASC.Common;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class InvoiceFormattedData
    {
        private OrganisationLogoManager _organisationLogoManager;
        private DaoFactory _daoFactory;

        public InvoiceFormattedData(DaoFactory daoFactory,
            OrganisationLogoManager organisationLogoManager)
        {
            _daoFactory = daoFactory;
            _organisationLogoManager = organisationLogoManager;
        }


        public int TemplateType { get; set; }
        public Tuple<string, string> Seller { get; set; }
        public int LogoBase64Id { get; set; }
        public string LogoBase64 { get; set; }
        public string LogoSrcFormat { get; set; }
        public Tuple<string, string> Number { get; set; }
        public List<Tuple<string, string>> Invoice { get; set; }
        public Tuple<string, string> Customer { get; set; }
        public List<string> TableHeaderRow { get; set; }
        public List<List<string>> TableBodyRows { get; set; }
        public List<Tuple<string, string>> TableFooterRows { get; set; }
        public Tuple<string, string> TableTotalRow { get; set; }
        public Tuple<string, string> Terms { get; set; }
        public Tuple<string, string> Notes { get; set; }
        public Tuple<string, string> Consignee { get; set; }

        public int DeliveryAddressID { get; set; }
        public int BillingAddressID { get; set; }

        public InvoiceFormattedData GetData(Invoice invoice, int billingAddressID, int deliveryAddressID)
        {
            return invoice.JsonData != null ? ReadData(invoice.JsonData) : CreateData(invoice, billingAddressID, deliveryAddressID);
        }

        public InvoiceFormattedData GetDataAfterLinesUpdated(Invoice invoice)
        {
            if (invoice.JsonData != null)
            {
                var oldData = ReadData(invoice.JsonData);
                return CreateDataAfterLinesUpdated(invoice, oldData);
            }
            else
            {
                return CreateData(invoice, 0, 0);
            }
        }

        private InvoiceFormattedData CreateData(Invoice invoice, int billingAddressID, int deliveryAddressID)
        {
            var data = new InvoiceFormattedData(_daoFactory, _organisationLogoManager);
            var sb = new StringBuilder();
            var list = new List<string>();
            var cultureInfo = string.IsNullOrEmpty(invoice.Language)
                ? CultureInfo.CurrentCulture
                : CultureInfo.GetCultureInfo(invoice.Language);

            #region TemplateType

            data.TemplateType = (int)invoice.TemplateType;

            #endregion

            #region Seller, LogoBase64, LogoSrcFormat

            var invoiceSettings = _daoFactory.GetInvoiceDao().GetSettings();

            if (!string.IsNullOrEmpty(invoiceSettings.CompanyName))
            {
                sb.Append(invoiceSettings.CompanyName);
            }

            if (!string.IsNullOrEmpty(invoiceSettings.CompanyAddress))
            {
                var obj = JsonDocument.Parse(invoiceSettings.CompanyAddress).RootElement;

                var str = obj.GetProperty("street").GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);

                str = obj.GetProperty("city").GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);

                str = obj.GetProperty("state").GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);

                str = obj.GetProperty("zip").GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);

                str = obj.GetProperty("country").GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);

                if (list.Count > 0)
                {
                    sb.AppendLine();
                    sb.Append(string.Join(", ", list));
                }
            }

            data.Seller =
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Seller", cultureInfo),
                    sb.ToString());

            if (invoiceSettings.CompanyLogoID != 0)
            {
                data.LogoBase64Id = invoiceSettings.CompanyLogoID;
                //data.LogoBase64 = OrganisationLogoManager.GetOrganisationLogoBase64(invoiceSettings.CompanyLogoID);
                data.LogoSrcFormat = _organisationLogoManager.OrganisationLogoSrcFormat;
            }

            #endregion

            #region Number

            data.Number =
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Invoice", cultureInfo),
                    invoice.Number);

            #endregion


            #region Invoice

            data.Invoice = new List<Tuple<string, string>>();
            data.Invoice.Add(
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("IssueDate", cultureInfo),
                    invoice.IssueDate.ToShortDateString()));

            if (!string.IsNullOrEmpty(invoice.PurchaseOrderNumber))
            {
                data.Invoice.Add(
                    new Tuple<string, string>(
                        CRMInvoiceResource.ResourceManager.GetString("PONumber", cultureInfo),
                        invoice.PurchaseOrderNumber));
            }
            data.Invoice.Add(
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("DueDate", cultureInfo),
                    invoice.DueDate.ToShortDateString()));

            #endregion


            #region Customer

            var customer = _daoFactory.GetContactDao().GetByID(invoice.ContactID);

            if (customer != null)
            {
                sb = new StringBuilder();

                sb.Append(customer.GetTitle());

                var billingAddress = billingAddressID != 0
                    ? _daoFactory.GetContactInfoDao().GetByID(billingAddressID)
                    : null;
                if (billingAddress != null && billingAddress.InfoType == ContactInfoType.Address &&
                    billingAddress.Category == (int)AddressCategory.Billing)
                {
                    list = new List<string>();

                    var obj = JsonDocument.Parse(billingAddress.Data).RootElement;

                    var str = obj.GetProperty("street").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("city").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("state").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("zip").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("country").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    if (list.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append(string.Join(", ", list));
                    }
                }

                data.Customer =
                    new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("BillTo", cultureInfo),
                        sb.ToString());
            }

            #endregion


            #region TableHeaderRow, TableBodyRows, TableFooterRows, TableTotalRow

            data.TableHeaderRow = new List<string>
                {
                    CRMInvoiceResource.ResourceManager.GetString("ItemCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("QuantityCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("PriceCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("DiscountCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("TaxCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("TaxCol", cultureInfo),
                    CRMInvoiceResource.ResourceManager.GetString("AmountCol", cultureInfo)
                };

            data.TableBodyRows = new List<List<string>>();

            var invoiceLines = invoice.GetInvoiceLines(_daoFactory);
            var invoiceTaxes = new Dictionary<int, decimal>();

            decimal subtotal = 0;
            decimal discount = 0;
            decimal amount = 0;

            foreach (var line in invoiceLines)
            {
                var item = _daoFactory.GetInvoiceItemDao().GetByID(line.InvoiceItemID);
                var tax1 = line.InvoiceTax1ID > 0
                    ? _daoFactory.GetInvoiceTaxDao().GetByID(line.InvoiceTax1ID)
                    : null;
                var tax2 = line.InvoiceTax2ID > 0
                    ? _daoFactory.GetInvoiceTaxDao().GetByID(line.InvoiceTax2ID)
                    : null;

                var subtotalValue = Math.Round(line.Quantity * line.Price, 2);
                var discountValue = Math.Round(subtotalValue * line.Discount / 100, 2);

                decimal rate = 0;
                if (tax1 != null)
                {
                    rate += tax1.Rate;
                    if (invoiceTaxes.ContainsKey(tax1.ID))
                    {
                        invoiceTaxes[tax1.ID] = invoiceTaxes[tax1.ID] +
                                                Math.Round((subtotalValue - discountValue) * tax1.Rate / 100, 2);
                    }
                    else
                    {
                        invoiceTaxes.Add(tax1.ID, Math.Round((subtotalValue - discountValue) * tax1.Rate / 100, 2));
                    }
                }
                if (tax2 != null)
                {
                    rate += tax2.Rate;
                    if (invoiceTaxes.ContainsKey(tax2.ID))
                    {
                        invoiceTaxes[tax2.ID] = invoiceTaxes[tax2.ID] +
                                                Math.Round((subtotalValue - discountValue) * tax2.Rate / 100, 2);
                    }
                    else
                    {
                        invoiceTaxes.Add(tax2.ID, Math.Round((subtotalValue - discountValue) * tax2.Rate / 100, 2));
                    }
                }

                decimal taxValue = Math.Round((subtotalValue - discountValue) * rate / 100, 2);
                decimal amountValue = Math.Round(subtotalValue - discountValue + taxValue, 2);

                subtotal += subtotalValue;
                discount += discountValue;
                amount += amountValue;

                data.TableBodyRows.Add(new List<string>
                    {
                        item.Title + (string.IsNullOrEmpty(line.Description) ? string.Empty : ": " + line.Description),
                        line.Quantity.ToString(CultureInfo.InvariantCulture),
                        line.Price.ToString(CultureInfo.InvariantCulture),
                        line.Discount.ToString(CultureInfo.InvariantCulture),
                        tax1 != null ? tax1.Name : string.Empty,
                        tax2 != null ? tax2.Name : string.Empty,
                        (subtotalValue - discountValue).ToString(CultureInfo.InvariantCulture)
                    });
            }

            data.TableFooterRows = new List<Tuple<string, string>>();
            data.TableFooterRows.Add(
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Subtotal", cultureInfo),
                    (subtotal - discount).ToString(CultureInfo.InvariantCulture)));

            foreach (var invoiceTax in invoiceTaxes)
            {
                var iTax = _daoFactory.GetInvoiceTaxDao().GetByID(invoiceTax.Key);
                data.TableFooterRows.Add(new Tuple<string, string>(
                    string.Format("{0} ({1}%)", iTax.Name, iTax.Rate),
                    invoiceTax.Value.ToString(CultureInfo.InvariantCulture)));
            }

            //data.TableFooterRows.Add(new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Discount", cultureInfo), "-" + discount.ToString(CultureInfo.InvariantCulture)));

            data.TableTotalRow =
                new Tuple<string, string>(
                    string.Format("{0} ({1})", CRMInvoiceResource.ResourceManager.GetString("Total", cultureInfo),
                        invoice.Currency), amount.ToString(CultureInfo.InvariantCulture));


            #endregion


            #region Terms

            data.Terms =
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Terms", cultureInfo),
                    invoice.Terms);

            #endregion


            #region Notes

            if (!string.IsNullOrEmpty(invoice.Description))
            {
                data.Notes =
                    new Tuple<string, string>(
                        CRMInvoiceResource.ResourceManager.GetString("ClientNotes", cultureInfo),
                        invoice.Description);
            }

            #endregion


            #region Consignee

            var consignee = _daoFactory.GetContactDao().GetByID(invoice.ConsigneeID);

            if (consignee != null)
            {
                sb = new StringBuilder();

                sb.Append(consignee.GetTitle());

                var deliveryAddress = deliveryAddressID != 0
                    ? _daoFactory.GetContactInfoDao().GetByID(deliveryAddressID)
                    : null;
                if (deliveryAddress != null && deliveryAddress.InfoType == ContactInfoType.Address &&
                    deliveryAddress.Category == (int)AddressCategory.Postal)
                {
                    list = new List<string>();

                    var obj = JsonDocument.Parse(deliveryAddress.Data).RootElement;

                    var str = obj.GetProperty("street").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("city").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("state").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("zip").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    str = obj.GetProperty("country").GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);

                    if (list.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append(string.Join(", ", list));
                    }
                }

                data.Consignee =
                    new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("ShipTo", cultureInfo),
                        sb.ToString());
            }

            #endregion

            #region Addresses

            data.BillingAddressID = billingAddressID;
            data.DeliveryAddressID = deliveryAddressID;

            #endregion

            return data;

        }

        private InvoiceFormattedData ReadData(string jsonData)
        {
            var data = new InvoiceFormattedData(_daoFactory, _organisationLogoManager);
            var jsonObj = JsonDocument.Parse(jsonData).RootElement;

            #region TemplateType

            data.TemplateType = jsonObj.GetProperty("TemplateType").GetInt32();

            #endregion


            #region Seller, LogoBase64, LogoSrcFormat

            JsonElement seller;
            if (jsonObj.TryGetProperty("Seller", out seller))
            {
                data.Seller = JsonSerializer.Deserialize<Tuple<string, string>>(seller.ToString());
            }

            data.LogoBase64 = jsonObj.GetProperty("LogoBase64").GetString();
            data.LogoBase64Id = !String.IsNullOrEmpty(jsonObj.GetProperty("LogoBase64Id").GetString()) ? jsonObj.GetProperty("LogoBase64Id").GetInt32() : 0;

            if (string.IsNullOrEmpty(data.LogoBase64) && data.LogoBase64Id != 0)
            {
                data.LogoBase64 = _organisationLogoManager.GetOrganisationLogoBase64(data.LogoBase64Id);
            }


            data.LogoSrcFormat = jsonObj.GetProperty("LogoSrcFormat").GetString();

            #endregion


            #region Number

            JsonElement number;
            if (jsonObj.TryGetProperty("Number", out number))
            {
                data.Number = JsonSerializer.Deserialize<Tuple<string, string>>(number.ToString());
            }

            #endregion


            #region Invoice

            JsonElement invoice;
            if (jsonObj.TryGetProperty("Invoice", out invoice))
            {
                data.Invoice = JsonSerializer.Deserialize<List<Tuple<string, string>>>(invoice.ToString());
            }

            #endregion


            #region Customer

            JsonElement customer;
            if (jsonObj.TryGetProperty("Customer", out customer))
            {
                data.Customer = JsonSerializer.Deserialize<Tuple<string, string>>(customer.ToString());
            }

            #endregion


            #region TableHeaderRow, TableBodyRows, TableFooterRows, Total

            JsonElement tableHeaderRow;
            if (jsonObj.TryGetProperty("TableHeaderRow", out tableHeaderRow))
            {
                data.TableHeaderRow = tableHeaderRow.EnumerateArray().Select(x => x.GetString()).ToList();
            }

            JsonElement tableBodyRows;
            if (jsonObj.TryGetProperty("TableBodyRows", out tableBodyRows))
            {
                data.TableBodyRows = JsonSerializer.Deserialize<List<List<string>>>(tableBodyRows.ToString());
            }

            JsonElement tableFooterRows;
            if (jsonObj.TryGetProperty("TableFooterRows", out tableFooterRows))
            {
                data.TableFooterRows = JsonSerializer.Deserialize<List<Tuple<string, string>>>(tableFooterRows.ToString());
            }

            JsonElement tableTotalRow;
            if (jsonObj.TryGetProperty("TableTotalRow", out tableTotalRow))
            {
                data.TableTotalRow = JsonSerializer.Deserialize<Tuple<string, string>>(tableTotalRow.ToString());
            }

            #endregion


            #region Terms

            JsonElement terms;
            if (jsonObj.TryGetProperty("Terms", out terms))
            {
                data.Terms = JsonSerializer.Deserialize<Tuple<string, string>>(terms.ToString());
            }

            #endregion


            #region Notes

            JsonElement notes;
            if (jsonObj.TryGetProperty("Notes", out notes))
            {
                data.Notes = JsonSerializer.Deserialize<Tuple<string, string>>(notes.ToString());
            }

            #endregion


            #region Consignee

            JsonElement consignee;
            if (jsonObj.TryGetProperty("Consignee", out consignee))
            {
                data.Consignee = JsonSerializer.Deserialize<Tuple<string, string>>(consignee.ToString());
            }

            #endregion


            #region Addresses

            data.DeliveryAddressID = !String.IsNullOrEmpty(jsonObj.GetProperty("DeliveryAddressID").GetString()) ? jsonObj.GetProperty("DeliveryAddressID").GetInt32() : 0;
            data.BillingAddressID = !String.IsNullOrEmpty(jsonObj.GetProperty("BillingAddressID").GetString()) ? jsonObj.GetProperty("BillingAddressID").GetInt32() : 0;

            #endregion

            return data;
        }

        private InvoiceFormattedData CreateDataAfterLinesUpdated(Invoice invoice,
            InvoiceFormattedData invoiceOldData)
        {
            var data = invoiceOldData;

            var cultureInfo = string.IsNullOrEmpty(invoice.Language)
                ? CultureInfo.CurrentCulture
                : CultureInfo.GetCultureInfo(invoice.Language);

            #region TableBodyRows, TableFooterRows, TableTotalRow

            data.TableBodyRows = new List<List<string>>();

            var invoiceLines = invoice.GetInvoiceLines(_daoFactory);
            var invoiceTaxes = new Dictionary<int, decimal>();

            decimal subtotal = 0;
            decimal discount = 0;
            decimal amount = 0;

            foreach (var line in invoiceLines)
            {
                var item = _daoFactory.GetInvoiceItemDao().GetByID(line.InvoiceItemID);
                var tax1 = line.InvoiceTax1ID > 0
                    ? _daoFactory.GetInvoiceTaxDao().GetByID(line.InvoiceTax1ID)
                    : null;
                var tax2 = line.InvoiceTax2ID > 0
                    ? _daoFactory.GetInvoiceTaxDao().GetByID(line.InvoiceTax2ID)
                    : null;

                var subtotalValue = Math.Round(line.Quantity * line.Price, 2);
                var discountValue = Math.Round(subtotalValue * line.Discount / 100, 2);

                decimal rate = 0;
                if (tax1 != null)
                {
                    rate += tax1.Rate;
                    if (invoiceTaxes.ContainsKey(tax1.ID))
                    {
                        invoiceTaxes[tax1.ID] = invoiceTaxes[tax1.ID] +
                                                Math.Round((subtotalValue - discountValue) * tax1.Rate / 100, 2);
                    }
                    else
                    {
                        invoiceTaxes.Add(tax1.ID, Math.Round((subtotalValue - discountValue) * tax1.Rate / 100, 2));
                    }
                }
                if (tax2 != null)
                {
                    rate += tax2.Rate;
                    if (invoiceTaxes.ContainsKey(tax2.ID))
                    {
                        invoiceTaxes[tax2.ID] = invoiceTaxes[tax2.ID] +
                                                Math.Round((subtotalValue - discountValue) * tax2.Rate / 100, 2);
                    }
                    else
                    {
                        invoiceTaxes.Add(tax2.ID, Math.Round((subtotalValue - discountValue) * tax2.Rate / 100, 2));
                    }
                }

                decimal taxValue = Math.Round((subtotalValue - discountValue) * rate / 100, 2);
                decimal amountValue = Math.Round(subtotalValue - discountValue + taxValue, 2);

                subtotal += subtotalValue;
                discount += discountValue;
                amount += amountValue;

                data.TableBodyRows.Add(new List<string>
                    {
                        item.Title + (string.IsNullOrEmpty(line.Description) ? string.Empty : ": " + line.Description),
                        line.Quantity.ToString(CultureInfo.InvariantCulture),
                        line.Price.ToString(CultureInfo.InvariantCulture),
                        line.Discount.ToString(CultureInfo.InvariantCulture),
                        tax1 != null ? tax1.Name : string.Empty,
                        tax2 != null ? tax2.Name : string.Empty,
                        (subtotalValue - discountValue).ToString(CultureInfo.InvariantCulture)
                    });
            }

            data.TableFooterRows = new List<Tuple<string, string>>();
            data.TableFooterRows.Add(
                new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Subtotal", cultureInfo),
                    (subtotal - discount).ToString(CultureInfo.InvariantCulture)));

            foreach (var invoiceTax in invoiceTaxes)
            {
                var iTax = _daoFactory.GetInvoiceTaxDao().GetByID(invoiceTax.Key);
                data.TableFooterRows.Add(new Tuple<string, string>(
                    string.Format("{0} ({1}%)", iTax.Name, iTax.Rate),
                    invoiceTax.Value.ToString(CultureInfo.InvariantCulture)));
            }

            //data.TableFooterRows.Add(new Tuple<string, string>(CRMInvoiceResource.ResourceManager.GetString("Discount", cultureInfo), "-" + discount.ToString(CultureInfo.InvariantCulture)));

            data.TableTotalRow =
                new Tuple<string, string>(
                    string.Format("{0} ({1})", CRMInvoiceResource.ResourceManager.GetString("Total", cultureInfo),
                        invoice.Currency), amount.ToString(CultureInfo.InvariantCulture));


            #endregion

            return data;
        }

    }
}