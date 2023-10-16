using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingFutures.Application.Abstraction;
using TradingFutures.Infrastructure.HuobiClientServices;
using TradingFutures.Infrastructure.TelegramServices;
using TradingFutures.Infrastructure.TradingApiClientServices;

namespace TradingFutures.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHuobiClientService, HuobiClientService>();

            services.AddSingleton<ITradingApiClientService, TradingApiClientService>();

            services.AddSingleton<ITelegramService, TelegramService>();

            return services;
        }
    }
}