using Krake.Core.Domain;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Domain.Portfolios;

public sealed class Portfolio : Entity
{
    private readonly List<PortfolioInvestment> _investments = [];

    private Portfolio()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public IReadOnlyList<PortfolioInvestment> Investments => _investments.AsReadOnly();

    public static Portfolio From(string name, string currency) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Currency = currency
    };

    public Success ChangeName(string name)
    {
        if (Name == name)
        {
            return Ok.Success;
        }

        Name = name;
        return Ok.Success;
    }

    public Success AddInvestment(PortfolioInvestment investment)
    {
        _investments.Add(investment);
        return Ok.Success;
    }
}