using System.Reflection;
using Cocona;
using Krake.Cli.Features;
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
// builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
//     new SqlConnectionFactory(builder.Configuration.GetConnectionString("KrakeDB")!));

builder.Services.AddFeatures(builder.Configuration);

var app = builder.Build();

app.AddCommand("import", (ComdirectImporterApp comdirectImporterApp) => comdirectImporterApp.Run())
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