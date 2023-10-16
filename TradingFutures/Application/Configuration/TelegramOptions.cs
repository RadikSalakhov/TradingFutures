namespace TradingFutures.Application.Configuration
{
    public class TelegramOptions
    {
        public string TelegramUrl { get; set; } = string.Empty;

        public string TelegramBotToken { get; set; } = string.Empty;

        public string TelegramChatId { get; set; } = string.Empty;
    }
}