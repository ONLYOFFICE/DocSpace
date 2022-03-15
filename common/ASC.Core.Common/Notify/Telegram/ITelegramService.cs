namespace ASC.Core.Common.Notify;

public interface ITelegramService
{
    void CreateOrUpdateClient(int tenantId, string token, int tokenLifespan, string proxy);
    void DisableClient(int tenantId);
    void RegisterUser(string userId, int tenantId, string token);
    void SendMessage(NotifyMessage m);
}
