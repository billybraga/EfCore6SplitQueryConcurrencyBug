using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace EfCore6SplitQueryConcurrencyBug
{
    public sealed class MemoryCacheExceptCommandCacheKey : IMemoryCache
    {
        private readonly HashSet<Type> keyTypeBlacklist;

        // Same limit as EF core
        private readonly MemoryCache cache = new(new MemoryCacheOptions { SizeLimit = 10240 });

        public MemoryCacheExceptCommandCacheKey()
        {
            this.keyTypeBlacklist = new HashSet<Type>
            {
                typeof(Microsoft.EntityFrameworkCore.Query.Internal.RelationalCommandCache)
                    .Assembly
                    .GetType("Microsoft.EntityFrameworkCore.Query.Internal.RelationalCommandCache+CommandCacheKey")!,
            };
        }

        public void Dispose()
        {
            this.cache.Dispose();
        }

        public bool TryGetValue(object key, out object value)
        {
            if (!ShouldCache(key))
            {
                value = null!;
                return false;
            }

            return this.cache.TryGetValue(key, out value);
        }

        public ICacheEntry CreateEntry(object key)
        {
            return this.cache.CreateEntry(key);
        }

        public void Remove(object key)
        {
            if (!ShouldCache(key))
            {
                return;
            }
            
            this.cache.Remove(key);
        }

        private bool ShouldCache(object key)
        {
            return !this.keyTypeBlacklist.Contains(key.GetType());
        }
    }
}