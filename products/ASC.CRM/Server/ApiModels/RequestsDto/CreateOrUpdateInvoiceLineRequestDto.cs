namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateInvoiceLineRequestDto
    {
        public int InvoiceId { get; set; }
        public int InvoiceItemId { get; set; }
        public int InvoiceTax1Id { get; set; }
        public int InvoiceTax2Id { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
    }
}
