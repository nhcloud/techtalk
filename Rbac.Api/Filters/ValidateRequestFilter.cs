using System.IdentityModel.Tokens.Jwt;
using Rbac.Api.Extensions;
using Rbac.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rbac.Api.Filters;

public class ValidateRequestFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (ContextManager.AppConfiguration.GetSection("AppSettings")["AnonymousAccess"] == "Allowed")
        {
            return;
        }
        if (Constants.AnonymousRoutes.Any(route => context.HttpContext.Request.Path.Value?.ToLower() == route))
        {
            return;
        }

        if (string.IsNullOrEmpty(context.HttpContext.Request.Headers.Authorization))
        {
            context.Result = new BadRequestObjectResult("Unauthorized access.");
            return;
        }

        if (context.ModelState.IsValid)
        {
            var auth = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var jwtToken = new JwtSecurityToken(auth);
            if (jwtToken.ValidTo <= DateTime.UtcNow)
            {
                context.Result = new BadRequestObjectResult("Invalid authorization token.");
            }
            return;
        }

        return;
    }
}