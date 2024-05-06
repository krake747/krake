using FluentValidation;

namespace Krake.Modules.Portfolios.Application.Portfolios.AddPortfolioInvestment;

internal sealed class AddPortfolioInvestmentCommandValidator : AbstractValidator<AddPortfolioInvestmentCommand>
{
    public AddPortfolioInvestmentCommandValidator()
    {
        RuleFor(c => c.PortfolioId).NotEmpty();
        RuleFor(c => c.InstrumentId).NotEmpty();
        // KK 20240506: Purchase Price can be negative, e.g. commodities like electricity or the oil event in 2020
        RuleFor(c => c.PurchasePrice).GreaterThanOrEqualTo(decimal.Zero);
    }
}