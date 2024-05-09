namespace Krake.Modules.Portfolios.Tests.Integration;

public sealed class InstrumentsEndpointTests(KrakeApiFactory factory) : IClassFixture<KrakeApiFactory>, IAsyncLifetime
{
    private readonly List<Guid> _createdIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}