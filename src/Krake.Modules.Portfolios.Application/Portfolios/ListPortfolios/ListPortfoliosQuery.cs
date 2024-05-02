using Krake.Core.Application.Messaging;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;

namespace Krake.Modules.Portfolios.Application.Portfolios.ListPortfolios;

public sealed record ListPortfoliosQuery
    : IQuery<IReadOnlyCollection<PortfolioResponse>>;