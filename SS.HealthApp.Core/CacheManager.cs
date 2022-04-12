using System;
using System.Runtime.Caching;

namespace SS.HealthApp.Core
{
    public class CacheManager<T>  where T :  class
    {

#if DEBUG
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static T Get(string CacheKey)
        {
            //Get the default MemoryCache to cache objects in memory
            ObjectCache cache = MemoryCache.Default;
            T objectFromCache = cache.Get(CacheKey) as T;

#if DEBUG
            if (objectFromCache != null)
            {
                logger.Debug(String.Format("CACHE: HIT Key {0}", CacheKey));
            }
            else
            {
                logger.Debug(String.Format("CACHE: MISS Key {0}", CacheKey));
            }
#endif

            return objectFromCache;

        }

        public static void Save(T ObjectToSave, string CacheKey, DateTimeOffset Expiration)
        {
            //Get the default MemoryCache to cache objects in memory
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = Expiration;

            cache.Add(CacheKey, ObjectToSave, policy);
#if DEBUG
            logger.Debug(String.Format("CACHE: SAVE Key {0}", CacheKey));
#endif

        }

    }
}
