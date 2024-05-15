using System.Reflection;
using FluentValidation;
using Krake.Core.Application.Messaging;
using Krake.Modules.Portfolios.Application;

namespace Krake.Modules.Portfolios.Tests.Architecture;

public class ApplicationTests
{
    private static readonly Assembly Application = typeof(IPortfoliosApplicationMarker).Assembly;

    [Fact]
    public void Command_ShouldBeSealed_And_HaveNameEndingWithCommand()
    {
        // Arrange
        var tests = Types.InAssembly(Application)
            .That()
            .ImplementInterface(typeof(ICommand<,>))
            .Should()
            .BeSealed()
            .And()
            .HaveNameEndingWith("Command", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void CommandHandler_ShouldNotBePublic_And_BeSealed_AndHaveNameEndingWithCommandHandler()
    {
        // Arrange
        var tests = Types.InAssembly(Application)
            .That()
            .ImplementInterface(typeof(ICommandHandler<,,>))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .And()
            .HaveNameEndingWith("CommandHandler", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }


    [Fact]
    public void Query_ShouldBeSealed_And_HaveNameEndingWithQuery()
    {
        var tests = Types.InAssembly(Application)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .BeSealed()
            .And()
            .HaveNameEndingWith("Query", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }


    [Fact]
    public void QueryHandler_ShouldNotBePublic_And_BeSealed_And_HaveNameEndingWithQueryHandler()
    {
        var tests = Types.InAssembly(Application)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .And()
            .HaveNameEndingWith("QueryHandler", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }


    [Fact]
    public void Validator_ShouldNotBePublic_And_BeSealed_And_HaveNameEndingWithValidator()
    {
        var tests = Types.InAssembly(Application)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .And()
            .HaveNameEndingWith("Validator", StringComparison.Ordinal);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }
}