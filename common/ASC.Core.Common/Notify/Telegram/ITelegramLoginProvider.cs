namespace ASC.Core.Common.Notify.Telegram;

public interface ITelegramLoginProvider
{
    string TelegramBotToken { get; }
    string TelegramBotName { get; }
    int TelegramAuthTokenLifespan { get; }
    string TelegramProxy { get; }
}
