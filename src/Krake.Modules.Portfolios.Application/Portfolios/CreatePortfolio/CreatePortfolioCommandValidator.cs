using FluentValidation;

namespace Krake.Modules.Portfolios.Application.Portfolios.CreatePortfolio;

internal sealed class CreatePortfolioCommandValidator : AbstractValidator<CreatePortfolioCommand>
{
    public CreatePortfolioCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Currency).NotEmpty().Length(3);
    }
}