﻿using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;

public sealed record GetPortfolioQuery(Guid PortfolioId)
    : IQuery<ErrorBase, PortfolioResponse>;

internal sealed class GetPortfolioQueryHandler(IReadOnlyPortfolioRepository portfolioRepository)
    : IQueryHandler<GetPortfolioQuery, ErrorBase, PortfolioResponse>
{
    public async Task<Result<ErrorBase, PortfolioResponse>> Handle(GetPortfolioQuery request,
        CancellationToken token = default)
    {
        var exists = await portfolioRepository.ExistsByIdAsync(request.PortfolioId, token);
        if (exists is false)
        {
            return PortfolioErrors.NotFound(request.PortfolioId);
        }

        var portfolio = await portfolioRepository.GetByIdAsync(request.PortfolioId, token);

        return new PortfolioResponse(portfolio!.Id, portfolio.Name, portfolio.Currency);
    }
}