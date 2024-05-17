namespace Krake.Api.Tests.Architecture;

public class ModuleTests
{
    private const string PortfoliosNamespace = "Krake.Modules.Portfolios";
    private const string PortfoliosIntegrationsNamespace = "Krake.Modules.Portfolios.Integrations";

    [Fact]
    public void PortfoliosModule_ShouldNotHaveDependencyOn()
    {
        Assert.True(true);
    }
}