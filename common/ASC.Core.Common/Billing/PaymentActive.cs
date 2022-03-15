namespace ASC.Core.Billing;

[Serializable]
public class PaymentLast
{
    public string ProductId { get; set; }
    public DateTime EndDate { get; set; }
    public bool Autorenewal { get; set; }
    public int PaymentStatus { get; set; }
    public int Quantity { get; set; }
}
