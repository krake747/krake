using FluentValidation;

namespace Krake.Modules.Portfolios.Application.Portfolios.DeletePortfolio;

internal sealed class DeletePortfolioCommandValidator : AbstractValidator<DeletePortfolioCommand>
{
    public DeletePortfolioCommandValidator()
    {
        RuleFor(c => c.PortfolioId).NotEmpty();
    }
}