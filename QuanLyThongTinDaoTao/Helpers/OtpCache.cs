// Helpers/OtpCache.cs
using Microsoft.Extensions.Caching.Memory;
using System;

namespace QuanLyThongTinDaoTao.Helpers
{
    public static class OtpCache
    {
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static void Set(string key, object value, int minutes = 5)
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(minutes));
        }

        public static object Get(string key)
        {
            _cache.TryGetValue(key, out var value);
            return value;
        }

        public static void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
