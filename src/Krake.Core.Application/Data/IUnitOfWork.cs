namespace Krake.Core.Application.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default);
}