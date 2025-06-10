using CookHouse.Models;
using System;
using System.Linq;
using CookHouse.DAL;
using System.Runtime.Caching;

namespace CookHouse.Filters
{
    public static class FirewallService
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        public static bool IsBlocked(string ip, int maxRequestsPerMinute = 30)
        {
            var key = $"req-count-{ip}";
            var currentCount = (int)(Cache.Get(key) ?? 0);

            if (currentCount >= maxRequestsPerMinute)
            {
                LogToDb(ip, currentCount);
                return true;
            }

            Cache.Set(key, currentCount + 1, DateTimeOffset.Now.AddSeconds(60));
            return false;
        }

        private static void LogToDb(string ip, int count)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var existing = unitOfWork.BlockedIpLogRepository.GetQuery(x => x.IP == ip && x.IsActive).FirstOrDefault();
                if (existing != null) return;
                unitOfWork.BlockedIpLogRepository.Insert(new BlockedIpLog
                {
                    IP = ip,
                    BlockedAt = DateTime.Now,
                    RequestCount = count
                });
                unitOfWork.Save();
            }
        }
    }
}