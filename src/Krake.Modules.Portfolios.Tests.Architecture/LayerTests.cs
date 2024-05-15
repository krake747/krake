using System.Reflection;
using Krake.Modules.Portfolios.Application;
using Krake.Modules.Portfolios.Domain;
using Krake.Modules.Portfolios.Infrastructure;
using Krake.Modules.Portfolios.Presentation;

namespace Krake.Modules.Portfolios.Tests.Architecture;

public sealed class LayerTests
{
    private static readonly Assembly Domain = typeof(IPortfoliosDomainMarker).Assembly;
    private static readonly Assembly Application = typeof(IPortfoliosApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(IPortfoliosInfrastructureMarker).Assembly;
    private static readonly Assembly Presentation = typeof(IPortfoliosPresentationMarker).Assembly;

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        // Arrange
        var tests = Types.InAssembly(Domain)
            .Should()
            .NotHaveDependencyOn(Application.GetName().Name);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange
        var tests = Types.InAssembly(Domain)
            .Should()
            .NotHaveDependencyOn(Infrastructure.GetName().Name);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange
        var tests = Types.InAssembly(Application)
            .Should()
            .NotHaveDependencyOn(Infrastructure.GetName().Name);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange
        var tests = Types.InAssembly(Application)
            .Should()
            .NotHaveDependencyOn(Presentation.GetName().Name);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange
        var tests = Types.InAssembly(Presentation)
            .Should()
            .NotHaveDependencyOn(Infrastructure.GetName().Name);

        // Act
        var result = tests.GetResult();

        // Assert
        result.FailingTypes.Should().BeNullOrEmpty();
    }
}