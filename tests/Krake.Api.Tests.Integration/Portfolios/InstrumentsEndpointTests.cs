using System.Net;
using System.Net.Http.Json;
using Krake.Api.Tests.Integration.Common;
using Krake.Modules.Portfolios.Application.Instruments.GetInstrument;
using Krake.Modules.Portfolios.Presentation.Instruments;

namespace Krake.Api.Tests.Integration.Portfolios;

public sealed class InstrumentsEndpointTests(KrakeApiFactory factory) : IClassFixture<KrakeApiFactory>, IAsyncLifetime
{
    private readonly List<Guid> _createdIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = factory.CreateClient();
        foreach (var id in _createdIds)
        {
            await httpClient.DeleteAsync($"instruments/{id}");
        }
    }

    [Fact]
    public async Task CreateInstrument_ShouldCreateNewInstrument_WhenCreateInstrumentRequestIsValid()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var request = new CreateInstrumentRequest
        {
            Name = "Krake Stock",
            Currency = "EUR",
            Country = "LU",
            Mic = "XLUX",
            Sector = "Technology",
            Symbol = "KRKE",
            Isin = "LUXXXXXXXXXX"
        };

        // Act
        var result = await httpClient.PostAsJsonAsync("instruments", request);
        var response = await result.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(response);

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}instruments/{response}");
    }

    [Fact]
    public async Task GetInstrument_ShouldReturnInstrument_WhenIdExists()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var request = new CreateInstrumentRequest
        {
            Name = "Krake Stock",
            Currency = "EUR",
            Country = "LU",
            Mic = "XLUX",
            Sector = "Technology",
            Symbol = "KRKE",
            Isin = "LUXXXXXXXXXX"
        };

        var created = await httpClient.PostAsJsonAsync("instruments", request);
        var id = await created.Content.ReadFromJsonAsync<Guid>();
        _createdIds.Add(id);

        var createdInstrument = new InstrumentResponse(
            id,
            request.Name,
            request.Currency,
            request.Country,
            request.Mic,
            request.Sector,
            request.Symbol,
            request.Isin);

        // Act
        var result = await httpClient.GetAsync($"instruments/{id}");
        var instrument = await result.Content.ReadFromJsonAsync<InstrumentResponse>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        instrument.Should().BeEquivalentTo(createdInstrument);
    }

    [Fact]
    public async Task ListInstruments_ShouldListAllInstruments_WhenInstrumentsAlreadyExist()
    {
        // Arrange
        var httpClient = factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync("instruments");
        var instruments = await result.Content.ReadFromJsonAsync<IEnumerable<InstrumentResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        instruments.Should().NotBeEmpty();
    }
}