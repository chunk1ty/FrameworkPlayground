using System;
using System.Collections.Concurrent;

namespace Cache
{
    public class MyMemoryCache<TKey, TValue>
    {
        private class MyCacheEntry<TValue>
        {
            public MyCacheEntry(TValue value, long absoluteExpirationTicks)
            {
                Value = value;
                AbsoluteExpirationTicks = absoluteExpirationTicks;
            }

            public TValue Value { get; }

            public long AbsoluteExpirationTicks { get; }
        }

        private readonly ConcurrentDictionary<TKey, MyCacheEntry<TValue>> _entries = new ConcurrentDictionary<TKey, MyCacheEntry<TValue>>();

        private readonly TimeSpan _absoluteExpiration;

        public MyMemoryCache(TimeSpan absoluteExpiration)
        {
            _absoluteExpiration = absoluteExpiration;
        }

        public void Set(TKey key, TValue value)
        {
            var myCacheEntry = new MyCacheEntry<TValue>(value, DateTime.UtcNow.Ticks);
            _entries.TryAdd(key, myCacheEntry);
        }

        public void Remove(TKey key)
        {
            _entries.TryRemove(key, out var myCacheEntry);
        }

        public TValue Get(TKey key)
        {
            if (_entries.TryGetValue(key, out MyCacheEntry<TValue> myCacheEntry))
            {
                if (CheckExpired(myCacheEntry.AbsoluteExpirationTicks))
                {
                    Remove(key);
                    return default(TValue);
                }

                return myCacheEntry.Value;
            }

            return default(TValue);
        }

        private bool CheckExpired(long absoluteExpirationTicks)
        {
            long utcNowTicks = DateTime.UtcNow.Ticks;
            long cacheEntryAbsoluteExpirationTicks = absoluteExpirationTicks + _absoluteExpiration.Ticks;
            if (cacheEntryAbsoluteExpirationTicks < utcNowTicks)
            {
                return true;
            }

            return false;
        }
    }
}
