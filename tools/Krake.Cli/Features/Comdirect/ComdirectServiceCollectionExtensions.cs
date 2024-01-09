using Krake.Cli.Features.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features.Comdirect;

public static class ComdirectServiceCollectionExtensions
{
    public static IServiceCollection AddComdirectFeatureModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddKeyedSingleton(new DirectoryManager(config["AppRootDirectory"]!), "comdirect");
        services.AddKeyedSingleton<ComdirectFileManager>("comdirect");
        services.AddKeyedSingleton<ComdirectImporterApp>("comdirect");
        return services;
    }
}