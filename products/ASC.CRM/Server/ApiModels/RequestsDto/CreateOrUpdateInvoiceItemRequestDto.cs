namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateInvoiceItemRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        public int InvoiceTax1id { get; set; }
        public int InvoiceTax2id { get; set; }

    }
}
