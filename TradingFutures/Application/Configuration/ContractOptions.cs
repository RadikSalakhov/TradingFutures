using Microsoft.Extensions.Configuration;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Application.Configuration
{
    public class ContractOptions
    {
        public string Asset { get; set; } = string.Empty;

        public decimal ContractSize { get; set; }

        public int VolumeStep { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Asset) && ContractSize > 0m && VolumeStep > 0;
        }

        public decimal GetCoinsStep()
        {
            return ContractSize * VolumeStep;
        }

        public static Dictionary<string, ContractOptions> ParseFromConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                return new();

            var resultDict = new Dictionary<string, ContractOptions>();

            var contractOptionsList = configuration.GetSection(nameof(ContractOptions)).Get<ContractOptions[]>() ?? Array.Empty<ContractOptions>();
            foreach (var contractOptions in contractOptionsList)
            {
                if (!contractOptions.IsValid())
                    continue;

                var contractCode = TradingPositionEntity.GetContractCode(contractOptions.Asset);
                if (!resultDict.ContainsKey(contractCode))
                    resultDict.Add(contractCode, contractOptions);
            }

            return resultDict;
        }
    }
}