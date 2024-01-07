using Krake.Application.Portfolios;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddPortfolioModule();
        return services;
    }
}