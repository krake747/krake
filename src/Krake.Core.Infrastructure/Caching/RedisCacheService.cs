using System.Buffers;
using System.Text.Json;
using Krake.Core.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Krake.Core.Infrastructure.Caching;

internal sealed class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        var bytes = await cache.GetAsync(key, token);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken token = default)
    {
        var bytes = Serialize(value);

        return cache.SetAsync(key, bytes, CacheOptions.Create(expiration), token);
    }

    public Task RemoveAsync(string key, CancellationToken token = default) =>
        cache.RemoveAsync(key, token);

    private static T Deserialize<T>(byte[] bytes) => JsonSerializer.Deserialize<T>(bytes)!;

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }
}