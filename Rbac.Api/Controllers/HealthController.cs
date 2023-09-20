using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Rbac.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpHead]
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public string GetHealth()
    {
        return $"{Environment.MachineName}|{DateTime.UtcNow.Ticks}|{Environment.GetEnvironmentVariable("REGION_NAME")}";
    }
}

