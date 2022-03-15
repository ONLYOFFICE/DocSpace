namespace ASC.Core.Billing;

[DebuggerDisplay("{QuotaId} ({State} before {DueDate})")]
[Serializable]
public class Tariff
{
    public int QuotaId { get; set; }
    public TariffState State { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime DelayDueDate { get; set; }
    public DateTime LicenseDate { get; set; }
    public bool Autorenewal { get; set; }
    public bool Prolongable { get; set; }
    public int Quantity { get; set; }

    public static Tariff CreateDefault()
    {
        return new Tariff
        {
            QuotaId = Tenant.DefaultTenant,
            State = TariffState.Paid,
            DueDate = DateTime.MaxValue,
            DelayDueDate = DateTime.MaxValue,
            LicenseDate = DateTime.MaxValue,
            Quantity = 1
        };
    }


    public override int GetHashCode()
    {
        return QuotaId.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Tariff t && t.QuotaId == QuotaId;
    }

    public bool EqualsByParams(Tariff t)
    {
        return t != null
            && t.QuotaId == QuotaId
            && t.DueDate == DueDate
            && t.Quantity == Quantity;
    }
}
