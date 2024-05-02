using Krake.Infrastructure.Email.Common;
using MimeKit;

namespace Krake.Infrastructure.Email.Services;

public interface IMailKitEmailService : IGenericEmailService<MimeMessage>;

public sealed class MailKitEmailService(IMailKitEmailSender sender, IMailKitEmailRetriever receiver)
    : IMailKitEmailService
{
    public bool Send(MimeMessage emailMessage)
    {
        var response = sender.Send(emailMessage);
        return response.Contains("2.0.0 OK");
    }

    public async Task<bool> SendAsync(MimeMessage emailMessage, CancellationToken token = default)
    {
        var response = await sender.SendAsync(emailMessage, token);
        return response.Contains("2.0.0 OK");
    }

    public MimeMessage Retrieve()
    {
        var mimeMessage = receiver.Retrieve();
        return new MimeMessage();
    }
}