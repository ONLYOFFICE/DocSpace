namespace ASC.Core.Billing;

[Serializable]
public class PaymentInfo
{
    public int ID { get; set; }
    public int Status { get; set; }
    public int PaymentSystemId { get; set; }
    public string CartId { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string Email { get; set; }
    public DateTime PaymentDate { get; set; }
    public Decimal Price { get; set; }
    public string PaymentCurrency { get; set; }
    public string PaymentMethod { get; set; }
    public int QuotaId { get; set; }
    public string ProductRef { get; set; }
    public string CustomerId { get; set; }
}
