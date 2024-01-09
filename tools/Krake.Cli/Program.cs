using System.Reflection;
using Krake.Cli.Features;
using Krake.Cli.Features.Comdirect;
using Krake.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

if (args.Length is 0)
{
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

services.AddSingleton<IConfiguration>(config);
services.AddSingleton(Log.Logger);
services.AddInfrastructureModule(config, "KrakeDB");
services.AddFeaturesModule(config);

var serviceProvider = services.BuildServiceProvider();

var app = args switch
{
    [var key] => serviceProvider.GetRequiredKeyedService<ComdirectImporterApp>(key),
    _ => throw new ArgumentException("Key not defined")
};

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception Krake.Cli console app terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}