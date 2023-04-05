using Microsoft.AspNetCore.Mvc.Filters;

namespace OpenAi.Api.Filters;

public class ValidateRequestFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        return;
    }
}