using HealthChecks.UI.Client;
using Krake.Api.Middleware;
using Krake.Api.Swagger;
using Krake.Modules.Portfolios.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Service registration starts here

Log.Information("Registering services");

builder.Host.UseSerilog(static (ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(SwashbuckleSchemaHelper.GetSchemaId);
    options.SchemaFilter<RequiredSchemaFilter>();
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("SqlDatabase")!)
    .AddRedis(builder.Configuration.GetConnectionString("RedisCache")!)
    .AddUrlGroup(new Uri(builder.Configuration["KeyCloak:HealthUrl"]!), HttpMethod.Get, "keycloak");

builder.Services.AddPortfoliosModule(builder.Configuration, Log.Logger);

// Service registration ends here

var app = builder.Build();

// Middleware registration starts here

Log.Information("Environment is {Mode} mode", app.Environment.IsDevelopment() ? "development" : "production");

Log.Information("Registering middleware");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

// app.UseHttpsRedirection();

// Middleware registration ends here

// API endpoints registration starts here

Log.Information("Registering API endpoints");

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

app.MapPortfoliosModuleEndpoints(Log.Logger);

// Endpoints registration ends here

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