using Microsoft.Extensions.Caching.Distributed;

namespace Krake.Core.Infrastructure.Caching;

internal static class CacheOptions
{
    private static readonly DistributedCacheEntryOptions DefaultExpiration = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : DefaultExpiration;
}