using Krake.Cli.Features.Comdirect;
using Krake.Cli.Features.FinanceData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features;

public static class FeaturesServiceCollectionExtensions
{
    public static IServiceCollection AddFeaturesModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddComdirectFeatureModule(config);
        services.AddFinanceDataModule(config);
        return services;
    }
}