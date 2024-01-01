using Microsoft.Extensions.DependencyInjection;

namespace Krake.Application.Portfolios;

public static class PortfolioModule
{
    public static IServiceCollection AddPortfolioModule(this IServiceCollection services)
    {
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        return services;
    }
}