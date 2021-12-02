namespace ASC.CRM.ApiModels
{
    public class CreateCurrencyRateRequestDto
    {
       public string FromCurrency { get; set; }
       public string ToCurrency { get; set; }
       public decimal Rate { get; set; }
    }
}
