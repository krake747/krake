using System.Net;
using System.Net.Http.Json;
using Krake.Api.Tests.Integration.Common;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;
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

    [Fact]
    public async Task GetPortfolio_ShouldReturnPortfolio_WhenIdExists()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var request = new CreatePortfolioRequest
        {
            Name = "Krake Test Portfolio",
            Currency = "EUR"
        };

        var created = await httpClient.PostAsJsonAsync("portfolios", request);
        var id = await created.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(id);

        var createdPortfolio = new PortfolioResponse(id, request.Name, request.Currency);

        // Act
        var result = await httpClient.GetAsync($"portfolios/{id}");
        var portfolio = await result.Content.ReadFromJsonAsync<PortfolioResponse>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        portfolio.Should().BeEquivalentTo(createdPortfolio);
    }

    [Fact]
    public async Task ListPortfolios_ShouldListAllPortfolios_WhenPortfoliosAlreadyExist()
    {
        // Arrange
        var httpClient = factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync("portfolios");
        var portfolios = await result.Content.ReadFromJsonAsync<IEnumerable<PortfolioResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        portfolios.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdatePortfolio_ShouldUpdatePortfolioProperties_WhenUpdatePortfolioRequestIsValid()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var createRequest = new CreatePortfolioRequest
        {
            Name = "Krake Test Portfolio",
            Currency = "EUR"
        };

        var created = await httpClient.PostAsJsonAsync("portfolios", createRequest);
        var id = await created.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(id);

        var updateRequest = new UpdatePortfolioRequest
        {
            Name = "Krake Updated Portfolio"
        };

        var updatedPortfolio = new PortfolioResponse(id, updateRequest.Name, createRequest.Currency);

        // Act
        var result = await httpClient.PutAsJsonAsync($"portfolios/{id}", updateRequest);
        var portfolio = await httpClient.GetFromJsonAsync<PortfolioResponse>($"portfolios/{id}");

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        portfolio.Should().BeEquivalentTo(updatedPortfolio);
    }

    [Fact]
    public async Task DeletePortfolio_ShouldDeletePortfolio_WhenIdExists()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var createRequest = new CreatePortfolioRequest
        {
            Name = "Krake Test Portfolio",
            Currency = "EUR"
        };

        var created = await httpClient.PostAsJsonAsync("portfolios", createRequest);
        var id = await created.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(id);

        // Act
        var result = await httpClient.DeleteAsync($"portfolios/{id}");

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}