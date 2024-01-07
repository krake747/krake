using Krake.Infrastructure.Email.Common;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

namespace Krake.Infrastructure.Email.Services;

public interface IMailKitEmailRetriever : IGenericEmailRetriever<MimeMessage>;

public sealed class MailKitEmailRetriever(EmailAppSettings appSettings) : IMailKitEmailRetriever
{
    public MimeMessage Retrieve()
    {
        ArgumentNullException.ThrowIfNull(appSettings.IncomingEmailServer);
        ArgumentNullException.ThrowIfNull(appSettings.EmailAuth);

        using var client = new ImapClient();
        var server = appSettings.IncomingEmailServer;
        var auth = appSettings.EmailAuth;
        var secureSocketOption = server.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
        client.Connect(server.Host, server.Port, secureSocketOption);
        client.Authenticate(auth.UserName, auth.Password);

        client.Inbox.Open(FolderAccess.ReadOnly);
        var maxEmailCount = client.Inbox.Count > 9 ? 10 : client.Inbox.Count;
        for (var i = 0; i < maxEmailCount; i++)
        {
            var message = client.Inbox.GetMessage(i);
            Console.WriteLine("Subject: {0}", message.Subject);
            Console.WriteLine("From: {0}", message.From);
        }

        client.Disconnect(true);
        return new MimeMessage();
    }


    public async Task<MimeMessage> RetrieveAsync(CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(appSettings.IncomingEmailServer);
        ArgumentNullException.ThrowIfNull(appSettings.EmailAuth);

        using var client = new ImapClient();
        var mailServer = appSettings.IncomingEmailServer;
        var server = appSettings.IncomingEmailServer;
        var auth = appSettings.EmailAuth;
        var secureSocketOption = mailServer.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
        await client.ConnectAsync(server.Host, server.Port, secureSocketOption, token);
        await client.AuthenticateAsync(auth.UserName, auth.Password, token);

        await client.Inbox.OpenAsync(FolderAccess.ReadOnly, token);
        var maxEmailCount = client.Inbox.Count > 9 ? 10 : client.Inbox.Count;
        for (var i = 0; i < maxEmailCount; i++)
        {
            var message = await client.Inbox.GetMessageAsync(i, token);
            Console.WriteLine("Subject: {0}", message.Subject);
            Console.WriteLine("From: {0}", message.From);
        }

        await client.DisconnectAsync(true, token);

        return new MimeMessage();
    }
}