namespace Krake.Infrastructure.Email.Common;

public interface IGenericEmailRetriever<TEmailMessage>
{
    TEmailMessage Retrieve();
    Task<TEmailMessage> RetrieveAsync(CancellationToken token = default);
}