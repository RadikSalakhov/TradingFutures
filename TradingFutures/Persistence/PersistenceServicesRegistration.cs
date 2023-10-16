using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.RepositoryServices;

namespace TradingFutures.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });

            services.AddTransient<ISettingsItemRepositoryService, SettingsItemRepositoryService>();

            services.AddTransient<ITradingConditionRepositoryService, TradingConditionRepositoryService>();

            services.AddTransient<ITradingPositionSettingsRepositoryService, TradingPositionSettingsRepositoryService>();

            services.AddTransient<ITradingProfileRepositoryService, TradingProfileRepositoryService>();

            services.AddTransient<ITradingTransactionRepositoryService, TradingTransactionRepositoryService>();

            return services;
        }
    }
}