using System.Reflection;
using Cocona;
using Krake.Cli.Features;
using Krake.Cli.Features.Comdirect;
using Krake.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = CoconaApp.CreateBuilder();

builder.Configuration
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSingleton(Log.Logger);

builder.Services.AddInfrastructureModule(builder.Configuration, "KrakeDB");
builder.Services.AddFeaturesModule(builder.Configuration);

var app = builder.Build();

app.AddCommand("import", (ServiceProvider sp) => sp.GetRequiredService<ComdirectImporterApp>().Run())
    .WithDescription("Import file");

try
{
#if DEBUG
    app.Services.GetRequiredService<ComdirectImporterApp>().Run();
#endif

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