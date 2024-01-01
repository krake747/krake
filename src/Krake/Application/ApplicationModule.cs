using Krake.Application.Portfolios;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddPortfolioModule();
        return services;
    }
}