using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace SignalRFunc
{
    /// <summary>
    ///
    /// </summary>

    public class Triggers
    {
        private const string HubName = "demo";

        [FunctionName("data")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            [CosmosDB("containers", "content",
                ConnectionStringSetting = "CosmosDBConnectionString")]
            IEnumerable<dynamic> documents, ILogger log)
        {
            log.LogInformation("HTTP trigger reading all documents.");

            return new OkObjectResult(documents);
        }

        [FunctionName("negotiate")]
        public static async Task<SignalRConnectionInfo> Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = HubName, ConnectionStringSetting = "AzureSignalRConnectionString")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("broadcast")]
        public static async Task Broadcast([CosmosDBTrigger(
                "containers",
                "content",
                ConnectionStringSetting = "CosmosDBConnectionString",
                LeaseCollectionName = "leases")]
            IEnumerable<dynamic> input, [SignalR(HubName = HubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("Broadcasting changed documents.");
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "updated",
                Arguments = new[] { (object)input }
            });
        }
    }
}