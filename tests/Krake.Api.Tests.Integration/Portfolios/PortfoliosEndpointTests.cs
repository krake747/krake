using System.Net;
using System.Net.Http.Json;
using Krake.Api.Tests.Integration.Common;
using Krake.Modules.Portfolios.Presentation.Portfolios;

namespace Krake.Api.Tests.Integration.Portfolios;

public sealed class PortfoliosEndpointTests(KrakeApiFactory factory) : IClassFixture<KrakeApiFactory>, IAsyncLifetime
{
    private readonly List<Guid> _createdIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = factory.CreateClient();

        foreach (var id in _createdIds)
        {
            await httpClient.DeleteAsync($"portfolios/{id}");
        }
    }

    [Fact]
    public async Task CreatePortfolio_ShouldCreateNewPortfolio_WhenCreatePortfolioRequestIsValid()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var request = new CreatePortfolioRequest
        {
            Name = "Krake Test Portfolio",
            Currency = "EUR"
        };

        // Act
        var result = await httpClient.PostAsJsonAsync("portfolios", request);
        var response = await result.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(response);

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}portfolios/{response}");
    }
}