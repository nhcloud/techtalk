using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.Functions
{
    public class HttpOp
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpOp(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        [FunctionName("HttpOp")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            HttpClient client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("http://www.github.com/azure/azure-functions");

            return new StatusCodeResult((int)response.StatusCode);
        }
    }
}

