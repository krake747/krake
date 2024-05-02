using Krake.Core.Domain;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Domain.Portfolios;

public sealed class Portfolio : Entity
{
    private Portfolio()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public static Portfolio From(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name
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
}