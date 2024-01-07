using Krake.Cli.Features.Comdirect;
using Krake.Cli.Features.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features;

public static class FeaturesServiceCollectionExtensions
{
    public static IServiceCollection AddFeaturesModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient(_ => new DirectoryManager(config["AppRootDirectory"]!));
        services.AddComdirectFeatureModule(config);
        return services;
    }
}