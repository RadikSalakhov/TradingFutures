using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Services;

namespace TradingFutures.Shared
{
    public static class SharedServicesRegistration
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
    }
}