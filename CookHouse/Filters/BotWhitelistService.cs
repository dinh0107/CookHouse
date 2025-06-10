using System.Linq;
using System.Net;

namespace CookHouse.Filters
{
    public static class BotWhitelistService
    {
        private static readonly string[] KnownBots = new[]
        {
            "googlebot",
            "bingbot",
            "facebookexternalhit",
            "telegrambot"
        };

        public static bool IsKnownBot(string userAgent, string ip = null)
        {
            if (string.IsNullOrEmpty(userAgent)) return false;

            var ua = userAgent.ToLower();
            if (KnownBots.Any(bot => ua.Contains(bot)))
            {
                return true;
            }

            // Optional: DNS reverse check nếu cần chính xác cao
            if (!string.IsNullOrEmpty(ip))
            {
                try
                {
                    var host = Dns.GetHostEntry(ip).HostName.ToLower();
                    if (host.EndsWith(".googlebot.com") || host.EndsWith(".google.com") || host.EndsWith(".search.msn.com"))
                    {
                        var ips = Dns.GetHostAddresses(host);
                        return ips.Any(a => a.ToString() == ip);
                    }
                }
                catch
                {
                    // ignore DNS error
                }
            }

            return false;
        }
    }
}