using FluentValidation;

namespace Krake.Application.Portfolios;

public sealed class PortfolioValidator : AbstractValidator<Portfolio>
{
    public PortfolioValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}