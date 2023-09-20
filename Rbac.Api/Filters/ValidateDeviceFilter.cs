using Rbac.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rbac.Api.Filters;

public class ValidateDeviceFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!IsMobile(context.HttpContext.Request.Headers.UserAgent))
        {
            context.Result = new BadRequestObjectResult("Request is not from supported device or os.");
        }
        return;
    }
    private static bool IsMobile(string userAgent)
    {
        var devices= ContextManager.AppConfiguration.GetSection("AppSettings")["MobileDevices"];//"android", "iphone", "mac", or "mobile"
        return string.IsNullOrEmpty(devices) || (!string.IsNullOrEmpty(userAgent) && devices.Split(',').Any(os => userAgent.Contains(os, StringComparison.InvariantCultureIgnoreCase)));
    }
}