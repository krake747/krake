using Krake.Core.Application.Messaging;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;

public sealed record GetPortfolioQuery(Guid PortfolioId)
    : IQuery<ErrorBase, PortfolioResponse>;