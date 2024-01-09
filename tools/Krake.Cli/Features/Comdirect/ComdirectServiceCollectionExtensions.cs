using Krake.Cli.Features.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features.Comdirect;

public static class ComdirectServiceCollectionExtensions
{
    private const string Key = "comdirect";

    public static IServiceCollection AddComdirectFeatureModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddKeyedSingleton(Key, new DirectoryManager(config["Apps:Comdirect:RootDirectory"]!));
        services.AddKeyedSingleton<ComdirectFileManager>(Key);
        services.AddKeyedSingleton<ComdirectImporterApp>(Key);
        return services;
    }
}