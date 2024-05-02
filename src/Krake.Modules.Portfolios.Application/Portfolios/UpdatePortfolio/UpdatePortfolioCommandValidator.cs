using FluentValidation;

namespace Krake.Modules.Portfolios.Application.Portfolios.UpdatePortfolio;

internal sealed class UpdatePortfolioCommandValidator : AbstractValidator<UpdatePortfolioCommand>
{
    public UpdatePortfolioCommandValidator()
    {
        RuleFor(c => c.PortfolioId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}