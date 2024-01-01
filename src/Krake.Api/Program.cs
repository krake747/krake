using Krake.Api.Endpoints;
using Krake.Application;
using Krake.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Service registration starts here

builder.Host.UseSerilog((context, lc) => lc.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("KrakeDB")!);

// Service registration ends here

var app = builder.Build();

// Middleware registration starts here

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api", () => "Krake Rest Web Api")
    .WithTags("Welcome")
    .WithName("GetWelcome")
    .WithOpenApi();

app.MapEndpoints();

// Middleware Registration ends here

try
{
    Log.Information("Starting Krake Web Api host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception Krake Web API host terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}