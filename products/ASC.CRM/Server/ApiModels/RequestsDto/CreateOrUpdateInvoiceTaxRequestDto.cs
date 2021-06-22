namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateInvoiceTaxRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
    }
}
