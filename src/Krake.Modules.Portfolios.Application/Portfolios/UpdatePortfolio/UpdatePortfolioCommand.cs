using Krake.Core.Application.Messaging;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Application.Portfolios.UpdatePortfolio;

public sealed record UpdatePortfolioCommand(Guid PortfolioId, string Name)
    : ICommand<ErrorBase, Success>;