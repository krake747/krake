using Krake.Core.Application.Data;
using Krake.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Krake.Api.Tests.Integration.Common;

public sealed class KrakeApiFactory : WebApplicationFactory<IKrakeApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .Build();

        builder.UseConfiguration(config);
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new SqlConnectionFactory(config.GetConnectionString("SqlDatabase")!));
        });
    }
}