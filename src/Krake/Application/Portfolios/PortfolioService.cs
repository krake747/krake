using FluentValidation;
using Krake.Application.Mapping;
using Krake.Core;

namespace Krake.Application.Portfolios;

public interface IPortfolioService
{
    Task<Result<Errors, Portfolio>> CreateAsync(CreatePortfolio createPortfolio, CancellationToken token = default);
    Task<Result<Error, Portfolio>> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default);

    Task<Result<Errors, Portfolio>> UpdateByIdAsync(Guid portfolioId, UpdatePortfolio updatePortfolio,
        CancellationToken token = default);

    Task<Result<Error, Deleted>> DeleteByIdAsync(Guid portfolioId, CancellationToken token = default);
}

public sealed class PortfolioService(IPortfolioRepository portfolioRepository, IValidator<Portfolio> validator)
    : IPortfolioService
{
    public async Task<Result<Errors, Portfolio>> CreateAsync(CreatePortfolio createPortfolio,
        CancellationToken token = default)
    {
        var portfolio = new Portfolio(Guid.NewGuid(), createPortfolio.Name);

        var validationResult = await validator.ValidateAsync(portfolio, token);
        if (validationResult.IsValid is false)
        {
            return new Errors(validationResult.Errors.MapToErrors());
        }

        var existsResult = await portfolioRepository.ExistsByIdAsync(portfolio.Id, token);
        if (existsResult.IsValue)
        {
            return portfolio;
        }

        var createdResult = await portfolioRepository.CreateAsync(portfolio, token);
        return createdResult.MapResult(error => new Errors([error]), _ => portfolio);
    }

    public async Task<Result<Error, Portfolio>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var portfolioResult = await portfolioRepository.GetByIdAsync(id, token);
        return portfolioResult;
    }

    public async Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default)
    {
        var portfoliosResult = await portfolioRepository.GetAllAsync(token);
        return portfoliosResult;
    }

    public async Task<Result<Errors, Portfolio>> UpdateByIdAsync(Guid id, UpdatePortfolio updatePortfolio,
        CancellationToken token = default)
    {
        var originalResult = await portfolioRepository.GetByIdAsync(id, token);
        if (originalResult.IsError)
        {
            return new Errors([originalResult.AsError]);
        }

        var originalPortfolio = originalResult.AsValue;
        var portfolio = new Portfolio(id, updatePortfolio.Name ?? originalPortfolio.Name);

        var validationResult = await validator.ValidateAsync(portfolio, token);
        if (validationResult.IsValid is false)
        {
            return new Errors(validationResult.Errors.MapToErrors());
        }

        var updatedResult = await portfolioRepository.UpdateAsync(portfolio, token);
        return updatedResult.MapResult(error => new Errors([error]), _ => portfolio);
    }

    public async Task<Result<Error, Deleted>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var existsResult = await portfolioRepository.ExistsByIdAsync(id, token);
        if (existsResult.IsError)
        {
            return existsResult.AsError;
        }

        var deletedResult = await portfolioRepository.DeleteByIdAsync(id, token);
        return deletedResult;
    }
}