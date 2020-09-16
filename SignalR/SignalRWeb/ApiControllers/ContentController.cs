using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using SignalRWeb.Hubs;
using SignalRWeb.Models;

namespace SignalRWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IHubContext<ContentHub> _contentHub;

        public ContentController(IHubContext<ContentHub> contentHub)
        {
            _contentHub = contentHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetChanges()
        {

            return new OkObjectResult(await Helper.GetDataAsync());
        }
        [HttpPost]
        public async Task<IActionResult> UpdatedChanges()
        {
            await _contentHub.Clients.All.SendAsync("Items", await Helper.GetDataAsync());
            return NoContent();
        }
    }
}
