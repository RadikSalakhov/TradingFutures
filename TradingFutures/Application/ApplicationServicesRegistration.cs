using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Application.Services;

namespace TradingFutures.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DevOptions>(configuration.GetSection(nameof(DevOptions)));

            services.Configure<GeneralOptions>(configuration.GetSection(nameof(GeneralOptions)));

            services.Configure<HuobiOptions>(configuration.GetSection(nameof(HuobiOptions)));

            services.Configure<TelegramOptions>(configuration.GetSection(nameof(TelegramOptions)));

            services.AddSingleton<ITradingAssistantService, TradingAssistantService>();

            services.AddSingleton<ITradingBotService, TradingBotService>();

            return services;
        }
    }
}