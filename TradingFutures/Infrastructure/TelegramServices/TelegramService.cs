using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Net;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Utils;

namespace TradingFutures.Infrastructure.TelegramServices
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;

        private readonly TelegramOptions _telegramOptions;

        private readonly IHuobiClientService _huobiClientService;

        private readonly ICacheService _serverCache;

        private readonly Queue _messagesQueue = new Queue();

        public TelegramService(
            ILogger<TelegramService> logger,
            IOptions<TelegramOptions> telegramOptions,
            IHuobiClientService huobiClientService,
            ICacheService serverCache)
        {
            _logger = logger;
            _telegramOptions = telegramOptions.Value;
            _huobiClientService = huobiClientService;
            _serverCache = serverCache;

            _huobiClientService.TransactionCompleted += huobiClientService_TransactionCompleted;
        }

        public async Task SendMessage(string message)
        {
            var url = _telegramOptions.TelegramUrl;
            var token = WebUtility.UrlEncode(_telegramOptions.TelegramBotToken);
            var chatId = _telegramOptions.TelegramChatId;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(chatId))
                return;

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            var requestUri = $"bot{token}/sendMessage?chat_id={chatId}&text={WebUtility.UrlEncode(message)}";

            await httpClient.GetAsync(requestUri);
        }

        public Task EnqueueMessage(string message)
        {
            lock (_messagesQueue)
            {
                _messagesQueue.Enqueue(message);
            }

            return Task.CompletedTask;
        }

        public async Task ProcessMessages()
        {
            var message = string.Empty;
            lock (_messagesQueue)
            {
                if (_messagesQueue.Count > 0)
                    message = _messagesQueue.Dequeue() as string;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                var serverSettings = _serverCache.ServerSettings.Get();
                if (serverSettings.TelegramNotificationsEnabled)
                    await SendMessage(message);
            }
        }

        private async Task huobiClientService_TransactionCompleted(TradingTransactionEntity tradingTransaction)
        {
            if (tradingTransaction == null)
                return;

            var priceStr = CommonUtilities.ToDefaultString(tradingTransaction.Price);
            var feeStr = CommonUtilities.ToDefaultString(tradingTransaction.Fee);

            var orderType = tradingTransaction.GetTradingOrderType();

            if (orderType.IsOpen())
            {
                var message = $"{orderType}: {tradingTransaction.ContractCode}; Price: {priceStr}; {tradingTransaction.Label}";
                _logger.LogInformation(message);

                await EnqueueMessage(message);
            }

            if (orderType.IsCLose())
            {
                var orderPNL = CommonUtilities.ToDefaultString(tradingTransaction.GetPNL());
                var message = $"{orderType}: {tradingTransaction.ContractCode}; Price: {priceStr}; PNL: {orderPNL}; {tradingTransaction.Label}";
                _logger.LogInformation(message);

                await EnqueueMessage(message);
            }
        }
    }
}