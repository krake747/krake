using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features.Comdirect;

public static class ComdirectServicesExtensions
{
    public static IServiceCollection AddComdirectServices(this IServiceCollection services)
    {
        services.AddSingleton<ComdirectFileManager>();
        return services;
    }
}