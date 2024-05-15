using System.Reflection;
using Krake.Core.Domain;
using Krake.Modules.Portfolios.Domain;

namespace Krake.Modules.Portfolios.Tests.Architecture;

public sealed class DomainTests
{
    private static readonly Assembly Domain = typeof(IPortfoliosDomainMarker).Assembly;

    [Fact]
    public void DomainEvents_ShouldBeSealed_And_HaveDomainEventPostfix()
    {
        // Arrange
        var tests = Types.InAssembly(Domain)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .BeSealed()
            .And()
            .HaveNameEndingWith("DomainEvent", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Entities_ShouldHavePrivateParameterlessConstructor()
    {
        // Arrange
        var tests = Types.InAssembly(Domain)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        // Act
        var failingTypes = (
            from entityType in tests
            let constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            where constructors.Any(c => c.IsPrivate && c.GetParameters().Length is 0) is false
            select entityType
        ).ToList();

        // Assert
        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void Entities_ShouldOnlyHavePrivateConstructors()
    {
        var tests = Types.InAssembly(Domain)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = (
            from entityType in tests
            let constructors = entityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            where constructors.Length is not 0
            select entityType
        ).ToList();

        failingTypes.Should().BeEmpty();
    }
}