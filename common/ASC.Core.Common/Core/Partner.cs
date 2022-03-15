namespace ASC.Core;

public class Partner
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Url { get; set; }
    public string Phone { get; set; }
    public string Language { get; set; }
    public string CompanyName { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public bool CountryHasVat { get; set; }
    public string Address { get; set; }
    public string VatId { get; set; }
    public DateTime CreationDate { get; set; }
    public PartnerStatus Status { get; set; }
    public string Comment { get; set; }
    public string Portal { get; set; }
    public bool PortalConfirmed { get; set; }
    public bool IsAdmin => PartnerType == PartnerType.Administrator;
    public decimal Limit { get; set; }
    public int Discount { get; set; }
    public string PayPalAccount { get; set; }
    public decimal Deposit { get; set; }
    public bool Removed { get; set; }
    public string Currency { get; set; }
    public string LogoUrl { get; set; }
    public string DisplayName { get; set; }
    public PartnerDisplayType DisplayType { get; set; }
    public string SupportPhone { get; set; }
    public string SupportEmail { get; set; }
    public string SalesEmail { get; set; }
    public string TermsUrl { get; set; }
    public string Theme { get; set; }
    public string RuAccount { get; set; }
    public string RuBank { get; set; }
    public string RuKs { get; set; }
    public string RuKpp { get; set; }
    public string RuBik { get; set; }
    public string RuInn { get; set; }
    public PartnerType PartnerType { get; set; }
    public PartnerPaymentMethod PaymentMethod { get; set; }
    public string PaymentUrl { get; set; }
    public decimal AvailableCredit { get; set; }
    public bool CustomEmailSignature { get; set; }
    public string AuthorizedKey { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Partner p && p.Id == Id;
    }

    public override int GetHashCode()
    {
        return (Id ?? string.Empty).GetHashCode();
    }

    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
}
