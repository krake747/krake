namespace Krake.Infrastructure.Email.Common;

public interface IGenericEmailService<TEmailMessage>
{
    bool Send(TEmailMessage emailMessage);
    Task<bool> SendAsync(TEmailMessage emailMessage, CancellationToken token = default);
    TEmailMessage Retrieve();
}