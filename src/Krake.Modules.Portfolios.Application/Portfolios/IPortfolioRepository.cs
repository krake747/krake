using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios;

public interface IPortfolioRepository : IReadOnlyPortfolioRepository
{
    Task<bool> CreateAsync(Portfolio portfolio, CancellationToken token = default);
    Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> AddPortfolioInvestmentAsync(PortfolioInvestment investment, CancellationToken token = default);
}

public interface IReadOnlyPortfolioRepository
{
    Task<IEnumerable<Portfolio>> ListAsync(CancellationToken token = default);
    Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
}