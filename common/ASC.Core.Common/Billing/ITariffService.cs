namespace ASC.Core.Billing;

[Scope(typeof(ConfigureTariffService))]
public interface ITariffService
{
    IDictionary<string, Dictionary<string, decimal>> GetProductPriceInfo(params string[] productIds);
    IEnumerable<PaymentInfo> GetPayments(int tenantId);
    string GetButton(int tariffId, string partnerId);
    Tariff GetTariff(int tenantId, bool withRequestToPaymentSystem = true);
    Uri GetShoppingUri(int? tenant, int quotaId, string affiliateId, string currency = null, string language = null, string customerId = null, string quantity = null);
    void ClearCache(int tenantId);
    void DeleteDefaultBillingInfo();
    void SaveButton(int tariffId, string partnerId, string buttonUrl);
    void SetTariff(int tenantId, Tariff tariff);
}
