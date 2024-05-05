using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios.DeletePortfolio;

public sealed record DeletePortfolioCommand(Guid PortfolioId)
    : ICommand<ErrorBase, Success>;

internal sealed class DeletePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
    : ICommandHandler<DeletePortfolioCommand, ErrorBase, Success>
{
    public async Task<Result<ErrorBase, Success>> Handle(DeletePortfolioCommand request,
        CancellationToken token = default)
    {
        var found = await portfolioRepository.ExistsByIdAsync(request.PortfolioId, token);
        if (found is false)
        {
            return PortfolioErrors.NotFound(request.PortfolioId);
        }

        _ = await portfolioRepository.DeleteByIdAsync(request.PortfolioId, token);

        return Ok.Success;
    }
}