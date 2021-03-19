using System.Collections.Generic;

using ASC.Api.Core;
using ASC.CRM.Core.Entities;

namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateInvoiceRequestDto
    {
        public string Number { get; set; }
        public ApiDateTime IssueDate { get; set; }
        public int TemplateType { get; set; }
        public int ContactId { get; set; }
        public int ConsigneeId { get; set; }
        public int EntityId { get; set; }
        public int BillingAddressID { get; set; }
        public int DeliveryAddressID { get; set; }
        public ApiDateTime DueDate { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Terms { get; set; }
        public string Description { get; set; }
        public IEnumerable<InvoiceLine> InvoiceLines { get; set; }
    }
}
