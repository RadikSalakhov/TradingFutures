using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TradingFutures.Persistence.DBModels;

namespace TradingFutures.Persistence.Contexts
{
    public class DataContext : DbContext
    {
        public DbSet<SettingsItemDB> GeneralSettingsItems { get; set; }

        public DbSet<TradingConditionDB> TradingConditions { get; set; }

        public DbSet<TradingPositionSettingsDB> TradingPositionSettings { get; set; }

        public DbSet<TradingProfileDB> TradingProfiles { get; set; }

        public DbSet<TradingTransactionDB> TradingTransactions { get; set; }

        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"../Server");

                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(path)
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("SqlServer");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.NoAction);
            }
        }
    }
}