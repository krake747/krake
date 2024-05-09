using Krake.Api.Tests.Integration.Common;

namespace Krake.Api.Tests.Integration.Instruments;

public sealed class InstrumentsEndpointTests(KrakeApiFactory factory) : IClassFixture<KrakeApiFactory>, IAsyncLifetime
{
    private readonly List<Guid> _createdIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}