using FluentValidation;

namespace Krake.Modules.Portfolios.Application.Instruments.CreateInstrument;

internal sealed class CreateInstrumentCommandValidator : AbstractValidator<CreateInstrumentCommand>
{
    public CreateInstrumentCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Currency).NotEmpty();
    }
}