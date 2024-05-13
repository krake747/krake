using Microsoft.Extensions.DependencyInjection;

namespace Krake.Core.Infrastructure.Authentication;

internal static class AuthenticationExtensions
{
    public static IServiceCollection AddCoreAuthentication(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication().AddJwtBearer();
        services.AddHttpContextAccessor();
        services.ConfigureOptions<JwtBearerConfigureOptions>();

        return services;
    }
}