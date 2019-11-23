using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DependencyInjection.Functions
{
    public class HttpOpNamed
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpOpNamed(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        [FunctionName("HttpOpNamed")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            HttpClient client = _httpClientFactory.CreateClient("githubapi");

            HttpResponseMessage response = await client.GetAsync("repos/azure/azure-functions-host/pulls");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsAsync<object>();
                return new OkObjectResult(result);
            }

            return new StatusCodeResult((int)response.StatusCode);
        }
    }
}
