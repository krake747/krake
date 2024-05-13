using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Krake.Core.Infrastructure.Authentication;

internal sealed class JwtBearerConfigureOptions(IConfiguration config)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigSectionName = "Authentication";

    public void Configure(JwtBearerOptions options)
    {
        config.GetSection(ConfigSectionName).Bind(options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}