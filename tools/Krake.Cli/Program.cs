using System.Reflection;
using Krake.Cli.Features;
using Krake.Cli.Features.Common;
using Krake.Core.Application.Data;
using Krake.Core.Infrastructure.Data;
using Krake.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Spectre.Console;

if (args.Length is 0)
{
    // args = ["finance-data"];
    args = ["comdirect"];
}

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(_ => config);
services.AddSingleton(_ => Log.Logger);
services.AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console);
services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(config.GetConnectionString("KrakeDb")!));
services.TryAddSingleton<TimeProvider>(_ => TimeProvider.System);
services.AddEmailModule(config);
services.AddFeaturesModule(config);

var serviceProvider = services.BuildServiceProvider();

var app = args switch
{
    [var key and "comdirect"] => serviceProvider.GetRequiredKeyedService<IImporterApplication>(key),
    [var key and "finance-data"] => serviceProvider.GetRequiredKeyedService<IImporterApplication>(key),
    _ => throw new ArgumentException("Key not defined")
};

try
{
    app.Run(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception Krake.Cli console app terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}