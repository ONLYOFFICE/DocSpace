namespace ASC.TelegramService.Core;

public class TenantTgClient
{
    public string Token { get; set; }
    public TelegramBotClient Client { get; set; }
    public string Proxy { get; set; }
    public int TokenLifeSpan { get; set; }
    public int TenantId { get; set; }
}
