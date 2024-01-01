using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Application.Portfolios;

public static class PortfolioModule
{
    public static IServiceCollection AddPortfolioModule(this IServiceCollection services)
    {
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddValidatorsFromAssemblyContaining(typeof(PortfolioValidator));
        return services;
    }
}