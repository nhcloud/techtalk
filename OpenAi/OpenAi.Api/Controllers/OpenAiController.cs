using Microsoft.AspNetCore.Mvc;

namespace OpenAi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenAiController : ControllerBase
    {
        [HttpGet]
        [Route("GetExec")]
        public async Task<string> GetAsync(string url)
        {
            var result = await new RestClient().GetAsync(url);
            return result;
        }
        [HttpPost]
        [Route("PostExec")]
        public async Task<string> PostAsync(string url, [FromBody] Dictionary<string, object> payLoad)
        {
            var result = await new RestClient().PostAsync(url, payLoad);
            return result;
        }
        [HttpPost]
        [Route("PostMultipartExec")]
        public async Task<string> PostMultipartAsync(string url, [FromBody] Dictionary<string, object> payLoad)
        {
            var result = await new RestClient().PostMultiPartAsync(url, payLoad);
            return result;
        }
    }
}