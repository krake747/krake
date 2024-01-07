using Krake.Infrastructure.Email.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Krake.Infrastructure.Email.Services;

public interface IMailKitEmailSender : IGenericEmailSender<MimeMessage>;

public sealed class MailKitEmailSender(EmailAppSettings appSettings) : IMailKitEmailSender
{
    public string Send(MimeMessage email)
    {
        ArgumentNullException.ThrowIfNull(appSettings.OutgoingEmailServer);
        ArgumentNullException.ThrowIfNull(appSettings.EmailAuth);

        using var smtp = new SmtpClient();
        var server = appSettings.OutgoingEmailServer;
        var auth = appSettings.EmailAuth;
        var secureSocketOption = server.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
        smtp.Connect(server.Host, server.Port, secureSocketOption);
        smtp.Authenticate(auth.UserName, auth.Password);
        var response = smtp.Send(email);
        smtp.Disconnect(true);

        return response;
    }

    public async Task<string> SendAsync(MimeMessage email, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(appSettings.OutgoingEmailServer);
        ArgumentNullException.ThrowIfNull(appSettings.EmailAuth);

        using var smtp = new SmtpClient();
        var server = appSettings.OutgoingEmailServer;
        var auth = appSettings.EmailAuth;
        var secureSocketOption = server.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
        await smtp.ConnectAsync(server.Host, server.Port, secureSocketOption, token);
        await smtp.AuthenticateAsync(auth.UserName, auth.Password, token);
        var response = await smtp.SendAsync(email, token);
        await smtp.DisconnectAsync(true, token);

        return response;
    }
}