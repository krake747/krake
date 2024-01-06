using Krake.Cli.Features.Comdirect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features;

public static class FeaturesServicesExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient(_ => new DirectoryManager(config["AppRootDirectory"]!));
        services.AddComdirectServices();
        services.AddSingleton<ComdirectImporterApp>();
        return services;
    }
}