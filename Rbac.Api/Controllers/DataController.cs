using System.Net;
using Rbac.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Rbac.Api.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]

public class DataController : ControllerBase
{
    private readonly IData _dataSvc;
    private readonly string _eventId;
    private readonly ILogger<DataController> _logger;

    public DataController(ILogger<DataController> logger, IData data)
    {
        _logger = logger;
        _dataSvc = data;
        _eventId = Guid.NewGuid().ToString();
    }


    [HttpPost]
    [Route("get")]
    [ProducesResponseType(typeof(Dictionary<string, object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Dictionary<string, object>>> GetAsync([FromBody] Dictionary<string, object> userInfo)
    {
        try
        {
            return new OkObjectResult(await _dataSvc.GetAsync(userInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(_eventId, ex);
            return BadRequest(ex.Message);
        }
    }


    [HttpPost]
    [Route("update")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> UpdateAsync([FromBody] Dictionary<string, string> device)
    {
        try
        {
            var deviceItem = device.ToDictionary<KeyValuePair<string, string>, string, object>(item => item.Key, item => item.Value);
            await _dataSvc.UpdateAsync(deviceItem);
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(_eventId, ex);
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    [Route("download")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DownloadAsync(string fileName)
    {
        try
        {
            return new OkObjectResult(await _dataSvc.DownloadAsync(fileName));
        }
        catch (Exception ex)
        {
            _logger.LogError(_eventId, ex);
            return BadRequest(ex.Message);
        }
        
    }
}