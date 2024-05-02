using Krake.Infrastructure.Email.Models;
using Krake.Infrastructure.Email.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Infrastructure.Email;

public static class EmailServiceCollectionExtensions
{
    public static IServiceCollection AddEmailModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(config.GetSection(EmailAppSettings.SectionName).Get<EmailAppSettings>()!);
        services.AddSingleton(config.GetSection(nameof(EmailTemplate)).Get<EmailTemplate>()!);
        services.AddScoped<IMailKitEmailSender, MailKitEmailSender>();
        services.AddScoped<IMailKitEmailRetriever, MailKitEmailRetriever>();
        services.AddScoped<IMailKitEmailService, MailKitEmailService>();
        return services;
    }
}