using Krake.Core.Application.Messaging;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Application.Portfolios.CreatePortfolio;

public sealed record CreatePortfolioCommand(string Name)
    : ICommand<ErrorBase, Guid>;