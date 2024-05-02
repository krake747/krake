using Krake.Core.Application.Messaging;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Application.Portfolios.DeletePortfolio;

public sealed record DeletePortfolioCommand(Guid PortfolioId)
    : ICommand<ErrorBase, Success>;