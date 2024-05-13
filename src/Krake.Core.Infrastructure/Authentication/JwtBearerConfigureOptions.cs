using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Krake.Core.Infrastructure.Authentication;

internal sealed class JwtBearerConfigureOptions(IConfiguration config)
    : IConfigureOptions<JwtBearerOptions>
{
    private const string ConfigSectionName = "Authentication";

    public void Configure(JwtBearerOptions options)
    {
        config.GetSection(ConfigSectionName).Bind(options);
    }
}