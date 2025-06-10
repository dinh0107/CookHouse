using System.Linq;
using System.Web.Mvc;

namespace CookHouse.Filters
{
    public class DdosProtectAttribute : ActionFilterAttribute
    {
        public int Limit { get; set; } = 30;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var descriptor = filterContext.ActionDescriptor;
            //Bỏ qua partialview
            var skip = descriptor.GetCustomAttributes(typeof(SkipDdosCheckAttribute), false).Any();
            if (skip)
            {
                return;
            }
            
            var ip = filterContext.HttpContext.Request.UserHostAddress;
            var ua = filterContext.HttpContext.Request.UserAgent;
            if (BotWhitelistService.IsKnownBot(ua, ip))
            {
                return;
            }

            if (FirewallService.IsBlocked(ip, Limit))
            {
                filterContext.Result = new HttpStatusCodeResult(429, "Too Many Requests");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}