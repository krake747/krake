namespace Krake.Application.Portfolios;

public sealed record Portfolio(Guid Id, string Name);

public sealed record CreatePortfolio(Guid Id, string Name);

public sealed record UpdatePortfolio(string Name);