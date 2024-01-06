using System.Data;
using System.Reflection;
using Cocona;
using Dapper;
using Krake.Application.Portfolios;
using Krake.Infrastructure.Database;
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
builder.Services.AddTransient<IDbConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("KrakeDB")!));

var app = builder.Build();

app.AddCommand("import", (
    [FromService] ILogger logger,
    [FromService] IDbConnectionFactory connectionFactory,
    [Argument(Name = "name")] string name) =>
{
    logger.Information("Hello {Name}", name);
    var connection = connectionFactory.CreateConnection();
    var firstPortfolio = connection.QuerySingle<Portfolio>("Select top 1 [Id], [Name] from portfolios");
    logger.Information("Found {Id} - {Name}", firstPortfolio.Id, firstPortfolio.Name);
}).WithDescription("Import file");

try
{
    var connectionFactory = app.Services.GetRequiredService<IDbConnectionFactory>();
    using (var connection = connectionFactory.CreateConnection())
    {
        if (connection.State is not ConnectionState.Open)
        {
            connection.Close();
            throw new Exception("Make sure database is running");
        }
    }

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