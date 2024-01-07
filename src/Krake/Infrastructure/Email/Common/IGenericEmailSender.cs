namespace Krake.Infrastructure.Email.Common;

public interface IGenericEmailSender<in TEmailMessage>
{
    string Send(TEmailMessage emailMessage);

    Task<string> SendAsync(TEmailMessage emailMessage, CancellationToken token = default);
}