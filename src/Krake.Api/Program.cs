using HealthChecks.UI.Client;
using Krake.Api.Middleware;
using Krake.Modules.Portfolios.Infrastructure;
using Krake.Modules.Portfolios.Presentation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Service registration starts here

builder.Host.UseSerilog(static (ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("SqlDatabase")!)
    .AddRedis(builder.Configuration.GetConnectionString("RedisCache")!);

builder.Services.AddPortfoliosModule(builder.Configuration);

// Service registration ends here

var app = builder.Build();

// Middleware registration starts here

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

// app.UseHttpsRedirection();

app.MapGet("/", () => "Krake Rest Web Api")
    .WithOpenApi()
    .WithTags("Welcome")
    .WithName("GetWelcome")
    .WithSummary("Welcome")
    .WithDescription("Welcome to Krake Rest Web Api");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapPortfoliosModuleEndpoints();

// Middleware Registration ends here

try
{
    Log.Information("Starting Krake Web Api host");
    app.Run();
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}